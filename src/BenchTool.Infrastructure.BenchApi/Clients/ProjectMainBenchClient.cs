using BenchTool.Contracts;

namespace BenchTool.Infrastructure.BenchApi.Clients;

internal sealed class ProjectMainBenchClient(HttpClient httpClient)
	: BenchClientBase(httpClient), IProjectMainBenchClient
{
	public Task<Result<ProjectMainResponse, BenchError>> GetAsync(string projectKey, CancellationToken cancellationToken = default)
	{
		return SendAsync<ProjectMainResponse>(
			HttpMethod.Get,
			$"https://2fyge0ilgc.execute-api.us-east-1.amazonaws.com/PRD2/projectMain/{projectKey}",
			"cross-site",
			null,
			cancellationToken);
	}
}
