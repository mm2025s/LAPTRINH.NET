param([string]$Docx = 'd:\LAPTRINH.NET\Lap6\BaoCao\BaoCao_Lab06_24000407.docx')

Add-Type -AssemblyName System.IO.Compression.FileSystem
Add-Type -AssemblyName System.Drawing

# Trích các ảnh trong gói ra thư mục tạm và phân tích số màu (phát hiện ảnh trống)
$tmp = Join-Path $env:TEMP ('docxcheck_' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force -Path $tmp | Out-Null

$zip = [System.IO.Compression.ZipFile]::OpenRead($Docx)
foreach ($e in $zip.Entries) {
    if ($e.FullName -like '*media*') {
        $ten = [System.IO.Path]::GetFileName($e.FullName)
        $dich = Join-Path $tmp $ten
        [System.IO.Compression.ZipFileExtensions]::ExtractToFile($e, $dich, $true)
    }
}
$zip.Dispose()

Get-ChildItem $tmp -Filter *.png | Sort-Object Name | ForEach-Object {
    $bmp = [System.Drawing.Bitmap]::FromFile($_.FullName)
    try {
        $mau = @{}
        for ($y = 0; $y -lt $bmp.Height; $y += 5) {
            for ($x = 0; $x -lt $bmp.Width; $x += 5) { $mau[$bmp.GetPixel($x, $y).ToArgb()] = 1 }
        }
        Write-Output ("{0,-16} {1,5}x{2,-5} so mau: {3,4}" -f $_.Name, $bmp.Width, $bmp.Height, $mau.Count)
    }
    finally { $bmp.Dispose() }
}
Remove-Item $tmp -Recurse -Force
