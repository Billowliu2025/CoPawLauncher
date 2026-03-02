# CoPaw Launcher - CoPaw 快捷启动器

一个基于 WPF (.NET 10) 的 CoPaw 快捷启动应用程序，内嵌 WebView2 浏览器直接访问 CoPaw 聊天界面。

## 功能特性

- ✅ **自动后台启动** - 打开程序时自动在后台执行 `copaw app`
- ✅ **内嵌浏览器** - 使用 WebView2 嵌入 http://127.0.0.1:8088/chat
- ✅ **简洁界面** - 提供状态显示和刷新功能
- ✅ **快速访问** - 一键启动 CoPaw 聊天界面
- ✅ **优雅退出** - 点击退出按钮可同时停止后台 CoPaw 服务
- ✅ **键盘快捷键** - 支持 F5 刷新、Esc 退出

## 系统要求

- Windows 10/11
- .NET 10.0 SDK
- WebView2 Runtime (通常 Windows 10/11 已预装)
- CoPaw 已安装并配置

## 构建项目

```bash
cd D:\AI\CoPawLauncher
dotnet build
```

## 运行程序

### 方式 1：直接运行编译后的程序
```bash
D:\AI\CoPawLauncher\bin\Debug\net10.0-windows\CoPawLauncher.exe
```

### 方式 2：使用 dotnet run
```bash
cd D:\AI\CoPawLauncher
dotnet run
```

### 方式 3：使用启动脚本
双击 `run.bat` 文件

## 发布为独立程序

如果需要生成不依赖 .NET SDK 的独立程序：

```bash
dotnet publish -c Release -r win-x64 --self-contained true
```

生成的程序位于：`bin\Release\net10.0-windows\win-x64\publish\`

## 项目结构

```
CoPawLauncher/
├── App.xaml              # 应用程序定义
├── App.xaml.cs           # 应用程序逻辑（后台启动 copaw）
├── MainWindow.xaml       # 主窗口界面
├── MainWindow.xaml.cs    # 主窗口逻辑（WebView2 初始化）
├── AssemblyInfo.cs       # 程序集信息
├── CoPawLauncher.csproj  # 项目文件
└── README.md             # 说明文档
```

## 技术栈

- **框架**: WPF (.NET 10.0)
- **浏览器组件**: Microsoft.Web.WebView2
- **进程管理**: System.Diagnostics.Process

## 注意事项

1. 首次运行可能需要安装 WebView2 Runtime
2. 确保 copaw 已正确安装并在系统 PATH 中
3. 确保 CoPaw 服务的 8088 端口可访问
4. **退出程序时会自动停止后台 copaw 进程**

## 快捷键

| 快捷键 | 功能 |
|--------|------|
| `F5` | 刷新页面 |
| `Esc` | 退出程序 |
| `Alt+F4` | 退出程序 |

## 自定义

### 修改启动参数
编辑 `App.xaml.cs` 中的 `StartCopawBackground()` 方法

### 修改默认 URL
编辑 `MainWindow.xaml.cs` 中的 `InitializeWebView()` 方法，修改 Navigate 的 URL

### 修改窗口大小
编辑 `MainWindow.xaml`，调整 Height 和 Width 属性

## 故障排除

### WebView2 初始化失败
- 确保已安装 WebView2 Runtime
- 下载地址：https://developer.microsoft.com/en-us/microsoft-edge/webview2/

### 无法连接 http://127.0.0.1:8088/chat
- 确保 copaw app 已成功启动
- 检查 8088 端口是否被占用
- 查看 copaw 日志

### copaw 命令找不到
- 确保 copaw 已添加到系统 PATH
- 或在 `App.xaml.cs` 中使用完整路径

## 许可证

MIT License
