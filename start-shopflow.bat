@echo off
setlocal EnableExtensions
title ShopFlow Launcher
cd /d "%~dp0"

set "ROOT=%~dp0"
set "API_DIR=%ROOT%backend\src\ShopFlow"
set "WEB_DIR=%ROOT%frontend"
set "API_PORT=5292"
set "WEB_PORT=5500"
set "API_URL=http://localhost:%API_PORT%"
set "WEB_URL=http://localhost:%WEB_PORT%"
set "ASPNETCORE_ENVIRONMENT=Development"

echo ==================================================
echo   ShopFlow - starting backend and frontend
echo ==================================================
echo.

echo [1/6] Checking .NET SDK...
where dotnet >nul 2>&1
if errorlevel 1 goto :no_dotnet
dotnet --list-sdks | findstr /b "10." >nul
if errorlevel 1 goto :no_sdk10
echo       OK
echo.

echo [2/6] Checking SQL Server LocalDB...
where sqllocaldb >nul 2>&1
if errorlevel 1 goto :no_localdb
echo       OK - the API starts the MSSQLLocalDB instance by itself on first connection.
goto :check_api

:no_localdb
echo       WARNING: sqllocaldb was not found.
echo       The API expects LocalDB unless you set another connection string in user secrets.
echo       Install it with Visual Studio - "Data storage and processing" workload.

:check_api
echo.
call :port_in_use %API_PORT%
if not errorlevel 1 goto :api_already_running

echo [3/6] Restoring packages and building the backend...
dotnet build "%API_DIR%\ShopFlow.csproj" -nologo -v q
if errorlevel 1 goto :build_failed
echo       OK
echo.

echo [4/6] Starting the backend on %API_URL% ...
start "ShopFlow API" /D "%API_DIR%" cmd /k dotnet run --no-build --no-launch-profile
goto :wait_api

:api_already_running
echo [3/6] Port %API_PORT% is already in use - assuming the API is already running.
echo [4/6] Skipping the backend start.

:wait_api
echo       Waiting for the API to answer - the first run creates the database and can take a minute...
call :wait_for_port %API_PORT% 180
if errorlevel 1 goto :api_timeout
echo       OK
echo.

echo [5/6] Starting the frontend on %WEB_URL% ...
call :port_in_use %WEB_PORT%
if not errorlevel 1 goto :web_already_running
start "ShopFlow Frontend" powershell -NoProfile -ExecutionPolicy Bypass -File "%ROOT%scripts\serve-frontend.ps1" -Root "%WEB_DIR%" -Port %WEB_PORT%
goto :wait_web

:web_already_running
echo       Port %WEB_PORT% is already in use - assuming the frontend is already being served.

:wait_web
call :wait_for_port %WEB_PORT% 30
if errorlevel 1 goto :web_timeout
echo       OK
echo.

echo [6/6] Opening the browser...
start "" "%WEB_URL%/index.html"
echo.
echo ==================================================
echo   ShopFlow is running
echo   Frontend : %WEB_URL%
echo   API      : %API_URL%/swagger
echo   Login    : admin@shopflow.local / Admin123!
echo.
echo   To stop: close the "ShopFlow API" and
echo   "ShopFlow Frontend" windows.
echo ==================================================
echo.
pause
exit /b 0

:port_in_use
netstat -ano | findstr /r /c:":%~1 .*LISTENING" >nul
exit /b %errorlevel%

:wait_for_port
powershell -NoProfile -Command "for ($i = 0; $i -lt %~2; $i++) { $c = New-Object Net.Sockets.TcpClient; try { $c.Connect('localhost', %~1); $c.Close(); exit 0 } catch { Start-Sleep -Seconds 1 } finally { $c.Dispose() } }; exit 1"
exit /b %errorlevel%

:no_dotnet
echo       ERROR: the dotnet command was not found.
echo       Install the .NET 10 SDK: https://dotnet.microsoft.com/download/dotnet/10.0
goto :fail

:no_sdk10
echo       ERROR: .NET is installed, but the .NET 10 SDK is missing.
echo       Installed SDKs:
dotnet --list-sdks
echo       Install the .NET 10 SDK: https://dotnet.microsoft.com/download/dotnet/10.0
goto :fail

:build_failed
echo       ERROR: the backend did not build. Read the messages above.
goto :fail

:api_timeout
echo       ERROR: the API did not start in time.
echo       Look at the "ShopFlow API" window to see the error.
goto :fail

:web_timeout
echo       ERROR: the frontend server did not start.
echo       Look at the "ShopFlow Frontend" window to see the error.
goto :fail

:fail
echo.
pause
exit /b 1
