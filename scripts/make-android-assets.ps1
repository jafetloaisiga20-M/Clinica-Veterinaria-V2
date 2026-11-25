# Genera las imagenes fuente del icono y splash para el APK de Salud Animal.
# Requiere Windows PowerShell y System.Drawing (incluido en Windows).
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

$root = Split-Path $PSScriptRoot -Parent
$assets = Join-Path $root 'Frontend\assets'
New-Item -ItemType Directory -Force -Path $assets | Out-Null

$teal = [System.Drawing.ColorTranslator]::FromHtml('#0d9488')
$tealDark = [System.Drawing.ColorTranslator]::FromHtml('#115e59')
$white = [System.Drawing.Color]::White
$whiteSoft = [System.Drawing.Color]::FromArgb(235, 255, 255, 255)

function Draw-Paw($graphics, [double]$scale) {
  $brush = New-Object System.Drawing.SolidBrush($white)
  $parts = @(
    @(342, 540, 340, 290),
    @(230, 390, 140, 170),
    @(380, 300, 140, 170),
    @(504, 300, 140, 170),
    @(654, 390, 140, 170)
  )
  foreach ($part in $parts) {
    $graphics.FillEllipse(
      $brush,
      [int]($part[0] * $scale),
      [int]($part[1] * $scale),
      [int]($part[2] * $scale),
      [int]($part[3] * $scale)
    )
  }
  $brush.Dispose()
}

function New-Canvas([int]$size, $background) {
  $bitmap = New-Object System.Drawing.Bitmap($size, $size)
  $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
  $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
  $graphics.Clear($background)
  return @($bitmap, $graphics)
}

# Icono legacy (fondo teal, pata blanca).
$icon = New-Canvas 1024 $teal
Draw-Paw $icon[1] 1.0
$icon[1].Dispose()
$icon[0].Save((Join-Path $assets 'icon-only.png'), [System.Drawing.Imaging.ImageFormat]::Png)
$icon[0].Dispose()

# Fondo del icono adaptativo.
$background = New-Canvas 1024 $teal
$background[1].Dispose()
$background[0].Save((Join-Path $assets 'icon-background.png'), [System.Drawing.Imaging.ImageFormat]::Png)
$background[0].Dispose()

# Primer plano del icono adaptativo (pata centrada dentro de la zona segura).
$foreground = New-Canvas 1024 ([System.Drawing.Color]::Transparent)
$foreground[1].TranslateTransform(512, 512)
$foreground[1].ScaleTransform(0.60, 0.60)
$foreground[1].TranslateTransform(-512, -512)
Draw-Paw $foreground[1] 1.0
$foreground[1].Dispose()
$foreground[0].Save((Join-Path $assets 'icon-foreground.png'), [System.Drawing.Imaging.ImageFormat]::Png)
$foreground[0].Dispose()

# Icono monocromatico (tema de Android 13+).
$mono = New-Canvas 1024 ([System.Drawing.Color]::Transparent)
$mono[1].TranslateTransform(512, 512)
$mono[1].ScaleTransform(0.60, 0.60)
$mono[1].TranslateTransform(-512, -512)
Draw-Paw $mono[1] 1.0
$mono[1].Dispose()
$mono[0].Save((Join-Path $assets 'icon-monochrome.png'), [System.Drawing.Imaging.ImageFormat]::Png)
$mono[0].Dispose()

function New-Splash([string]$fileName, $background) {
  $size = 2732
  $splash = New-Canvas $size $background
  $graphics = $splash[1]
  $graphics.TranslateTransform(1366, 1000)
  $graphics.ScaleTransform(2.2, 2.2)
  $graphics.TranslateTransform(-512, -512)
  Draw-Paw $graphics 1.0
  $graphics.ResetTransform()

  $format = New-Object System.Drawing.StringFormat
  $format.Alignment = [System.Drawing.StringAlignment]::Center
  $format.LineAlignment = [System.Drawing.StringAlignment]::Center
  $titleFont = New-Object System.Drawing.Font('Segoe UI', 150, [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Pixel)
  $subtitleFont = New-Object System.Drawing.Font('Segoe UI', 72, [System.Drawing.FontStyle]::Regular, [System.Drawing.GraphicsUnit]::Pixel)
  $softBrush = New-Object System.Drawing.SolidBrush($whiteSoft)
  $graphics.DrawString('Salud Animal', $titleFont, $softBrush, (New-Object System.Drawing.RectangleF(0, 1840, $size, 260)), $format)
  $graphics.DrawString("Cl$([char]0x00ED)nica Veterinaria", $subtitleFont, $softBrush, (New-Object System.Drawing.RectangleF(0, 2100, $size, 170)), $format)

  $titleFont.Dispose()
  $subtitleFont.Dispose()
  $softBrush.Dispose()
  $format.Dispose()
  $graphics.Dispose()
  $splash[0].Save((Join-Path $assets $fileName), [System.Drawing.Imaging.ImageFormat]::Png)
  $splash[0].Dispose()
}

New-Splash 'splash.png' $teal
New-Splash 'splash-dark.png' $tealDark

Write-Host "Imagenes generadas en $assets"
Get-ChildItem $assets -Filter '*.png' | Select-Object Name, Length
