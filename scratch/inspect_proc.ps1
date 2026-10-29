$p = Get-Process Lab05_MultiForm -ErrorAction SilentlyContinue
if (-not $p) { "Khong co tien trinh" ; return }
"PID=$($p.Id) Threads=$($p.Threads.Count)"
foreach ($t in $p.Threads) {
    "  thread $($t.Id) state=$($t.ThreadState) wait=$($t.WaitReason)"
}
"CPU=" + $p.TotalProcessorTime.TotalSeconds
$ws = [math]::Round($p.WorkingSet64 / 1MB, 1)
"WorkingSet(MB)=$ws"
