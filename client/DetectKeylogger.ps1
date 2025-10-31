# Keylogger Detection and Removal Script

Write-Host "🔍 KEYLOGGER DETECTION SCRIPT" -ForegroundColor Yellow
Write-Host "=" * 50

# 1. Process Analysis
Write-Host "`n1. 🔍 SUSPICIOUS PROCESSES:" -ForegroundColor Green
$suspiciousProcesses = Get-Process | Where-Object {
    $_.ProcessName -like "*System*" -or 
    $_.ProcessName -like "*Security*" -or 
    $_.ProcessName -like "*Update*" -or
    $_.ProcessName -like "*Defender*" -or
    $_.Path -like "*Temp*" -or 
    $_.Path -like "*AppData*"
}

foreach ($proc in $suspiciousProcesses) {
    Write-Host "   PID: $($proc.Id) | Name: $($proc.ProcessName) | Path: $($proc.Path)" -ForegroundColor Red
}

# 2. Network Connections
Write-Host "`n2. 🌐 SUSPICIOUS NETWORK CONNECTIONS:" -ForegroundColor Green
$connections = Get-NetTCPConnection | Where-Object {$_.State -eq "Established"} 
foreach ($conn in $connections) {
    $process = Get-Process -Id $conn.OwningProcess -ErrorAction SilentlyContinue
    if ($process) {
        Write-Host "   $($conn.LocalAddress):$($conn.LocalPort) -> $($conn.RemoteAddress):$($conn.RemotePort) | Process: $($process.ProcessName)" -ForegroundColor Yellow
    }
}

# 3. Registry Persistence
Write-Host "`n3. 🔑 REGISTRY PERSISTENCE CHECK:" -ForegroundColor Green
$runKeys = @(
    "HKCU:\Software\Microsoft\Windows\CurrentVersion\Run",
    "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Run"
)

foreach ($key in $runKeys) {
    Write-Host "   Checking: $key" -ForegroundColor Cyan
    try {
        $entries = Get-ItemProperty -Path $key -ErrorAction Stop
        $entries.PSObject.Properties | ForEach-Object {
            if ($_.Name -notmatch "PS.*" -and $_.Value -match "(System|Security|Update|Defender)") {
                Write-Host "      SUSPICIOUS: $($_.Name) = $($_.Value)" -ForegroundColor Red
            }
        }
    } catch {
        Write-Host "      Error accessing registry key" -ForegroundColor Red
    }
}

# 4. Startup Folder
Write-Host "`n4. 📂 STARTUP FOLDER CHECK:" -ForegroundColor Green
$startupPath = "$env:APPDATA\Microsoft\Windows\Start Menu\Programs\Startup"
$startupFiles = Get-ChildItem $startupPath -ErrorAction SilentlyContinue
foreach ($file in $startupFiles) {
    if ($file.Name -match "(System|Security|Update|Defender)") {
        Write-Host "   SUSPICIOUS STARTUP FILE: $($file.FullName)" -ForegroundColor Red
    }
}

# 5. Scheduled Tasks
Write-Host "`n5. ⏰ SCHEDULED TASKS CHECK:" -ForegroundColor Green
$tasks = schtasks /query /fo csv | ConvertFrom-Csv | Where-Object {
    $_.TaskName -match "(System|Security|Update|Defender)" -and
    $_.TaskName -notmatch "Microsoft.*Windows"
}
foreach ($task in $tasks) {
    Write-Host "   SUSPICIOUS TASK: $($task.TaskName)" -ForegroundColor Red
}

# 6. File System Scan
Write-Host "`n6. 📁 SUSPICIOUS FILES:" -ForegroundColor Green
$suspiciousPaths = @(
    "C:\Windows\Temp",
    "C:\Users\$env:USERNAME\Documents", 
    "C:\Users\$env:USERNAME\AppData\Roaming\Microsoft\Defender",
    "C:\ProgramData"
)

foreach ($path in $suspiciousPaths) {
    Write-Host "   Scanning: $path" -ForegroundColor Cyan
    try {
        $files = Get-ChildItem $path -Recurse -Force -ErrorAction SilentlyContinue | Where-Object {
            ($_.Name -match "(System|Security|Update|Defender).*\.exe") -and
            ($_.Name -notmatch "Windows.*Update")
        }
        foreach ($file in $files) {
            Write-Host "      SUSPICIOUS FILE: $($file.FullName)" -ForegroundColor Red
        }
    } catch {
        Write-Host "      Error scanning $path" -ForegroundColor Yellow
    }
}

# 7. Log Files Detection
Write-Host "`n7. 📝 ENCRYPTED LOG FILES:" -ForegroundColor Green
$logPatterns = @("*.dat", "*.log", "*keylog*", "*svc*")
foreach ($pattern in $logPatterns) {
    $logFiles = Get-ChildItem C:\ProgramData -Recurse -Include $pattern -Force -ErrorAction SilentlyContinue
    foreach ($log in $logFiles) {
        if ($log.Directory.Name -match "^[0-9a-f]{8}$") {  # GUID pattern
            Write-Host "   SUSPICIOUS LOG: $($log.FullName)" -ForegroundColor Red
        }
    }
}

# 8. Memory Analysis
Write-Host "`n8. 💾 MEMORY USAGE ANALYSIS:" -ForegroundColor Green
$highMemoryProcesses = Get-Process | Where-Object {$_.WorkingSet -gt 50MB} | Sort-Object WorkingSet -Descending | Select-Object -First 10
foreach ($proc in $highMemoryProcesses) {
    $memoryMB = [math]::Round($proc.WorkingSet/1MB, 2)
    if ($proc.ProcessName -match "(System|Security|Update)" -and $proc.ProcessName -notmatch "Windows") {
        Write-Host "   HIGH MEMORY SUSPICIOUS: $($proc.ProcessName) - ${memoryMB}MB" -ForegroundColor Red
    }
}

Write-Host "`n🛡️ DETECTION COMPLETE!" -ForegroundColor Green
Write-Host "Check RED entries for potential keylogger components." -ForegroundColor Yellow

# Optional: Kill suspicious processes
Write-Host "`n❓ Kill suspicious processes? (y/N): " -ForegroundColor Yellow -NoNewline
$response = Read-Host
if ($response -eq 'y' -or $response -eq 'Y') {
    foreach ($proc in $suspiciousProcesses) {
        try {
            Stop-Process -Id $proc.Id -Force
            Write-Host "✅ Killed process: $($proc.ProcessName) (PID: $($proc.Id))" -ForegroundColor Green
        } catch {
            Write-Host "❌ Failed to kill: $($proc.ProcessName)" -ForegroundColor Red
        }
    }
}
