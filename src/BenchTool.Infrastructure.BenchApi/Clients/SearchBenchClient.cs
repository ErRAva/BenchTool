using BenchTool.Contracts;

namespace BenchTool.Infrastructure.BenchApi.Clients;

internal sealed class SearchBenchClient(HttpClient httpClient)
	: BenchClientBase(httpClient), ISearchBenchClient
{
	public Task<Result<SearchResponse, BenchError>> SearchAsync(string startDate, string endDate, string searchString, CancellationToken cancellationToken = default)
	{
		return SendAsync<SearchResponse>(
			HttpMethod.Post,
			"https://mysched-searchandmatchsvc.accenture.com/api/search/",
			"same-site",
			CreateContent(startDate, endDate, searchString),
			cancellationToken);
	}

	private static SearchRequest CreateContent(string startDate, string endDate, string searchString)
	{
		return new SearchRequest
		{
			query = searchString,
			min_score = 0,
			size = 100,
			offset = 0,
			filters =
			[
				new Filter
				{
					name = "demandStatus",
					type = "categorical",
					selections =
					[
						"Open - New",
						"Open - In Process",
						"Open - Need Project Feedback",
						"Open - Confirming Candidate"
					]
				},
				new Filter
				{
					name = "country",
					type = "categorical",
					selections = ["USA"]
				},
				new Filter
				{
					name = "level",
					type = "categorical",
					selections =
					[
						"10000090",
						"10000080",
						"10000070"
					]
				},
				new Filter
				{
					name = "roleStartDate",
					type = "date",
					from = startDate,
					to = endDate
				}
			],
			multipliers = new Multipliers
			{
				region_multiplier = new Region_Multiplier { enabled = true }
			},
			fieldsToReturn =
			[
				"AFEPath",
				"isAva",
				"isGcp",
				"isNationalSecurity",
				"deliveryCenter",
				"id",
				"roleTitle",
				"roleStartDate",
				"roleEndDate",
				"metroCity",
				"projectIndustry",
				"keyTermsIDs",
				"nativeLanguage",
				"locationCoordinates",
				"region",
				"specializationMultiplier",
				"level",
				"client",
				"demandIndustryPath",
				"profileIndustryPath",
				"aspirationalSpecializationBonus",
				"buildingSpecializationBonus",
				"level",
				"geographicUnit",
				"marketUnit",
				"country",
				"lcrMultiplier",
				"roleIsSold",
				"isReadyToHardLock",
				"languages",
				"RoleLocationTypeName",
				"RoleLocationTypePercentName",
				"languageMultiplier",
				"acceptingResume",
				"timezone_utc_hrs",
				"timezone_utc_offset",
				"expected_languagesMultiplier"
			]
		};
	}
}
