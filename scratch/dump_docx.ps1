param([string]$Src = "d:\LAPTRINH.NET\scratch\docxsample\word\document.xml", [string]$Out = "d:\LAPTRINH.NET\scratch\docxsample\document_pretty.xml")

[xml]$xml = Get-Content $Src -Raw -Encoding UTF8
$nsm = New-Object System.Xml.XmlNamespaceManager($xml.NameTable)
$nsm.AddNamespace('w', 'http://schemas.openxmlformats.org/wordprocessingml/2006/main')

$lines = New-Object System.Collections.Generic.List[string]
$i = 0
foreach ($node in $xml.document.body.ChildNodes) {
    if ($node.LocalName -eq 'p') {
        $ps = $node.SelectSingleNode('w:pPr/w:pStyle', $nsm)
        $style = if ($ps) { $ps.GetAttribute('val', $nsm.LookupNamespace('w')) } else { '' }
        $text = ''
        foreach ($t in $node.SelectNodes('.//w:t', $nsm)) { $text += $t.InnerText }
        $hasImg = ($node.SelectNodes('.//*[local-name()="drawing"]').Count -gt 0)
        $pb = ($node.SelectNodes('.//w:br[@w:type="page"]', $nsm).Count -gt 0)
        $mark = if ($hasImg) { '[IMG]' } elseif ($pb) { '[PAGEBREAK]' } else { '' }
        $lines.Add(("[$i] " + $style + " " + $mark + " | " + $text))
    }
    elseif ($node.LocalName -eq 'tbl') {
        $lines.Add("[$i] <TABLE>")
    }
    elseif ($node.LocalName -eq 'sectPr') {
        $lines.Add('--- sectPr ---')
    }
    $i++
}
$lines | Set-Content $Out -Encoding UTF8
Write-Output ("So phan tu: " + $i)
