using BenchTool.Contracts;

namespace BenchTool.Domain;

public interface IRoleInfoService
{
	Task<BenchCsvData> GetRoleInfoAsync(string roleId, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<BenchCsvData>> GetRoleInfosAsync(IEnumerable<string> roleIds, CancellationToken cancellationToken = default);
}
