using BenchTool.Contracts;
using BenchTool.Infrastructure.BenchApi.Upstream;

namespace BenchTool.Infrastructure.BenchApi.Clients;

internal sealed class ProjectExecAssignClient(HttpClient httpClient)
	: BenchClientBase(httpClient), IProjectExecAssignClient
{
	public async Task<Result<ProjectExecAssignDetails, BenchError>> GetAsync(string roleId, CancellationToken cancellationToken = default)
	{
		var upstream = await SendAsync<ProjectExecAssignResponse>(
			HttpMethod.Get,
			$"https://2fyge0ilgc.execute-api.us-east-1.amazonaws.com/PRD2/projectExecAssign?%24top=1&%24filter=Id%20eq%20{roleId}",
			"cross-site",
			null,
			cancellationToken);

		if (!upstream.IsSuccess)
			return upstream.Errors!.First();

		var first = upstream.Value!.value?.FirstOrDefault();
		if (first is null)
		{
			return new BenchError { Code = "NotFound", Message = "No project exec assign value returned" };
		}

		return new ProjectExecAssignDetails
		{
			LevelFrom = first.LevelFrom,
			LevelTo = first.LevelTo,
			PrimarySkill = first.PrimarySkill,
			SecondarySkill = first.SecondarySkill,
			OtherSkills = first.OtherSkills,
			Description = first.Description,
			AcceptingResume = first.AcceptingResume,
		};
	}
}
