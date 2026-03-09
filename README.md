# LogAnalyzer 🔍

A powerful, modern log file analyzer with stack trace detection and smart filtering capabilities.

## ✨ Features

- **Multi-file Support** - Open multiple log files in tabs
- **Smart Filtering** - Filter by log level (Error, Warning, Info, Debug, Trace)
- **Text Search** - Real-time search with highlighting
- **Stack Trace Detection** - Automatically detects GCC, Java, .NET, Python errors
- **Context Menu** - Right-click for quick actions
- **Drag & Drop** - Just drag files into the window
- **Auto-save** - Never lose your filtered view
- **Export** - Save results as CSV, JSON, or plain text
- **Modern UI** - Clean, rounded, professional interface

## 🚀 Quick Start

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download) or later.

### Build and Run

```bash
# Clone or download the project
cd LogAnalyzer

# Build
dotnet build

# Run
dotnet run --project LogAnalyzer.UI

# Publish release version
dotnet publish LogAnalyzer.UI -c Release -o publish
```

### Using the Application
- Click "Open Log File" or drag files into the window
- Use tabs to switch between different log files
- Filter by level using the dropdown
- Search for specific text in the search box
- Right-click on any line for more options
- Export filtered results with "Export Results"

## 📊 Supported Log Formats
- Common Log Format - timestamp [LEVEL] source: message
- JSON Logs - Structured logging (Serilog, NLog, etc.)
- Stack Traces - GCC, Clang, Java, .NET, Python

## 🛠️ Building from Source
```bash
# Windows (x64)
dotnet publish LogAnalyzer.UI -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

#### Published files will be in publish/ directory

## 🏗️ Project Structure
```text
LogAnalyzer/
├── LogAnalyzer.Core/           # Core logic
│   ├── Models/                 # LogEntry, LogLevel, etc.
│   ├── Parsers/                # Log parsers
│   ├── Filters/                # Filtering logic
│   └── Analytics/              # Analysis engine
├── LogAnalyzer.UI/             # WPF UI
│   ├── ViewModels/             # MVVM view models
│   ├── Views/                  # XAML windows
│   └── Converters/             # Value converters
└── LogAnalyzer.Tests/          # Unit tests
```