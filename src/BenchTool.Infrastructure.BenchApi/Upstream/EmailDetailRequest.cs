namespace BenchTool.Infrastructure.BenchApi.Upstream;

public class EmailDetailRequest
{
	public Roledetailparameter RoleDetailParameter { get; set; }
}


public class Roledetailparameter
{
	public string RoleKey { get; set; }
	public string UserEnterpriseId { get; set; }
}