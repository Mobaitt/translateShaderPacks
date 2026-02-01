using System;
using System.IO;
using System.Text.Json;

namespace translateShaderPacks.Services;

public class AppConfig
{
    public string ApiKey { get; set; } = "";
    public string BaseUrl { get; set; } = "https://api.deepseek.com";
    public string ModelName { get; set; } = "deepseek-chat";
    public string SourcePath { get; set; } = "shaders/lang/en_US.lang";

    public string TargetPath { get; set; } = "shaders/lang/zh_CN.lang";

    // 翻译行数
    public int TranslateLines { get; set; } = 10;
}

public static class ConfigService
{
    private static readonly string ConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

    public static AppConfig Load()
    {
        if (!File.Exists(ConfigPath)) return new AppConfig();
        try
        {
            var json = File.ReadAllText(ConfigPath);
            return JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
        }
        catch
        {
            return new AppConfig();
        }
    }

    public static void Save(AppConfig config)
    {
        var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ConfigPath, json);
    }
}