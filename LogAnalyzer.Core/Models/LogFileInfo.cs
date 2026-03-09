using System;
using System.Collections.Generic;

namespace LogAnalyzer.Core.Models;

/// <summary>
/// Contains metadata and statistics about a processed log file.
/// </summary>
public class LogFileInfo
{
    /// <summary>
    /// Gets or sets the full path to the log file.
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the log file.
    /// </summary>
    public string FileName => System.IO.Path.GetFileName(FilePath);

    /// <summary>
    /// Gets or sets the size of the file in bytes.
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Gets or sets the list of parsed log entries.
    /// </summary>
    public List<LogEntry> Entries { get; set; } = new();

    /// <summary>
    /// Gets or sets the timestamp of the first log entry.
    /// </summary>
    public DateTime FirstEntryTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the last log entry.
    /// </summary>
    public DateTime LastEntryTimestamp { get; set; }

    /// <summary>
    /// Gets or sets the total number of entries in the log file.
    /// </summary>
    public int TotalEntries => Entries.Count;

    /// <summary>
    /// Gets or sets the count of entries by log level.
    /// </summary>
    public Dictionary<LogLevel, int> Statistics { get; set; } = new();

    /// <summary>
    /// Gets or sets the time taken to parse the file in milliseconds.
    /// </summary>
    public long ParseTimeMs { get; set; }

    /// <summary>
    /// Gets or sets any errors that occurred during parsing.
    /// </summary>
    public string? ParseError { get; set; }

    /// <summary>
    /// Indicates whether the file was parsed successfully.
    /// </summary>
    public bool IsValid => string.IsNullOrEmpty(ParseError);
}