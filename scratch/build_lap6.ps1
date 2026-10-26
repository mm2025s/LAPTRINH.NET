# Scratch build script: bien dich Lab 6 bang csc .NET Framework 4.8 (khong can MSBuild/NuGet)
#   powershell -File scratch\build_lap6.ps1 [-Run] [-Shots]
param(
    [switch]$Run,
    [switch]$Shots
)

$ErrorActionPreference = 'Stop'
$root  = 'd:\LAPTRINH.NET'
$src   = Join-Path $root 'Lap6'
$out   = Join-Path $root 'scratch\build6'
$csc   = 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe'
$bld   = Join-Path $root 'scratch\SolutionBuilder\SolutionBuilder.exe'

New-Item -ItemType Directory -Force -Path $out | Out-Null

# 1. Gom danh sach file nguon
$files = Get-ChildItem $src -Recurse -Include *.cs |
         Where-Object { $_.FullName -notmatch '\\(obj|bin)\\' } |
         ForEach-Object { $_.FullName.Substring($src.Length + 1).Replace('\', '/') } |
         Sort-Object
$list = Join-Path $root 'scratch\lap6_files.txt'
$files | Out-File -Encoding utf8 $list

# 2. Ghep thanh 1 file
$all = Join-Path $root 'scratch\lap6_all.cs'
& $bld $src $list > $all
if ($LASTEXITCODE -ne 0) { throw 'Ghep file nguon that bai' }

# 3. Bien dich
$refs = @(
    '/r:System.dll', '/r:System.Core.dll', '/r:System.Drawing.dll',
    '/r:System.Windows.Forms.dll', '/r:System.Data.dll', '/r:System.Xml.dll'
)
& $csc /nologo /target:winexe /utf8output /codepage:65001 /nowarn:414 `
      /out:"$out\Lab06_ThreeLayer.exe" @refs $all 2>&1 | Out-String | Write-Host

if (-not (Test-Path "$out\Lab06_ThreeLayer.exe")) { throw 'BIEN DICH LOI' }
Write-Host "BUILD OK -> $out\Lab06_ThreeLayer.exe"

# 4. Chay
if ($Shots) {
    & "$out\Lab06_ThreeLayer.exe" --screenshots
}
elseif ($Run) {
    Start-Process "$out\Lab06_ThreeLayer.exe"
}
