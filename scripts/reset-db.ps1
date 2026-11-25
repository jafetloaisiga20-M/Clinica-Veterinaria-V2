#Requires -Version 5.1
<#
.SYNOPSIS
    Reinicia la base de datos local (borra todos los datos y la vuelve a crear).
.DESCRIPTION
    Util para QA cuando se necesita empezar de cero. NO usar contra la base
    de produccion compartida sin coordinar con el equipo.
.EXAMPLE
    .\scripts\reset-db.ps1 -WindowsAuth -Force
#>
[CmdletBinding()]
param(
    [string]$Server = ".\SQLEXPRESS",
    [string]$User,
    [string]$Password,
    [switch]$WindowsAuth,
    [bool]$TrustServerCertificate = $true,
    [string]$SqlCmd = "",
    [switch]$Force
)

$ErrorActionPreference = "Stop"
$Database = "Veterinaria_SaludAnimal"

if (-not $Force) {
    Write-Host "ATENCION: se va a borrar la base de datos $Database en $Server." -ForegroundColor Red
    $answer = Read-Host "Escribe BORRAR para continuar"
    if ($answer -ne "BORRAR") {
        Write-Host "Cancelado." -ForegroundColor Yellow
        exit 0
    }
}

if (-not $SqlCmd) {
    $command = Get-Command sqlcmd -ErrorAction SilentlyContinue
    if (-not $command) { throw "sqlcmd no esta instalado." }
    $SqlCmd = $command.Source
}

$baseArgs = @("-S", $Server, "-b")
if ($WindowsAuth) { $baseArgs += "-E" }
else {
    if (-not $User) { throw "Indica -User y -Password, o usa -WindowsAuth." }
    $baseArgs += @("-U", $User)
    if ($Password) { $baseArgs += @("-P", $Password) } else { $baseArgs += "-P" }
}
if ($TrustServerCertificate) { $baseArgs += "-C" }

$dropQuery = @"
IF DB_ID(N'$Database') IS NOT NULL
BEGIN
    ALTER DATABASE [$Database] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [$Database];
END
"@

Write-Host ">> Eliminando $Database..." -ForegroundColor Cyan
& $SqlCmd @baseArgs -d master -Q $dropQuery
if ($LASTEXITCODE -ne 0) { throw "No se pudo eliminar la base de datos." }

$initParams = @{
    Server    = $Server
    SqlCmd    = $SqlCmd
    WindowsAuth = $WindowsAuth
    TrustServerCertificate = $TrustServerCertificate
}
if (-not $WindowsAuth) {
    $initParams.User = $User
    $initParams.Password = $Password
}

& (Join-Path $PSScriptRoot "init-db.ps1") @initParams
