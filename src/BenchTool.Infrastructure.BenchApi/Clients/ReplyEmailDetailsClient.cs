using BenchTool.Contracts;
using BenchTool.Infrastructure.BenchApi.Configuration;
using BenchTool.Infrastructure.BenchApi.Upstream;
using Microsoft.Extensions.Options;

namespace BenchTool.Infrastructure.BenchApi.Clients;

internal sealed class ReplyEmailDetailsClient(HttpClient httpClient, IOptions<BenchToolConfig> benchToolConfig)
	: BenchClientBase(httpClient), IReplyEmailDetailsClient
{
	private readonly BenchToolConfig _options = benchToolConfig.Value;

	public async Task<Result<ReplyEmailDetails, BenchError>> GetAsync(string roleId, CancellationToken cancellationToken = default)
	{
		var upstream = await SendAsync<ReplyEmailDetailsResponse>(
			HttpMethod.Post,
			"https://myschedulingsvc.accenture.com/services/SendEmails-service/ReplyEmailDetails",
			"same-site",
			new EmailDetailRequest
			{
				RoleDetailParameter = new Roledetailparameter
				{
					RoleKey = roleId,
					UserEnterpriseId = _options.UserEnterpriseId
				}
			},
			cancellationToken);

		if (!upstream.IsSuccess)
			return upstream.Errors!.First();

		var v = upstream.Value!;
		return new ReplyEmailDetails
		{
			ClientName = v.ClientName,
			RoleTitle = v.RoleTitle,
			RoleStartDate = v.RoleStartDate,
			CsdSEmail = v.CSDSEmail,
			TfsEmail = v.TFSEmail,
			SourcingChannelFulfillmentContactEmail = v.SourcingChannelFulfillmentContactEmail,
		};
	}
}
