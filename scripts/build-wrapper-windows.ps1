[CmdletBinding()]
param(
    [ValidateSet('Release')]
    [string] $Configuration = 'Release',

    [string] $MsvcRoot = $env:WXSHARP_MSVC_ROOT
)

$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$wxSourceDirectory = Join-Path $repositoryRoot 'third-party\Windows'
$wxLibraryDirectory = Join-Path $wxSourceDirectory 'lib\vc_x64_dll'
$nativeBuildDirectory = Join-Path $repositoryRoot 'build\native-shared-x64'
$stageDirectory = Join-Path $repositoryRoot 'build\stage\win-x64\native'

if ($MsvcRoot) {
    $setupScript = Join-Path $MsvcRoot 'setup_x64.bat'
    if (-not (Test-Path -LiteralPath $setupScript -PathType Leaf)) {
        throw "The x64 MSVC setup script was not found: $setupScript"
    }
    $environmentLines = & cmd.exe /d /s /c "`"call `"$setupScript`" >nul && set`""
    if ($LASTEXITCODE -ne 0) { throw "Failed to initialize MSVC from $setupScript" }
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

$requiredWxFiles = @(
    (Join-Path $wxLibraryDirectory 'wxbase33u.lib'),
    (Join-Path $wxLibraryDirectory 'wxmsw33u_core.lib')
)
foreach ($requiredFile in $requiredWxFiles) {
    if (-not (Test-Path -LiteralPath $requiredFile -PathType Leaf)) {
        throw "The wxWidgets build is missing. Run build-wxwidgets-windows.ps1 first: $requiredFile"
    }
}

& (Join-Path $PSScriptRoot 'build-native.ps1') `
    -Configuration $Configuration `
    -Architecture x64 `
    -Generator 'NMake Makefiles' `
    -BuildDirectory $nativeBuildDirectory `
    -WxWidgetsRoot $wxSourceDirectory `
    -WxWidgetsLibDir $wxLibraryDirectory `
    -SharedWxWidgets `
    -StaticMsvcRuntime
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

& (Join-Path $PSScriptRoot 'stage-windows.ps1') `
    -Configuration $Configuration `
    -Architecture x64 `
    -BuildDirectory $nativeBuildDirectory `
    -OutputDirectory $stageDirectory `
    -WxWidgetsRuntimeDirectory $wxLibraryDirectory

$originalPath = $env:PATH
try {
    $env:PATH = "$stageDirectory;$originalPath"
    & ctest --test-dir $nativeBuildDirectory -C $Configuration --output-on-failure
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}
finally {
    $env:PATH = $originalPath
}

$dumpbinPath = (Get-Command dumpbin.exe -ErrorAction Stop).Source
Get-ChildItem -LiteralPath $stageDirectory -Filter '*.dll' -File | ForEach-Object {
    & (Join-Path $PSScriptRoot 'verify-windows-dependencies.ps1') `
        -BinaryPath $_.FullName `
        -DumpbinPath $dumpbinPath
}

Write-Host "Wrapper build completed without rebuilding wxWidgets: $stageDirectory"
