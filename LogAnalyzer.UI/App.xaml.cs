using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace LogAnalyzer.UI;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Initializes a new instance of the App class and sets up global exception handling.
    /// </summary>
    public App()
    {
        AppDomain.CurrentDomain.UnhandledException += OnCurrentDomainUnhandledException;
        DispatcherUnhandledException += OnDispatcherUnhandledException;
    }

    /// <summary>
    /// Handles unhandled exceptions from the current application domain.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event data containing the exception information.</param>
    private void OnCurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception;
        ShowFatalError("Unhandled Exception", exception);
    }

    /// <summary>
    /// Handles unhandled exceptions from the WPF dispatcher.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">Event data containing the exception information.</param>
    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        ShowFatalError("Dispatcher Exception", e.Exception);
        e.Handled = true;
    }

    /// <summary>
    /// Displays a fatal error message to the user and logs it to a file.
    /// </summary>
    /// <param name="title">The title of the error.</param>
    /// <param name="ex">The exception that occurred, if any.</param>
    private void ShowFatalError(string title, Exception? ex)
    {
        var errorMessage = $"Critical Error: {title}\n\n";
        
        if (ex != null)
        {
            errorMessage += $"Message: {ex.Message}\n";
            errorMessage += $"Stack Trace: {ex.StackTrace}\n";
            
            if (ex.InnerException != null)
            {
                errorMessage += $"\nInner Exception: {ex.InnerException.Message}";
            }
        }

        try
        {
            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error_log.txt");
            File.AppendAllText(logPath, $"{DateTime.Now}: {errorMessage}\n\n");
        }
        catch { }

        MessageBox.Show(errorMessage, "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    /// <summary>
    /// Raises the Startup event and initializes the main window.
    /// </summary>
    /// <param name="e">Event data for the Startup event.</param>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        try
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
        catch (Exception ex)
        {
            ShowFatalError("Startup Failed", ex);
            Shutdown();
        }
    }
}