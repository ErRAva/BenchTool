using BenchTool.Contracts;
using BenchTool.Infrastructure.BenchApi.Clients;
using BenchTool.Infrastructure.BenchApi.Configuration;
using BenchTool.Infrastructure.BenchApi.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BenchTool.Infrastructure.BenchApi.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddBenchToolBenchApi(this IServiceCollection services, IConfiguration configuration)
	{
		services
			.AddOptions<BenchToolConfig>()
			.Bind(configuration.GetSection(nameof(BenchToolConfig)))
			.ValidateOnStart();

		services.AddSingleton<IValidateOptions<BenchToolConfig>, BenchToolConfigValidator>();

		services.AddTransient<RequestBlockingDelegatingHandler>();

		services
			.AddHttpClient<IProjectExecAssignClient, ProjectExecAssignClient>(BenchHttpClientConfiguration.ConfigureClient)
			.AddHttpMessageHandler<RequestBlockingDelegatingHandler>();

		services
			.AddHttpClient<IReplyEmailDetailsClient, ReplyEmailDetailsClient>(BenchHttpClientConfiguration.ConfigureClient)
			.AddHttpMessageHandler<RequestBlockingDelegatingHandler>();

		services
			.AddHttpClient<ISearchBenchClient, SearchBenchClient>(BenchHttpClientConfiguration.ConfigureClient)
			.AddHttpMessageHandler<RequestBlockingDelegatingHandler>();

		services
			.AddHttpClient<IProjectMainBenchClient, ProjectMainBenchClient>(BenchHttpClientConfiguration.ConfigureClient)
			.AddHttpMessageHandler<RequestBlockingDelegatingHandler>();

		return services;
	}
}

internal static class BenchHttpClientConfiguration
{
	public static void ConfigureClient(IServiceProvider serviceProvider, HttpClient httpClient)
	{
		var opts = serviceProvider.GetRequiredService<IOptions<BenchToolConfig>>().Value;

		httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json, text/plain, */*");
		httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept-language", "en-US,en;q=0.9");
		httpClient.DefaultRequestHeaders.TryAddWithoutValidation("origin", opts.Origin);
		httpClient.DefaultRequestHeaders.TryAddWithoutValidation("priority", "u=1, i");
		httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua", "\"Chromium\";v=\"146\", \"Not-A.Brand\";v=\"24\", \"Google Chrome\";v=\"146\"");
		httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
		httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-ch-ua-platform", "\"Windows\"");
		httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-fetch-dest", "empty");
		httpClient.DefaultRequestHeaders.TryAddWithoutValidation("sec-fetch-mode", "cors");
		httpClient.DefaultRequestHeaders.TryAddWithoutValidation("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/146.0.0.0 Safari/537.36");
		httpClient.DefaultRequestHeaders.TryAddWithoutValidation("authorization", $"Bearer {opts.AuthToken}");
	}
}
