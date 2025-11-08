param(
    [string]$PngPath = "Assets\app.png",
    [string]$IcoPath = "Assets\app.ico"
)

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$source = Join-Path $root $PngPath
$dest = Join-Path $root $IcoPath

if (-not (Test-Path $source)) {
    Write-Error "PNG file not found: $source"
    exit 1
}

Write-Host "Converting PNG to ICO..."
Write-Host "  Source: $source"
Write-Host "  Destination: $dest"

Add-Type -AssemblyName System.Drawing

# PNG を読み込み
$png = [System.Drawing.Image]::FromFile($source)

# ICO ファイルストリームを作成
$iconStream = [System.IO.MemoryStream]::new()
$writer = [System.IO.BinaryWriter]::new($iconStream)

# ICO ヘッダー (6 bytes)
$writer.Write([UInt16]0)      # Reserved (must be 0)
$writer.Write([UInt16]1)      # Type (1 = ICO)
$writer.Write([UInt16]1)      # Number of images

# アイコンディレクトリエントリ (16 bytes)
$width = [Math]::Min($png.Width, 256)
$height = [Math]::Min($png.Height, 256)

$writer.Write([byte]($width -eq 256 ? 0 : $width))    # Width (0 = 256)
$writer.Write([byte]($height -eq 256 ? 0 : $height))  # Height (0 = 256)
$writer.Write([byte]0)        # Color palette (0 = no palette)
$writer.Write([byte]0)        # Reserved

$writer.Write([UInt16]1)      # Color planes
$writer.Write([UInt16]32)     # Bits per pixel

# PNG データとして保存
$pngStream = [System.IO.MemoryStream]::new()
$png.Save($pngStream, [System.Drawing.Imaging.ImageFormat]::Png)
$pngBytes = $pngStream.ToArray()

$writer.Write([UInt32]$pngBytes.Length)  # Image size
$writer.Write([UInt32]22)                 # Image offset (6 + 16)

# PNG データを書き込み
$writer.Write($pngBytes)

# ファイルに保存
[System.IO.File]::WriteAllBytes($dest, $iconStream.ToArray())

# クリーンアップ
$writer.Close()
$iconStream.Close()
$pngStream.Close()
$png.Dispose()

Write-Host "Done! ICO file created: $dest"
