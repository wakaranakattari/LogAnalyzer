using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LogAnalyzer.Core.Models;

namespace LogAnalyzer.Core.Parsers;

/// <summary>
/// Parses common log formats like IIS, NLog, and Log4Net.
/// </summary>
public class CommonLogParser : ILogParser
{
    private static readonly Regex LogPattern = new Regex(
        @"^(?<timestamp>\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}(?:\.\d{3})?)\s+\[(?<level>[A-Z]+)\]\s+(?:(?<source>[^\s]+)\s+)?(?<message>.*)$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Dictionary<string, LogLevel> LevelMapping = new(StringComparer.OrdinalIgnoreCase)
    {
        ["TRACE"] = LogLevel.Trace,
        ["DEBUG"] = LogLevel.Debug,
        ["INFO"] = LogLevel.Info,
        ["INFORMATION"] = LogLevel.Info,
        ["WARN"] = LogLevel.Warning,
        ["WARNING"] = LogLevel.Warning,
        ["ERROR"] = LogLevel.Error,
        ["FAIL"] = LogLevel.Error,
        ["CRITICAL"] = LogLevel.Critical,
        ["FATAL"] = LogLevel.Critical
    };

    /// <inheritdoc/>
    public bool CanParse(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        return extension == ".log" || extension == ".txt";
    }

    /// <inheritdoc/>
    public async Task<List<LogEntry>> ParseAsync(string filePath, IProgress<int>? progress = null)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Log file not found: {filePath}");

        var entries = new List<LogEntry>();
        var lines = await File.ReadAllLinesAsync(filePath);
        var totalLines = lines.Length;

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var entry = ParseLine(line, i + 1);
            if (entry != null)
            {
                entries.Add(entry);
            }

            // Report progress every 1000 lines
            if (progress != null && i % 1000 == 0)
            {
                progress.Report((i * 100) / totalLines);
            }
        }

        progress?.Report(100);
        return entries;
    }

    /// <summary>
    /// Parses a single line of log text.
    /// </summary>
    /// <param name="line">The log line to parse.</param>
    /// <param name="lineNumber">The line number in the original file.</param>
    /// <returns>A LogEntry object if parsing succeeds; otherwise, null.</returns>
    private LogEntry? ParseLine(string line, int lineNumber)
    {
        var match = LogPattern.Match(line);
        if (!match.Success)
            return null;

        try
        {
            var timestampStr = match.Groups["timestamp"].Value;
            var levelStr = match.Groups["level"].Value;
            var source = match.Groups["source"].Value;
            var message = match.Groups["message"].Value;

            if (!DateTime.TryParse(timestampStr, out var timestamp))
                return null;

            if (!LevelMapping.TryGetValue(levelStr, out var level))
                level = LogLevel.Info;

            return new LogEntry
            {
                Timestamp = timestamp,
                Level = level,
                Source = source,
                Message = message,
                LineNumber = lineNumber,
                Properties = new Dictionary<string, string>
                {
                    ["RawLine"] = line
                }
            };
        }
        catch
        {
            return null;
        }
    }
}