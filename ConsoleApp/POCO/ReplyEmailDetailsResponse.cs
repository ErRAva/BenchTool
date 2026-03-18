
using CsvHelper.Configuration.Attributes;

namespace ConsoleApp.POCO;

public record ReplyEmailDetailsResponse
{
	[Ignore]
	public string odatametadata { get; set; }

	public string RoleTitle { get; set; }
	public int RoleId { get; set; }

	[Ignore]
	public string ProjectName { get; set; }
	[Ignore]
	public int ProjectKey { get; set; }
	[Ignore]
	public int ProjectLocationKey { get; set; }
	public string ClientName { get; set; }
	public string RoleStartDate { get; set; }
	public string RoleEndDate { get; set; }
	public string CSDSEmail { get; set; }
	public string CSDS { get; set; }
	public string RoleContact { get; set; }
	[Ignore]
	public string EnterpriseId { get; set; }
	[Ignore]
	public int AccessKey { get; set; }
	[Ignore]
	public int ProfileKey { get; set; }
	[Ignore]
	public string ProfileEmail { get; set; }
	[Ignore]
	public string UserFirstName { get; set; }
	public string TFSEmail { get; set; }
	public string SourcingChannelFulfillmentContactFullName { get; set; }
	[Ignore]
	public string SourcingChannelFulfillmentContactFirstName { get; set; }
	public string SourcingChannelFulfillmentContactEmail { get; set; }
	[Ignore]
	public string EmailSubject { get; set; }
	[Ignore]
	public string EmailBody { get; set; }
}