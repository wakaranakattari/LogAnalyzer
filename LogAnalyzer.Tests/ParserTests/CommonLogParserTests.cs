using System;
using System.IO;
using System.Threading.Tasks;
using LogAnalyzer.Core.Models;
using LogAnalyzer.Core.Parsers;
using Xunit;

namespace LogAnalyzer.Tests.ParserTests;

public class CommonLogParserTests
{
    private readonly CommonLogParser _parser;

    public CommonLogParserTests()
    {
        _parser = new CommonLogParser();
    }

    [Fact]
    public void CanParse_LogFile_ReturnsTrue()
    {
        var result = _parser.CanParse("test.log");
        Assert.True(result);
    }

    [Fact]
    public async Task ParseAsync_ValidLogLine_ParsesCorrectly()
    {
        var tempFile = Path.GetTempFileName();
        var logLine = "2024-01-15 10:30:45 [ERROR] TestSource: Something went wrong";
        await File.WriteAllTextAsync(tempFile, logLine);

        var entries = await _parser.ParseAsync(tempFile);

        Assert.Single(entries);
        var entry = entries[0];
        Assert.Equal(new DateTime(2024, 1, 15, 10, 30, 45), entry.Timestamp);
        Assert.Equal(LogLevel.Error, entry.Level);
        
        File.Delete(tempFile);
    }
}