using System;
using System.Collections.Generic;
using System.Linq;
using LogAnalyzer.Core.Models;

namespace LogAnalyzer.Core.Analytics;

/// <summary>
/// Provides comprehensive statistical analysis for log entries.
/// </summary>
public static class Statistics
{
    /// <summary>
    /// Calculates error frequency over specified time intervals.
    /// </summary>
    /// <param name="entries">The collection of log entries to analyze.</param>
    /// <param name="intervalMinutes">Size of each time interval in minutes. Default is 60.</param>
    /// <returns>A dictionary mapping time intervals to error counts.</returns>
    /// <exception cref="ArgumentNullException">Thrown when entries is null.</exception>
    public static Dictionary<DateTime, int> GetErrorFrequency(
        IEnumerable<LogEntry> entries, 
        int intervalMinutes = 60)
    {
        if (entries == null)
            throw new ArgumentNullException(nameof(entries));

        var errors = entries.Where(e => e.Level == LogLevel.Error || e.Level == LogLevel.Critical);
        
        return errors
            .GroupBy(e => new DateTime(
                e.Timestamp.Year,
                e.Timestamp.Month,
                e.Timestamp.Day,
                e.Timestamp.Hour,
                (e.Timestamp.Minute / intervalMinutes) * intervalMinutes,
                0))
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    /// <summary>
    /// Gets the most frequent error messages in the log entries.
    /// </summary>
    /// <param name="entries">The collection of log entries to analyze.</param>
    /// <param name="topCount">Number of top messages to return. Default is 10.</param>
    /// <returns>A list of tuples containing the error message and its occurrence count.</returns>
    /// <exception cref="ArgumentNullException">Thrown when entries is null.</exception>
    public static List<(string Message, int Count)> GetTopErrors(
        IEnumerable<LogEntry> entries, 
        int topCount = 10)
    {
        if (entries == null)
            throw new ArgumentNullException(nameof(entries));

        return entries
            .Where(e => e.Level == LogLevel.Error || e.Level == LogLevel.Critical)
            .GroupBy(e => e.Message)
            .Select(g => (Message: g.Key, Count: g.Count()))
            .Where(x => !string.IsNullOrWhiteSpace(x.Message))
            .OrderByDescending(x => x.Count)
            .Take(topCount)
            .ToList();
    }

    /// <summary>
    /// Gets error statistics grouped by source component.
    /// </summary>
    /// <param name="entries">The collection of log entries to analyze.</param>
    /// <returns>A dictionary mapping source names to error counts.</returns>
    /// <exception cref="ArgumentNullException">Thrown when entries is null.</exception>
    public static Dictionary<string, int> GetErrorsBySource(IEnumerable<LogEntry> entries)
    {
        if (entries == null)
            throw new ArgumentNullException(nameof(entries));

        return entries
            .Where(e => e.Level == LogLevel.Error || e.Level == LogLevel.Critical)
            .GroupBy(e => e.Source)
            .Where(g => !string.IsNullOrEmpty(g.Key))
            .OrderByDescending(g => g.Count())
            .ToDictionary(g => g.Key, g => g.Count());
    }

    /// <summary>
    /// Gets the time range of the log entries.
    /// </summary>
    /// <param name="entries">The collection of log entries to analyze.</param>
    /// <returns>A tuple containing the earliest and latest timestamps.</returns>
    /// <exception cref="ArgumentNullException">Thrown when entries is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when entries collection is empty.</exception>
    public static (DateTime Start, DateTime End) GetTimeRange(IEnumerable<LogEntry> entries)
    {
        if (entries == null)
            throw new ArgumentNullException(nameof(entries));

        var entryList = entries.ToList();
        if (!entryList.Any())
            throw new InvalidOperationException("Cannot get time range from empty collection");

        return (entryList.Min(e => e.Timestamp), entryList.Max(e => e.Timestamp));
    }

    /// <summary>
    /// Gets summary statistics for all log levels.
    /// </summary>
    /// <param name="entries">The collection of log entries to analyze.</param>
    /// <returns>A dictionary mapping each log level to its count.</returns>
    /// <exception cref="ArgumentNullException">Thrown when entries is null.</exception>
    public static Dictionary<LogLevel, int> GetLevelSummary(IEnumerable<LogEntry> entries)
    {
        if (entries == null)
            throw new ArgumentNullException(nameof(entries));

        return Enum.GetValues<LogLevel>()
            .ToDictionary(
                level => level,
                level => entries.Count(e => e.Level == level));
    }
}