using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using translateShaderPacks.Context;
using translateShaderPacks.Models;

namespace translateShaderPacks.Services;

public class AiTranslationService
{
    private static readonly HttpClient HttpClient = new();

    public async Task<List<string>?> TranslateBatchAsync(List<string> texts, AppConfig config)
    {
        if (texts.Count == 0) return [];
        if (string.IsNullOrWhiteSpace(config.ApiKey)) return texts;

        // 1. 准备请求数据
        var prompt = "你是一个 Minecraft 光影包专业翻译。请翻译以下词条，严格按行输出翻译结果，不要包含原文和解释：\n" +
                     string.Join("\n", texts);

        var requestData = new ChatRequest(
            config.ModelName,
            [new Message("user", prompt)],
            0.3f
        );

        // 2. 构建请求
        var url = $"{config.BaseUrl.TrimEnd('/')}/v1/chat/completions";
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("Authorization", $"Bearer {config.ApiKey}");
        
        // 使用 AppJsonContext.Default.ChatRequest 避免反射
        request.Content = JsonContent.Create(requestData, AppJsonContext.Default.ChatRequest);

        try
        {
            var response = await HttpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return [];

            // 3. 解析响应 (同样使用 Source Gen)
            var result = await response.Content.ReadFromJsonAsync(AppJsonContext.Default.ChatResponse);
            
            var content = result?.choices.FirstOrDefault()?.message.content;
            return content?.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"AI 翻译出错: {ex.Message}");
            return [];
        }
    }
}