using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogAnalyzer.Core.Models;

namespace LogAnalyzer.Core.Parsers;

/// <summary>
/// Defines the interface for log file parsers.
/// </summary>
public interface ILogParser
{
    /// <summary>
    /// Determines whether this parser can handle the specified file.
    /// </summary>
    /// <param name="filePath">Path to the log file.</param>
    /// <returns>True if the parser can parse the file; otherwise, false.</returns>
    bool CanParse(string filePath);

    /// <summary>
    /// Asynchronously parses a log file and extracts log entries.
    /// </summary>
    /// <param name="filePath">Path to the log file to parse.</param>
    /// <param name="progress">Optional progress reporter for parsing status.</param>
    /// <returns>A task representing the asynchronous operation, containing the list of parsed log entries.</returns>
    /// <exception cref="System.IO.FileNotFoundException">Thrown when the specified file doesn't exist.</exception>
    /// <exception cref="System.UnauthorizedAccessException">Thrown when access to the file is denied.</exception>
    Task<List<LogEntry>> ParseAsync(string filePath, IProgress<int>? progress = null);
}