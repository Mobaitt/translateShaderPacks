using CommunityToolkit.Mvvm.ComponentModel;

namespace translateShaderPacks.Models;

public partial class LangEntry : ObservableObject
{
    // 键名 (例如: option.VIDEO_SETTINGS)
    public string Key { get; set; } = string.Empty;

    // 原始英文值
    public string EnglishValue { get; set; } = string.Empty;

    // 用户编辑的中文值
    [ObservableProperty] private string _chineseValue = string.Empty;

    // 原始完整行（用于保留注释和空行）
    public string RawLine { get; set; } = string.Empty;

    // 判断是否为注释或空行
    public bool IsTranslatable => !string.IsNullOrWhiteSpace(Key);


    // 是否正在翻译
    [ObservableProperty] private bool _isTranslating  = false;
} 