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

### 界面功能
- 📋 **下拉菜单** - 左上角菜单按钮提供便捷操作
  - 🔄 刷新页面
  - 🚪 退出（保持服务）- 仅关闭窗口，CoPaw 服务继续运行
  - ⚡ 强制退出 - 关闭窗口并停止 CoPaw 服务
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

### 主界面
```
┌─────────────────────────────────────────────────────────┐
│ ☰ CoPaw 快捷启动器              [_] [□] [×]            │
├─────────────────────────────────────────────────────────┤
│                                                         │
│                                                         │
│              WebView2 浏览器区域                         │
│           (http://127.0.0.1:8088/chat)                 │
│                                                         │
│                                                         │
├─────────────────────────────────────────────────────────┤
│ ✓ 状态：CoPaw 已就绪                    [🔄 刷新]       │
└─────────────────────────────────────────────────────────┘
```

### 下拉菜单
```
┌─────────────────────────────┐
│ 🔄 刷新页面                  │
├─────────────────────────────┤
│ 🚪 退出（保持服务）          │
│    仅关闭窗口，服务继续运行   │
│ ⚡ 强制退出                  │
│    关闭窗口并停止服务        │
├─────────────────────────────┤
│ ℹ️ 关于 CoPaw Launcher       │
└─────────────────────────────┘
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
├── AssemblyInfo.cs             # 程序集信息
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
   - **关于**: 查看应用信息

### 窗口控制
- **最小化**: 点击 `_` 按钮或按 `Win + ↓`
- **最大化/还原**: 点击 `□` 按钮
- **关闭**: 点击 `×` 按钮或按 `Alt + F4`

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

### 菜单显示异常
**问题**: 下拉菜单位置不正确

**解决方案**:
1. 确保窗口未被其他程序遮挡
2. 尝试移动窗口位置
3. 重启程序

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
- **项目地址**: https://github.com/Billowliu2025/CoPawLauncher

## 🙏 致谢

- [MaterialDesignInXamlToolkit](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit) - Material Design UI 库
- [Microsoft WebView2](https://docs.microsoft.com/en-us/microsoft-edge/webview2/) - Microsoft Edge WebView2
- .NET Team - .NET 10.0 框架

---

<div align="center">

**Made with ❤️ by Bill**

如果这个项目对你有帮助，请给一个 ⭐ Star！

</div>
