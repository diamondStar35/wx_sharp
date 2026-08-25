[CmdletBinding()]
param(
    [ValidateSet('Release')]
    [string] $Configuration = 'Release',

    [string] $PackageVersion = '0.1.0-preview.1',

    [string] $MsvcRoot = $env:WXSHARP_MSVC_ROOT
)

$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$nativeBuildDirectory = Join-Path $repositoryRoot 'build\native-shared-x64'
$wxSourceDirectory = Join-Path $repositoryRoot 'build\dependencies\source'
$wxLibraryDirectory = Join-Path $repositoryRoot 'build\dependencies\wx-shared-static-crt-x64\lib\vc_x64_dll'
$stageDirectory = Join-Path $repositoryRoot 'build\stage\win-x64\native'
$standaloneDirectory = Join-Path $repositoryRoot 'build\standalone-test\win-x64'
$packageDirectory = Join-Path $repositoryRoot 'build\packages'
$packageSmokeDirectory = Join-Path $repositoryRoot 'build\package-smoke'
$packageSmokePackages = Join-Path $packageSmokeDirectory 'packages'
$nativeLibrary = Join-Path $stageDirectory 'wxsharp.dll'
$packagePath = Join-Path $packageDirectory "WxSharp.$PackageVersion.nupkg"
$originalPath = $env:PATH
$originalNativeLibrary = $env:WXSHARP_NATIVE_LIBRARY

try {
    . (Join-Path $PSScriptRoot 'build-wxwidgets-windows.ps1') -MsvcRoot $MsvcRoot
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

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

    $env:PATH = "$stageDirectory;$originalPath"
    & ctest --test-dir $nativeBuildDirectory -C $Configuration --output-on-failure
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    if (Test-Path -LiteralPath $standaloneDirectory) {
        [System.IO.Directory]::Delete($standaloneDirectory, $true)
    }
    New-Item -ItemType Directory -Path $standaloneDirectory -Force | Out-Null
    Get-ChildItem -LiteralPath $stageDirectory -Filter '*.dll' -File |
        Copy-Item -Destination $standaloneDirectory
    Copy-Item -LiteralPath (Join-Path $nativeBuildDirectory 'wxsharp_c_abi_smoke.exe') -Destination $standaloneDirectory
    & (Join-Path $standaloneDirectory 'wxsharp_c_abi_smoke.exe')
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    $dumpbinPath = if ($MsvcRoot) {
        Get-ChildItem -LiteralPath (Join-Path $MsvcRoot 'VC\Tools\MSVC') -Filter dumpbin.exe -File -Recurse |
            Where-Object FullName -Like '*\bin\Hostx64\x64\dumpbin.exe' |
            Sort-Object FullName -Descending |
            Select-Object -First 1 -ExpandProperty FullName
    }
    Get-ChildItem -LiteralPath $standaloneDirectory -Filter '*.dll' -File | ForEach-Object {
        & (Join-Path $PSScriptRoot 'verify-windows-dependencies.ps1') `
            -BinaryPath $_.FullName `
            -DumpbinPath $dumpbinPath
    }

    $env:WXSHARP_NATIVE_LIBRARY = $nativeLibrary
    & dotnet run --project (Join-Path $repositoryRoot 'tests\WxSharp.Smoke\WxSharp.Smoke.csproj') -c $Configuration
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    New-Item -ItemType Directory -Path $packageDirectory -Force | Out-Null
    & dotnet pack (Join-Path $repositoryRoot 'src\WxSharp\WxSharp.csproj') `
        -c $Configuration `
        -o $packageDirectory `
        -p:PackageVersion=$PackageVersion `
        -p:WindowsNativeAssetsDir=$stageDirectory
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    & (Join-Path $PSScriptRoot 'validate-package.ps1') -PackagePath $packagePath

    $env:PATH = $originalPath
    Remove-Item Env:WXSHARP_NATIVE_LIBRARY -ErrorAction SilentlyContinue

    $packageSmokeProject = Join-Path $repositoryRoot 'tests\WxSharp.PackageSmoke\WxSharp.PackageSmoke.csproj'
    if (Test-Path -LiteralPath $packageSmokeDirectory) {
        [System.IO.Directory]::Delete($packageSmokeDirectory, $true)
    }
    & dotnet restore $packageSmokeProject `
        --source $packageDirectory `
        --packages $packageSmokePackages `
        --force `
        --no-cache `
        -p:WxSharpPackageVersion=$PackageVersion
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    & dotnet run --project $packageSmokeProject `
        -c $Configuration `
        --no-restore `
        -p:RestorePackagesPath=$packageSmokePackages `
        -p:WxSharpPackageVersion=$PackageVersion
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

    Write-Host "Standalone native test: $standaloneDirectory"
    Write-Host "Windows pipeline completed: $packagePath"
}
finally {
    $env:PATH = $originalPath
    if ($null -eq $originalNativeLibrary) {
        Remove-Item Env:WXSHARP_NATIVE_LIBRARY -ErrorAction SilentlyContinue
    } else {
        $env:WXSHARP_NATIVE_LIBRARY = $originalNativeLibrary
    }
}
