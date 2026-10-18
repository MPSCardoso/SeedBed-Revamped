$ErrorActionPreference = 'Stop'
$repoPath = Split-Path $PSScriptRoot -Parent
$fixtureRoot = Join-Path $PSScriptRoot ('.tmp/restore-' + [Guid]::NewGuid().ToString('N'))
$fixtureBuild = Join-Path $fixtureRoot 'build'
$fixtureCache = Join-Path $fixtureRoot '.cache/dependencies/Fixture/locked-revision'
New-Item -ItemType Directory -Path $fixtureBuild, $fixtureCache -Force | Out-Null
try {
    $restoreScript = Join-Path $fixtureBuild 'Restore-Dependencies.ps1'
    Copy-Item -LiteralPath (Join-Path $repoPath 'build/Restore-Dependencies.ps1') -Destination $restoreScript
    $sourcePath = Join-Path $fixtureCache 'Fixture.cs'
    Set-Content -LiteralPath $sourcePath -Value '// pinned source' -Encoding utf8
    $expectedHash = (Get-FileHash -LiteralPath $sourcePath -Algorithm SHA256).Hash
    $manifest = @{ dependencies = @(@{ name = 'Fixture'; revision = 'locked-revision'; files = @(@{ name = 'Fixture.cs'; url = 'https://example.invalid/not-requested'; sha256 = $expectedHash }) }) }
    $manifest | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath (Join-Path $fixtureBuild 'dependencies.lock.json') -Encoding utf8
    & $restoreScript -Name Fixture
    if ((Get-FileHash -LiteralPath $sourcePath -Algorithm SHA256).Hash -ne $expectedHash) { throw 'Valid cached source was unexpectedly modified.' }
    Set-Content -LiteralPath $sourcePath -Value '// tampered source' -Encoding utf8
    $rejected = $false
    try { & $restoreScript -Name Fixture } catch {
        if ($_.Exception.Message -notlike '*Hash mismatch in cached*') { throw }
        $rejected = $true
    }
    if (!$rejected) { throw 'A tampered cached source was accepted.' }
    Write-Host 'PASS: pinned dependency hashes (valid cache and tampered cache rejection).'
} finally {
    $resolvedFixture = (Resolve-Path -LiteralPath $fixtureRoot).ProviderPath
    $expectedParent = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '.tmp')) + [IO.Path]::DirectorySeparatorChar
    if (!$resolvedFixture.StartsWith($expectedParent, [StringComparison]::OrdinalIgnoreCase)) { throw 'Refusing cleanup outside the test fixture directory.' }
    Remove-Item -LiteralPath $resolvedFixture -Recurse -Force
}

