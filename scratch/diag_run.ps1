# =====================================================================
#  CHẨN ĐOÁN BỘ CHỤP ẢNH LAB 5
#  Ghi lại mọi thứ ra file để biết tiến trình dừng ở đâu.
# =====================================================================
param([string]$Exe = "d:\LAPTRINH.NET\scratch\build5\Lab05_MultiForm.exe")

$log = "d:\LAPTRINH.NET\scratch\diag_run.txt"
"=== BAT DAU $(Get-Date -Format 'HH:mm:ss') ===" | Out-File $log -Encoding UTF8

$psi = New-Object System.Diagnostics.ProcessStartInfo
$psi.FileName = $Exe
$psi.Arguments = "--screenshots"
$psi.UseShellExecute = $false
$psi.RedirectStandardOutput = $true
$psi.RedirectStandardError = $true
$psi.CreateNoWindow = $true
$psi.WorkingDirectory = Split-Path $Exe

$p = New-Object System.Diagnostics.Process
$p.StartInfo = $psi
$p.Start() | Out-Null

$out = $p.StandardOutput.ReadToEndAsync()
$err = $p.StandardError.ReadToEndAsync()

if ($p.WaitForExit(90000)) {
    "THOAT: code=$($p.ExitCode)" | Out-File $log -Append -Encoding UTF8
} else {
    "TIMEOUT - kill" | Out-File $log -Append -Encoding UTF8
    try { $p.Kill() } catch { }
}

# Cho luong doc output ket thuc
$out.Wait(5000) | Out-Null
$err.Wait(5000) | Out-Null
"--- STDOUT ---" | Out-File $log -Append -Encoding UTF8
$out.Result | Out-File $log -Append -Encoding UTF8
"--- STDERR ---" | Out-File $log -Append -Encoding UTF8
$err.Result | Out-File $log -Append -Encoding UTF8
"=== KET THUC $(Get-Date -Format 'HH:mm:ss') ===" | Out-File $log -Append -Encoding UTF8

Get-ChildItem "d:\LAPTRINH.NET\Lap5\BaoCao" -ErrorAction SilentlyContinue |
    ForEach-Object { "  $($_.Name)  $($_.Length)  $($_.LastWriteTime.ToString('HH:mm:ss'))" } |
    Out-File $log -Append -Encoding UTF8
