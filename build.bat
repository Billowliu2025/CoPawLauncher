@echo off
echo ========================================
echo   CoPaw Launcher 构建工具
echo ========================================
echo.

cd /d "%~dp0"

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
    echo   1. 运行 run.bat 启动程序
    echo   2. 运行 create-shortcut.bat 创建桌面快捷方式
    echo   3. 运行 publish.bat 发布独立版本
    echo.
) else (
    echo.
    echo 构建失败，请检查错误信息
    echo.
)

pause
