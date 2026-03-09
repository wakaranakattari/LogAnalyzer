using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using LogAnalyzer.Core.Models;
using LogAnalyzer.UI.Converters;

namespace LogAnalyzer.UI.ViewModels;

/// <summary>
/// View model for a log entry, providing UI-specific properties.
/// </summary>
public class LogEntryViewModel : INotifyPropertyChanged
{
    private bool _isSelected;
    private string _displayColor;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogEntryViewModel"/> class.
    /// </summary>
    /// <param name="entry">The original log entry.</param>
    /// <param name="parent">The parent MainViewModel.</param>
    /// <exception cref="ArgumentNullException">Thrown when entry is null.</exception>
    public LogEntryViewModel(LogEntry entry, MainViewModel? parent = null)
    {
        OriginalEntry = entry ?? throw new ArgumentNullException(nameof(entry));
        _displayColor = GetColorForLevel(entry.Level);
        Parent = parent;

        CopyMessageCommand = new RelayCommand(_ => CopyMessage());
        CopyLineCommand = new RelayCommand(_ => CopyLine());
        FilterBySourceCommand = new RelayCommand(_ => FilterBySource());
        FilterByLevelCommand = new RelayCommand(_ => FilterByLevel());
    }

    /// <summary>
    /// Gets the parent MainViewModel.
    /// </summary>
    public MainViewModel? Parent { get; }

    /// <summary>
    /// Gets the original log entry.
    /// </summary>
    public LogEntry OriginalEntry { get; }

    /// <summary>
    /// Gets or sets a value indicating whether this entry is selected.
    /// </summary>
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets the display color for this log entry based on its level.
    /// </summary>
    public string DisplayColor => _displayColor;

    /// <summary>
    /// Gets the log level (needed for binding).
    /// </summary>
    public LogLevel Level => OriginalEntry.Level;

    /// <summary>
    /// Gets the formatted timestamp for display.
    /// </summary>
    public string FormattedTimestamp => OriginalEntry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff");

    /// <summary>
    /// Gets the log level as a string.
    /// </summary>
    public string LevelString => OriginalEntry.Level.ToString();

    /// <summary>
    /// Gets the source component.
    /// </summary>
    public string Source => OriginalEntry.Source;

    /// <summary>
    /// Gets the message content.
    /// </summary>
    public string Message => OriginalEntry.Message;

    /// <summary>
    /// Gets the tooltip text with additional details.
    /// </summary>
    public string Tooltip => $"Line: {OriginalEntry.LineNumber}\n" +
                             $"Thread: {OriginalEntry.ThreadId}\n" +
                             $"Properties: {OriginalEntry.Properties.Count}";

    /// <summary>
    /// Command to copy message to clipboard.
    /// </summary>
    public ICommand CopyMessageCommand { get; }

    /// <summary>
    /// Command to copy full line to clipboard.
    /// </summary>
    public ICommand CopyLineCommand { get; }

    /// <summary>
    /// Command to filter by source.
    /// </summary>
    public ICommand FilterBySourceCommand { get; }

    /// <summary>
    /// Command to filter by level.
    /// </summary>
    public ICommand FilterByLevelCommand { get; }

    private void CopyMessage()
    {
        System.Windows.Clipboard.SetText(Message);
    }

    private void CopyLine()
    {
        System.Windows.Clipboard.SetText($"{FormattedTimestamp} [{LevelString}] {Source}: {Message}");
    }

    private void FilterBySource()
    {
        if (Parent != null)
        {
            Parent.SearchText = $"source:{Source}";
        }
    }

    private void FilterByLevel()
    {
        if (Parent != null)
        {
            Parent.SelectedLogLevel = Level;
        }
    }

    /// <summary>
    /// Determines the display color based on log level.
    /// </summary>
    private static string GetColorForLevel(LogLevel level)
    {
        return level switch
        {
            LogLevel.Trace => "#9E9E9E",
            LogLevel.Debug => "#2196F3",
            LogLevel.Info => "#009688",
            LogLevel.Warning => "#FF9800",
            LogLevel.Error => "#F44336",
            LogLevel.Critical => "#B71C1C",
            _ => "#000000"
        };
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}