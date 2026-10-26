param([string]$Exe)

Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing

if (-not $Exe) { $Exe = "d:\LAPTRINH.NET\scratch\build5\Lab05_MultiForm.exe" }

$asm = [System.Reflection.Assembly]::LoadFrom($Exe)
$t = $asm.GetType("Lab05_MultiForm.BaoCaoTools.TrinhChupAnh")
if ($t -eq $null) {
    $asm.GetTypes() | Where-Object { $_.Name -like "*Trinh*" } | ForEach-Object { Write-Output ("Type: " + $_.FullName) }
    return
}

$m = $t.GetMethod("TimThuMucBaoCao", [System.Reflection.BindingFlags]"NonPublic,Static")
Write-Output ("TimThuMucBaoCao() = " + $m.Invoke($null, @()))
Write-Output ("BaseDirectory   = " + [AppDomain]::CurrentDomain.BaseDirectory)
$f = $t.GetMethod("Chay", [System.Reflection.BindingFlags]"Public,Static")
$f.Invoke($null, @()) | Out-Null
$m2 = $t.GetField("_thuMucAnh", [System.Reflection.BindingFlags]"NonPublic,Static").GetValue($null)
Write-Output ("_thuMucAnh sau khi Chay = " + $m2)
