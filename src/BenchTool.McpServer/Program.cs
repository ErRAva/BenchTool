using System.Text.Json;
using BenchTool.Domain;
using BenchTool.Infrastructure.BenchApi.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BenchTool.McpServer;

internal static class Program
{
	private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

	private static async Task Main(string[] args)
	{
		var builder = Host.CreateApplicationBuilder(args);

		// MCP stdio transport requires stdout to contain ONLY JSON-RPC messages.
		builder.Logging.ClearProviders();
		builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);
		builder.Logging.SetMinimumLevel(LogLevel.Warning);

		builder.Configuration
			.AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"), optional: true, reloadOnChange: true)
			.AddUserSecrets(typeof(Program).Assembly, optional: true)
			.AddEnvironmentVariables();

		builder.Services
			.AddBenchToolBenchApi(builder.Configuration)
			.AddScoped<IRoleInfoService, RoleInfoService>();

		using var host = builder.Build();
		using var scope = host.Services.CreateScope();
		var roleInfoService = scope.ServiceProvider.GetRequiredService<IRoleInfoService>();

		Console.Error.WriteLine("BenchTool.McpServer started (stdio)");

		string? line;
		while ((line = await Console.In.ReadLineAsync()) is not null)
		{
			if (string.IsNullOrWhiteSpace(line))
				continue;

			try
			{
				using var doc = JsonDocument.Parse(line);
				var root = doc.RootElement;

				if (!root.TryGetProperty("method", out var methodEl))
					continue;

				var method = methodEl.GetString();
				var hasId = root.TryGetProperty("id", out var idEl);

				switch (method)
				{
					case "initialize":
						if (!hasId) break;
						await WriteResponseAsync(idEl, CreateInitializeResult(root));
						break;

					case "notifications/initialized":
						break;

					case "tools/list":
						if (!hasId) break;
						await WriteResponseAsync(idEl, new
						{
							tools = new object[]
							{
								new
								{
									name = "bench.get_role_info",
									title = "Get role info",
									description = "Fetch and aggregate role information for a given RoleId.",
									inputSchema = new
									{
										type = "object",
										properties = new
										{
											roleId = new { type = "string", description = "RoleId to query" }
										},
										required = new[] { "roleId" }
									}
								}
							},
							nextCursor = (string?)null
						});
						break;

					case "tools/call":
						if (!hasId) break;
						await HandleToolsCallAsync(idEl, root, roleInfoService);
						break;

					default:
						if (!hasId) break;
						await WriteErrorAsync(idEl, -32601, $"Method not found: {method}");
						break;
				}
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex);
			}
		}
	}

	private static object CreateInitializeResult(JsonElement root)
	{
		var requested = root.TryGetProperty("params", out var p) && p.TryGetProperty("protocolVersion", out var v)
			? v.GetString()
			: "2025-11-25";

		return new
		{
			protocolVersion = requested,
			capabilities = new
			{
				tools = new { listChanged = false },
				logging = new { }
			},
			serverInfo = new
			{
				name = "BenchTool.McpServer",
				title = "BenchTool MCP Server",
				version = "0.1.0"
			},
			instructions = "Provides bench.get_role_info(roleId)"
		};
	}

	private static async Task HandleToolsCallAsync(JsonElement idEl, JsonElement root, IRoleInfoService roleInfoService)
	{
		if (!root.TryGetProperty("params", out var p))
		{
			await WriteErrorAsync(idEl, -32602, "Missing params");
			return;
		}

		var name = p.TryGetProperty("name", out var n) ? n.GetString() : null;
		if (!string.Equals(name, "bench.get_role_info", StringComparison.Ordinal))
		{
			await WriteErrorAsync(idEl, -32602, $"Unknown tool: {name}");
			return;
		}

		if (!p.TryGetProperty("arguments", out var a) || !a.TryGetProperty("roleId", out var roleIdEl))
		{
			await WriteResponseAsync(idEl, new
			{
				content = new[] { new { type = "text", text = "Missing required argument: roleId" } },
				isError = true
			});
			return;
		}

		var roleId = roleIdEl.GetString();
		if (string.IsNullOrWhiteSpace(roleId))
		{
			await WriteResponseAsync(idEl, new
			{
				content = new[] { new { type = "text", text = "roleId must be a non-empty string" } },
				isError = true
			});
			return;
		}

		var result = await roleInfoService.GetRoleInfoAsync(roleId);
		var json = JsonSerializer.Serialize(result, JsonOptions);
		var isError = result.Errors is not null && result.Errors.Any();

		await WriteResponseAsync(idEl, new
		{
			content = new[] { new { type = "text", text = json } },
			structuredContent = result,
			isError
		});
	}

	private static async Task WriteResponseAsync(JsonElement idEl, object result)
	{
		object id = idEl.ValueKind == JsonValueKind.Number ? idEl.GetInt32() : idEl.GetString()!;
		var response = new
		{
			jsonrpc = "2.0",
			id,
			result
		};

		await Console.Out.WriteLineAsync(JsonSerializer.Serialize(response, JsonOptions));
		await Console.Out.FlushAsync();
	}

	private static async Task WriteErrorAsync(JsonElement idEl, int code, string message)
	{
		object id = idEl.ValueKind == JsonValueKind.Number ? idEl.GetInt32() : idEl.GetString()!;
		var response = new
		{
			jsonrpc = "2.0",
			id,
			error = new { code, message }
		};

		await Console.Out.WriteLineAsync(JsonSerializer.Serialize(response, JsonOptions));
		await Console.Out.FlushAsync();
	}
}
