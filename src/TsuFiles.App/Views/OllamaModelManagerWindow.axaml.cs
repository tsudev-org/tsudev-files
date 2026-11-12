using Avalonia.Controls;
using Avalonia.Interactivity;
using TsuFiles.App.ViewModels;

namespace TsuFiles.App.Views;

public partial class OllamaModelManagerWindow : Window
{
    public OllamaModelManagerWindow()
    {
        InitializeComponent();
        var vm = new OllamaModelManagerViewModel();
        vm.OwnerWindow = this;
        DataContext = vm;
    }

    private void OnClose(object? sender, RoutedEventArgs e) => Close();
}
