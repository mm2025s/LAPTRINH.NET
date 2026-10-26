param(
    [string]$Exe = 'd:\LAPTRINH.NET\scratch\build6\Lab06_ThreeLayer.exe',
    [string]$KichBan = ''
)

$ErrorActionPreference = 'Continue'

$args0 = @('--screenshots')
if ($KichBan.Length -gt 0) { $args0 += $KichBan }

$p = Start-Process -FilePath $Exe -ArgumentList $args0 -Wait -PassThru
Write-Host ("ExitCode = " + $p.ExitCode)

$dir = 'd:\LAPTRINH.NET\Lap6\BaoCao'
if (Test-Path $dir) {
    Get-ChildItem $dir | Select-Object Name, Length | Format-Table -AutoSize | Out-String | Write-Host
    $log = Join-Path $dir 'nhat_ky_chup_anh.txt'
    if (Test-Path $log) { Get-Content $log -Encoding UTF8 | Write-Host }
    $err = Join-Path $dir 'chup_anh_loi.txt'
    if (Test-Path $err) { Write-Host '--- chup_anh_loi.txt ---'; Get-Content $err -Encoding UTF8 | Select-Object -First 30 | Write-Host }
}
else {
    Write-Host "Chua co thu muc $dir"
}
