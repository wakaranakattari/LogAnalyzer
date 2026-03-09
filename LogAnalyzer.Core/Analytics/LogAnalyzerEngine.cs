using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LogAnalyzer.Core.Models;
using LogAnalyzer.Core.Parsers;

namespace LogAnalyzer.Core.Analytics;

/// <summary>
/// Main engine for log analysis operations.
/// </summary>
public class LogAnalyzerEngine
{
    private readonly List<LogFileInfo> _loadedFiles = new();

    /// <summary>
    /// Gets the list of currently loaded log files.
    /// </summary>
    public IReadOnlyList<LogFileInfo> LoadedFiles => _loadedFiles.AsReadOnly();

    /// <summary>
    /// Occurs when a file parsing operation completes.
    /// </summary>
    public event EventHandler<FileParsedEventArgs>? FileParsed;

    /// <summary>
    /// Occurs when parsing progress is updated.
    /// </summary>
    public event EventHandler<ProgressEventArgs>? ProgressUpdated;

    /// <summary>
    /// Asynchronously loads and parses a log file.
    /// </summary>
    /// <param name="filePath">Path to the log file to load.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task LoadFileAsync(string filePath)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var fileInfo = new LogFileInfo { FilePath = filePath };

        try
        {
            var fileSize = new FileInfo(filePath).Length;
            fileInfo.FileSize = fileSize;

            var parser = ParserFactory.GetParser(filePath);
            
            var progress = new Progress<int>(percent =>
            {
                ProgressUpdated?.Invoke(this, new ProgressEventArgs(filePath, percent));
            });

            var entries = await parser.ParseAsync(filePath, progress);
            
            fileInfo.Entries = entries;
            fileInfo.ParseTimeMs = stopwatch.ElapsedMilliseconds;
            
            if (entries.Any())
            {
                fileInfo.FirstEntryTimestamp = entries.Min(e => e.Timestamp);
                fileInfo.LastEntryTimestamp = entries.Max(e => e.Timestamp);
            }

            // Calculate statistics
            fileInfo.Statistics = entries
                .GroupBy(e => e.Level)
                .ToDictionary(g => g.Key, g => g.Count());

            _loadedFiles.Add(fileInfo);
            
            FileParsed?.Invoke(this, new FileParsedEventArgs(fileInfo, null));
        }
        catch (Exception ex)
        {
            fileInfo.ParseError = ex.Message;
            FileParsed?.Invoke(this, new FileParsedEventArgs(fileInfo, ex));
        }
    }

    /// <summary>
    /// Gets all log entries from all loaded files.
    /// </summary>
    /// <returns>Combined collection of all log entries.</returns>
    public IEnumerable<LogEntry> GetAllEntries()
    {
        return _loadedFiles.SelectMany(f => f.Entries);
    }

    /// <summary>
    /// Clears all loaded files from memory.
    /// </summary>
    public void ClearAll()
    {
        _loadedFiles.Clear();
    }

    /// <summary>
    /// Removes a specific file from memory.
    /// </summary>
    /// <param name="filePath">Path of the file to remove.</param>
    public bool RemoveFile(string filePath)
    {
        var file = _loadedFiles.FirstOrDefault(f => f.FilePath == filePath);
        if (file != null)
        {
            return _loadedFiles.Remove(file);
        }
        return false;
    }
}

/// <summary>
/// Event arguments for file parsed event.
/// </summary>
public class FileParsedEventArgs : EventArgs
{
    public FileParsedEventArgs(LogFileInfo fileInfo, Exception? error)
    {
        FileInfo = fileInfo;
        Error = error;
    }

    public LogFileInfo FileInfo { get; }
    public Exception? Error { get; }
    public bool Success => Error == null;
}

/// <summary>
/// Event arguments for progress update event.
/// </summary>
public class ProgressEventArgs : EventArgs
{
    public ProgressEventArgs(string filePath, int percent)
    {
        FilePath = filePath;
        Percent = percent;
    }

    public string FilePath { get; }
    public int Percent { get; }
}