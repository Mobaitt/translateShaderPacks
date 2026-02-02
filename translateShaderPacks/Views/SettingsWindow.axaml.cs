using Avalonia.Controls;
using Avalonia.Interactivity;
using translateShaderPacks.ViewModels;

namespace translateShaderPacks.Views;

public partial class SettingsWindow : Window
{
    // 构造函数接收当前的配置对象
    public SettingsWindow()
    {
        InitializeComponent();
    }


    private void OnConfirmClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is SettingsWindowViewModel vm)
        {
            vm.SaveConfig();
        }

        Close(true);
    }
}