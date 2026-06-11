# Chạy WebClient (Vite dev server) không cần cd thủ công
$clientDir = Join-Path $PSScriptRoot 'BaseCore.WebClient'

Push-Location $clientDir
try {
    if (-not (Test-Path (Join-Path $clientDir 'node_modules'))) {
        Write-Host 'node_modules chưa có -> chạy npm install...' -ForegroundColor Yellow
        npm install
    }
    npm run dev
}
finally {
    Pop-Location
}
