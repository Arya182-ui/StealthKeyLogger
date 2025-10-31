# Anti-Debug Protection Test Script
Write-Host "=== Anti-Debug Protection Test ===" -ForegroundColor Yellow

# Test 1: Normal execution
Write-Host "`n1. Testing Normal Execution:" -ForegroundColor Green
try {
    $process = Start-Process "C:\Windows\Temp\SystemUpdate.exe" -PassThru -WindowStyle Hidden
    Start-Sleep -Seconds 3
    
    if ($process.HasExited) {
        Write-Host "   Process exited quickly (Good - normal behavior)" -ForegroundColor Green
    } else {
        Write-Host "   Process still running (PID: $($process.Id))" -ForegroundColor Green
        $process | Stop-Process -Force
    }
} catch {
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 2: Debug environment simulation
Write-Host "`n2. Testing Debug Environment Detection:" -ForegroundColor Green

# Set debug environment variable
$env:DEBUG = "1"
$env:_DEBUG = "1"

try {
    $process = Start-Process "C:\Windows\Temp\SystemUpdate.exe" -PassThru -WindowStyle Hidden
    Start-Sleep -Seconds 2
    
    if ($process.HasExited) {
        Write-Host "   Process detected debug environment and exited (GOOD!)" -ForegroundColor Green
    } else {
        Write-Host "   Process running despite debug environment (Check needed)" -ForegroundColor Yellow
        $process | Stop-Process -Force
    }
} catch {
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Clean up environment
Remove-Item Env:DEBUG -ErrorAction SilentlyContinue
Remove-Item Env:_DEBUG -ErrorAction SilentlyContinue

# Test 3: Process detection
Write-Host "`n3. Testing Process Detection:" -ForegroundColor Green
$debugProcesses = @("devenv", "windbg", "x64dbg", "ida", "ida64", "ollydbg", "immunitydebugger", "processhacker", "cheatengine")

foreach ($proc in $debugProcesses) {
    $running = Get-Process -Name $proc -ErrorAction SilentlyContinue
    if ($running) {
        Write-Host "   Found analysis tool: $proc (PID: $($running.Id))" -ForegroundColor Red
    }
}

Write-Host "`n=== Test Complete ===" -ForegroundColor Yellow
