using Microsoft.Extensions.Options;

namespace BenchTool.Infrastructure.BenchApi.Configuration;

internal sealed class BenchToolConfigValidator : IValidateOptions<BenchToolConfig>
{
	public ValidateOptionsResult Validate(string? name, BenchToolConfig options)
	{
		// Allow "blocked" harness runs without secrets.
		if (options.ShouldBlockTheRequest)
			return ValidateOptionsResult.Success;

		var failures = new List<string>();

		if (string.IsNullOrWhiteSpace(options.AuthToken))
			failures.Add($"{nameof(BenchToolConfig.AuthToken)} is required");

		if (string.IsNullOrWhiteSpace(options.Origin))
			failures.Add($"{nameof(BenchToolConfig.Origin)} is required");

		if (string.IsNullOrWhiteSpace(options.UserEnterpriseId))
			failures.Add($"{nameof(BenchToolConfig.UserEnterpriseId)} is required");

		return failures.Count == 0
			? ValidateOptionsResult.Success
			: ValidateOptionsResult.Fail(failures);
	}
}
