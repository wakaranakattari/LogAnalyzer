using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using LogAnalyzer.Core.Models;

namespace LogAnalyzer.Core.Parsers;

/// <summary>
/// Parses JSON-formatted log files.
/// </summary>
public class JsonLogParser : ILogParser
{
    /// <inheritdoc/>
    public bool CanParse(string filePath)
    {
        return filePath.EndsWith(".json", StringComparison.OrdinalIgnoreCase);
    }

    /// <inheritdoc/>
    public async Task<List<LogEntry>> ParseAsync(string filePath, IProgress<int>? progress = null)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Log file not found: {filePath}");

        var entries = new List<LogEntry>();
        var jsonContent = await File.ReadAllTextAsync(filePath);

        progress?.Report(50);

        try
        {
            using var document = JsonDocument.Parse(jsonContent);
            var root = document.RootElement;

            // Handle both single object and array of objects
            if (root.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in root.EnumerateArray())
                {
                    var entry = ParseJsonEntry(item);
                    if (entry != null)
                        entries.Add(entry);
                }
            }
            else if (root.ValueKind == JsonValueKind.Object)
            {
                var entry = ParseJsonEntry(root);
                if (entry != null)
                    entries.Add(entry);
            }
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException("Failed to parse JSON log file", ex);
        }

        progress?.Report(100);
        return entries;
    }

    /// <summary>
    /// Parses a JSON element into a LogEntry object.
    /// </summary>
    /// <param name="element">The JSON element to parse.</param>
    /// <returns>A LogEntry object if parsing succeeds; otherwise, null.</returns>
    private LogEntry? ParseJsonEntry(JsonElement element)
    {
        try
        {
            var entry = new LogEntry();

            // Try to extract common fields
            if (element.TryGetProperty("@t", out var timestampProp) ||
                element.TryGetProperty("timestamp", out timestampProp) ||
                element.TryGetProperty("time", out timestampProp))
            {
                if (DateTime.TryParse(timestampProp.GetString(), out var timestamp))
                    entry.Timestamp = timestamp;
            }

            if (element.TryGetProperty("@l", out var levelProp) ||
                element.TryGetProperty("level", out levelProp) ||
                element.TryGetProperty("logLevel", out levelProp))
            {
                entry.Level = ParseLogLevel(levelProp.GetString() ?? "Info");
            }

            if (element.TryGetProperty("@m", out var messageProp) ||
                element.TryGetProperty("message", out messageProp) ||
                element.TryGetProperty("msg", out messageProp))
            {
                entry.Message = messageProp.GetString() ?? string.Empty;
            }

            if (element.TryGetProperty("@s", out var sourceProp) ||
                element.TryGetProperty("source", out sourceProp) ||
                element.TryGetProperty("logger", out sourceProp))
            {
                entry.Source = sourceProp.GetString() ?? string.Empty;
            }

            // Store all properties
            foreach (var prop in element.EnumerateObject())
            {
                entry.Properties[prop.Name] = prop.Value.ToString();
            }

            return entry;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Converts a string log level to the corresponding enum value.
    /// </summary>
    /// <param name="levelStr">The string representation of the log level.</param>
    /// <returns>The corresponding LogLevel enum value.</returns>
    private LogLevel ParseLogLevel(string levelStr)
    {
        return levelStr.ToLowerInvariant() switch
        {
            "trace" => LogLevel.Trace,
            "debug" => LogLevel.Debug,
            "info" or "information" => LogLevel.Info,
            "warn" or "warning" => LogLevel.Warning,
            "error" => LogLevel.Error,
            "critical" or "fatal" => LogLevel.Critical,
            _ => LogLevel.Info
        };
    }
}