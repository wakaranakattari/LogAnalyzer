using System;
using System.Collections.Generic;

namespace LogAnalyzer.Core.Models;

/// <summary>
/// Represents a single log entry parsed from a log file.
/// </summary>
public class LogEntry
{
    /// <summary>
    /// Gets or sets the timestamp when the log entry was created.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the severity level of the log entry.
    /// </summary>
    public LogLevel Level { get; set; }

    /// <summary>
    /// Gets or sets the main message content of the log entry.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the source component or class that generated the log.
    /// </summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the thread ID that produced the log entry.
    /// </summary>
    public int ThreadId { get; set; }

    /// <summary>
    /// Gets or sets additional custom properties associated with the log entry.
    /// </summary>
    public Dictionary<string, string> Properties { get; set; } = new();

    /// <summary>
    /// Gets or sets the line number in the original log file.
    /// </summary>
    public int LineNumber { get; set; }

    /// <summary>
    /// Returns a string representation of the log entry.
    /// </summary>
    /// <returns>Formatted string containing key log entry information.</returns>
    public override string ToString()
    {
        return $"{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Source}: {Message}";
    }
}