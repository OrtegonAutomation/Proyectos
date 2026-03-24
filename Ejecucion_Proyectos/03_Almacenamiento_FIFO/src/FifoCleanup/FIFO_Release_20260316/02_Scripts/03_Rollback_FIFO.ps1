param(
    [string]$ReleasePath = "",
    [string]$InstallPath = "C:\Apps\FIFO"
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($ReleasePath)) {
    throw "Debe indicar -ReleasePath"
}

$backupRoot = Join-Path $ReleasePath "04_Backup_Original"
if (-not (Test-Path $backupRoot)) { throw "No existe carpeta de backups: $backupRoot" }

$lastBackup = Get-ChildItem -Path $backupRoot -Directory |
    Sort-Object LastWriteTime -Descending |
    Select-Object -First 1

if ($null -eq $lastBackup) { throw "No hay backups disponibles para rollback." }

if (Test-Path $InstallPath) {
    Remove-Item -Path $InstallPath -Recurse -Force
}

Copy-Item -Path $lastBackup.FullName -Destination $InstallPath -Recurse -Force
Write-Host "Rollback completado desde: $($lastBackup.FullName)"