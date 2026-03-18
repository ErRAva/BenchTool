using ConsoleApp.POCO;
using ConsoleApp.Startup;
using Microsoft.Extensions.Options;

namespace ConsoleApp.BenchClients;

public class ProjectExecAssignBenchClient(HttpClient httpClient, IOptions<BenchToolConfig> benchToolConfig) 
	: BenchClientBase<ProjectExecAssignResponse>(httpClient, benchToolConfig)
{
	public Task<Result<ProjectExecAssignResponse, BenchError>> SendAsync(string roleId)
	{
		// Elide the await since we aren't doing any processing on the result before returning it
		return SendAsync(
			HttpMethod.Get, 
			$"https://2fyge0ilgc.execute-api.us-east-1.amazonaws.com/PRD2/projectExecAssign?%24top=1&%24filter=Id%20eq%20{roleId}", 
			"cross-site");
	}
}