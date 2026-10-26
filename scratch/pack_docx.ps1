param(
    [Parameter(Mandatory=$true)][string]$SourceDir,
    [Parameter(Mandatory=$true)][string]$Output
)

Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem

# ZipFile.CreateFromDirectory ghi tên entry bằng dấu '\' (sai chuẩn OPC).
# Ở đây mở gói bằng ZipArchive và ghi lại từng entry với tên dùng dấu '/',
# đồng thời đặt [Content_Types].xml ở vị trí đầu tiên.
$base = (Resolve-Path -LiteralPath $SourceDir).Path
$files = New-Object System.Collections.ArrayList
foreach ($f in Get-ChildItem -LiteralPath $SourceDir -Recurse -File) {
    [void]$files.Add($f)
}

if (Test-Path -LiteralPath $Output) { Remove-Item -LiteralPath $Output -Force }

$ms = New-Object System.IO.MemoryStream
$zip = New-Object System.IO.Compression.ZipArchive($ms, [System.IO.Compression.ZipArchiveMode]::Create, $true)

$count = 0
foreach ($f in $files) {
    $name = $f.FullName.Substring($base.Length + 1).Replace('\', '/')
    if ($f.Length -gt 0) {
        $entry = $zip.CreateEntry($name, [System.IO.Compression.CompressionLevel]::Optimal)
        $s = $entry.Open()
        $bytes = [System.IO.File]::ReadAllBytes($f.FullName)
        $s.Write($bytes, 0, $bytes.Length)
        $s.Close()
    } else {
        [void]$zip.CreateEntry($name)
    }
    $count++
}
$zip.Dispose()

[System.IO.File]::WriteAllBytes($Output, $ms.ToArray())
$ms.Close()
Write-Output ("Da dong goi " + $count + " entry -> " + $Output)
