using System.Net;
using System.Net.Http.Json;
using System.Text;
using BenchTool.Common;
using BenchTool.Contracts;

namespace BenchTool.Infrastructure.BenchApi.Clients;

internal abstract class BenchClientBase(HttpClient httpClient)
{
	protected async Task<Result<TOutput, BenchError>> SendAsync<TOutput>(HttpMethod httpMethod, string uriString, string secFetchSite, object? contentObject = null, CancellationToken cancellationToken = default)
		where TOutput : class
	{
		var request = CreateBaseRequestMessage(httpMethod, uriString, secFetchSite, contentObject);
		var response = await httpClient.SendAsync(request, cancellationToken);

		if (!response.IsSuccessStatusCode)
		{
			return new BenchError
			{
				Code = response.StatusCode.ToString(),
				Message = "Failed to fetch data"
			};
		}

		var output = await response.Content.ReadFromJsonAsync<TOutput>(cancellationToken);
		if (output is not null)
			return output;

		return new BenchError
		{
			Code = nameof(HttpStatusCode.InternalServerError),
			Message = "Failed parse returned data"
		};
	}

	private static HttpRequestMessage CreateBaseRequestMessage(HttpMethod httpMethod, string uriString, string secFetchSite, object? contentObject)
	{
		var request = new HttpRequestMessage
		{
			Method = httpMethod,
			RequestUri = new Uri(uriString),
			Headers = { { "sec-fetch-site", secFetchSite } },
		};

		if (contentObject is not null)
		{
			var jsonString = contentObject.ToJsonString();
			request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
		}

		return request;
	}
}
