namespace ConsoleApp.POCO;

public class Level
{
	public Class1[] Property1 { get; set; }
}

public class Class1
{
	public int LevelKey { get; set; }
	public string LevelCode { get; set; }
	public string LevelName { get; set; }
	public int LevelSequence { get; set; }
	public string LevelGroupKey { get; set; }
	public string LevelGroupName { get; set; }
	public string[] RoleNameSlider { get; set; }
}