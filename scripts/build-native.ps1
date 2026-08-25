[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string] $Configuration = 'Release',

    [ValidateSet('Win32', 'x64', 'ARM64')]
    [string] $Architecture = 'x64',

    [string] $WxWidgetsRoot = $env:WXWIDGETS_ROOT,

    [string] $WxWidgetsLibDir = $env:WXWIDGETS_LIB_DIR,

    [string] $ToolchainFile = $env:CMAKE_TOOLCHAIN_FILE,

    [string] $Generator,

    [switch] $SharedWxWidgets,

    [switch] $StaticMsvcRuntime,

    [string] $BuildDirectory
)

$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$sourceDirectory = Join-Path $repositoryRoot 'src\WxSharp.Native'
if (-not $BuildDirectory) {
    $BuildDirectory = Join-Path $repositoryRoot "build\native-$Architecture"
}
$isWindowsHost = $env:OS -eq 'Windows_NT'

if (-not $WxWidgetsRoot -and $isWindowsHost -and $Architecture -eq 'x64') {
    $vendoredRoot = Join-Path $repositoryRoot 'third-party\Windows'
    $vendoredLib = Join-Path $vendoredRoot 'lib\vc_x64_dll'
    if ((Test-Path -LiteralPath $vendoredRoot -PathType Container) -and
        (Test-Path -LiteralPath $vendoredLib -PathType Container)) {
        $WxWidgetsRoot = $vendoredRoot
        $WxWidgetsLibDir = $vendoredLib
        $SharedWxWidgets = $true
        $StaticMsvcRuntime = $true
    }
}

if ($WxWidgetsRoot -and -not (Test-Path -LiteralPath $WxWidgetsRoot -PathType Container)) {
    throw "wxWidgets root does not exist: $WxWidgetsRoot"
}
if ($WxWidgetsLibDir -and -not (Test-Path -LiteralPath $WxWidgetsLibDir -PathType Container)) {
    throw "wxWidgets library directory does not exist: $WxWidgetsLibDir"
}
if ($isWindowsHost -and -not $WxWidgetsRoot) {
    throw 'wxWidgets was not found. Run build-wxwidgets-windows.ps1, or pass -WxWidgetsRoot and -WxWidgetsLibDir.'
}

$configureArguments = @('-S', $sourceDirectory, '-B', $BuildDirectory)
if ($Generator) {
    $configureArguments += @('-G', $Generator)
    if ($Generator -like 'Visual Studio*') {
        $configureArguments += @('-A', $Architecture)
    }
} else {
    $configureArguments += @('-A', $Architecture)
}
if ($WxWidgetsRoot) {
    $configureArguments += "-DwxWidgets_ROOT_DIR=$WxWidgetsRoot"
}
if ($WxWidgetsLibDir) {
    $configureArguments += "-DwxWidgets_LIB_DIR=$WxWidgetsLibDir"
}
if ($ToolchainFile) {
    $configureArguments += "-DCMAKE_TOOLCHAIN_FILE=$ToolchainFile"
}
if ($SharedWxWidgets) {
    $configureArguments += '-DwxWidgets_USE_REL_AND_DBG=OFF'
}
if ($StaticMsvcRuntime) {
    $configureArguments += '-DWXSHARP_STATIC_MSVC_RUNTIME=ON'
}
if ($Generator -eq 'NMake Makefiles') {
    $configureArguments += "-DCMAKE_BUILD_TYPE=$Configuration"
}

& cmake @configureArguments
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

& cmake --build $BuildDirectory --config $Configuration --parallel
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Native build completed: $BuildDirectory"
