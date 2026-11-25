#Requires -Version 5.1
<#
.SYNOPSIS
    Crea y carga la base de datos Veterinaria_SaludAnimal.
.DESCRIPTION
    Ejecuta database/00_create_database.sql, 01_schema.sql, 02_stored_procedures.sql
    y 03_seed.sql. Es seguro ejecutarlo varias veces: no borra datos.
.EXAMPLE
    .\scripts\init-db.ps1 -Server ".\SQLEXPRESS" -WindowsAuth
.EXAMPLE
    .\scripts\init-db.ps1 -Server "miservidor.database.windows.net" -User "sqladmin" -Password "MiClave!"
#>
[CmdletBinding()]
param(
    [string]$Server = ".\SQLEXPRESS",
    [string]$User,
    [string]$Password,
    [switch]$WindowsAuth,
    [bool]$TrustServerCertificate = $true,
    [string]$SqlCmd = ""
)

$ErrorActionPreference = "Stop"

$Database = "Veterinaria_SaludAnimal"
$Root = Split-Path -Parent $PSScriptRoot
$DatabaseDir = Join-Path $Root "database"

if (-not (Test-Path (Join-Path $DatabaseDir "00_create_database.sql"))) {
    throw "No se encontro la carpeta database/. Ejecuta el script desde el repositorio."
}

if (-not $SqlCmd) {
    $command = Get-Command sqlcmd -ErrorAction SilentlyContinue
    if (-not $command) {
        throw "sqlcmd no esta instalado. Instala Microsoft SQL Server Command Line Utilities o usa Docker Compose."
    }
    $SqlCmd = $command.Source
}

$baseArgs = @("-S", $Server, "-b", "-f", "65001")
if ($WindowsAuth) {
    $baseArgs += "-E"
}
else {
    if (-not $User) {
        throw "Indica -User y -Password, o usa -WindowsAuth para autenticacion de Windows."
    }
    $baseArgs += @("-U", $User)
    if ($Password) { $baseArgs += @("-P", $Password) }
    else { $baseArgs += "-P" }  # sqlcmd pedira la contrasena
}

if ($TrustServerCertificate) { $baseArgs += "-C" }

function Invoke-SqlFile {
    param(
        [Parameter(Mandatory)][string]$Path,
        [Parameter(Mandatory)][string]$TargetDatabase
    )

    Write-Host ">> $(Split-Path -Leaf $Path) -> $TargetDatabase" -ForegroundColor Cyan
    & $SqlCmd @baseArgs -d $TargetDatabase -i $Path
    if ($LASTEXITCODE -ne 0) {
        throw "Fallo la ejecucion de $(Split-Path -Leaf $Path) (codigo $LASTEXITCODE)."
    }
}

Write-Host "Servidor: $Server" -ForegroundColor Yellow
Write-Host "Base de datos: $Database" -ForegroundColor Yellow

Invoke-SqlFile -Path (Join-Path $DatabaseDir "00_create_database.sql") -TargetDatabase "master"
Invoke-SqlFile -Path (Join-Path $DatabaseDir "01_schema.sql") -TargetDatabase $Database
Invoke-SqlFile -Path (Join-Path $DatabaseDir "02_stored_procedures.sql") -TargetDatabase $Database
Invoke-SqlFile -Path (Join-Path $DatabaseDir "03_seed.sql") -TargetDatabase $Database

Write-Host ""
Write-Host "Base de datos lista: $Database" -ForegroundColor Green
