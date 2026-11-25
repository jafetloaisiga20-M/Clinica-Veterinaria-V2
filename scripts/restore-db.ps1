#Requires -Version 5.1
<#
.SYNOPSIS
    Restaura un respaldo BACPAC en una base de datos destino.
.DESCRIPTION
    Por seguridad se debe restaurar en una base NUEVA (por ejemplo
    Veterinaria_Restore) y luego apuntar la API ahi. No sobrescribas la base
    de produccion sin antes verificar el respaldo.
.EXAMPLE
    .\scripts\restore-db.ps1 `
      -BacpacFile ".\backups\Veterinaria_SaludAnimal-20261123-120000.bacpac" `
      -TargetConnectionString "Server=.;Database=Veterinaria_Restore;Integrated Security=True;TrustServerCertificate=True" `
      -Force
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$BacpacFile,
    [Parameter(Mandatory)][string]$TargetConnectionString,
    [string]$SqlPackage = "",
    [switch]$Force
)

$ErrorActionPreference = "Stop"

# DacFx 162 esta compilado para .NET 8. Si la maquina solo tiene .NET 10,
# esto permite que sqlpackage se ejecute igual (roll-forward de runtime).
$env:DOTNET_ROLL_FORWARD = "Major"

if (-not (Test-Path $BacpacFile)) {
    throw "No existe el archivo: $BacpacFile"
}

if (-not $Force) {
    Write-Host "ATENCION: se importara el respaldo sobre la base indicada en TargetConnectionString." -ForegroundColor Red
    Write-Host "(Sin la opcion -Force se cancelara esta operacion)." -ForegroundColor Red
    $answer = Read-Host "Escribe RESTAURAR para continuar"
    if ($answer -ne "RESTAURAR") {
        Write-Host "Cancelado." -ForegroundColor Yellow
        exit 0
    }
}

$hashFile = "$BacpacFile.sha256"
if (Test-Path $hashFile) {
    $expected = (Get-Content $hashFile | Select-Object -First 1).Split(" ")[0]
    $actual = (Get-FileHash -Algorithm SHA256 -Path $BacpacFile).Hash
    if ($expected -ne $actual) {
        throw "El SHA256 no coincide. El respaldo esta corrupto o fue modificado."
    }
    Write-Host ">> SHA256 verificado." -ForegroundColor Green
}

if (-not $SqlPackage) {
    $command = Get-Command sqlpackage -ErrorAction SilentlyContinue
    if (-not $command) {
        throw "sqlpackage no esta instalado. Ejecuta: dotnet tool install --global microsoft.sqlpackage"
    }
    $SqlPackage = $command.Source
}

Write-Host ">> Importando respaldo..." -ForegroundColor Cyan
& $SqlPackage "/Action:Import" `
    "/SourceFile:$BacpacFile" `
    "/TargetConnectionString:$TargetConnectionString"

if ($LASTEXITCODE -ne 0) {
    throw "La restauracion fallo (codigo $LASTEXITCODE)."
}

Write-Host ""
Write-Host "Respaldo restaurado correctamente." -ForegroundColor Green
Write-Host "Verifica los datos y luego actualiza ConnectionStrings__default de la API."
