using System;
using System.Windows;
using LogAnalyzer.UI.ViewModels;

namespace LogAnalyzer.UI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>
    /// Initializes a new instance of the MainWindow class.
    /// </summary>
    public MainWindow()
    {
        try
        {
            InitializeComponent();

            var viewModel = new MainViewModel();
            DataContext = viewModel;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error initializing MainWindow: {ex.Message}\n\n{ex.StackTrace}",
                          "Initialization Error",
                          MessageBoxButton.OK,
                          MessageBoxImage.Error);
            throw;
        }
    }

    /// <summary>
    /// Handles the DragEnter event for file drag-and-drop.
    /// </summary>
    private void Window_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = true;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
    }

    /// <summary>
    /// Handles the Drop event for file drag-and-drop.
    /// </summary>
    private async void Window_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length > 0 && DataContext is MainViewModel viewModel)
            {
                viewModel.IsLoading = true;
                foreach (var file in files)
                {
                    await viewModel.LoadFileAsync(file);
                }
                viewModel.IsLoading = false;
            }
        }
    }
}