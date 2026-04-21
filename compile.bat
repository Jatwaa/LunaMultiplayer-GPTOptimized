@echo off
setlocal

REM Generate timestamp for log file using WMIC (locale independent)
for /f "tokens=2 delims==" %%i in ('wmic os get LocalDateTime /value 2^>nul') do set "datetime=%%i"
if defined datetime (
    set "timestamp=%datetime:~0,4%%datetime:~4,2%%datetime:~6,2%%datetime:~8,2%%datetime:~10,2%%datetime:~12,2%"
) else (
    REM Fallback to date and time if WMIC fails
    set "timestamp=%DATE:~-4%%DATE:~4,2%%DATE:~7,2%_%TIME:~0,2%%TIME:~3,2%%TIME:~6,2%"
    set "timestamp=%timestamp: =0%"
)

set "log_file=compile_log_%timestamp%.txt"

echo [LMP] Cleaning solution...
dotnet clean "LunaMultiPlayer.sln" >nul 2>&1

echo [LMP] Starting compilation of Client and Server...
echo [LMP] Log file: %log_file%

dotnet build "LunaMultiPlayer.sln" -c Release > "%log_file%" 2>&1

if %errorlevel% equ 0 (
    echo [SUCCESS] Compilation completed successfully! Log saved to %log_file%
) else (
    echo [ERROR] Compilation failed with error code %errorlevel%. Log saved to %log_file%
)

REM Clean up old logs: keep only the 100 newest (by name, which sorts by timestamp)
for /f "skip=100 delims=" %%F in ('dir /b /a-d /o:-n compile_log_*.txt 2^>nul') do del "%%F"

endlocal
if %errorlevel% neq 0 pause