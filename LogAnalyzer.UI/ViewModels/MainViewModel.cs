using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using LogAnalyzer.Core.Analytics;
using LogAnalyzer.Core.Filters;
using LogAnalyzer.Core.Models;
using LogAnalyzer.UI.Converters;
using Microsoft.Win32;

namespace LogAnalyzer.UI.ViewModels;

/// <summary>
/// ViewModel for the main window, handling log file loading, filtering, auto-save and display.
/// </summary>
public class MainViewModel : INotifyPropertyChanged
{
    private readonly LogAnalyzerEngine _engine;
    private ObservableCollection<LogEntryViewModel> _filteredEntries;
    private LogFilter _currentFilter;
    private string _searchText = string.Empty;
    private bool _isLoading;
    private LogEntryViewModel? _selectedEntry;
    private LogLevel _selectedLogLevel = LogLevel.Info;
    private DateTime? _selectedDate;
    private string _statusText = "Ready";
    private ObservableCollection<FileTabViewModel> _openFiles = new();
    private FileTabViewModel? _selectedFile;
    private bool _isInternalChange = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainViewModel"/> class.
    /// </summary>
    public MainViewModel()
    {
        _engine = new LogAnalyzerEngine();
        _filteredEntries = new ObservableCollection<LogEntryViewModel>();
        _currentFilter = new LogFilter();

        OpenFileCommand = new RelayCommand(async _ => await OpenFileAsync());
        ClearAllCommand = new RelayCommand(_ => ClearAll(), _ => LoadedFiles.Any());
        ExportCommand = new RelayCommand(async _ => await ExportAsync(), _ => FilteredEntries.Any());
        CloseFileCommand = new RelayCommand(_ => CloseFile(_));

        // Auto-filter when search text, level or date changes
        PropertyChanged += (_, e) =>
        {
            if (_isInternalChange) return;

            if (e.PropertyName == nameof(SearchText) ||
                e.PropertyName == nameof(SelectedLogLevel) ||
                e.PropertyName == nameof(SelectedDate))
            {
                ApplyFilter();
            }
        };

        _engine.FileParsed += OnFileParsed;
        _engine.ProgressUpdated += OnProgressUpdated;
    }

    /// <summary>
    /// Gets the collection of loaded files.
    /// </summary>
    public ObservableCollection<LogFileInfo> LoadedFiles { get; } = new();

