using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using translateShaderPacks.Services;

namespace translateShaderPacks.Views;

public partial class SettingsWindow : Window
{
    public string SourcePath { get; private set; } = "";
    public string TargetPath { get; private set; } = "";
    public string ApiKey { get; private set; } = "";

    private readonly AppConfig _config;

    // 构造函数接收当前的配置对象
    public SettingsWindow(AppConfig config)
    {
        InitializeComponent();
        _config = config;

        // 初始化时将配置值填入文本框
        this.FindControl<TextBox>("ApiKeyBox")!.Text = _config.ApiKey;
        this.FindControl<TextBox>("BaseUrlBox")!.Text = _config.BaseUrl;
        this.FindControl<TextBox>("ModelBox")!.Text = _config.ModelName;
        this.FindControl<TextBox>("SourceBox")!.Text = _config.SourcePath;
        this.FindControl<TextBox>("TargetBox")!.Text = _config.TargetPath;
    }
    

    private void OnConfirmClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // 1. 从 UI 获取最新值并更新到对象
        _config.ApiKey = this.FindControl<TextBox>("ApiKeyBox")?.Text ?? "";
        _config.BaseUrl = this.FindControl<TextBox>("BaseUrlBox")?.Text ?? "https://api.deepseek.com";
        _config.ModelName = this.FindControl<TextBox>("ModelBox")?.Text ?? "deepseek-chat";
        _config.SourcePath = this.FindControl<TextBox>("SourceBox")?.Text ?? "shaders/lang/en_US.lang";
        _config.TargetPath = this.FindControl<TextBox>("TargetBox")?.Text ?? "shaders/lang/zh_CN.lang";

        // 2. 调用服务保存到 config.json
        ConfigService.Save(_config);

        // 3. 关闭窗口并返回 true，告知主窗口配置已更改
        Close(true);
    }
}