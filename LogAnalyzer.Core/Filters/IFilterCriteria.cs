using System;
using LogAnalyzer.Core.Models;

namespace LogAnalyzer.Core.Filters;

/// <summary>
/// Defines a filter criteria for log entries.
/// </summary>
public interface IFilterCriteria
{
    /// <summary>
    /// Determines whether a log entry matches the filter criteria.
    /// </summary>
    /// <param name="entry">The log entry to check.</param>
    /// <returns>True if the entry matches; otherwise, false.</returns>
    bool Matches(LogEntry entry);
}

/// <summary>
/// Filter criteria based on log level.
/// </summary>
public class LevelFilter : IFilterCriteria
{
    private readonly LogLevel[] _levels;

    /// <summary>
    /// Initializes a new instance of the LevelFilter class.
    /// </summary>
    /// <param name="levels">The log levels to include.</param>
    public LevelFilter(params LogLevel[] levels)
    {
        _levels = levels ?? Array.Empty<LogLevel>();
    }

    /// <inheritdoc/>
    public bool Matches(LogEntry entry)
    {
        return _levels.Contains(entry.Level);
    }
}

/// <summary>
/// Filter criteria based on text search.
/// </summary>
public class TextFilter : IFilterCriteria
{
    private readonly string _searchText;
    private readonly bool _caseSensitive;

    /// <summary>
    /// Initializes a new instance of the TextFilter class.
    /// </summary>
    /// <param name="searchText">The text to search for.</param>
    /// <param name="caseSensitive">Whether the search should be case-sensitive.</param>
    public TextFilter(string searchText, bool caseSensitive = false)
    {
        _searchText = searchText ?? string.Empty;
        _caseSensitive = caseSensitive;
    }

    /// <inheritdoc/>
    public bool Matches(LogEntry entry)
    {
        if (string.IsNullOrEmpty(_searchText))
            return true;

        var comparison = _caseSensitive 
            ? StringComparison.Ordinal 
            : StringComparison.OrdinalIgnoreCase;

        return entry.Message.Contains(_searchText, comparison) ||
               entry.Source.Contains(_searchText, comparison);
    }
}

/// <summary>
/// Filter criteria based on date/time range.
/// </summary>
public class DateRangeFilter : IFilterCriteria
{
    private readonly DateTime? _startDate;
    private readonly DateTime? _endDate;

    /// <summary>
    /// Initializes a new instance of the DateRangeFilter class.
    /// </summary>
    /// <param name="startDate">The start date (inclusive).</param>
    /// <param name="endDate">The end date (inclusive).</param>
    public DateRangeFilter(DateTime? startDate, DateTime? endDate)
    {
        _startDate = startDate;
        _endDate = endDate;
    }

    /// <inheritdoc/>
    public bool Matches(LogEntry entry)
    {
        if (_startDate.HasValue && entry.Timestamp < _startDate.Value)
            return false;

        if (_endDate.HasValue && entry.Timestamp > _endDate.Value)
            return false;

        return true;
    }
}