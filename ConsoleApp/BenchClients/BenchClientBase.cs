using System.Net;
using ConsoleApp.POCO;
using System.Net.Http.Json;
using System.Text;
using ConsoleApp.Startup;
using ConsoleApp.Utils;
using Microsoft.Extensions.Options;

namespace ConsoleApp.BenchClients;

public abstract class BenchClientBase<TOutput>(HttpClient httpClient, IOptions<BenchToolConfig> benchToolConfig) 
	where TOutput : class, new()
{
	protected readonly BenchToolConfig BenchToolOptions = benchToolConfig.Value;

	protected async Task<Result<TOutput, BenchError>> SendAsync(HttpMethod httpMethod, string uriString, string secFetchSite, object? contentObject = null) 
	{
		var request = CreateBaseRequestMessage(httpMethod, uriString, secFetchSite, contentObject);
		var response = await httpClient.SendAsync(request);

		return response.IsSuccessStatusCode
			//? await response.Content.ReadFromJsonAsync<TOutput>() // ToDo: Ugh
			? await ReadFromJsonAsync2(response) // ToDo: Ugh
			: new BenchError
			{
				Code = response.StatusCode.ToString(),
				Message = "Failed to fetch data"
			};
	}

	private static async Task<Result<TOutput, BenchError>> ReadFromJsonAsync2(HttpResponseMessage response)
	{
		string code;
		string failedToFetchData;

		if (response.IsSuccessStatusCode)
		{
			var output = await response.Content.ReadFromJsonAsync<TOutput>();
			if (output != null)
				return output;

			code = nameof(HttpStatusCode.InternalServerError);
			failedToFetchData = "Failed parse returned data";
		}
		else
		{
			code = response.StatusCode.ToString();
			failedToFetchData = "Failed to fetch data";
		}

		return new BenchError
		{
			Code = code,
			Message = failedToFetchData
		};
	}

	private static HttpRequestMessage CreateBaseRequestMessage<T>(HttpMethod httpMethod, string uriString, string secFetchSite, T? contentObject = default)
	{
		var baseRequestMessage = CreateBaseRequestMessage(httpMethod, uriString, secFetchSite);

		if (contentObject is not null)
		{
			baseRequestMessage.Content = CreateContent<T>(contentObject);
		}

		return baseRequestMessage;
	}

	private static HttpRequestMessage CreateBaseRequestMessage(HttpMethod httpMethod, string uriString, string secFetchSite)
	{
		return new HttpRequestMessage
		{
			Method = httpMethod,
			RequestUri = new Uri(uriString),
			Headers = { { "sec-fetch-site", secFetchSite } },
		};
	}

	private static StringContent CreateContent<T>(T contentObject)
	{
		var jsonString = contentObject.ToJsonString();
		var stringContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
		return stringContent;
	}
}