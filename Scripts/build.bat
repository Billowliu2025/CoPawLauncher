@echo off
echo ========================================
echo   CoPaw Launcher 构建工具
echo ========================================
echo.

:: 切换到项目根目录（脚本位于 Scripts\ 子目录）
cd /d "%~dp0.."

echo 正在还原 NuGet 包...
dotnet restore

echo.
echo 正在构建项目...
dotnet build -c Debug

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo   构建成功！
    echo ========================================
    echo.
    echo 可执行文件位置:
    echo %CD%\bin\Debug\net10.0-windows\CoPawLauncher.exe
    echo.
    echo 下一步:
    echo   1. 运行 Scripts\run.bat 启动程序
    echo   2. 运行 Scripts\create-shortcut.bat 创建桌面快捷方式
    echo   3. 运行 Scripts\publish.bat 发布独立版本
    echo.
) else (
    echo.
    echo 构建失败，请检查错误信息
    echo.
)

pause
