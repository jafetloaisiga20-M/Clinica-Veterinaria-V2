#Requires -Version 5.1
<#
.SYNOPSIS
    Verifica que la API este arriba y que la base de datos responda.
.DESCRIPTION
    Hace login con un usuario administrador y consulta los endpoints protegidos.
    Sale con codigo 1 si algun endpoint falla.
.EXAMPLE
    .\scripts\smoke-test.ps1 -BaseUrl "http://localhost:5000"
.EXAMPLE
    .\scripts\smoke-test.ps1 -BaseUrl "http://localhost:8081" -Username "admin" -Password "Admin123!"
#>
[CmdletBinding()]
param(
    [string]$BaseUrl = "http://localhost:8081",
    [string]$Username = "admin",
    [string]$Password = "Admin123!"
)

$ErrorActionPreference = "Stop"
$BaseUrl = $BaseUrl.TrimEnd("/")
$failed = 0
$token = $null

Write-Host ">> Verificando API en $BaseUrl" -ForegroundColor Cyan

# 1. Health (publico)
try {
    $health = Invoke-RestMethod -Uri "$BaseUrl/health" -TimeoutSec 30
    if ($health.status -eq "ok" -and $health.database -eq "ok") {
        Write-Host "[OK]   Health + base de datos -> $BaseUrl/health" -ForegroundColor Green
    }
    else {
        Write-Host "[FAIL] Health respondio pero la base no esta OK: $($health | ConvertTo-Json -Compress)" -ForegroundColor Red
        $failed++
    }
}
catch {
    Write-Host "[FAIL] Health -> $($_.Exception.Message)" -ForegroundColor Red
    $failed++
}

# 2. Login (necesario para los GET protegidos)
if ($Username -and $Password) {
    try {
        $body = @{ Username = $Username; Password = $Password } | ConvertTo-Json
        $login = Invoke-RestMethod -Method Post -Uri "$BaseUrl/api/Auth/Login" -ContentType "application/json" -Body $body -TimeoutSec 30
        $token = $login.token
        if ($token) {
            Write-Host "[OK]   Login $Username" -ForegroundColor Green
        }
        else {
            Write-Host "[FAIL] Login no devolvio token." -ForegroundColor Red
            $failed++
        }
    }
    catch {
        Write-Host "[FAIL] Login $Username -> $($_.Exception.Message)" -ForegroundColor Red
        $failed++
    }
}

# 3. Endpoints protegidos
$checks = @(
    @{ Name = "Listar clientes";  Path = "/api/Cliente" },
    @{ Name = "Listar pacientes"; Path = "/api/Paciente" },
    @{ Name = "Listar citas";     Path = "/api/Citas" }
)

foreach ($check in $checks) {
    $url = "$BaseUrl$($check.Path)"
    if (-not $token) {
        Write-Host "[SKIP] $($check.Name) -> falta token de login" -ForegroundColor Yellow
        $failed++
        continue
    }
    try {
        $headers = @{ Authorization = "Bearer $token" }
        $response = Invoke-WebRequest -Uri $url -Headers $headers -UseBasicParsing -TimeoutSec 30
        if ($response.StatusCode -ge 200 -and $response.StatusCode -lt 300) {
            Write-Host "[OK]   $($check.Name) -> $url ($($response.StatusCode))" -ForegroundColor Green
        }
        else {
            Write-Host "[FAIL] $($check.Name) -> $url ($($response.StatusCode))" -ForegroundColor Red
            $failed++
        }
    }
    catch {
        Write-Host "[FAIL] $($check.Name) -> $url ($($_.Exception.Message))" -ForegroundColor Red
        $failed++
    }
}

if ($failed -gt 0) {
    Write-Host ""
    Write-Host "$failed verificacion(es) fallaron." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Todo responde correctamente." -ForegroundColor Green
