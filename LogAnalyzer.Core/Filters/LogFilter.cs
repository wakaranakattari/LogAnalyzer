using System;
using System.Collections.Generic;
using System.Linq;
using LogAnalyzer.Core.Models;

namespace LogAnalyzer.Core.Filters;

/// <summary>
/// Provides filtering functionality for log entries.
/// </summary>
public class LogFilter
{
    private readonly List<IFilterCriteria> _criteria = new();

    /// <summary>
    /// Adds a filter criteria to the filter.
    /// </summary>
    /// <param name="criteria">The criteria to add.</param>
    public void AddCriteria(IFilterCriteria criteria)
    {
        _criteria.Add(criteria ?? throw new ArgumentNullException(nameof(criteria)));
    }

    /// <summary>
    /// Removes all filter criteria.
    /// </summary>
    public void Clear()
    {
        _criteria.Clear();
    }

    /// <summary>
    /// Filters a collection of log entries based on all active criteria.
    /// </summary>
    /// <param name="entries">The entries to filter.</param>
    /// <returns>A filtered collection of log entries.</returns>
    public IEnumerable<LogEntry> Filter(IEnumerable<LogEntry> entries)
    {
        if (entries == null)
            throw new ArgumentNullException(nameof(entries));

        if (!_criteria.Any())
            return entries;

        return entries.Where(entry => _criteria.All(c => c.Matches(entry)));
    }

    /// <summary>
    /// Creates a combined filter that requires all criteria to match.
    /// </summary>
    /// <param name="criteria">The filter criteria to combine.</param>
    /// <returns>A new LogFilter instance with all criteria.</returns>
    public static LogFilter CreateAnd(params IFilterCriteria[] criteria)
    {
        var filter = new LogFilter();
        foreach (var c in criteria)
        {
            filter.AddCriteria(c);
        }
        return filter;
    }
}