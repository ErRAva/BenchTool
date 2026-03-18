namespace ConsoleApp.POCO;

public class SearchResponse
{
	public Hits hits { get; set; }
	public bool timed_out { get; set; }
	public int took { get; set; }
}
public class Hits
{
	public Hit[] hits { get; set; }
	public float max_score { get; set; }
	public int total { get; set; }
}

public class Hit
{
	public string _id { get; set; }
	public string _index { get; set; }
	public float _score { get; set; }
	public _Source _source { get; set; }
	public string _type { get; set; }
	public int position { get; set; }
}

public class _Source
{
	public string AFEPath { get; set; }
	public string RoleLocationTypeName { get; set; }
	public string RoleLocationTypePercentName { get; set; }
	public string acceptingResume { get; set; }
	public string client { get; set; }
	public string country { get; set; }
	public string deliveryCenter { get; set; }
	public string demandIndustryPath { get; set; }
	public string expected_languagesMultiplier { get; set; }
	public string id { get; set; }
	public string isGcp { get; set; }
	public string isNationalSecurity { get; set; }
	public bool isReadyToHardLock { get; set; }
	public string keyTermsIDs { get; set; }
	public string languageMultiplier { get; set; }
	public Language[] languages { get; set; }
	public string lcrMultiplier { get; set; }
	public string[] level { get; set; }
	public string locationCoordinates { get; set; }
	public string marketUnit { get; set; }
	public string metroCity { get; set; }
	public string nativeLanguage { get; set; }
	public string projectIndustry { get; set; }
	public string roleEndDate { get; set; }
	public string roleIsSold { get; set; }
	public string roleStartDate { get; set; }
	public string roleTitle { get; set; }
	public string specializationMultiplier { get; set; }
	public int timezone_utc_hrs { get; set; }
	public string timezone_utc_offset { get; set; }
}

public class Language
{
	public string langKey { get; set; }
	public string languageName { get; set; }
	public string languageType { get; set; }
	public string proficiency { get; set; }
	public string proficiencyKey { get; set; }
}