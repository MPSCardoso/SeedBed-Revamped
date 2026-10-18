param([string]$Name)
$ErrorActionPreference = 'Stop'
$repoPath = Split-Path $PSScriptRoot -Parent
$manifest = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'dependencies.lock.json') -Raw | ConvertFrom-Json
$dependencies = @($manifest.dependencies | Where-Object { !$Name -or $_.name -eq $Name })
if ($dependencies.Count -eq 0) { throw "Unknown dependency: $Name" }
foreach ($dependency in $dependencies) {
    $cachePath = Join-Path $repoPath ".cache/dependencies/$($dependency.name)/$($dependency.revision)"
    New-Item -ItemType Directory -Path $cachePath -Force | Out-Null
    foreach ($file in $dependency.files) {
        $filePath = Join-Path $cachePath $file.name
        if (!(Test-Path -LiteralPath $filePath)) {
            $downloadPath = "$filePath.download"
            try {
                Invoke-WebRequest -Uri $file.url -OutFile $downloadPath
                $actualHash = (Get-FileHash -LiteralPath $downloadPath -Algorithm SHA256).Hash
                if ($actualHash -ne $file.sha256) { throw "Hash mismatch for $($dependency.name)/$($file.name)" }
                Move-Item -LiteralPath $downloadPath -Destination $filePath
            } finally {
                if (Test-Path -LiteralPath $downloadPath) { Remove-Item -LiteralPath $downloadPath }
            }
        }
        if ((Get-FileHash -LiteralPath $filePath -Algorithm SHA256).Hash -ne $file.sha256) {
            throw "Hash mismatch in cached $($dependency.name)/$($file.name). Restore the pinned file before building."
        }
    }
}
