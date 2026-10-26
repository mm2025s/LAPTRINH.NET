$dir = "d:\LAPTRINH.NET\Lap5\Forms"
Get-ChildItem $dir -Filter *.Designer.cs | ForEach-Object {
    $txt = [IO.File]::ReadAllText($_.FullName)
    $m = [regex]::Matches($txt, 'Load \+= (\w+);')
    $ten = @()
    foreach ($x in $m) { $ten += $x.Groups[1].Value }
    Write-Output ($_.Name + " -> " + ($ten -join ","))
}
