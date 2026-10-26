param([string]$Docx)
Add-Type -AssemblyName System.IO.Compression.FileSystem

function Get-Entry($zip, $name) {
    $e = $zip.GetEntry($name)
    if ($e -eq $null) { $e = $zip.GetEntry($name.Replace('/', '\')) }
    if ($e -eq $null) { $e = $zip.GetEntry($name.Replace('\', '/')) }
    return $e
}

function Read-Entry($zip, $name) {
    $e = Get-Entry $zip $name
    if ($e -eq $null) { throw ("Khong tim thay entry: " + $name) }
    $sr = New-Object System.IO.StreamReader($e.Open())
    $txt = $sr.ReadToEnd()
    $sr.Close()
    return $txt
}

$zip = [System.IO.Compression.ZipFile]::OpenRead($Docx)
$names = @()
$media = @()
foreach ($e in $zip.Entries) {
    $names += $e.FullName
    if ($e.FullName -like '*.xml' -or $e.FullName -like '*.rels') {
        $txt = Read-Entry $zip $e.FullName
        try {
            $x = New-Object System.Xml.XmlDocument
            $x.LoadXml($txt)
            Write-Output ("XML OK   " + $e.FullName + "  (" + $txt.Length + " chars)")
        } catch {
            Write-Output ("XML FAIL " + $e.FullName + " :: " + $_.Exception.Message)
        }
    } else {
        $media += $e.FullName
        Write-Output ("BIN      " + $e.FullName + "  (" + $e.Length + " bytes)")
    }
}

$docXml = Read-Entry $zip 'word/document.xml'
$rels = Read-Entry $zip 'word/_rels/document.xml.rels'
$zip.Dispose()

$embeds = [regex]::Matches($docXml, 'r:embed="([^"]+)"') | ForEach-Object { $_.Groups[1].Value }
Write-Output ("Anh chen trong document: " + $embeds.Count + " / media: " + $media.Count)
foreach ($id in $embeds) {
    $m = [regex]::Match($rels, 'Id="' + $id + '"[^>]*Target="([^"]+)"')
    $ok = "KHONG TIM THAY REL"
    if ($m.Success) {
        $target = $m.Groups[1].Value
        $found = $false
        foreach ($n in $names) { if ($n -eq ('word/' + $target) -or $n -eq ('word\' + $target)) { $found = $true } }
        if ($found) { $ok = "OK " + $target } else { $ok = "THIEU FILE " + $target }
    }
    Write-Output ("  " + $id + " -> " + $ok)
}
Write-Output ("So lan thieu anh (fallback): " + ([regex]::Matches($docXml, '\[Thi.u .nh')).Count)
Write-Output ("So doan van: " + ([regex]::Matches($docXml, '<w:p>|<w:p ')).Count)
Write-Output ("So bang: " + ([regex]::Matches($docXml, '<w:tbl>')).Count)
