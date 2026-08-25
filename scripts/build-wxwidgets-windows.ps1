[CmdletBinding()]
param(
    [string] $MsvcRoot = $env:WXSHARP_MSVC_ROOT,

    [string] $Version = '3.3.3',

    [switch] $Rebuild
)

$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$dependencyRoot = Join-Path $repositoryRoot 'build\dependencies'
$downloadDirectory = Join-Path $dependencyRoot 'downloads'
$sourceDirectory = Join-Path $dependencyRoot 'source'
$buildDirectory = Join-Path $dependencyRoot 'wx-shared-static-crt-x64'
$archivePath = Join-Path $downloadDirectory "wxWidgets-$Version.7z"
$archiveUrl = "https://github.com/wxWidgets/wxWidgets/releases/download/v$Version/wxWidgets-$Version.7z"
$expectedHash = '8DD5CCBFA24FCCFE05C82475382B0A0C5A571DB9EF7FBB70FA84971A6EA57081'

if ($Version -ne '3.3.3') {
    throw 'The download checksum is pinned to wxWidgets 3.3.3. Update the script before selecting another version.'
}

if ($MsvcRoot) {
    $setupScript = Join-Path $MsvcRoot 'setup_x64.bat'
    if (-not (Test-Path -LiteralPath $setupScript -PathType Leaf)) {
        throw "The x64 MSVC setup script was not found: $setupScript"
    }

    $environmentLines = & cmd.exe /d /s /c "`"call `"$setupScript`" >nul && set`""
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to initialize the MSVC environment from $setupScript"
    }
    foreach ($line in $environmentLines) {
        $separator = $line.IndexOf('=')
        if ($separator -gt 0) {
            [System.Environment]::SetEnvironmentVariable(
                $line.Substring(0, $separator),
                $line.Substring($separator + 1),
                'Process')
        }
    }
} elseif (-not (Get-Command cl.exe -ErrorAction SilentlyContinue)) {
    throw 'Run from an x64 Native Tools prompt, pass -MsvcRoot, or set WXSHARP_MSVC_ROOT.'
}

New-Item -ItemType Directory -Path $downloadDirectory -Force | Out-Null
if (-not (Test-Path -LiteralPath $archivePath -PathType Leaf)) {
    Write-Host "Downloading wxWidgets $Version source..."
    Invoke-WebRequest -Uri $archiveUrl -OutFile $archivePath
}

$actualHash = (Get-FileHash -LiteralPath $archivePath -Algorithm SHA256).Hash
if ($actualHash -ne $expectedHash) {
    throw "wxWidgets archive checksum mismatch. Expected $expectedHash; found $actualHash."
}

$versionHeader = Join-Path $sourceDirectory 'include\wx\version.h'
if (-not (Test-Path -LiteralPath $versionHeader -PathType Leaf)) {
    if (Test-Path -LiteralPath $sourceDirectory) {
        $resolvedSource = [System.IO.Path]::GetFullPath($sourceDirectory)
        $resolvedDependencies = [System.IO.Path]::GetFullPath($dependencyRoot)
        if (-not $resolvedSource.StartsWith($resolvedDependencies + [System.IO.Path]::DirectorySeparatorChar, [System.StringComparison]::OrdinalIgnoreCase)) {
            throw "Refusing to replace source directory outside $resolvedDependencies"
        }
        [System.IO.Directory]::Delete($resolvedSource, $true)
    }
    New-Item -ItemType Directory -Path $sourceDirectory -Force | Out-Null
    Push-Location $sourceDirectory
    try {
        & cmake -E tar xf $archivePath
        if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    }
    finally {
        Pop-Location
    }
    if (-not (Test-Path -LiteralPath $versionHeader -PathType Leaf)) {
        throw "wxWidgets extraction did not produce $versionHeader"
    }
}

if ($Rebuild -and (Test-Path -LiteralPath $buildDirectory)) {
    $resolvedBuild = [System.IO.Path]::GetFullPath($buildDirectory)
    $resolvedDependencies = [System.IO.Path]::GetFullPath($dependencyRoot)
    if (-not $resolvedBuild.StartsWith($resolvedDependencies + [System.IO.Path]::DirectorySeparatorChar, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to replace build directory outside $resolvedDependencies"
    }
    [System.IO.Directory]::Delete($resolvedBuild, $true)
}

& cmake -S $sourceDirectory -B $buildDirectory -G 'NMake Makefiles' `
    -DCMAKE_BUILD_TYPE=Release `
    -DwxBUILD_SHARED=ON `
    -DwxBUILD_USE_STATIC_RUNTIME=ON `
    '-DwxBUILD_VENDOR=' `
    -DwxBUILD_SAMPLES=OFF `
    -DwxBUILD_TESTS=OFF `
    -DwxUSE_WEBVIEW=OFF
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

& cmake --build $buildDirectory --target wxcore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Shared wxWidgets SDK with static MSVC runtime ready: $buildDirectory"
