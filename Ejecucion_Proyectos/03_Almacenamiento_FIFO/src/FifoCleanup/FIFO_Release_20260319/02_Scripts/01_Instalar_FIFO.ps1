param(
    [string]$SourceReleasePath = "",
    [string]$InstallPath = "C:\Apps\FIFO"
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($SourceReleasePath)) {
    throw "Debe indicar -SourceReleasePath"
}

$appSource = Join-Path $SourceReleasePath "01_App"
if (-not (Test-Path $appSource)) {
    throw "No existe la carpeta de aplicación: $appSource"
}

$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$backupRoot = Join-Path $SourceReleasePath "04_Backup_Original"
New-Item -ItemType Directory -Force -Path $backupRoot | Out-Null

if (Test-Path $InstallPath) {
    $backupPath = Join-Path $backupRoot ("FIFO_Backup_" + $timestamp)
    Copy-Item -Path $InstallPath -Destination $backupPath -Recurse -Force
}

New-Item -ItemType Directory -Force -Path $InstallPath | Out-Null
Copy-Item -Path (Join-Path $appSource "*") -Destination $InstallPath -Recurse -Force

$exePath = Join-Path $InstallPath "FifoCleanup.exe"
if (Test-Path $exePath) {
    try {
        $taskName = "FIFO_AutoStart"
        $action = New-ScheduledTaskAction -Execute $exePath -WorkingDirectory $InstallPath
        $trigger = New-ScheduledTaskTrigger -AtStartup
        $principal = New-ScheduledTaskPrincipal -UserId "SYSTEM" -LogonType ServiceAccount -RunLevel Highest

        Register-ScheduledTask -TaskName $taskName -Action $action -Trigger $trigger -Principal $principal -Force | Out-Null
        Write-Host "Tarea programada registrada: $taskName (inicio automático al reiniciar)"
    }
    catch {
        Write-Warning "No se pudo registrar la tarea FIFO_AutoStart. Ejecute el script como Administrador. Detalle: $($_.Exception.Message)"
    }
}

Write-Host "Instalación completada en: $InstallPath"