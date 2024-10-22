using Hackathon;
using Hackathon.DataProviders;
using Microsoft.Extensions.Configuration;

namespace JuniorWebApp.Tests;

public class FileReadingTests
{
    [SetUp]
    public void SetUp()
    {
    }

    [Test]
    public void EmptyFileTest_ShouldThrowArgumentException()
    {
        Assert.Catch<ArgumentException>(() => CsvReader.ReadCsv<Employee>(""));
    }

    [Test]
    public void NoFileTest_ShouldThrowFileNotFoundException()
    {
        Assert.Catch<FileNotFoundException>(() => CsvReader.ReadCsv<Employee>(@".\Resources\sdfdfgfds.csv"));
    }

    [Test]
    public void PartOfFileTest_ShouldThrowIncorrectEmployeesDataException()
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        config["JuniorsPath"] = @".\Resources\PartOfFileJuniors.csv";
        config["TeamLeadsPath"] = @".\Resources\PartOfFileTeamLeads.csv";
        var dataLoader = new CsvDataLoader(config);
        Assert.Catch<IncorrectEmployeesDataException>(() =>
            new Hackathon.Hackathon(20, new TeamBuildingStrategy(), new RandomWishlistGenerator(), dataLoader));
    }

    [Test]
    public void IncorrectIdTest_ShouldThrowFormatException()
    {
        Assert.Catch<FormatException>(() => CsvReader.ReadCsv<Junior>(@".\Resources\EmptyValue2.csv"));
    }

    [Test]
    public void IsValueEmptyTest1_ShouldThrowFormatException()
    {
        Assert.Catch<FormatException>(() => CsvReader.ReadCsv<Junior>(@".\Resources\EmptyValue1.csv"));
    }

    [Test]
    public void IsValueEmptyTest2_ShouldThrowFormatException()
    {
        Assert.Catch<FormatException>(() => CsvReader.ReadCsv<Junior>(@".\Resources\EmptyValue2.csv"));
    }
}