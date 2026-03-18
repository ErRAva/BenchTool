using System.Net.Http.Headers;
using ConsoleApp.POCO;
using ConsoleApp.Startup;
using Microsoft.Extensions.Options;

namespace ConsoleApp.BenchClients;

public class ReplyEmailBenchClient(HttpClient httpClient, IOptions<BenchToolConfig> benchToolConfig) 
	: BenchClientBase<ReplyEmailDetailsResponse>(httpClient, benchToolConfig)
{
	public Task<Result<ReplyEmailDetailsResponse, BenchError>> SendAsync(string roleId)
	{
		// Elide the await since we aren't doing any processing on the result before returning it
		return SendAsync(
			HttpMethod.Post, 
			"https://myschedulingsvc.accenture.com/services/SendEmails-service/ReplyEmailDetails", 
			"same-site",
			new EmailDetailRequest
			{
				RoleDetailParameter = new Roledetailparameter
				{
					RoleKey = roleId,
					UserEnterpriseId = BenchToolOptions.UserEnterpriseId
				}
			});
	}
}