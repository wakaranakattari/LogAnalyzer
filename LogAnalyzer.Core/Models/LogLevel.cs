namespace LogAnalyzer.Core.Models;

/// <summary>
/// Represents the severity level of a log entry.
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// Very detailed diagnostic information.
    /// </summary>
    Trace,
    
    /// <summary>
    /// Debugging information useful during development.
    /// </summary>
    Debug,
    
    /// <summary>
    /// Informational messages about normal operations.
    /// </summary>
    Info,
    
    /// <summary>
    /// Warning messages about potential issues.
    /// </summary>
    Warning,
    
    /// <summary>
    /// Error messages about failures that don't stop the application.
    /// </summary>
    Error,
    
    /// <summary>
    /// Critical errors that may cause application termination.
    /// </summary>
    Critical
}