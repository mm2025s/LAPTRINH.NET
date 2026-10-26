REM ============================================================
REM  Lap3 + Lap4 runner (dung cho VS 18 dang chay tren may)
REM  - Build lai 2 project bang MSBuild
REM  - Mo Lap4 (WinForms) va Lap3 (Console)
REM ============================================================
@echo off
setlocal

set MSBUILD="C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe"

echo [1/4] Build Lap3...
%MSBUILD% "d:\LAPTRINH.NET\lap3\Lab03_OOPAdvanced.csproj" -t:Build -p:RestorePackagesConfig=false -v:minimal -nologo
if errorlevel 1 goto :fail

echo [2/4] Build Lap4...
%MSBUILD% "d:\LAPTRINH.NET\Lap4\Lab04_WinFormsBasic.csproj" -t:Build -v:minimal -nologo
if errorlevel 1 goto :fail

echo [3/4] Mo Lap4 WinForms...
start "" "d:\LAPTRINH.NET\Lap4\bin\Debug\Lab04_WinFormsBasic.exe"

echo [4/4] Mo Lap3 Console...
start "" "d:\LAPTRINH.NET\lap3\bin\Debug\net8.0\Lab03_OOPAdvanced.exe"

echo.
echo Xong. Nhap phim de dong cua so nay.
pause
exit /b 0

:fail
echo.
echo BUILD LOI. Xem thong bao phia tren.
pause
exit /b 1
