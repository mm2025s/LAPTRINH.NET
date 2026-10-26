# Phan tich anh chup Lab 6: do phan tram pixel khong phai nen de phat hien anh "trong"
param([string]$Dir = 'd:\LAPTRINH.NET\Lap6\BaoCao')

Add-Type -AssemblyName System.Drawing

Get-ChildItem $Dir -Filter *.png | Sort-Object Name | ForEach-Object {
    $bmp = [System.Drawing.Bitmap]::FromFile($_.FullName)
    try {
        $w = $bmp.Width; $h = $bmp.Height
        $mau = @{}
        $buoc = 3
        $tong = 0
        for ($y = 0; $y -lt $h; $y += $buoc) {
            for ($x = 0; $x -lt $w; $x += $buoc) {
                $c = $bmp.GetPixel($x, $y).ToArgb()
                if ($mau.ContainsKey($c)) { $mau[$c] = $mau[$c] + 1 } else { $mau[$c] = 1 }
                $tong++
            }
        }
        $sapXep = $mau.GetEnumerator() | Sort-Object -Property Value -Descending
        $nen = $sapXep | Select-Object -First 1
        $ti = [math]::Round(100.0 * (1 - ($nen.Value / $tong)), 1)
        $mauKhac = $sapXep.Count
        Write-Output ("{0,-30} {1,5}x{2,-5} mau khac nhau: {3,5}  pixel khong phai nen: {4,5}%" -f $_.Name, $w, $h, $mauKhac, $ti)
    }
    finally { $bmp.Dispose() }
}
