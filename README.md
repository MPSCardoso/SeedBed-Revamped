# SeedBed Revamped

**English** · [Português (Brasil)](README.pt-BR.md)

Maintained by **Tetus**, based on [the original SeedBed by blacks7ar](https://thunderstore.io/c/valheim/p/blacks7ar/SeedBed/). Full credit to blacks7ar for the original code, seed bed, assets and concept. Grow seeds, vegetables, berries and mushrooms in a compact seed bed without cultivating a large field.

Version **1.3.1** adds the full Poteitr cycle: PoteitrSeeds → Poteitr and Poteitr → PoteitrSeeds. Deep North kale and oats, their seeds, lingonberries, native Farming XP when planting and harvesting, and native harvest statistics remain supported. Default conversions consume 5 items and produce 15, with 27 conversions in total.

Custom YAML conversions remain configurable. Upgrading from 1.3.0 adds only missing Poteitr entries once, preserving custom destinations, quantities and removed older crops. Earlier installations also receive the Deep North defaults. The migration saves a .pre-poteitr.bak backup without overwriting existing backups; invalid YAML is left untouched and retried on the next start. The local Defaults Version advances to 2 after a successful migration, so later removals are respected.

[Download on Hexium](https://valheim.hexium.gg/mods/Tetus/SeedBed_Revamped) · [GitHub project](https://github.com/MPSCardoso/SeedBed-Revamped)

## Validation status

SeedBed Revamped 1.3.0 has been tested on a multiplayer server, with all mod features validated by the maintainer. Planting, Farming XP and The Farmer achievement progress had also been validated in solo play with Unshamed enabled. The maintainer confirmed Poteitr validation in a test world for 1.3.1. See the [test record and checklist](STATUS_E_PROXIMOS_PASSOS.md); a separate Poteitr multiplayer test has not been reported.

## Achievements

SeedBed Revamped records harvests through Valheim's native APIs and preserves the game's eligibility rules. It does not remove the global achievement restriction in modded sessions. To earn achievements with mods, use an achievement-enabling mod such as [Unshamed by Azumatt](https://thunderstore.io/c/valheim/p/Azumatt/Unshamed/). The solo Farmer validation used Unshamed 1.0.8.

Berries and agricultural crops use different harvest categories; using seeds is not the deciding factor. Batch harvest credit counts SeedAmount plants, not ProducedItems drops.

## Installation

Close the game, disable the original SeedBed package and install **SeedBed.dll** from the repository root into BepInEx/plugins/SeedBedRevamped, or use your mod manager when the matching release is published. Keep only one active SeedBed DLL and store backups outside BepInEx/plugins. The server and every client need version **1.3.1**. Managers and YamlDotNet are embedded and do not need separate installation.

The internal blacks7ar.SeedBed plugin ID, blacks7ar.SeedBed.cfg/.yml configuration names, SeedBed assembly and SeedBed.Functions.Germination component are preserved for compatibility with existing installations and placed beds. Revamped replaces the original; do not install both together.

Backups are stored in artifacts/backups, outside the plugins folder. The build does not install files into Gale or change player settings.

## Release package

build.ps1 generates artifacts/releases/SeedBed_Revamped-1.3.1.zip after validation. It includes the manifest, a 256×256 icon, the [publication README](docs/README_THUNDERSTORE.md), changelog, credits/licenses and one DLL in BepInEx/plugins/SeedBedRevamped. The 1.3.1 package is prepared locally; publication is a separate step. The published 1.3.0 package has an empty website_url; the Hexium and GitHub pages are linked above.

To package an already validated DLL again:

~~~powershell
pwsh -NoProfile -File build/Create-Package.ps1
~~~

## Development

Requirements: .NET 10 SDK, PowerShell 7, and local Valheim and BepInEx installations.

1. Copy BuildPaths.example.props to BuildPaths.local.props.
2. Set ValheimPath to valheim_Data/Managed and BepInExCore to the profile's BepInEx/core directory.
3. Run:

~~~powershell
pwsh -NoProfile -File ./build.ps1
~~~

The script restores locked dependency versions, builds Release, runs regression checks against the packaged DLL, and only then updates the root DLL and creates the ZIP. It backs up the previous version. Override local paths with -ValheimPath and -BepInExCore; -NuGetSource accepts a local/offline NuGet source.

To compile without updating the distribution:

~~~powershell
dotnet build src/SeedBed.csproj -c Release
~~~

The distributable DLL is src/bin/Release/netstandard2.1/package/SeedBed.dll. The DLL outside the package directory is an intermediate artifact that requires separate dependencies.

## Repository layout

- src/SeedBed: mod code and metadata; Functions contains Unity/Valheim integration.
- src/assets and src/translations: original bundle and translations, embedded under the same logical names.
- build: dependency projects, hashes, preparation and packaging scripts.
- tests: regression checks for conversions, migration, resources, translations and packaging.
- .cache: restored official sources and local packages, ignored by Git.
- artifacts/backups: pre-cleanup snapshot and DLL backups, ignored by Git.

Publicized game references are generated in obj directories. No game DLL is tracked or distributed with the mod.

## Dependencies

YamlDotNet 16.0.0 comes from NuGet. PieceManager, ServerSync and LocalizationManager are built from official sources at pinned revisions, restored into a cache and checked with SHA-256. build/dependencies.lock.json records sources and hashes; packages.lock.json files lock NuGet packages. The build fails if a cached source does not match its expected hash.

ILRepack.Lib.MSBuild.Task 2.0.46 embeds the dependencies in one DLL; BepInEx.AssemblyPublicizer.MSBuild 0.4.3 prepares local references. Do not edit cached sources: deliberate dependency changes must update the manifest and pass validation again.

[Credits and licenses](docs/THIRD_PARTY_NOTICES.md) · [Verified vanilla APIs (Portuguese)](docs/VALHEIM_API.md) · [Status and in-game test record (Portuguese)](STATUS_E_PROXIMOS_PASSOS.md) · [Original mod demonstration](https://www.youtube.com/watch?v=C85sJ1sT49w)
