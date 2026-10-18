param([string]$DistributionDll)
$ErrorActionPreference = 'Stop'
$repoPath = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$manifestPath = Join-Path $repoPath 'manifest.json'
$manifest = Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json
if ($manifest.name -notmatch '^[A-Za-z0-9_]{1,128}$') { throw 'Invalid Thunderstore package name.' }
if ($manifest.description.Length -eq 0 -or $manifest.description.Length -gt 250) { throw 'Description must contain 1-250 characters.' }
if ($manifest.version_number -notmatch '^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)$') { throw 'Invalid package version.' }
if (-not $manifest.dependencies -or @($manifest.dependencies | Where-Object { $_ -notmatch '^[A-Za-z0-9_]+-[A-Za-z0-9_]+-\d+\.\d+\.\d+$' }).Count) {
    throw 'Invalid package dependencies.'
}
if ($manifest.website_url -and $manifest.website_url -notmatch '^https?://') { throw 'Invalid website URL.' }

$dllPath = if ($DistributionDll) { [IO.Path]::GetFullPath($DistributionDll, $repoPath) } else { Join-Path $repoPath 'SeedBed.dll' }
$testedDllPath = Join-Path $repoPath 'src/bin/Release/netstandard2.1/package/SeedBed.dll'
$dllHash = (Get-FileHash -LiteralPath $dllPath -Algorithm SHA256).Hash
if ((Get-FileHash -LiteralPath $testedDllPath -Algorithm SHA256).Hash -ne $dllHash) {
    throw 'Distribution DLL must match the merged build artifact. Run build.ps1 first.'
}
$assemblyName = [Reflection.AssemblyName]::GetAssemblyName($dllPath)
if ($assemblyName.Name -ne 'SeedBed' -or $assemblyName.Version.ToString(3) -ne $manifest.version_number) {
    throw 'DLL identity/version does not match the package.'
}
$versionInfo = [Diagnostics.FileVersionInfo]::GetVersionInfo($dllPath)
if ($versionInfo.ProductName -ne 'SeedBed Revamped' -or $versionInfo.CompanyName -ne 'Tetus') {
    throw 'DLL must identify SeedBed Revamped and its maintainer.'
}
$iconPath = Join-Path $repoPath 'icon.png'
$icon = [IO.File]::ReadAllBytes($iconPath)
if ($icon.Length -lt 24 -or [Convert]::ToHexString($icon[0..7]) -ne '89504E470D0A1A0A' -or
    [Convert]::ToHexString($icon[16..23]) -ne '0000010000000100') {
    throw 'Package icon must be a 256x256 PNG.'
}

$files = [ordered]@{
    'manifest.json' = $manifestPath
    'icon.png' = $iconPath
    'README.md' = (Join-Path $repoPath 'docs/README_THUNDERSTORE.md')
    'CHANGELOG.md' = (Join-Path $repoPath 'CHANGELOG.md')
    'CREDITS.md' = (Join-Path $repoPath 'CREDITS.md')
    'THIRD_PARTY_NOTICES.md' = (Join-Path $repoPath 'docs/THIRD_PARTY_NOTICES.md')
    'BepInEx/plugins/SeedBedRevamped/SeedBed.dll' = $dllPath
}
foreach ($source in $files.Values) {
    if (-not (Test-Path -LiteralPath $source -PathType Leaf)) { throw "Missing package file: $source" }
}
$releasePath = Join-Path $repoPath 'artifacts/releases'
New-Item -ItemType Directory -Path $releasePath -Force | Out-Null
$zipPath = Join-Path $releasePath ($manifest.name + '-' + $manifest.version_number + '.zip')
$temporaryPath = Join-Path $releasePath ([Guid]::NewGuid().ToString('N') + '.zip.tmp')
try {
    $zip = [IO.Compression.ZipFile]::Open($temporaryPath, [IO.Compression.ZipArchiveMode]::Create)
    try {
        foreach ($entry in $files.GetEnumerator()) {
            [IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip, $entry.Value, $entry.Key, [IO.Compression.CompressionLevel]::Optimal) | Out-Null
        }
    } finally { $zip.Dispose() }

    $zip = [IO.Compression.ZipFile]::OpenRead($temporaryPath)
    try {
        $actualEntries = @($zip.Entries | ForEach-Object { $_.FullName })
        if (@(Compare-Object @($files.Keys) $actualEntries).Count -ne 0) { throw 'ZIP contents differ from the release file list.' }
        foreach ($entry in $files.GetEnumerator()) {
            $stream = $zip.GetEntry($entry.Key).Open()
            try { $actualHash = [Convert]::ToHexString([Security.Cryptography.SHA256]::HashData($stream)) }
            finally { $stream.Dispose() }
            if ($actualHash -ne (Get-FileHash -LiteralPath $entry.Value -Algorithm SHA256).Hash) {
                throw "ZIP entry failed integrity check: $($entry.Key)"
            }
        }
    } finally { $zip.Dispose() }
    [IO.File]::Move($temporaryPath, $zipPath, $true)
    Write-Host "Package ready: $zipPath"
    Write-Host "DLL SHA-256: $dllHash"
    Write-Host "ZIP SHA-256: $((Get-FileHash -LiteralPath $zipPath -Algorithm SHA256).Hash)"
} finally {
    if (Test-Path -LiteralPath $temporaryPath) { Remove-Item -LiteralPath $temporaryPath }
}
