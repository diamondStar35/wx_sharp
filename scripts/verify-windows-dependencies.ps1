[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string] $BinaryPath,

    [string] $DumpbinPath
)

$ErrorActionPreference = 'Stop'

$resolvedBinary = (Resolve-Path -LiteralPath $BinaryPath).Path
if (-not $DumpbinPath) {
    $DumpbinPath = (Get-Command dumpbin.exe -ErrorAction Stop).Source
}
if (-not (Test-Path -LiteralPath $DumpbinPath -PathType Leaf)) {
    throw "dumpbin was not found: $DumpbinPath"
}
$output = & $DumpbinPath /dependents $resolvedBinary 2>&1
if ($LASTEXITCODE -ne 0) {
    throw "dumpbin failed while inspecting $resolvedBinary"
}

$forbidden = @($output | Where-Object {
    $_ -match '^\s*(MSVCP[^\s]*\.dll|VCRUNTIME[^\s]*\.dll|api-ms-win-crt-[^\s]*\.dll|ucrtbase\.dll|msvcrt\.dll)\s*$'
})
if ($forbidden.Count -ne 0) {
    throw "Unexpected redistributable dependency in $resolvedBinary`: $($forbidden.Trim() -join ', ')"
}

Write-Host "Static dependency check passed: $resolvedBinary"
