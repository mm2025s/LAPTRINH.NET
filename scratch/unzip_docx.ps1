param([string]$FilePath = "D:\LAPTRINH.NET\lap3\BaoCao\BaoCao_Lab03_24000407.docx", [string]$OutDir = "d:\LAPTRINH.NET\scratch\docxsample")

Add-Type -AssemblyName System.IO.Compression.FileSystem
if (Test-Path $OutDir) { Remove-Item $OutDir -Recurse -Force }
New-Item -ItemType Directory -Path $OutDir | Out-Null
[System.IO.Compression.ZipFile]::ExtractToDirectory($FilePath, $OutDir)
Get-ChildItem $OutDir -Recurse -File | Select-Object @{n='Path';e={$_.FullName.Substring($OutDir.Length+1)}}, Length | Format-Table -AutoSize
