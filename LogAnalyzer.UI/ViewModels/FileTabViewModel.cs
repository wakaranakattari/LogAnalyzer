using System;
using System.Collections.ObjectModel;
using LogAnalyzer.Core.Models;

namespace LogAnalyzer.UI.ViewModels;

/// <summary>
/// ViewModel for a file tab.
/// </summary>
public class FileTabViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileTabViewModel"/> class.
    /// </summary>
    /// <param name="fileInfo">The log file information.</param>
    public FileTabViewModel(LogFileInfo fileInfo)
    {
        FileInfo = fileInfo ?? throw new ArgumentNullException(nameof(fileInfo));
        FileName = System.IO.Path.GetFileName(fileInfo.FilePath);
    }

    /// <summary>
    /// Gets the log file information.
    /// </summary>
    public LogFileInfo FileInfo { get; }

    /// <summary>
    /// Gets the file name for display.
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// Gets or sets the entries for this tab.
    /// </summary>
    public ObservableCollection<LogEntryViewModel> Entries { get; set; } = new();
}