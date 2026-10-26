param([string]$Exe = "d:\LAPTRINH.NET\scratch\build5\Lab05_MultiForm.exe", [int]$TimeoutMs = 180000)

$before = @{}
if (Test-Path "d:\LAPTRINH.NET\Lap5\BaoCao") {
    Get-ChildItem "d:\LAPTRINH.NET\Lap5\BaoCao" | ForEach-Object { $before[$_.Name] = $_.LastWriteTime }
}

$p = Start-Process -FilePath $Exe -ArgumentList "--screenshots" -PassThru -WindowStyle Hidden
$ok = $p.WaitForExit($TimeoutMs)
if ($ok) { Write-Output ("Exited=True Code=" + $p.ExitCode) }
else { Stop-Process -Id $p.Id -Force -ErrorAction SilentlyContinue; Write-Output "Exited=False (killed)" }

$dir = "d:\LAPTRINH.NET\Lap5\BaoCao"
Write-Output ("Thu muc ton tai: " + (Test-Path $dir))
if (Test-Path $dir) {
    Get-ChildItem $dir | Sort-Object Name | ForEach-Object {
        $cu = $before.ContainsKey($_.Name)
        Write-Output ("  {0}  {1,8} bytes  {2}  {3}" -f $_.Name, $_.Length, $_.LastWriteTime.ToString("HH:mm:ss"), $(if ($cu) { "(cu - khong ghi de)" } else { "MOI" }))
    }
}
