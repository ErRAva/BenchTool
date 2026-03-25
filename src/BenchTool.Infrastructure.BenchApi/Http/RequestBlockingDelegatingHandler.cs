using System.Net;
using System.Net.Http.Json;
using BenchTool.Infrastructure.BenchApi.Configuration;
using Microsoft.Extensions.Options;

namespace BenchTool.Infrastructure.BenchApi.Http;

public sealed class RequestBlockingDelegatingHandler(IOptions<BenchToolConfig> benchToolConfig) : DelegatingHandler
{
	private readonly BenchToolConfig _benchToolConfig = benchToolConfig.Value;

	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		if (!_benchToolConfig.ShouldBlockTheRequest)
			return base.SendAsync(request, cancellationToken);

		var response = new { Status = "Blocked" };

		var httpResponseMessage = new HttpResponseMessage
		{
			StatusCode = HttpStatusCode.Locked,
			Content = JsonContent.Create(response),
		};

		return Task.FromResult(httpResponseMessage);
	}
}
