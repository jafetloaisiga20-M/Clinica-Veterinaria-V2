#Requires -Version 5.1
<#
.SYNOPSIS
    Genera un respaldo BACPAC de la base de datos.
.DESCRIPTION
    BACPAC incluye esquema + datos y funciona tanto con SQL Server local
    como con Azure SQL Database. Se puede automatizar en GitHub Actions
    (ver .github/workflows/backup-db.yml).
.EXAMPLE
    .\scripts\backup-db.ps1 -ConnectionString "Server=.;Database=Veterinaria_SaludAnimal;Integrated Security=True;TrustServerCertificate=True"
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$ConnectionString,
    [string]$OutputDirectory = "",
    [string]$SqlPackage = ""
)

$ErrorActionPreference = "Stop"

# DacFx 162 esta compilado para .NET 8. Si la maquina solo tiene .NET 10,
# esto permite que sqlpackage se ejecute igual (roll-forward de runtime).
$env:DOTNET_ROLL_FORWARD = "Major"

if (-not $OutputDirectory) {
    $OutputDirectory = Join-Path (Split-Path -Parent $PSScriptRoot) "backups"
}

New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null

if (-not $SqlPackage) {
    $command = Get-Command sqlpackage -ErrorAction SilentlyContinue
    if (-not $command) {
        throw @"
sqlpackage no esta instalado. Instalalo con:
    dotnet tool install --global microsoft.sqlpackage
Luego cierra y abre la terminal (o agrega %USERPROFILE%\.dotnet\tools al PATH).
"@
    }
    $SqlPackage = $command.Source
}

$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$target = Join-Path $OutputDirectory "Veterinaria_SaludAnimal-$timestamp.bacpac"

Write-Host ">> Exportando base de datos..." -ForegroundColor Cyan
& $SqlPackage "/Action:Export" `
    "/SourceConnectionString:$ConnectionString" `
    "/TargetFile:$target" `
    "/OverwriteFiles:True"

if ($LASTEXITCODE -ne 0 -or -not (Test-Path $target)) {
    throw "El respaldo fallo (codigo $LASTEXITCODE)."
}

$hash = (Get-FileHash -Algorithm SHA256 -Path $target).Hash
$hashFile = "$target.sha256"
"$hash  $(Split-Path -Leaf $target)" | Set-Content -Encoding ASCII -Path $hashFile

$sizeMb = [math]::Round((Get-Item $target).Length / 1MB, 2)
Write-Host ""
Write-Host "Respaldo listo: $target ($sizeMb MB)" -ForegroundColor Green
Write-Host "SHA256: $hash"
Write-Host "Guarda una copia fuera del repositorio (Drive, OneDrive, almacenamiento en la nube)."
