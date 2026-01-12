@echo off
echo ============================================
echo Building DesktopLoginApp as portable .exe
echo ============================================

REM Clean previous build
if exist "bin\Release" rmdir /s /q "bin\Release"

REM Publish as self-contained single-file executable
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true

if %errorlevel% neq 0 (
    echo.
    echo ============================================
    echo Build FAILED!
    echo ============================================
    pause
    exit /b %errorlevel%
)

echo.
echo ============================================
echo Build successful! Moving files...
echo ============================================

REM Create destination folder if not exists
if not exist "..\wwwroot\DesktopLoginApp" mkdir "..\wwwroot\DesktopLoginApp"

REM Copy all files from publish folder
xcopy /Y /E /I "bin\Release\net9.0-windows\win-x64\publish\*" "..\wwwroot\DesktopLoginApp\"

if %errorlevel% neq 0 (
    echo.
    echo ============================================
    echo Copy FAILED!
    echo ============================================
    pause
    exit /b %errorlevel%
)

echo.
echo ============================================
echo SUCCESS!
echo Files copied to: D:\station-c\wwwroot\DesktopLoginApp\
echo ============================================
echo.
echo Main executable: DesktopLoginApp.exe
echo Package ready for download via web app
echo.

pause
