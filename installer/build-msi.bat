@echo off
REM ============================================
REM  CoPaw Launcher MSI 安装包构建脚本
REM  
REM  前置条件：
REM    1. .NET SDK 已安装
REM    2. WiX Toolset v5 已安装：dotnet tool install --global wix --version 5.0.2
REM    3. WiX UI 扩展已安装：wix extension add WixToolset.UI.wixext/5.0.2
REM ============================================

setlocal enabledelayedexpansion

echo.
echo ========================================
echo   CoPaw Launcher MSI 构建工具
echo ========================================
echo.

REM 切换到项目根目录
cd /d "%~dp0\.."

REM ---- 第 1 步：发布应用 ----
echo [1/3] 正在发布应用程序...
dotnet publish -c Release -r win-x64 --self-contained true -o "installer\publish" /p:PublishSingleFile=false
if %errorlevel% neq 0 (
    echo [错误] 发布失败！
    pause
    exit /b 1
)
echo       发布完成。
echo.

REM ---- 第 2 步：构建 MSI ----
echo [2/3] 正在构建 MSI 安装包...
cd installer
wix build CoPawLauncher.wxs -ext WixToolset.UI.wixext -o CoPawLauncher.msi -arch x64
if %errorlevel% neq 0 (
    echo [错误] MSI 构建失败！
    pause
    exit /b 1
)
cd ..
echo       MSI 构建完成。
echo.

REM ---- 第 3 步：输出结果 ----
echo [3/3] 清理临时文件...
if exist "installer\publish" rmdir /s /q "installer\publish"
if exist "installer\CoPawLauncher.wixpdb" del /q "installer\CoPawLauncher.wixpdb"

echo.
echo ========================================
echo   构建成功！
echo   安装包位置：installer\CoPawLauncher.msi
echo ========================================
echo.

pause
