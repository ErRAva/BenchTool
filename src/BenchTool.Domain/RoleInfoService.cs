using BenchTool.Common;
using BenchTool.Contracts;

namespace BenchTool.Domain;

public sealed class RoleInfoService(
	IReplyEmailDetailsClient replyEmailDetailsClient,
	IProjectExecAssignClient projectExecAssignClient)
	: IRoleInfoService
{
	public async Task<BenchCsvData> GetRoleInfoAsync(string roleId, CancellationToken cancellationToken = default)
	{
		var replyEmailTask = replyEmailDetailsClient.GetAsync(roleId, cancellationToken);
		var execAssignTask = projectExecAssignClient.GetAsync(roleId, cancellationToken);

		await Task.WhenAll(replyEmailTask, execAssignTask);

		var replyEmail = await replyEmailTask;
		var execAssign = await execAssignTask;

		if (replyEmail.IsSuccess && execAssign.IsSuccess)
		{
			return new BenchCsvData
			{
				RoleId = roleId,
				Client = replyEmail.Value.ClientName,
				Title = replyEmail.Value.RoleTitle,
				Skills = ParseSkills(execAssign.Value),
				StartDate = replyEmail.Value.RoleStartDate,
				Level = $"{execAssign.Value.LevelFrom.GetLevelName()} - {execAssign.Value.LevelTo.GetLevelName()}",
				Contact1Email = replyEmail.Value.CsdSEmail,
				Contact2Email = replyEmail.Value.TfsEmail,
				Contact3Email = replyEmail.Value.SourcingChannelFulfillmentContactEmail,
				RoleDescription = execAssign.Value.Description,
				AcceptingResumes = execAssign.Value.AcceptingResume,
			};
		}

		var errors = (replyEmail.Errors ?? []).Select(e => e.ToString() ?? "Error")
			.Union((execAssign.Errors ?? []).Select(e => e.ToString() ?? "Error"))
			.ToArray();

		return new BenchCsvData
		{
			RoleId = roleId,
			Errors = errors.Length == 0 ? ["Unknown error"] : errors
		};
	}

	public async Task<IReadOnlyList<BenchCsvData>> GetRoleInfosAsync(IEnumerable<string> roleIds, CancellationToken cancellationToken = default)
	{
		var results = await roleIds
			.Select(r => GetRoleInfoAsync(r, cancellationToken))
			.WhenAll();

		return results;
	}

	private static string ParseSkills(ProjectExecAssignDetails projectExecAssign)
	{
		var primarySkill = SplitSkillString(projectExecAssign.PrimarySkill);
		var secondarySkill = SplitSkillString(projectExecAssign.SecondarySkill);
		var otherSkills = SplitSkillString(projectExecAssign.OtherSkills);

		return primarySkill
			.Union(secondarySkill)
			.Union(otherSkills)
			.JoinString();
	}

	private static List<string> SplitSkillString(string skillString)
	{
		if (string.IsNullOrWhiteSpace(skillString))
			return [];

		// Input looks like: "...>Something~:SkillName>Something~:OtherSkill"
		return skillString
			.Split('>')
			.Select(s => s.Split("~:")
				.ElementAtOrDefault(1) ?? string.Empty)
			.Where(s => !string.IsNullOrWhiteSpace(s))
			.ToList();
	}
}
