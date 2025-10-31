# Keylogger Cleanup Script
Write-Host "🧹 CLEANING KEYLOGGER TRACES..." -ForegroundColor Red

# 1. Remove Registry Entries
Write-Host "1. Removing Registry Persistence..." -ForegroundColor Yellow
try {
    Remove-ItemProperty -Path "HKCU:\Software\Microsoft\Windows\CurrentVersion\Run" -Name "DefenderSecurity" -ErrorAction SilentlyContinue
    Remove-ItemProperty -Path "HKCU:\Software\Microsoft\Windows\CurrentVersion\Run" -Name "SystemFrameworks" -ErrorAction SilentlyContinue  
    Remove-ItemProperty -Path "HKCU:\Software\Microsoft\Windows\CurrentVersion\Run" -Name "WindowsUpdate" -ErrorAction SilentlyContinue
    Write-Host "   ✅ Registry entries removed" -ForegroundColor Green
} catch {
    Write-Host "   ❌ Error removing registry entries" -ForegroundColor Red
}

# 2. Kill any running processes
Write-Host "2. Killing suspicious processes..." -ForegroundColor Yellow
$processNames = @("LoggerApp", "defender", "SystemUpdate", "SecurityUpdate")
foreach ($procName in $processNames) {
    $processes = Get-Process -Name $procName -ErrorAction SilentlyContinue
    foreach ($proc in $processes) {
        try {
            Stop-Process -Id $proc.Id -Force
            Write-Host "   ✅ Killed: $($proc.ProcessName) (PID: $($proc.Id))" -ForegroundColor Green
        } catch {
            Write-Host "   ❌ Failed to kill: $($proc.ProcessName)" -ForegroundColor Red
        }
    }
}

# 3. Remove files
Write-Host "3. Removing keylogger files..." -ForegroundColor Yellow
$filePaths = @(
    "C:\Windows\Temp\SystemUpdate.exe",
    "C:\Users\$env:USERNAME\Documents\SystemUpdate.exe",
    "C:\Users\$env:USERNAME\AppData\Roaming\Microsoft\Defender\defender.exe",
    "bin\Release\net9.0-windows\win-x64\publish\LoggerApp.exe"
)

foreach ($filePath in $filePaths) {
    if (Test-Path $filePath) {
        try {
            Remove-Item $filePath -Force
            Write-Host "   ✅ Removed: $filePath" -ForegroundColor Green
        } catch {
            Write-Host "   ❌ Failed to remove: $filePath" -ForegroundColor Red
        }
    }
}

# 4. Clean temp directories
Write-Host "4. Cleaning temp directories..." -ForegroundColor Yellow
$tempDirs = @($env:TEMP, "C:\Windows\Temp", "C:\ProgramData")
foreach ($tempDir in $tempDirs) {
    try {
        Get-ChildItem $tempDir -Recurse -Include "*keylog*", "*system*", "*security*" -Force -ErrorAction SilentlyContinue | Remove-Item -Force -Recurse -ErrorAction SilentlyContinue
        Write-Host "   ✅ Cleaned: $tempDir" -ForegroundColor Green
    } catch {
        Write-Host "   ⚠️  Partial cleanup: $tempDir" -ForegroundColor Yellow
    }
}

# 5. Remove scheduled tasks
Write-Host "5. Removing scheduled tasks..." -ForegroundColor Yellow
$taskNames = @("MicrosoftSecurityUpdate", "SystemFrameworkUpdate", "DefenderUpdate")
foreach ($taskName in $taskNames) {
    try {
        Unregister-ScheduledTask -TaskName $taskName -Confirm:$false -ErrorAction SilentlyContinue
        Write-Host "   ✅ Removed task: $taskName" -ForegroundColor Green
    } catch {
        Write-Host "   ⚠️  Task not found: $taskName" -ForegroundColor Yellow
    }
}

Write-Host "`n🎉 CLEANUP COMPLETE!" -ForegroundColor Green
Write-Host "Keylogger traces have been removed from the system." -ForegroundColor Cyan
