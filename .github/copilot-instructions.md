# Project Guidelines — CoPawLauncher

## Overview
CoPaw Launcher 是一个 WPF 桌面应用，作为 CoPaw AI 聊天服务的快捷启动器，后台运行 `copaw app` 进程并通过内嵌 WebView2 加载 `http://127.0.0.1:8088/chat`。

## Code Style
- **C# 最新语法**：file-scoped namespaces、nullable reference types（`enable`）、隐式 usings
- **私有字段**：下划线前缀 `_copawProcess`、`_isExiting`
- **事件处理器命名**：`控件名_事件名`，如 `MenuButton_Click`、`DarkThemeRadio_Checked`
- **注释语言**：中文 XML 文档注释和行内注释
- **代码分区**：使用 `#region` 组织窗口控制和功能区块
- 参考 [MainWindow.xaml.cs](MainWindow.xaml.cs) 和 [ThemeManager.cs](ThemeManager.cs)

## Architecture
- **纯 Code-Behind 模式**，无 MVVM / ViewModel / DI 容器，所有逻辑在 `.xaml.cs` 中
- **对话框**用 `UserControl` + MaterialDesign `DialogHost.Show()` 模态覆盖层（非独立 Window）
- **进程管理**在 [App.xaml.cs](App.xaml.cs)：通过 `System.Diagnostics.Process` 启动/终止 `copaw app`
- **主题切换**通过静态类 [ThemeManager.cs](ThemeManager.cs)，使用 `PaletteHelper` + `Theme.SetBaseTheme()` / `Theme.SetPrimaryColor()` 标准 API
- **自定义无边框窗口**：`WindowStyle="None"` + `AllowsTransparency="True"` + MaterialDesign 样式

## Build and Test
```bash
dotnet build -c Debug          # 构建
dotnet run                     # 运行
dotnet publish -c Release -r win-x64 --self-contained true -o "publish"  # 发布
```
也可使用 `build.bat`、`run.bat`、`publish.bat` 脚本。无单元测试。

## Project Conventions
- **目标框架**：`net10.0-windows`，依赖 `MaterialDesignThemes 5.1.0` 和 `Microsoft.Web.WebView2 1.0.2210.55`
- 版本和作者信息集中在 [Directory.Build.props](Directory.Build.props)
- XAML 样式定义在 [App.xaml](App.xaml)（全局 `MenuButtonStyle` 等）和各窗口资源中
- XAML `x:Name` 统一使用 PascalCase（如 `WebView`、`StatusText`、`StatusIcon`）
- 异常处理多为 try-catch + `Debug.WriteLine` 或 `MessageBox.Show`，catch 中常静默处理
- 设置当前不持久化（重启后恢复默认深色+蓝色主题）

## Integration Points
- **CoPaw CLI**：外部进程 `copaw app`，由 App.xaml.cs 管理生命周期（`Process.Start` / `Process.Kill`）
- **WebView2**：嵌入 Edge 内核浏览器，导航到本地服务 `http://127.0.0.1:8088/chat`
- **GitHub 仓库**：`https://github.com/Billowliu2025/CoPawLauncher`（关于对话框中的链接）
