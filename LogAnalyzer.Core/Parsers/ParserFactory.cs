using System;
using System.Collections.Generic;
using System.Linq;

namespace LogAnalyzer.Core.Parsers;

/// <summary>
/// Factory class for creating and managing log parsers.
/// </summary>
public static class ParserFactory
{
    private static readonly List<ILogParser> _parsers = new();

    /// <summary>
    /// Static constructor to register default parsers.
    /// </summary>
    static ParserFactory()
    {
        RegisterParser(new CommonLogParser());
        RegisterParser(new JsonLogParser());
    }

    /// <summary>
    /// Registers a new parser in the factory.
    /// </summary>
    /// <param name="parser">The parser to register.</param>
    /// <exception cref="ArgumentNullException">Thrown when parser is null.</exception>
    public static void RegisterParser(ILogParser parser)
    {
        if (parser == null)
            throw new ArgumentNullException(nameof(parser));

        _parsers.Add(parser);
    }

    /// <summary>
    /// Gets an appropriate parser for the specified file.
    /// </summary>
    /// <param name="filePath">Path to the log file.</param>
    /// <returns>A parser that can handle the file.</returns>
    /// <exception cref="NotSupportedException">Thrown when no suitable parser is found.</exception>
    public static ILogParser GetParser(string filePath)
    {
        var parser = _parsers.FirstOrDefault(p => p.CanParse(filePath));
        
        if (parser == null)
            throw new NotSupportedException($"No parser available for file: {filePath}");

        return parser;
    }

    /// <summary>
    /// Gets all registered parsers.
    /// </summary>
    /// <returns>A list of all registered parsers.</returns>
    public static IReadOnlyList<ILogParser> GetAllParsers()
    {
        return _parsers.AsReadOnly();
    }
}