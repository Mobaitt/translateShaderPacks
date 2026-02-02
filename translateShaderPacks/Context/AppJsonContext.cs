using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace translateShaderPacks.Context;

// 定义我们需要处理的所有 JSON 模型
public record ChatRequest(string model, List<Message> messages, float temperature);
public record Message(string role, string content);
public record ChatResponse(List<Choice> choices);
public record Choice(Message message);

// 核心：告诉 AOT 提前生成这些类的序列化逻辑
[JsonSerializable(typeof(ChatRequest))]
[JsonSerializable(typeof(ChatResponse))]
[JsonSerializable(typeof(List<string>))]
internal partial class AppJsonContext : JsonSerializerContext
{
}