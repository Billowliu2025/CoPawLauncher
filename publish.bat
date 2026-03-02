@echo off
echo ========================================
echo   CoPaw Launcher 发布工具
echo ========================================
echo.
echo 正在发布 Release 版本...
echo.

cd /d "%~dp0"

dotnet publish -c Release -r win-x64 --self-contained true -o "publish"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo   发布成功！
    echo ========================================
    echo.
    echo 发布位置：%CD%\publish\
    echo.
    echo 可执行文件：CoPawLauncher.exe
    echo.
    echo 提示：此版本为独立版本，无需安装 .NET 运行时
    echo.
) else (
    echo.
    echo 发布失败，请检查错误信息
    echo.
)

pause
