param([string]$ValheimPath, [string]$BepInExCore, [string]$NuGetSource)
$ErrorActionPreference = 'Stop'
$properties = @()
if ($ValheimPath) { $properties += "-p:ValheimPath=$ValheimPath" }
if ($BepInExCore) { $properties += "-p:BepInExCore=$BepInExCore" }
$restoreSource = @()
if ($NuGetSource) { $restoreSource = @('--source', [IO.Path]::GetFullPath($NuGetSource, $PSScriptRoot)) }
$previousBuildServer = $env:DOTNET_CLI_USE_MSBUILD_SERVER
$previousNodeReuse = $env:MSBUILDDISABLENODEREUSE
$env:DOTNET_CLI_USE_MSBUILD_SERVER = '0'
$env:MSBUILDDISABLENODEREUSE = '1'
Push-Location $PSScriptRoot
try {
    dotnet restore src/SeedBed.csproj --locked-mode --disable-parallel -m:1 -nr:false @properties @restoreSource
    if ($LASTEXITCODE -ne 0) { throw 'Dependency restore failed.' }
    dotnet build src/SeedBed.csproj -c Release --no-restore -m:1 -nr:false -p:UseSharedCompilation=false @properties
    if ($LASTEXITCODE -ne 0) { throw 'Release build failed.' }
    dotnet restore tests/SeedBed.RegressionChecks.csproj --locked-mode --disable-parallel -m:1 -nr:false @properties @restoreSource
    if ($LASTEXITCODE -ne 0) { throw 'Test restore failed.' }
    dotnet build tests/SeedBed.RegressionChecks.csproj -c Release --no-restore -m:1 -nr:false -p:UseSharedCompilation=false @properties
    if ($LASTEXITCODE -ne 0) { throw 'Regression build failed; distribution DLL was not changed.' }
    dotnet tests/bin/Release/net10.0/SeedBed.RegressionChecks.dll
    if ($LASTEXITCODE -ne 0) { throw 'Regression checks failed; distribution DLL was not changed.' }
    pwsh -NoProfile -File tests/DependencyRestore.Tests.ps1
    if ($LASTEXITCODE -ne 0) { throw 'Dependency integrity checks failed; distribution DLL was not changed.' }
    $packagePath = Join-Path $PSScriptRoot 'src/bin/Release/netstandard2.1/package/SeedBed.dll'
    $distributionPath = Join-Path $PSScriptRoot 'SeedBed.dll'
    $packageHash = (Get-FileHash -LiteralPath $packagePath -Algorithm SHA256).Hash
    pwsh -NoProfile -File build/Create-Package.ps1 -DistributionDll $packagePath
    if ($LASTEXITCODE -ne 0) { throw 'Release ZIP validation failed; distribution DLL was not changed.' }
    if ((Test-Path -LiteralPath $distributionPath) -and (Get-FileHash -LiteralPath $distributionPath -Algorithm SHA256).Hash -ne $packageHash) {
        $backupPath = Join-Path $PSScriptRoot 'artifacts/backups'
        New-Item -ItemType Directory -Path $backupPath -Force | Out-Null
        Copy-Item -LiteralPath $distributionPath -Destination (Join-Path $backupPath ("SeedBed-before-publish-" + (Get-Date -Format 'yyyyMMdd-HHmmss-fff') + '.dll'))
    }
    Copy-Item -LiteralPath $packagePath -Destination $distributionPath -Force
    if ((Get-FileHash -LiteralPath $distributionPath -Algorithm SHA256).Hash -ne $packageHash) { throw 'Distribution copy verification failed.' }
    Write-Host "Distribution ready: SeedBed.dll (SHA-256: $packageHash)"
} finally {
    Pop-Location
    $env:DOTNET_CLI_USE_MSBUILD_SERVER = $previousBuildServer
    $env:MSBUILDDISABLENODEREUSE = $previousNodeReuse
}

