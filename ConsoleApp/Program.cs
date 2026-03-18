using ConsoleApp.Startup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ConsoleApp;

internal class Program
{
	private static async Task Main(string[] args)
	{
		var host = Host
			.CreateApplicationBuilder(args)
			.SetUpBenchTool()
			.Build();

		using var serviceScope = host.Services.CreateScope();
		var services = serviceScope.ServiceProvider;
		try
		{
			var roleInfoProcessor = services.GetService<RoleInfoProcessor>();
			if (roleInfoProcessor != null)
			{
				var roleIds = GetRoleIds();
				//await roleInfoProcessor.ProcessRole(roleIds.First());
				await roleInfoProcessor.ProcessRoles(roleIds);
			}
			else
			{
				Console.WriteLine("RoleInfoProcessor service is not registered.");
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"An error occurred: {ex.Message}");
		}
	}
	private static string[] GetRoleIds()
	{
		return [
			"6215662",
			//"6108886",
			//"6219444",
			//"6172779",
			//"6167542",
			//"6204394",
			//"6205133",
			//"6169125",
			//"6073092",
			//"6087466",
			//"6148128",
			//"6093751",
			//"6165224",
			//"6205420",
			//"6218350",
			//"6191479",
			//"6195107",
			//"6220350",
			//"5580296",
			//"6220324",
			//"6182216",
			//"6188724",
			//"6188718",
			//"6185431",
			//"6195010",
			//"6174424",
			//"6031063",
			//"6183152",
			//"6218267",
			//"6131565",
			//"6155957",
		];
	}
}