using ConsoleApp.POCO;
using ConsoleApp.Startup;
using Microsoft.Extensions.Options;

namespace ConsoleApp.BenchClients;

public class ProjectMainBenchClient(HttpClient httpClient, IOptions<BenchToolConfig> benchToolConfig) 
	: BenchClientBase<ProjectMainResponse>(httpClient, benchToolConfig)
{
	public Task<Result<ProjectMainResponse, BenchError>> SendAsync(string projectKey)
	{
		// Elide the await since we aren't doing any processing on the result before returning it
		return SendAsync(
			HttpMethod.Get, 
			$"https://2fyge0ilgc.execute-api.us-east-1.amazonaws.com/PRD2/projectMain/{projectKey}", 
			"cross-site");
	}
}