using BenchTool.Domain;
using BenchTool.Infrastructure.BenchApi.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BenchTool.Cli;

internal static class Program
{
	private static async Task<int> Main(string[] args)
	{
		var builder = Host.CreateApplicationBuilder(args);
		builder.Configuration
			.AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"), optional: true, reloadOnChange: true)
			.AddUserSecrets(typeof(Program).Assembly, optional: true)
			.AddEnvironmentVariables();

		builder.Services
			.AddBenchToolBenchApi(builder.Configuration)
			.AddScoped<IRoleInfoService, RoleInfoService>();

		using var host = builder.Build();
		using var scope = host.Services.CreateScope();

		var svc = scope.ServiceProvider.GetRequiredService<IRoleInfoService>();
		var options = CliOptions.Parse(args);

		var results = await svc.GetRoleInfosAsync(options.RoleIds);

		if (!string.IsNullOrWhiteSpace(options.CsvPath))
		{
			await new MyCsvWriter().Write(options.CsvPath!, results.ToList());
			Console.WriteLine($"Wrote {results.Count} record(s) to {options.CsvPath}");
			return 0;
		}

		foreach (var r in results)
			Console.WriteLine(r.ToString());

		return 0;
	}
}

internal sealed record CliOptions(IReadOnlyList<string> RoleIds, string? CsvPath)
{
	public static CliOptions Parse(string[] args)
	{
		var roleIds = new List<string>();
		string? csv = null;

		for (var i = 0; i < args.Length; i++)
		{
			switch (args[i])
			{
				case "--role-id" when i + 1 < args.Length:
					roleIds.Add(args[++i]);
					break;
				case "--role-ids" when i + 1 < args.Length:
					roleIds.AddRange(args[++i]
						.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
					break;
				case "--csv" when i + 1 < args.Length:
					csv = args[++i];
					break;
			}
		}

		if (roleIds.Count == 0)
			roleIds.Add("6215662");

		return new CliOptions(roleIds, csv);
	}
}
