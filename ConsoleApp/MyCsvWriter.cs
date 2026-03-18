using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace ConsoleApp;

public interface IOutput
{
	Task Write<T>(string pathToFileCsv, ICollection<T> records);
}

public class MyCsvWriter : IOutput
{
	public async Task Write<T>(string pathToFileCsv, ICollection<T> records)
	{
		var writeDetails = ConfigureWriteDetails(pathToFileCsv);

		await using var stream = File.Open(pathToFileCsv, writeDetails.FileMode);
		await using var writer = new StreamWriter(stream);
		await using var csv = new CsvWriter(writer, writeDetails.WriterConfiguration);
		csv.WriteHeader<T>();
		await csv.NextRecordAsync();

		await csv.WriteRecordsAsync(records);
	}

	private static WriterDetails ConfigureWriteDetails(string pathToFileCsv)
	{
		var exists = File.Exists(pathToFileCsv);

		return new WriterDetails
		{
			FileMode = exists
				? FileMode.Append
				: FileMode.Create,
			WriterConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				NewLine = Environment.NewLine,
				HasHeaderRecord = !exists,
			}
		};
	}

	private class WriterDetails
	{
		public FileMode FileMode { get; init; }
		public required CsvConfiguration WriterConfiguration { get; init; }
	}
}

//public sealed class RoleDataMap : ClassMap<ReplyEmailDetailsResponse>
//{
//	public RoleDataMap()
//	{
//		Map(m => m.RoleId).Index(0).Name("RoleId");
//		Map(m => m.RoleTitle).Index(1).Name("RoleTitle");
//		Map(m => m.RoleStartDate).Index(2).Name("RoleStartDate");
//		Map(m => m.CSDSEmail).Index(3).Name("CSDSEmail");
//		Map(m => m.RoleContact).Index(4).Name("RoleContact");
//		Map(m => m.ClientName).Index(5).Name("ClientName");
//	}
//}

//public sealed class BenchDataMap : ClassMap<BenchCsvData>
//{
//	public BenchDataMap()
//	{
//		Map(m => m.RoleId).Index(0).Name("RoleId");
//		Map(m => m.Client).Index(1).Name("Client");
//		Map(m => m.Title).Index(2).Name("Title");
//		Map(m => m.Skills).Index(3).Name("Skills");
//		Map(m => m.StartDate).Index(4).Name("StartDate");
//		Map(m => m.Level).Index(5).Name("Level");
//		Map(m => m.Contact1Email).Index(6).Name("Contact1Email");
//		Map(m => m.Contact2Email).Index(7).Name("Contact2Email");
//		Map(m => m.Contact3Email).Index(8).Name("Contact3Email");
//		Map(m => m.RoleDescription).Index(10).Name("RoleDescription");
//	}
//}