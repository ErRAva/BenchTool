using System.Text.Json;

namespace BenchTool.Common;

public static class Extensions
{
	public static async Task<T[]> WhenAll<T>(this IEnumerable<Task<T>> tasks)
		=> await Task.WhenAll(tasks);

	public static string GetLevelName(this string level)
	{
		if (string.IsNullOrWhiteSpace(level))
			return string.Empty;

		return level switch
		{
			"Accenture Leadership" => "Accenture Leadership",
			"5" => "Associate Director",
			"6" => "Senior Manager",
			"7" => "Manager",
			"8" => "Associate Manager",
			"9" => "Team Lead/Consultant",
			"10" => "Senior Analyst",
			"11" => "Analyst",
			"12" => "Associate",
			"13" => "New Associate",
			_ => string.Empty
		};
	}

	public static string ToJsonString<T>(this T value)
		=> JsonSerializer.Serialize(value);

	public static string JoinString<T>(this IEnumerable<T?>? values, string separator = ", ")
		=> values is null ? string.Empty : string.Join(separator, values);

	public static IEnumerable<T> ToEnumerable<T>(this T? value) where T : class
		=> value is not null ? [value] : [];
}
