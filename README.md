# CoPaw Launcher - CoPaw 快捷启动器

<div align="center">

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet)
![WPF](https://img.shields.io/badge/WPF-UI-0078D7?style=for-the-badge&logo=windows)
![MaterialDesign](https://img.shields.io/badge/MaterialDesign-5.1.0-E91E63?style=for-the-badge&logo=materialdesign)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)

**一个美观现代的 CoPaw 快捷启动应用程序**

[功能特性](#功能特性) • [快速开始](#快速开始) • [使用说明](#使用说明) • [截图预览](#截图预览)

</div>

---

## 📖 简介

CoPaw Launcher 是一个基于 WPF (.NET 10) 开发的桌面应用程序，采用 MaterialDesign 设计风格，提供现代化的用户界面。程序启动时自动在后台运行 CoPaw 服务，并通过内嵌的 WebView2 浏览器直接访问 CoPaw 聊天界面，让您无需打开浏览器即可快速使用 CoPaw。

## ✨ 功能特性

### 核心功能
- 🚀 **自动后台启动** - 打开程序时自动在后台执行 `copaw app`
- 🌐 **内嵌浏览器** - 使用 WebView2 嵌入 http://127.0.0.1:8088/chat
- 🎨 **MaterialDesign UI** - 采用 MaterialDesignInXamlToolkit 美化界面
- 📱 **现代化设计** - 深色主题、圆角设计、流畅动画

### 主题与个性化
- 🌓 **全局主题切换** - 支持深色/浅色主题实时切换
- 🎨 **12 种主色调** - 红、粉、紫、靛、蓝、青、绿、橙等颜色可选
- 🔄 **实时生效** - 主题切换后立即应用到所有 UI 元素
- 💾 **恢复默认** - 一键恢复深色主题 + 蓝色主色调

### 界面功能
- 📋 **下拉菜单** - 左上角菜单按钮提供便捷操作
  - 🔄 刷新页面
  - 🚪 退出（保持服务）- 仅关闭窗口，CoPaw 服务继续运行
  - ⚡ 强制退出 - 关闭窗口并停止 CoPaw 服务
  - ⚙️ 设置 - 自定义主题和外观
  - ℹ️ 关于 - 查看应用信息
- 🎛️ **窗口控制** - 最小化、最大化/还原、关闭按钮
- 📊 **状态栏** - 实时显示 CoPaw 服务状态
- 🖼️ **自定义图标** - 应用程序和快捷方式均使用自定义图标

### 便捷操作
- ⌨️ **键盘快捷键**
  - `F5` - 刷新页面
  - `Esc` - 关闭菜单
  - `Alt+F4` - 退出程序

## 🖥️ 截图预览

### 主界面（深色主题）
```
┌─────────────────────────────────────────────────────────┐
│ ☰ CoPaw 快捷启动器       [🔄] │ [_] [□] [×]           │
├─────────────────────────────────────────────────────────┤
│                                                         │
│                                                         │
│              WebView2 浏览器区域                         │
│           (http://127.0.0.1:8088/chat)                 │
│                                                         │
│                                                         │
├─────────────────────────────────────────────────────────┤
│ ✓ 状态：CoPaw 已就绪                                    │
└─────────────────────────────────────────────────────────┘
```

### 下拉菜单
```
┌─────────────────────────────┐
│ 🔄 刷新页面                  │
├─────────────────────────────┤
│ 🚪 退出（保持服务）          │
│ ⚡ 强制退出                  │
├─────────────────────────────┤
│ ⚙️ 设置                     │
├─────────────────────────────┤
│ ℹ️ 关于                     │
└─────────────────────────────┘
```

### 设置对话框
```
┌──────────────────────────────────┐
│  ⚙️ 设置                         │
│  自定义应用外观和行为             │
├──────────────────────────────────┤
│ ┌──────────────────────────────┐ │
│ │ 🎨 风格设置                  │ │
│ │                              │ │
│ │ 主题模式                     │ │
│ │ ◉ 深色  ○ 浅色               │ │
│ │                              │ │
│ │ 主色调                       │ │
│ │ 🔴 🩷 🟣 🟣 🟣               │ │
│ │ 🟣 🔵 🔵 🔵 🟢               │ │
│ │ 🟢 🟢 🟠                     │ │
│ │                              │ │
│ │ [恢复默认设置]               │ │
│ └──────────────────────────────┘ │
│ ┌──────────────────────────────┐ │
│ │ ℹ️ 关于应用                  │ │
│ │ CoPaw Launcher v1.0.0        │ │
│ └──────────────────────────────┘ │
├──────────────────────────────────┤
│              [取消] [保存]       │
└──────────────────────────────────┘
```

## 📋 系统要求

- **操作系统**: Windows 10/11 (64-bit)
- **.NET 版本**: .NET 10.0 SDK
- **浏览器组件**: WebView2 Runtime (Windows 10/11 通常已预装)
- **依赖服务**: CoPaw 已安装并配置

## 🚀 快速开始

### 1. 克隆项目

```bash
git clone https://github.com/Billowliu2025/CoPawLauncher.git
cd CoPawLauncher
```

### 2. 构建项目

**方式一：使用构建脚本**
```bash
build.bat
```

**方式二：使用 dotnet 命令**
```bash
dotnet build
```

### 3. 运行程序

**方式一：直接运行**
```bash
bin\Debug\net10.0-windows\CoPawLauncher.exe
```

**方式二：使用启动脚本**
```bash
run.bat
```

**方式三：使用 dotnet run**
```bash
dotnet run
```

### 4. 创建桌面快捷方式

```bash
create-shortcut.bat
```

## 📁 项目结构

```
CoPawLauncher/
├── App.xaml                    # 应用程序定义与主题配置
├── App.xaml.cs                 # 应用程序逻辑（后台启动 copaw）
├── MainWindow.xaml             # 主窗口界面（MaterialDesign）
├── MainWindow.xaml.cs          # 主窗口逻辑（WebView2 + 菜单）
├── AboutDialog.xaml            # 关于对话框界面
├── AboutDialog.xaml.cs         # 关于对话框逻辑
├── SettingsDialog.xaml         # 设置对话框界面
├── SettingsDialog.xaml.cs      # 设置对话框逻辑
├── ThemeManager.cs             # 全局主题管理器
├── AssemblyInfo.cs             # 程序集信息
├── Directory.Build.props       # 版本管理配置
├── CoPawLauncher.csproj        # 项目配置文件
├── app.ico                     # 应用程序图标（多尺寸）
├── logo.png                    # 原始 Logo 图片
├── convert_icon.py             # PNG 转 ICO 图标转换脚本
├── build.bat                   # 构建脚本
├── run.bat                     # 启动脚本
├── publish.bat                 # 发布独立版本脚本
├── create-shortcut.bat         # 创建桌面快捷方式脚本
└── README.md                   # 项目说明文档
```

## 🛠️ 技术栈

| 技术 | 版本 | 说明 |
|------|------|------|
| **框架** | .NET 10.0 | 最新的 .NET 框架 |
| **UI 框架** | WPF | Windows Presentation Foundation |
| **UI 库** | MaterialDesignThemes 5.1.0 | Material Design 设计风格 |
| **浏览器** | WebView2 1.0.2210.55 | Microsoft Edge WebView2 |
| **进程管理** | System.Diagnostics | .NET 进程管理 |
| **版本管理** | Directory.Build.props | 统一版本配置 |

## 📖 使用说明

### 启动程序
1. 双击运行 `CoPawLauncher.exe` 或 `run.bat`
2. 程序自动在后台启动 CoPaw 服务
3. 主窗口显示 CoPaw 聊天界面

### 使用菜单
1. 点击左上角 **☰** 菜单按钮
2. 选择所需操作：
   - **刷新页面**: 重新加载聊天界面
   - **退出（保持服务）**: 仅关闭窗口，CoPaw 服务继续运行
   - **强制退出**: 关闭窗口并停止 CoPaw 服务
   - **设置**: 自定义主题和外观
   - **关于**: 查看应用信息

### 主题设置
1. 点击菜单 → **设置**
2. 选择主题模式：
   - **深色**: 适合夜间使用，保护视力
   - **浅色**: 适合白天使用，清晰明亮
3. 选择主色调（12 种颜色）：
   - 红色、粉色、紫色、深紫、靛蓝
   - 蓝色、浅蓝、青色、蓝绿、绿色、浅绿、橙色
4. 点击 **保存** 应用设置
5. 点击 **恢复默认设置** 返回深色主题 + 蓝色

### 窗口控制
- **最小化**: 点击 `_` 按钮或按 `Win + ↓`
- **最大化/还原**: 点击 `□` 按钮
- **关闭**: 点击 `×` 按钮或按 `Alt + F4`
- **刷新**: 点击 `🔄` 按钮或按 `F5`

### 状态栏
- **✓ 绿色图标**: CoPaw 服务正常运行
- **⚠ 黄色图标**: 正在加载或刷新
- **✗ 红色图标**: 加载失败或错误

## 🎨 自定义

### 更换图标
1. 准备一张 PNG 图片（建议 512x512 或更大）
2. 命名为 `logo.png` 并替换项目中的文件
3. 运行 `python convert_icon.py` 生成新的 ICO 文件
4. 重新构建项目：`dotnet build`

### 修改主题颜色
编辑 `App.xaml` 中的主题配置：
```xml
<materialDesign:BundledTheme 
    BaseTheme="Dark" 
    PrimaryColor="Blue" 
    SecondaryColor="LightBlue" />
```

可用颜色：`Blue`, `Red`, `Green`, `Purple`, `Orange`, `Teal` 等

### 修改默认 URL
编辑 `MainWindow.xaml.cs` 中的 `InitializeWebView()` 方法：
```csharp
webView.CoreWebView2.Navigate("http://127.0.0.1:8088/chat");
```

### 修改窗口大小
编辑 `MainWindow.xaml`：
```xml
<Window ... Height="650" Width="1000">
```

### 修改版本信息
编辑 `Directory.Build.props`：
```xml
<VersionPrefix>1.1.0</VersionPrefix>
<AssemblyVersion>1.1.0.0</AssemblyVersion>
<FileVersion>1.1.0.0</FileVersion>
```

## 🔧 故障排除

### WebView2 初始化失败
**问题**: 程序提示 WebView2 初始化失败

**解决方案**:
1. 确保已安装 WebView2 Runtime
2. 下载地址：https://developer.microsoft.com/en-us/microsoft-edge/webview2/
3. 安装后重启程序

### 无法连接 http://127.0.0.1:8088/chat
**问题**: 状态栏显示加载失败

**解决方案**:
1. 确保 copaw app 已成功启动
2. 检查 8088 端口是否被占用：`netstat -ano | findstr 8088`
3. 查看 copaw 日志文件
4. 重启 copaw 服务

### copaw 命令找不到
**问题**: 启动时提示找不到 copaw 命令

**解决方案**:
1. 确保 copaw 已添加到系统 PATH
2. 或在 `App.xaml.cs` 中使用完整路径：
```csharp
FileName = @"C:\Path\To\copaw.exe"
```

### 图标不显示
**问题**: 桌面快捷方式图标为默认图标

**解决方案**:
1. 刷新桌面（右键 → 刷新）
2. 重新运行 `create-shortcut.bat`
3. 重启资源管理器

### 主题切换不生效
**问题**: 切换主题后部分控件颜色未变化

**解决方案**:
1. 确保使用的是最新版本的 MaterialDesignThemes (5.1.0+)
2. 重启应用程序
3. 清除 bin/obj 文件夹后重新构建

## 📦 发布为独立程序

生成不依赖 .NET SDK 的独立程序：

```bash
dotnet publish -c Release -r win-x64 --self-contained true -o publish
```

发布后的文件位于 `publish` 文件夹，可直接分发给其他用户。

## 📄 许可证

本项目采用 [MIT License](LICENSE) 开源协议。

## 🤝 贡献

欢迎提交 Issue 和 Pull Request！

1. Fork 本项目
2. 创建特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 开启 Pull Request

## 📬 联系方式

- **GitHub**: [@Billowliu2025](https://github.com/Billowliu2025)
- **项目地址**: [https://github.com/Billowliu2025/CoPawLauncher](https://github.com/Billowliu2025/CoPawLauncher)

## 🙏 致谢

- [MaterialDesignInXamlToolkit](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit) - Material Design UI 库
- [Microsoft WebView2](https://docs.microsoft.com/en-us/microsoft-edge/webview2/) - Microsoft Edge WebView2
- .NET Team - .NET 10.0 框架

## 📝 更新日志

### v1.0.0 (2025-03-02)
- ✨ 初始版本发布
- 🎨 MaterialDesign 5.1.0 UI 设计
- 🌓 全局主题切换（深色/浅色）
- 🎨 12 种主色调可选
- ⚙️ 设置对话框
- ℹ️ 关于对话框（带 GitHub 链接）
- 🔄 刷新按钮（右上角）
- 📋 下拉菜单（刷新、退出、设置、关于）
- 🖼️ 自定义应用图标
- 📦 Directory.Build.props 版本管理
- 🌐 WebView2 内嵌浏览器
- 🚀 自动后台启动 CoPaw 服务
- ⚡ 强制退出（停止服务）
- 🚪 保持服务退出

---

<div align="center">

**Made with ❤️ by Bill**

如果这个项目对你有帮助，请给一个 ⭐ Star！

[📦 下载最新版本](https://github.com/Billowliu2025/CoPawLauncher/releases) • [📖 查看文档](https://github.com/Billowliu2025/CoPawLauncher#readme) • [🐛 报告问题](https://github.com/Billowliu2025/CoPawLauncher/issues)

</div>
