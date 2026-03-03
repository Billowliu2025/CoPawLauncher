# Project Guidelines — CoPawLauncher

## Overview
CoPaw Launcher 是一个 WPF 桌面应用，作为 CoPaw AI 聊天服务的快捷启动器，后台运行 `copaw app` 进程并通过内嵌 WebView2 加载 `http://127.0.0.1:8088/chat`。

## Code Style
- **C# 最新语法**：file-scoped namespaces、nullable reference types（`enable`）、隐式 usings
- **私有字段**：下划线前缀，如 `_isMaximized`、`_downloadCts`
- **事件处理器命名**：`控件名_事件名`，如 `MenuButton_Click`、`DarkThemeRadio_Checked`
- **注释语言**：中文 XML 文档注释和行内注释
- **代码分区**：使用 `#region` 组织功能区块，如 `#region 窗口拖动与调整大小`
- 参考 [Views/MainWindow.xaml.cs](Views/MainWindow.xaml.cs) 和 [Services/ThemeManager.cs](Services/ThemeManager.cs)

## Architecture
```
CoPawLauncher/
├── App.xaml / App.xaml.cs         # 应用入口：主题加载、启动 ProcessManager
├── GlobalUsings.cs                # global using CoPawLauncher.{Models,Services,Views}
├── Assets/                        # favicon.ico（EXE+标题图标）、Copaw.ico（MSI图标）、Copaw.png
├── Models/                        # 数据模型（CoPawLauncher.Models 命名空间）
│   ├── UpdateCheckResult.cs       # 更新检查结果 DTO
│   └── GitHubModels.cs            # GitHub API 响应实体
├── Services/                      # 业务服务（CoPawLauncher.Services 命名空间）
│   ├── ProcessManager.cs          # copaw app 进程生命周期（Start/Stop/IsCopawRunning）
│   ├── SettingsStore.cs           # SQLite 设置持久化（%LOCALAPPDATA%\CoPawLauncher\settings.db）
│   ├── ThemeManager.cs            # MaterialDesign 主题切换（PaletteHelper）
│   └── UpdateChecker.cs           # GitHub Releases 更新检查 + 下载 + 安装
├── Views/                         # 视图（CoPawLauncher.Views 命名空间）
│   ├── MainWindow.xaml/.cs        # 主窗口：无边框、标题栏拖动、边缘缩放、WebView2
│   ├── AboutDialog.xaml/.cs       # 关于对话框：3阶段更新流（检查→下载→安装）
│   └── SettingsDialog.xaml/.cs    # 设置对话框：主题切换、颜色选择、自定义图标
└── Scripts/                       # 开发脚本（build/run/publish/create-shortcut/convert_icon）
```

- **纯 Code-Behind 模式**，无 MVVM / ViewModel / DI 容器
- **对话框**用 `UserControl` + `DialogHost.Show("RootDialog")` 模态覆盖层（非独立 Window）
- **无边框窗口**：`WindowStyle="None"` + `AllowsTransparency="True"` + Win32 SC_SIZE 边缘拖拽缩放
- **进程管理**委托给 [Services/ProcessManager.cs](Services/ProcessManager.cs)；`App.StopCopawProcess()` 为兼容入口
- **更新检查**优先重定向方式（无 API 限额），API 403 时回退构造下载 URL

## Build and Test
```bash
dotnet build -c Debug                                              # 构建
dotnet run                                                         # 运行
dotnet publish -c Release -r win-x64 --self-contained true -o "installer\publish" /p:PublishSingleFile=false
cd installer && wix build CoPawLauncher.wxs -ext WixToolset.UI.wixext -o "CoPawLauncher-X.Y.Z-win-x64.msi" -arch x64
```
也可使用 `Scripts\build.bat`、`Scripts\run.bat`、`Scripts\publish.bat`。无单元测试。

## Project Conventions
- **目标框架**：`net10.0-windows`，依赖 `MaterialDesignThemes 5.1.0`、`Microsoft.Web.WebView2 1.0.2210.55`、`Microsoft.Data.Sqlite 9.0.2`
- 版本和作者信息集中在 [Directory.Build.props](Directory.Build.props)；**发布时改版本，发布后重置为 1.0.0**
- 全局命名空间引用在 [GlobalUsings.cs](GlobalUsings.cs)，子层代码无需重复 using
- XAML 样式定义在 [App.xaml](App.xaml)（全局 `MenuButtonStyle`）和各视图资源中
- XAML `x:Name` 统一 PascalCase（`WebView`、`StatusText`、`TitleBar`）
- 设置通过 `SettingsStore` 持久化到 SQLite：键常量 `KeyIsDarkTheme`、`KeyPrimaryColor`、`KeyCustomIconFile`
- 异常处理：try-catch + `Debug.WriteLine`，UI 错误用 `MessageBox.Show`，网络/文件操作静默处理

## Integration Points
- **CoPaw CLI**：外部进程 `copaw app`，由 [Services/ProcessManager.cs](Services/ProcessManager.cs) 管理（`Process.Start` / `Process.Kill()`）
- **WebView2**：嵌入 Edge 内核，用户数据目录 `%LOCALAPPDATA%\CoPawLauncher`，导航到 `http://127.0.0.1:8088/chat`
- **GitHub Releases**：更新检查——重定向获取 tag（`/releases/latest` 302）→ API 获取下载详情 → 403 时按命名规则构造 URL（`CoPawLauncher-{ver}-win-x64.msi`）
- **MSI 安装包**：WiX v5，perUser 安装，[installer/CoPawLauncher.wxs](installer/CoPawLauncher.wxs)，图标使用 `Assets/Copaw.ico`
- **GitHub 仓库**：`https://github.com/Billowliu2025/CoPawLauncher`
