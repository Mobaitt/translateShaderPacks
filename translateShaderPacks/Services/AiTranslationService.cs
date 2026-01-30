using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using OpenAI;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;

namespace translateShaderPacks.Services;

public class AiTranslationService
{
    public async Task<List<string>?> TranslateBatchAsync(List<string> texts, AppConfig config)
    {
        if (texts.Count == 0) return new();
        if (string.IsNullOrWhiteSpace(config.ApiKey)) return texts;

        // 1. 初始化 SDK (适配自定义 BaseUrl)
        var sdk = new OpenAIService(new OpenAiOptions()
        {
            ApiKey = config.ApiKey,
            BaseDomain = config.BaseUrl.TrimEnd('/') // 这里的 BaseDomain 会自动补全 /v1/chat/completions
        });

        // 2. 构建 Prompt
        var prompt = "你是一个 Minecraft 光影包专业翻译。请翻译以下词条，严格按行输出翻译结果，不要包含原文和解释：\n" +
                     string.Join("\n", texts);

        // 3. 调用成熟的请求模型
        var completionResult = await sdk.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
        {
            Messages = new List<ChatMessage>
            {
                ChatMessage.FromUser(prompt)
            },
            Model = config.ModelName,
            Temperature = 0.3f,
            MaxTokens = 1000 // 批量翻译时建议设置大一点
        });

        // 4. 处理响应
        if (completionResult.Successful)
        {
            var content = completionResult.Choices.First().Message.Content;
            return content?.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToList();
        }
        else
        {
            // SDK 提供了非常详细的错误信息
            Debug.WriteLine($"AI 翻译失败: {completionResult.Error?.Message}");
            return new List<string>();
        }
    }
}