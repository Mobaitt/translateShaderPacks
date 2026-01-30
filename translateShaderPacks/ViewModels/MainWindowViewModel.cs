using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using translateShaderPacks.Models;
using translateShaderPacks.Services;

namespace translateShaderPacks.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty] private string _toastMessage = "";
    [ObservableProperty] private bool _isToastVisible;
    [ObservableProperty] private string _toastBackground = "#2d882d"; // 默认绿色

    [ObservableProperty] private string? _zipPath;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _searchText = string.Empty;
    private AppConfig _config = ConfigService.Load();
    private readonly AiTranslationService _aiService = new();

    // 原始完整数据源
    private List<LangEntry> _fullContent = new();

    // 绑定到 UI ListBox 的集合
    [ObservableProperty] private ObservableCollection<LangEntry> _filteredList = [];

    // 搜索文本改变时，自动触发筛选
    partial void OnSearchTextChanged(string value) => ApplyFilter();


    // 封装一个自动消失的提示方法
    private async Task ShowToast(string message, bool isError = false)
    {
        ToastMessage = message;
        ToastBackground = isError ? "#cc2222" : "#2d882d"; // 错误用红色，成功用绿色
        IsToastVisible = true;

        await Task.Delay(2500); // 停留2.5秒
        IsToastVisible = false;
    }


    [RelayCommand]
    private async Task UploadZipAsync(Window owner)
    {
        var storageProvider = owner.StorageProvider;
        var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "选择着色器包 Zip 文件",
            FileTypeFilter = [new FilePickerFileType("Zip") { Patterns = ["*.zip"] }]
        });

        if (files.Count == 0) return;

        IsBusy = true;
        try
        {
            ZipPath = files[0].Path.LocalPath;

            await Task.Run(() => LoadLangFile(ZipPath, _config.SourcePath));
            // 加载成功弹窗
            _ = ShowToast($"📂 已成功加载 {_fullContent.Count} 条数据"); // 使用 _ = 异步触发不阻塞
        }
        catch (Exception ex)
        {
            _ = ShowToast("❌ 加载失败，请检查路径", true);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void LoadLangFile(string zipPath, string entryPath)
    {
        var tempEntries = new List<LangEntry>();
        using (var archive = ZipFile.OpenRead(zipPath))
        {
            var entry = archive.GetEntry(entryPath);
            if (entry == null) return;

            using var reader = new StreamReader(entry.Open(), Encoding.UTF8);
            while (reader.ReadLine() is { } line)
            {
                // 简单的解析逻辑：包含 = 且不以 # 开头
                if (!line.Contains('=') || line.TrimStart().StartsWith("#")) continue;
                var parts = line.Split('=', 2);
                tempEntries.Add(new LangEntry
                {
                    Key = parts[0].Trim(),
                    EnglishValue = parts[1].Trim(),
                    ChineseValue = parts[1].Trim(), // 初始预填
                    RawLine = line
                });
            }
        }

        _fullContent = tempEntries;
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var query = string.IsNullOrWhiteSpace(SearchText)
            ? _fullContent
            : _fullContent.Where(x => x.IsTranslatable &&
                                      (x.Key.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                       x.EnglishValue.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            FilteredList = new ObservableCollection<LangEntry>(query);
        });
    }

    [RelayCommand]
    private async Task AutoTranslateAsync()
    {
        var targets = _fullContent.Where(x => x.IsTranslatable && x.ChineseValue == x.EnglishValue).ToList();
        if (targets.Count == 0) return;

        IsBusy = true;

        // 每次翻译 10 条，避免超过模型上下文限制
        int batchSize = 10;
        for (int i = 0; i < targets.Count; i += batchSize)
        {
            var currentBatch = targets.Skip(i).Take(batchSize).ToList();
            var results = await _aiService.TranslateBatchAsync(currentBatch.Select(t =>
            {
                t.IsTranslating = true;
                return t.EnglishValue;
            }).ToList(), _config);

            for (int j = 0; j < results.Count && j < currentBatch.Count; j++)
            {
                currentBatch[j].ChineseValue = results[j];
                currentBatch[j].IsTranslating = false;
            }
        }

        IsBusy = false;
    }

    [RelayCommand]
    private async Task SaveZipAsync(Window owner)
    {
        if (string.IsNullOrEmpty(ZipPath)) return;
        IsBusy = true;

        try
        {
            await Task.Run(() =>
            {
                using var archive = ZipFile.Open(ZipPath, ZipArchiveMode.Update);
                var targetPath = _config.TargetPath;
                archive.GetEntry(targetPath)?.Delete();

                var newEntry = archive.CreateEntry(targetPath);
                using var writer = new StreamWriter(newEntry.Open(), new UTF8Encoding(false));
                foreach (var item in _fullContent)
                {
                    writer.WriteLine(item.IsTranslatable ? $"{item.Key}={item.ChineseValue}" : item.RawLine);
                }
            });

            // 保存成功弹窗
        }
        catch (Exception ex)
        {
        }
        finally
        {
            IsBusy = false;
        }
    }


    [RelayCommand]
    private void ClearAll()
    {
        // 清空原始数据和当前过滤后的数据
        _fullContent.Clear();
        FilteredList.Clear();
        ZipPath = string.Empty;
        SearchText = string.Empty;
    }


    [RelayCommand]
    private async Task OpenSettingsAsync(Window owner)
    {
        var dialog = new Views.SettingsWindow(_config); // 传入当前配置
        if (await dialog.ShowDialog<bool>(owner))
        {
            _config = ConfigService.Load(); // 重新加载保存后的配置
        }
    }
}