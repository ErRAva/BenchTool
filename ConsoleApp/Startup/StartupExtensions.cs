using ConsoleApp.BenchClients;
using HttpClientToCurl.Config;
using HttpClientToCurl.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Json;

namespace ConsoleApp.Startup;

internal static class StartupExtensions
{
	extension(HostApplicationBuilder builder)
	{
		public HostApplicationBuilder SetUpBenchTool()
		{
			builder.SetBenchToolConfigValues();
			builder.Services.AddHttpClients(builder.Configuration);
			builder.Services.RegisterBenchServices();

			return builder;
		}

		public void SetBenchToolConfigValues()
		{
			builder.Configuration
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddUserSecrets<BenchToolConfig>()
				.AddEnvironmentVariables();

			builder.Services
				.AddOptions<BenchToolConfig>()
				.BindConfiguration(nameof(BenchToolConfig))
				.ValidateDataAnnotations()
				.ValidateOnStart();
		}
	}

	extension(IServiceCollection services)
	{
		public IServiceCollection RegisterBenchServices()
		{
			return services
				.AddScoped<RoleInfoProcessor>();
		}

		public IServiceCollection AddHttpClients(ConfigurationManager configurationManager)
		{
			var benchToolConfig = configurationManager
				.GetSection(nameof(BenchToolConfig))
				.Get<BenchToolConfig>();

			var configurationSection = configurationManager
				.GetSection("HttpClientToCurl");
			var httpClientToCurl = configurationSection
				.Get<CompositConfig>();

			services.AddTransient<RequestBlockingDelegatingHandler>();
			services.AddAllHttpClientToCurl(configurationManager);

			return services
				.AddBenchHttpClient<ProjectExecAssignBenchClient>(benchToolConfig)
				.AddBenchHttpClient<ReplyEmailBenchClient>(benchToolConfig);
		}

		public IServiceCollection AddBenchHttpClient<T>(BenchToolConfig benchToolConfig) where T : class
		{
			var httpClientBuilder = services
				.AddHttpClient<T>(MyConfigureClient)
				.AddHttpMessageHandler<RequestBlockingDelegatingHandler>();

			//if (benchToolConfig.ShouldOutputCurl)
			//	httpClientBuilder.AddCurlLogging();

			return services;
		}
	}

	private static void MyConfigureClient(IServiceProvider serviceProvider, HttpClient httpClient)
	{
		var benchToolOptions = serviceProvider.GetRequiredService<IOptions<BenchToolConfig>>().Value;
		httpClient.DefaultRequestHeaders.Add("accept", "application/json, text/plain, */*" );
		httpClient.DefaultRequestHeaders.Add("accept-language", "en-US,en;q=0.9" );
		httpClient.DefaultRequestHeaders.Add("origin", "https://mysched.accenture.com" );
		httpClient.DefaultRequestHeaders.Add("priority", "u=1, i" );
		httpClient.DefaultRequestHeaders.Add("sec-ch-ua", "\"Chromium\";v=\"146\", \"Not-A.Brand\";v=\"24\", \"Google Chrome\";v=\"146\"" );
		httpClient.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0" );
		httpClient.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"" );
		httpClient.DefaultRequestHeaders.Add("sec-fetch-dest", "empty" );
		httpClient.DefaultRequestHeaders.Add("sec-fetch-mode", "cors" );
		httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/146.0.0.0 Safari/537.36" );
		httpClient.DefaultRequestHeaders.Add("authorization", $"Bearer {benchToolOptions.AuthToken}");
	}
}

public class RequestBlockingDelegatingHandler(IOptions<BenchToolConfig> benchToolConfig) : DelegatingHandler
{
	private readonly BenchToolConfig _benchToolConfig = benchToolConfig.Value;

	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		if (!_benchToolConfig.ShouldBlockTheRequest)
			return base.SendAsync(request, cancellationToken);

		var response = new { Status = "Blocked", };

		var httpResponseMessage = new HttpResponseMessage
		{
			StatusCode = HttpStatusCode.Locked,
			Content = JsonContent.Create(response),
		};

		return Task.FromResult(httpResponseMessage);
	}
}

//public class CurlDelegatingHandler(IOptions<BenchToolConfig> benchToolConfig) : DelegatingHandler
//{
//	private readonly BenchToolConfig _benchToolConfig = benchToolConfig.Value;

//	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
//	{
//		if (_benchToolConfig.ShouldOutputCurl)
//			request.GenerateCurlInConsole(request.RequestUri,);

//		return base.SendAsync(request, cancellationToken);
//	}
//}