@echo off
REM ============================================================
REM  Cai dat Visual Studio 2022 Community
REM  Workload: .NET desktop development
REM  (kem .NET 8 SDK + .NET Framework 4.8 targeting pack)
REM  => Build/run duoc ca Lap3 (net8.0) va Lap4 (net48)
REM
REM  CACH DUNG: chuot phai file nay -> "Run as administrator"
REM ============================================================

set INSTALLER="C:\Program Files (x86)\Microsoft Visual Studio\Installer\setup.exe"

echo Dang mo Visual Studio Installer...
echo Neu hop thoai UAC hien ra, bam YES.

%INSTALLER% install ^
  --productId Microsoft.VisualStudio.Product.Community ^
  --channelId VisualStudio.17.Release ^
  --add Microsoft.VisualStudio.Workload.ManagedDesktop ^
  --includeRecommended ^
  --passive --norestart

echo.
echo Hoan tat (exit code %ERRORLEVEL%).
echo Mo Visual Studio Installer de kiem tra tien do cai dat.
pause
