📖 Minecraft Shader Pack 汉化工具
这是一个轻量、高效的 Minecraft 光影包（Shader Pack）汉化辅助工具。它支持直接修改 Zip 压缩包内的语言文件，并接入 AI 实现智能批量翻译。

✨ 功能特性
无需解压：直接读取和修改 .zip 格式的光影包。

AI 智能翻译：支持接入 DeepSeek、OpenAI 等兼容接口，一键批量汉化。

高度自定义：可自由配置源语言文件路径（如 en_US.lang）和目标路径。

实时搜索：内置过滤功能，快速定位需要修改的词条。

轻量便携：单文件绿色版，无依赖，即开即用。

原生交互：内置丝滑的浮动提示（Toast）系统。

🚀 快速开始
1. 配置 AI 接口
点击工具栏的 ⚙️ 设置 按钮。

输入你的 API Key（支持 DeepSeek、OpenAI 兼容平台）。

配置 Base URL（例如：https://api.deepseek.com）。

设置你常用的光影包语言文件路径（默认为 shaders/lang/en_US.lang）。

2. 开始汉化
点击 📂 打开 Zip 载入光影文件。

在界面左侧查看原始配置项，右侧输入或使用 🤖 自动翻译。

完成后点击 💾 保存汉化包，工具将自动将翻译后的内容写入压缩包。

🛠️ 开发与构建
项目基于 Avalonia UI (MVVM) 构建。

编译环境
.NET 8.0 SDK 或更高版本

Visual Studio 2022 或 JetBrains Rider

构建最小体积发布版
运行项目根目录下的 publish.bat，或者使用以下命令：

Bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -p:EnableCompressionInSingleFile=true
📁 项目结构
Plaintext
translateShaderPacks/
├── Models/          # 数据模型 (LangEntry)
├── ViewModels/      # 业务逻辑 (MVVM)
├── Views/           # 界面文件 (Avalonia XAML)
├── Services/        # 核心服务 (AI翻译、配置文件管理)
├── Assets/          # 静态资源 (图标)
└── publish.bat      # 自动化打包脚本
⚠️ 注意事项
隐私安全：config.json 包含你的 API Key，请勿将其上传至任何公开代码库。

路径规范：请确保 Zip 包内的路径与设置中的路径匹配。

📄 开源协议
本项目采用 MIT License 开源。
