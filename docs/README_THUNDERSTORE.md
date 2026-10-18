# SeedBed Revamped

Maintained by **Tetus**, based on [SeedBed by **blacks7ar**](https://thunderstore.io/c/valheim/p/blacks7ar/SeedBed/). Full credit to blacks7ar for the original mod, code, seed bed, assets and concept.

Grow seeds, vegetables, berries and mushrooms in a compact seed bed without cultivating a large field.

## What's new

- **Deep North crops:** kale and oats, including their seed conversions, plus lingonberries.
- **Native Farming XP when planting:** also retains XP when harvesting, with a configurable amount.
- **Harvest progress for eligible achievements:** uses Valheim's native statistics and achievement checks. The Farmer progress was validated in solo play with Unshamed enabled.

Existing conversions, configurable growth time, harvest delay, recipe and server synchronization are retained. Default batches consume 5 items and produce 15; harvest credit counts 5 plants, not 15 drops. Custom YAML conversions are preserved, with a one-time migration and backup for new defaults.

## Achievements

Valheim blocks achievements in modded sessions. SeedBed Revamped records native harvest statistics but does not bypass that global restriction. To earn achievements with mods, use an achievement-enabling mod such as [Unshamed by Azumatt](https://thunderstore.io/c/valheim/p/Azumatt/Unshamed/). The solo validation used Unshamed 1.0.8.

Different harvest categories apply to berries and agricultural crops; test The Farmer with a crop such as carrots.

## Installation and upgrades

1. Install BepInExPack for Valheim and close the game.
2. Disable the original SeedBed package. Keep only one active SeedBed DLL; store backups outside BepInEx/plugins.
3. Install SeedBed Revamped with your mod manager, or copy SeedBed.dll into BepInEx/plugins/SeedBedRevamped.
4. Install the same **1.3.0** version on the server and every client.

Managers and YamlDotNet are included in the single DLL. BepInEx and the game remain external.

The original blacks7ar.SeedBed.cfg and blacks7ar.SeedBed.yml configuration names, plugin ID and seed bed identity are retained for compatibility with existing configurations and placed beds. Revamped replaces the original and cannot be used alongside it.

## Credits

- **blacks7ar:** original SeedBed creator, base code, seed bed, assets, icon and concept.
- **Tetus:** Revamped maintenance and the features listed above.
- **Azumatt:** PieceManager and the separately installed Unshamed.
- **Tykea / blaxxun:** ServerSync and LocalizationManager.
- **Antoine Aubry and contributors:** YamlDotNet.

Third-party license notices and full credits are included in the package.
