@echo off
setlocal EnableExtensions
cd /d "%~dp0"

if not "%~1"=="" goto run

for /f %%a in ('echo prompt $E^| cmd') do set "ESC=%%a"

echo.
echo   %ESC%[36mvischeck%ESC%[0m
echo.
echo   maps
echo     %ESC%[36m1%ESC%[0m  official
echo     %ESC%[36m2%ESC%[0m  both
echo.
set /p MAPS=  select [1] 
if "%MAPS%"=="" set "MAPS=1"
if "%MAPS%"=="1" (
  set "SCOPE="
) else if "%MAPS%"=="2" (
  set "SCOPE=--workshop"
) else (
  echo   %ESC%[31munknown maps choice%ESC%[0m
  goto end
)

echo.
echo   files
echo     %ESC%[36m1%ESC%[0m  tri
echo     %ESC%[36m2%ESC%[0m  vphys
echo     %ESC%[36m3%ESC%[0m  both
echo.
set /p FILES=  select [1] 
if "%FILES%"=="" set "FILES=1"
if "%FILES%"=="1" (
  set "FORMAT=--tri"
) else if "%FILES%"=="2" (
  set "FORMAT=--vphys"
) else if "%FILES%"=="3" (
  set "FORMAT=--tri --vphys"
) else (
  echo   %ESC%[31munknown files choice%ESC%[0m
  goto end
)

echo.
set "ARGS=%SCOPE% %FORMAT%"
dotnet run --project src -- %ARGS%
goto end

:run
dotnet run --project src -- %*

:end
echo.
pause
