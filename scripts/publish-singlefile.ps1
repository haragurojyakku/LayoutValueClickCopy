param(
    [switch]$Release
)

$configuration = if ($Release) { 'Release' } else { 'Debug' }

Write-Host "Publishing single-file (win-x64, self-contained) in $configuration..."

$root = Split-Path -Parent $PSScriptRoot

dotnet publish `
  "$root/LayoutValueClickCopy.csproj" `
  -c $configuration `
  -r win-x64 `
  -p:PublishSingleFile=true `
  -p:SelfContained=true `
  -p:PublishTrimmed=false `
  -p:IncludeNativeLibrariesForSelfExtract=true `
  -p:EnableCompressionInSingleFile=true `
  -o "$root/publish/win-x64-single"

Write-Host "Done. Output -> publish/win-x64-single"
