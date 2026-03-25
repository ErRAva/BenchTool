namespace BenchTool.Contracts;

public record ReplyEmailDetails
{
	public required string ClientName { get; init; }
	public required string RoleTitle { get; init; }
	public required string RoleStartDate { get; init; }
	public required string CsdSEmail { get; init; }
	public required string TfsEmail { get; init; }
	public required string SourcingChannelFulfillmentContactEmail { get; init; }
}

public record ProjectExecAssignDetails
{
	public required string LevelFrom { get; init; }
	public required string LevelTo { get; init; }
	public required string PrimarySkill { get; init; }
	public required string SecondarySkill { get; init; }
	public required string OtherSkills { get; init; }
	public required string Description { get; init; }
	public required string AcceptingResume { get; init; }
}

public interface IReplyEmailDetailsClient
{
	Task<Result<ReplyEmailDetails, BenchError>> GetAsync(string roleId, CancellationToken cancellationToken = default);
}

public interface IProjectExecAssignClient
{
	Task<Result<ProjectExecAssignDetails, BenchError>> GetAsync(string roleId, CancellationToken cancellationToken = default);
}
