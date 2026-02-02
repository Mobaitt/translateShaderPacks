using CommunityToolkit.Mvvm.ComponentModel;

namespace translateShaderPacks.Models;

public class AppConfig : ObservableObject
{
    public string ApiKey { get; set; } = "";
    public string BaseUrl { get; set; } = "https://api.deepseek.com";
    public string ModelName { get; set; } = "deepseek-chat";
    public string SourcePath { get; set; } = "shaders/lang/en_US.lang";

    public string TargetPath { get; set; } = "shaders/lang/zh_CN.lang";

    // 翻译行数
    public int TranslateLines { get; set; } = 10;
}