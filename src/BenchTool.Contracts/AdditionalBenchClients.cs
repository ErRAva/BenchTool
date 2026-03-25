namespace BenchTool.Contracts;

public interface ISearchBenchClient
{
	Task<Result<SearchResponse, BenchError>> SearchAsync(string startDate, string endDate, string searchString, CancellationToken cancellationToken = default);
}

public interface IProjectMainBenchClient
{
	Task<Result<ProjectMainResponse, BenchError>> GetAsync(string projectKey, CancellationToken cancellationToken = default);
}
