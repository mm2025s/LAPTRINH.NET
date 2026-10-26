param([int]$TimeoutMs = 180000)

Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing

$exe = "d:\LAPTRINH.NET\scratch\build5\Lab05_MultiForm.exe"
$asm = [System.Reflection.Assembly]::LoadFrom($exe)
$t = $asm.GetType("Lab05_MultiForm.BaoCaoTools.TrinhChupAnh")
$chay = $t.GetMethod("Chay", [System.Reflection.BindingFlags]"Public,Static")

$sw = [Diagnostics.Stopwatch]::StartNew()
try {
    $chay.Invoke($null, @())
} catch {
    Write-Output ("EXC: " + $_.Exception.GetType().Name + " - " + $_.Exception.Message)
} finally {
    Write-Output ("Chay() ket thuc sau " + $sw.ElapsedMilliseconds + " ms")
}

$dir = "d:\LAPTRINH.NET\Lap5\BaoCao"
if (Test-Path $dir) {
    Get-ChildItem $dir | Sort-Object Name | ForEach-Object {
        Write-Output ("  {0}  {1} bytes  {2}" -f $_.Name, $_.Length, $_.LastWriteTime.ToString("HH:mm:ss"))
    }
}
