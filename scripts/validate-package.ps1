[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string] $PackagePath
)

$ErrorActionPreference = 'Stop'

$resolvedPackage = (Resolve-Path -LiteralPath $PackagePath).Path
Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem

$archive = [System.IO.Compression.ZipFile]::OpenRead($resolvedPackage)
try {
    $entries = @($archive.Entries | ForEach-Object { $_.FullName })
    $required = @(
        'lib/net8.0/WxSharp.dll',
        'lib/net9.0/WxSharp.dll',
        'lib/net10.0/WxSharp.dll',
        'runtimes/win-x64/native/wxsharp.dll',
        'licenses/wxWidgets.txt',
        'README.md'
    )

    $missing = @($required | Where-Object { $_ -notin $entries })
    if ($missing.Count -ne 0) {
        throw "Package is missing required entries: $($missing -join ', ')"
    }

    $wxBase = @($entries | Where-Object { $_ -like 'runtimes/win-x64/native/wxbase*.dll' })
    $wxCore = @($entries | Where-Object { $_ -like 'runtimes/win-x64/native/wxmsw*_core*.dll' })
    if ($wxBase.Count -ne 1 -or $wxCore.Count -ne 1) {
        throw 'Package must contain exactly one wxWidgets base DLL and one core DLL.'
    }
    $expectedNative = @('runtimes/win-x64/native/wxsharp.dll', $wxBase[0], $wxCore[0])

    $unexpectedNative = @($entries | Where-Object {
        $_ -like 'runtimes/win-x64/native/*' -and $_ -notin $expectedNative
    })
    if ($unexpectedNative.Count -ne 0) {
        throw "Package contains unexpected Windows native assets: $($unexpectedNative -join ', ')"
    }

    Write-Host "Package layout verified: $resolvedPackage"
}
finally {
    $archive.Dispose()
}
