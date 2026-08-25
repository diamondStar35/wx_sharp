[CmdletBinding()]
param(
    [ValidateSet('Release')]
    [string] $Configuration = 'Release',

    [ValidateSet('x64')]
    [string] $Architecture = 'x64',

    [string] $BuildDirectory,

    [string] $OutputDirectory,

    [string] $WxWidgetsRuntimeDirectory
)

$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
if (-not $BuildDirectory) {
    $BuildDirectory = Join-Path $repositoryRoot "build\native-$Architecture"
}
if (-not $OutputDirectory) {
    $OutputDirectory = Join-Path $repositoryRoot "build\stage\win-$Architecture\native"
}

$nativeOutput = Join-Path $BuildDirectory $Configuration
$nativeLibrary = Join-Path $nativeOutput 'wxsharp.dll'
if (-not (Test-Path -LiteralPath $nativeLibrary -PathType Leaf)) {
    $nativeLibrary = Join-Path $BuildDirectory 'wxsharp.dll'
}
$assets = @($nativeLibrary)
if ($WxWidgetsRuntimeDirectory) {
    if (-not (Test-Path -LiteralPath $WxWidgetsRuntimeDirectory -PathType Container)) {
        throw "wxWidgets runtime directory does not exist: $WxWidgetsRuntimeDirectory"
    }
    $wxBase = @(Get-ChildItem -LiteralPath $WxWidgetsRuntimeDirectory -Filter 'wxbase*.dll' -File)
    $wxCore = @(Get-ChildItem -LiteralPath $WxWidgetsRuntimeDirectory -Filter 'wxmsw*_core*.dll' -File)
    if ($wxBase.Count -ne 1 -or $wxCore.Count -ne 1) {
        throw "Expected exactly one wxWidgets base DLL and one core DLL in $WxWidgetsRuntimeDirectory"
    }
    $assets += @($wxBase[0].FullName, $wxCore[0].FullName)
}

foreach ($asset in $assets) {
    if (-not (Test-Path -LiteralPath $asset -PathType Leaf)) {
        throw "Required Windows runtime asset is missing: $asset"
    }
}

$resolvedOutput = [System.IO.Path]::GetFullPath($OutputDirectory)
$resolvedBuildRoot = [System.IO.Path]::GetFullPath((Join-Path $repositoryRoot 'build'))
if (-not $resolvedOutput.StartsWith($resolvedBuildRoot + [System.IO.Path]::DirectorySeparatorChar, [System.StringComparison]::OrdinalIgnoreCase)) {
    throw "The staging directory must remain below $resolvedBuildRoot. Actual: $resolvedOutput"
}

if (Test-Path -LiteralPath $resolvedOutput) {
    [System.IO.Directory]::Delete($resolvedOutput, $true)
}
New-Item -ItemType Directory -Path $resolvedOutput -Force | Out-Null

foreach ($asset in $assets) {
    Copy-Item -LiteralPath $asset -Destination $resolvedOutput
}

Write-Host "Windows runtime staged: $resolvedOutput"
Get-ChildItem -LiteralPath $resolvedOutput -File | Select-Object Name, Length
