param(
    [string]$InstallPath = "C:\Apps\FIFO",
    [int]$WaitSeconds = 10
)

$ErrorActionPreference = "Stop"

$exe = Join-Path $InstallPath "FifoCleanup.exe"
$config = Join-Path $InstallPath "fifo_config.json"
$bitacoraDir = Join-Path $InstallPath "bitacora"

if (-not (Test-Path $exe)) { throw "No existe ejecutable: $exe" }
if (-not (Test-Path $config)) { throw "No existe configuración: $config" }
if (-not (Test-Path $bitacoraDir)) { throw "No existe carpeta bitácora: $bitacoraDir" }

Write-Host "Iniciando aplicación para prueba..."
$p = Start-Process -FilePath $exe -WorkingDirectory $InstallPath -PassThru
Start-Sleep -Seconds $WaitSeconds

$csv = Get-ChildItem -Path $bitacoraDir -Filter "bitacora_*.csv" -ErrorAction SilentlyContinue |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1

if ($null -eq $csv) {
    try { Stop-Process -Id $p.Id -Force -ErrorAction SilentlyContinue } catch {}
    throw "Smoke test falló: no se generó archivo de bitácora."
}

Write-Host "Smoke test OK. Bitácora detectada: $($csv.FullName)"

try { Stop-Process -Id $p.Id -Force -ErrorAction SilentlyContinue } catch {}