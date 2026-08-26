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
$stageDirectory = Join-Path $repositoryRoot 'build\stage\win-x64\native'
$standaloneDirectory = Join-Path $repositoryRoot 'build\standalone-test\win-x64'
$packageDirectory = Join-Path $repositoryRoot 'build\packages'
$packageSmokeDirectory = Join-Path $repositoryRoot 'build\package-smoke'
$packageSmokePackages = Join-Path $packageSmokeDirectory 'packages'
$nativeLibrary = Join-Path $stageDirectory 'wx.dll'
$packagePath = Join-Path $packageDirectory "WxSharp.$PackageVersion.nupkg"
$originalPath = $env:PATH
$originalNativeLibrary = $env:WXSHARP_NATIVE_LIBRARY
$vendoredWxDlls = @(
    (Join-Path $repositoryRoot 'third-party\Windows\lib\vc_x64_dll\wxbase333u_vc_x64.dll'),
    (Join-Path $repositoryRoot 'third-party\Windows\lib\vc_x64_dll\wxmsw333u_core_vc_x64.dll')
)
$vendoredWxHashes = @{}
foreach ($vendoredDll in $vendoredWxDlls) {
    if (-not (Test-Path -LiteralPath $vendoredDll -PathType Leaf)) { throw "Missing vendored wxWidgets runtime: $vendoredDll" }
    $vendoredWxHashes[$vendoredDll] = (Get-FileHash -LiteralPath $vendoredDll -Algorithm SHA256).Hash
}

try {
    # Normal development and packaging reuse the pinned wxWidgets binaries in third-party\Windows. Rebuilding
    # wxWidgets is an explicit maintenance operation handled only by build-wxwidgets-windows.ps1.
    & (Join-Path $PSScriptRoot 'build-wrapper-windows.ps1') `
        -Configuration $Configuration `
        -MsvcRoot $MsvcRoot
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    foreach ($vendoredDll in $vendoredWxDlls) {
        $currentHash = (Get-FileHash -LiteralPath $vendoredDll -Algorithm SHA256).Hash
        if ($currentHash -ne $vendoredWxHashes[$vendoredDll]) {
            throw "The normal Windows pipeline modified a vendored wxWidgets DLL: $vendoredDll"
        }
    }

    $env:PATH = "$stageDirectory;$originalPath"

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
    & dotnet run --project (Join-Path $repositoryRoot 'tests\WxSharp.Smoke\WxSharp.Smoke.csproj') `
        -c $Configuration --no-build -- --callback-exception
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
    & dotnet run --project (Join-Path $repositoryRoot 'tests\WxSharp.Smoke\WxSharp.Smoke.csproj') `
        -c $Configuration --no-build -- --init-false
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
