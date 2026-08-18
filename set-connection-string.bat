@echo off
REM ---------------------------------------------------------------------------
REM  Creates the environment variable holding the SQL Server connection string.
REM
REM  appsettings.json stores only the NAME of this variable, never the value,
REM  so no credential is ever committed to source control.
REM
REM  Usage:
REM     set-connection-string.bat
REM         uses the default local SQL Server connection string below.
REM
REM     set-connection-string.bat "Server=...;Database=...;User Id=...;Password=..."
REM         uses the connection string you pass instead.
REM ---------------------------------------------------------------------------

set "VARIABLE_NAME=CUSTOMERPORTAL_SQLCONNECTION"

if "%~1"=="" (
    set "CONNECTION_STRING=Data Source=localhost;Initial Catalog=DigitalOnboarding;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False"
) else (
    set "CONNECTION_STRING=%~1"
)

REM A database name is required - without one, "dotnet ef database update" has no
REM database to create and would build the schema in master.
echo %CONNECTION_STRING% | findstr /I /C:"Initial Catalog=" /C:"Database=" >nul
if errorlevel 1 (
    echo.
    echo   ERROR: the connection string has no "Initial Catalog=" or "Database=".
    echo   Add a database name and run this again.
    echo.
    exit /b 1
)

REM Persist for the current Windows user.
setx "%VARIABLE_NAME%" "%CONNECTION_STRING%" >nul
if errorlevel 1 (
    echo.
    echo   ERROR: could not persist the environment variable.
    echo.
    exit /b 1
)

REM Also set it in this shell, so the current window can be used straight away.
set "%VARIABLE_NAME%=%CONNECTION_STRING%"

echo.
echo   Environment variable created.
echo.
echo     Name  : %VARIABLE_NAME%
echo     Value : %CONNECTION_STRING%
echo.
echo   Persisted for your Windows user account.
echo   Terminals, Visual Studio and IIS that are already open will not see it
echo   until they are restarted.
echo.
echo   Next:
echo     dotnet tool restore
echo     dotnet ef database update --project CustomerPortal.Infrastructure --startup-project CustomerPortal.Api
echo     dotnet run --project CustomerPortal.Api
echo.
