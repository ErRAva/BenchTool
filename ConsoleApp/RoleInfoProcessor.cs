using ConsoleApp.BenchClients;
using ConsoleApp.POCO;
using ConsoleApp.Utils;

namespace ConsoleApp;

// ToDo: Clean up this class
public class RoleInfoProcessor(ReplyEmailBenchClient replyEmailBenchClient, ProjectExecAssignBenchClient projectExecAssignBenchClient)
{
	public async Task ProcessRole(string roleId)
	{
		await ProcessRoles(roleId.ToEnumerable().ToArray());
	}

	public async Task ProcessRoles(string[] roleIds)
	{
		var rolesData = await roleIds
			.Select(GetRoleDataForId)
			.WhenAll();

		foreach (var roleData in rolesData)
		{
			Console.WriteLine(roleData.ToString());
		}

		await new MyCsvWriter().Write(
			@$"C:\Temp\bench-{DateTime.Now:yyyyMMddHHmmss}.csv",
			rolesData);
	}

	private async Task<BenchCsvData> GetRoleDataForId(string roleId)
	{
		var replyEmailDataTask = replyEmailBenchClient.SendAsync(roleId);
		var projectExecAssignDataTask = projectExecAssignBenchClient.SendAsync(roleId);

		await Task.WhenAll(replyEmailDataTask, projectExecAssignDataTask);

		var replyEmailDetailsResponse = replyEmailDataTask.Result;
		var projectExecAssignResponse = projectExecAssignDataTask.Result;

		if (replyEmailDetailsResponse.IsSuccess && projectExecAssignResponse.IsSuccess)
		{
			var replyEmailDetails = replyEmailDetailsResponse.Value;
			var projectExecAssign = projectExecAssignResponse.Value.value.FirstOrDefault();

			var benchCsvData = new BenchCsvData
			{
				RoleId = roleId,
				Client = replyEmailDetails.ClientName,
				Title = replyEmailDetails.RoleTitle,
				Skills = ParseSkills(projectExecAssign),
				StartDate = replyEmailDetails.RoleStartDate,
				Level = $"{projectExecAssign.LevelFrom.GetLevelName()} - {projectExecAssign.LevelTo.GetLevelName()}",
				Contact1Email = replyEmailDetails.CSDSEmail,
				Contact2Email = replyEmailDetails.TFSEmail,
				Contact3Email = replyEmailDetails.SourcingChannelFulfillmentContactEmail,
				RoleDescription = projectExecAssign.Description,
				AcceptingResumes = projectExecAssign.AcceptingResume
			};

			return benchCsvData;
		}

		// Handle errors as needed, for now we just return an empty object
		var errorThing = CreateErrorThing(replyEmailDetailsResponse, projectExecAssignResponse);
		var errorThing2 = CreateErrorThing2(replyEmailDetailsResponse, projectExecAssignResponse);
		var errorThing3 = CreateErrorThing3(replyEmailDetailsResponse, projectExecAssignResponse);
		return new BenchCsvData
		{
			RoleId = roleId,
			Errors = errorThing
		};
	}

	private static IEnumerable<string> CreateErrorThing(Result<ReplyEmailDetailsResponse, BenchError> replyEmailDetailsResponse, Result<ProjectExecAssignResponse, BenchError> projectExecAssignResponse)
	{
		var benchErrors = replyEmailDetailsResponse.Errors
			.Union(projectExecAssignResponse.Errors);
		var errorThing = benchErrors
			.Select(e => e.ToString() ?? "Unknown Error");
		return errorThing;
	}

	private static IEnumerable<string> CreateErrorThing2(Result<ReplyEmailDetailsResponse, BenchError> replyEmailDetailsResponse, Result<ProjectExecAssignResponse, BenchError> projectExecAssignResponse)
	{
		var joinString = replyEmailDetailsResponse.Errors?.JoinString();
		var s = projectExecAssignResponse.Errors?.JoinString();
		return
		[
			joinString ?? "Error",
			s ?? "Error"
		];
	}

	private static string CreateErrorThing3(Result<ReplyEmailDetailsResponse, BenchError> replyEmailDetailsResponse, Result<ProjectExecAssignResponse, BenchError> projectExecAssignResponse)
	{
		var benchErrors = replyEmailDetailsResponse.Errors
			.Union(projectExecAssignResponse.Errors);
		var errorThing = benchErrors
			.Select(e => e.ToString() ?? "Unknown Error");
		return errorThing.JoinString();
	}

	private static string ParseSkills(Value? projectExecAssignResponse)
	{
		if (projectExecAssignResponse is null)
			return string.Empty;

		var primarySkill = SplitSkillString(projectExecAssignResponse.PrimarySkill);
		var secondarySkill = SplitSkillString(projectExecAssignResponse.SecondarySkill);
		var otherSkills = SplitSkillString(projectExecAssignResponse.OtherSkills);

		var allSkills = primarySkill
			.Union(secondarySkill)
			.Union(otherSkills);

		var allSkillsString = allSkills
			.JoinString();

		return allSkillsString;
	}

	private static List<string> SplitSkillString(string skillString)
	{
		if (string.IsNullOrWhiteSpace(skillString))
			return [];

		return skillString
			.Split('>')
			.Select(s => s.Split("~:")[1])
			.ToList();
	}
}