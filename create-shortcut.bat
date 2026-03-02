@echo off
echo 正在创建桌面快捷方式...

set SCRIPT_DIR=%~dp0
set EXE_PATH=%SCRIPT_DIR%bin\Debug\net10.0-windows\CoPawLauncher.exe
set DESKTOP=%USERPROFILE%\Desktop

if exist "%EXE_PATH%" (
    powershell -Command "$WshShell = New-Object -ComObject WScript.Shell; $Shortcut = $WshShell.CreateShortcut('%DESKTOP%\CoPaw Launcher.lnk'); $Shortcut.TargetPath = '%EXE_PATH%'; $Shortcut.WorkingDirectory = '%SCRIPT_DIR%'; $Shortcut.Description = 'CoPaw 快捷启动器'; $Shortcut.Save()"
    
    if %ERRORLEVEL% EQU 0 (
        echo 快捷方式已创建到桌面！
        echo.
        echo 文件位置：%DESKTOP%\CoPaw Launcher.lnk
    ) else (
        echo 创建快捷方式失败
    )
) else (
    echo 错误：找不到可执行文件
    echo 路径：%EXE_PATH%
    echo.
    echo 请先运行 build.bat 或手动构建项目
)

pause
