#Requires -Version 5.1
<#
.SYNOPSIS
    Simula un mensaje entrante de WhatsApp a la API (para pruebas locales).
.DESCRIPTION
    Envia un payload como el que enviaria Meta a /api/MensajeSocial/webhooks/whatsapp.
    No requiere credenciales: la validacion de firma se omite si no hay AppSecret.
.EXAMPLE
    .\scripts\test-webhook.ps1
.EXAMPLE
    .\scripts\test-webhook.ps1 -Texto "Quiero cita para Firulais manana 10am" -Remitente "50588889999"
#>
[CmdletBinding()]
param(
    [string]$BaseUrl = "http://localhost:5000",
    [string]$Texto = "Hola, quiero agendar una cita para mi mascota",
    [string]$Remitente = "50577771111"
)

$ErrorActionPreference = "Stop"
$BaseUrl = $BaseUrl.TrimEnd("/")

$payload = @{
    entry = @(
        @{
            changes = @(
                @{
                    value = @{
                        messages = @(
                            @{
                                from = $Remitente
                                id = "wamid.test.$([DateTimeOffset]::UtcNow.ToUnixTimeSeconds())"
                                timestamp = [string][DateTimeOffset]::UtcNow.ToUnixTimeSeconds()
                                text = @{ body = $Texto }
                            }
                        )
                    }
                }
            )
        }
    )
} | ConvertTo-Json -Depth 10

Write-Host ">> Enviando mensaje simulado a $BaseUrl ..." -ForegroundColor Cyan
$resp = Invoke-WebRequest -Method Post -Uri "$BaseUrl/api/MensajeSocial/webhooks/whatsapp" -ContentType "application/json" -Body $payload -UseBasicParsing
Write-Host "Respuesta: $($resp.StatusCode) $($resp.StatusDescription)" -ForegroundColor Green
Write-Host ""
Write-Host "Verifica el mensaje en: $BaseUrl/api/MensajeSocial" -ForegroundColor Yellow
Write-Host "O en el frontend: http://localhost:5173 (pestana Mensajes)" -ForegroundColor Yellow