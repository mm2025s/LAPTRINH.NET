# Scratch build script: bien dich Lab 7 bang csc .NET Framework 4.8 (khong can MSBuild/NuGet)
#   powershell -File scratch\build_lap7.ps1 [-Run] [-Shots]
#
# Luu y: truoc khi chay -Shots, phai tao CSDL QLSinhVien truoc:
#   powershell -File scratch\build_lap7.ps1 -Db     (tao/ghien lai CSDL tren LocalDB)
param(
    [switch]$Run,
    [switch]$Shots,
    [switch]$Db
)

$ErrorActionPreference = 'Stop'
$root  = 'd:\LAPTRINH.NET'
$src   = Join-Path $root 'Lap7'
$out   = Join-Path $root 'scratch\build7'
$csc   = 'C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe'
$bld   = Join-Path $root 'scratch\SolutionBuilder\SolutionBuilder.exe'
$sql   = Join-Path $src 'SQL\Lab07_QLSinhVien.sql'

New-Item -ItemType Directory -Force -Path $out | Out-Null

# ----------------------------------------------------------------- F1. CSDL
if ($Db) {
    $sqllocaldb = 'C:\Program Files\Microsoft SQL Server\170\Tools\Binn\SqlLocalDB.exe'
    if (Test-Path $sqllocaldb) { & $sqllocaldb start MSSQLLocalDB | Write-Host }
    Write-Host "==> Chay $sql tren (localdb)\MSSQLLocalDB ..."
    & sqlcmd -S '(localdb)\MSSQLLocalDB' -E -i $sql -f 65001 | Out-String | Write-Host
    if ($LASTEXITCODE -ne 0) { throw 'TAO CSDL THAT BAI' }
    Write-Host '==> CSDL QLSinhVien da san sang.'
    if (-not ($Run -or $Shots)) { return }
}

# ----------------------------------------------------------------- F2. Gom nguon
$files = Get-ChildItem $src -Recurse -Include *.cs |
         Where-Object { $_.FullName -notmatch '\\(obj|bin)\\' } |
         ForEach-Object { $_.FullName.Substring($src.Length + 1).Replace('\', '/') } |
         Sort-Object
$list = Join-Path $root 'scratch\lap7_files.txt'
$files | Out-File -Encoding utf8 $list

# ----------------------------------------------------------------- F3. Ghep thanh 1 file
$all = Join-Path $root 'scratch\lap7_all.cs'
& $bld $src $list > $all
if ($LASTEXITCODE -ne 0) { throw 'Ghep file nguon that bai' }

# ----------------------------------------------------------------- F4. Bien dich
$refs = @(
    '/r:System.dll', '/r:System.Core.dll', '/r:System.Drawing.dll',
    '/r:System.Windows.Forms.dll', '/r:System.Data.dll', '/r:System.Xml.dll',
    '/r:System.Configuration.dll'
)
& $csc /nologo /target:winexe /utf8output /codepage:65001 /nowarn:414 `
      /out:"$out\Lab07_ADONET_Connection.exe" @refs $all 2>&1 | Out-String | Write-Host

if (-not (Test-Path "$out\Lab07_ADONET_Connection.exe")) { throw 'BIEN DICH LOI' }
Write-Host "BUILD OK -> $out\Lab07_ADONET_Connection.exe"

# Copy App.config de ConfigurationManager doc duoc chuoi ket noi
Copy-Item (Join-Path $src 'App.config') (Join-Path $out 'Lab07_ADONET_Connection.exe.config') -Force

# ----------------------------------------------------------------- F5. Chay
if ($Shots) {
    & "$out\Lab07_ADONET_Connection.exe" --screenshots
}
elseif ($Run) {
    Start-Process "$out\Lab07_ADONET_Connection.exe"
}
