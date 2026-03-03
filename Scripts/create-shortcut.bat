@echo off
echo ========================================
echo   Create Desktop Shortcut
echo ========================================
echo.

:: 项目根目录（脚本位于 Scripts\ 子目录）
set ROOT_DIR=%~dp0..\

set EXE_PATH=%ROOT_DIR%bin\Debug\net10.0-windows\CoPawLauncher.exe
set ICON_PATH=%ROOT_DIR%Assets\app.ico
set DESKTOP=%USERPROFILE%\Desktop

if not exist "%EXE_PATH%" (
    echo [ERROR] Executable not found
    echo Path: %EXE_PATH%
    echo.
    echo Please run Scripts\build.bat first
    pause
    exit /b 1
)

echo Creating shortcut...
echo.

:: Create PowerShell script
set PS_SCRIPT=%TEMP%\create_shortcut.ps1
(
echo $WshShell = New-Object -ComObject WScript.Shell
echo $Shortcut = $WshShell.CreateShortcut('%DESKTOP%\CoPaw Launcher.lnk'^)
echo $Shortcut.TargetPath = '%EXE_PATH%'
echo $Shortcut.WorkingDirectory = '%ROOT_DIR%'
echo $Shortcut.Description = 'CoPaw Launcher'
echo $Shortcut.IconLocation = '%ICON_PATH%'
echo $Shortcut.Save^(^)
) > "%PS_SCRIPT%"

powershell -ExecutionPolicy Bypass -File "%PS_SCRIPT%"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo [SUCCESS] Shortcut created on Desktop!
    echo.
    echo Location: %DESKTOP%\CoPaw Launcher.lnk
    echo.
    echo Note: If icon does not appear immediately, try refreshing desktop
) else (
    echo.
    echo [ERROR] Failed to create shortcut
)

:: Cleanup
del "%PS_SCRIPT%" 2>nul
pause
