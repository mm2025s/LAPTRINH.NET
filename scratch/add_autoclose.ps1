$dir = "d:\LAPTRINH.NET\Lap5\Forms"
$ten = @("frmChiTietSinhVien","frmChonKhoa","frmDangNhap","frmDanhSach","frmGioiThieu","frmKiemTraDuLieu","frmMain","frmSinhVien","frmTimKiem")
$dau = "            // Chế độ chụp ảnh báo cáo (--screenshots): tự đóng Form để bộ chụp chạy tiếp kịch bản sau."
$dong = "            if (Environment.GetEnvironmentVariable(`"LAB5_SCREENSHOT`") == `"1`") { Shown += delegate { BeginInvoke((MethodInvoker)Close); }; }"
foreach ($n in $ten) {
    $path = Join-Path $dir ($n + ".Designer.cs")
    $txt = [IO.File]::ReadAllText($path)
    $mau = "            Load += " + $n + "_Load;"
    if ($txt.Contains($dau)) { Write-Output ($n + " -> da co, bo qua"); continue }
    if (-not $txt.Contains($mau)) { Write-Output ($n + " -> KHONG TIM THAY dong Load"); continue }
    $moi = $mau + "`r`n" + $dau + "`r`n" + $dong
    $txt = $txt.Replace($mau, $moi)
    [IO.File]::WriteAllText($path, $txt, (New-Object System.Text.UTF8Encoding($true)))
    Write-Output ($n + " -> OK")
}
