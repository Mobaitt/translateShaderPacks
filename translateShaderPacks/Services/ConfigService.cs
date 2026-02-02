using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using translateShaderPacks.Models;

namespace translateShaderPacks.Services;

[JsonSerializable(typeof(AppConfig))]
public partial class AppConfigContext : JsonSerializerContext
{
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
            return JsonSerializer.Deserialize<AppConfig>(json, AppConfigContext.Default.AppConfig) ?? new AppConfig();
        }
        catch
        {
            return new AppConfig();
        }
    }

    public static void Save(AppConfig config)
    {
        var json = JsonSerializer.Serialize(config, AppConfigContext.Default.AppConfig);
        File.WriteAllText(ConfigPath, json);
    }
}