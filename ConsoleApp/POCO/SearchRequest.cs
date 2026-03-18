namespace ConsoleApp.POCO;

public class SearchRequest
{
	public string query { get; set; }
	public int min_score { get; set; }
	public int size { get; set; }
	public int offset { get; set; }
	public Filter[] filters { get; set; }
	public Multipliers multipliers { get; set; }
	public string[] fieldsToReturn { get; set; }
}

public class Multipliers
{
	public Region_Multiplier region_multiplier { get; set; }
}

public class Region_Multiplier
{
	public bool enabled { get; set; }
}

public class Filter
{
	public string name { get; set; }
	public string type { get; set; }
	public string[] selections { get; set; }
	public string from { get; set; }
	public string to { get; set; }
}
