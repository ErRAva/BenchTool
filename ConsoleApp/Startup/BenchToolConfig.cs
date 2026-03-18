using System.ComponentModel.DataAnnotations;

namespace ConsoleApp.Startup;

public record BenchToolConfig
{
	[Required(AllowEmptyStrings = false)]
	public required string AuthToken { get; init; }
	[Required(AllowEmptyStrings = false)]
	public required string Origin { get; init; }
	[Required(AllowEmptyStrings = false)]
	public required string UserEnterpriseId { get; init; }
	[Required(AllowEmptyStrings = false)]
	public required bool ShouldBlockTheRequest { get; init; }
	public required bool ShouldOutputCurl { get; init; }
	public long ProfileKey { get; init; }
}