    /// <summary>
    /// Gets the collection of open file tabs.
    /// </summary>
    public ObservableCollection<FileTabViewModel> OpenFiles
    {
        get => _openFiles;
        private set
        {
            _openFiles = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the selected file tab.
    /// </summary>
    public FileTabViewModel? SelectedFile
    {
        get => _selectedFile;
        set
        {
            if (_selectedFile != value)
            {
                _selectedFile = value;
                OnPropertyChanged();

                if (value != null)
                {
                    ApplyFilter();
                }
            }
        }
    }

    /// <summary>
    /// Gets the filtered log entries for display.
    /// </summary>
    public ObservableCollection<LogEntryViewModel> FilteredEntries
    {
        get => _filteredEntries;
        private set
        {
            _filteredEntries = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the search text for filtering.
    /// </summary>
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText != value)
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the application is loading.
    /// </summary>
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
            StatusText = value ? "Loading..." : "Ready";
        }
    }

    /// <summary>
    /// Gets or sets the currently selected log entry.
    /// </summary>
    public LogEntryViewModel? SelectedEntry
    {
        get => _selectedEntry;
        set
        {
            _selectedEntry = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the selected log level for filtering.
    /// </summary>
    public LogLevel SelectedLogLevel
    {
        get => _selectedLogLevel;
        set
        {
            if (_selectedLogLevel != value)
            {
                _selectedLogLevel = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected date for filtering.
    /// </summary>
    public DateTime? SelectedDate
    {
        get => _selectedDate;
        set
        {
            if (_selectedDate != value)
            {
                _selectedDate = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the status text.
    /// </summary>
    public string StatusText
    {
        get => _statusText;
        set
        {
            _statusText = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    /// Gets the command to open a log file.
    /// </summary>
    public ICommand OpenFileCommand { get; }

    /// <summary>
    /// Gets the command to clear all loaded files.
    /// </summary>
    public ICommand ClearAllCommand { get; }

    /// <summary>
    /// Gets the command to export filtered results.
    /// </summary>
    public ICommand ExportCommand { get; }

    /// <summary>
    /// Gets the command to close a file tab.
    /// </summary>
    public ICommand CloseFileCommand { get; }

    /// <summary>
    /// Loads a file asynchronously (for drag and drop).
    /// </summary>
    /// <param name="filePath">The file path to load.</param>
    public async Task LoadFileAsync(string filePath)
    {
        await _engine.LoadFileAsync(filePath);
    }

    private async Task OpenFileAsync()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Log files (*.log;*.txt;*.json)|*.log;*.txt;*.json|All files (*.*)|*.*",
            Multiselect = true
        };

        if (dialog.ShowDialog() == true)
        {
            IsLoading = true;
            foreach (var filePath in dialog.FileNames)
            {
                await _engine.LoadFileAsync(filePath);
            }
            IsLoading = false;
        }
    }

    private void OnFileParsed(object? sender, FileParsedEventArgs e)
    {
        App.Current.Dispatcher.Invoke(() =>
        {
            LoadedFiles.Add(e.FileInfo);

            // Create new tab
            var tab = new FileTabViewModel(e.FileInfo);
            tab.Entries = new ObservableCollection<LogEntryViewModel>(
                e.FileInfo.Entries.Select(entry => new LogEntryViewModel(entry, this)));
            OpenFiles.Add(tab);

            // Select first tab if none selected
            if (SelectedFile == null && OpenFiles.Any())
            {
                SelectedFile = OpenFiles.First();
            }

            OnPropertyChanged(nameof(LoadedFiles));
            StatusText = $"Loaded: {e.FileInfo.FileName}";
        });
    }

    private void OnProgressUpdated(object? sender, ProgressEventArgs e)
    {
        App.Current.Dispatcher.Invoke(() =>
        {
            StatusText = $"Parsing {System.IO.Path.GetFileName(e.FilePath)}: {e.Percent}%";
        });
    }

    private void ApplyFilter()
    {
        if (_isInternalChange) return;

        _currentFilter.Clear();

        // Filter by log level
        if (SelectedLogLevel != LogLevel.Trace)
        {
            _currentFilter.AddCriteria(new LevelFilter(SelectedLogLevel));
        }

        // Filter by search text
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            _currentFilter.AddCriteria(new TextFilter(SearchText));
        }

        // Filter by date
        if (SelectedDate.HasValue)
        {
            var startOfDay = SelectedDate.Value.Date;
            var endOfDay = startOfDay.AddDays(1).AddTicks(-1);
            _currentFilter.AddCriteria(new DateRangeFilter(startOfDay, endOfDay));
        }

        // Get entries from selected tab or all files
        IEnumerable<LogEntry> allEntries;
        if (SelectedFile != null)
        {
            allEntries = SelectedFile.FileInfo.Entries;
        }
        else
        {
            allEntries = _engine.GetAllEntries();
        }

        var filtered = _currentFilter.Filter(allEntries);

        FilteredEntries = new ObservableCollection<LogEntryViewModel>(
            filtered.Select(e => new LogEntryViewModel(e, this)));

        StatusText = $"Filtered: {FilteredEntries.Count} entries";

        AutoSave();
    }

    private void CloseFile(object parameter)
    {
        if (parameter is FileTabViewModel tab)
        {
            OpenFiles.Remove(tab);
            _engine.RemoveFile(tab.FileInfo.FilePath);
            LoadedFiles.Remove(tab.FileInfo);

            if (SelectedFile == tab)
            {
                SelectedFile = OpenFiles.FirstOrDefault();
            }

            if (!OpenFiles.Any())
            {
                FilteredEntries.Clear();
            }
        }
    }

    private void ClearAll()
    {
        _engine.ClearAll();
        LoadedFiles.Clear();
        OpenFiles.Clear();
        FilteredEntries.Clear();
        SelectedFile = null;
        OnPropertyChanged(nameof(LoadedFiles));
        StatusText = "Cleared all files";
    }

    private async Task ExportAsync()
    {
        var dialog = new SaveFileDialog
        {
            Filter = "CSV files (*.csv)|*.csv|JSON files (*.json)|*.json|Text files (*.txt)|*.txt",
            DefaultExt = "csv",
            FileName = $"log_export_{DateTime.Now:yyyyMMdd_HHmmss}"
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                var entries = FilteredEntries.Select(e => e.OriginalEntry);
                await ExportToFileAsync(entries, dialog.FileName);
                StatusText = $"Exported {FilteredEntries.Count} entries";

                System.Windows.MessageBox.Show(
                    $"Exported {FilteredEntries.Count} entries.",
                    "Export Complete",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusText = $"Export failed: {ex.Message}";
                System.Windows.MessageBox.Show(
                    $"Error: {ex.Message}",
                    "Export Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
        }
    }

    private async Task ExportToFileAsync(IEnumerable<LogEntry> entries, string filePath)
    {
        var extension = System.IO.Path.GetExtension(filePath).ToLower();

        if (extension == ".csv")
        {
            var lines = new[] { "Timestamp,Level,Source,Message" }
                .Concat(entries.Select(e =>
                    $"\"{e.Timestamp:yyyy-MM-dd HH:mm:ss}\",{e.Level},\"{e.Source}\",\"{e.Message.Replace("\"", "\"\"")}\""));

            await System.IO.File.WriteAllLinesAsync(filePath, lines);
        }
        else if (extension == ".json")
        {
            var json = System.Text.Json.JsonSerializer.Serialize(entries, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });
            await System.IO.File.WriteAllTextAsync(filePath, json);
        }
        else
        {
            var lines = entries.Select(e => e.ToString());
            await System.IO.File.WriteAllLinesAsync(filePath, lines);
        }
    }

    /// <summary>
    /// Auto-save filtered logs to temp file.
    /// </summary>
    private async void AutoSave()
    {
        try
        {
            var tempPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "LogAnalyzer_autosave.json");

            var entries = FilteredEntries.Select(e => e.OriginalEntry);
            var json = System.Text.Json.JsonSerializer.Serialize(entries, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            await System.IO.File.WriteAllTextAsync(tempPath, json);
            StatusText = $"Filtered: {FilteredEntries.Count} entries (Auto-saved)";
        }
        catch
        {
            // Ignore auto-save errors
        }
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}