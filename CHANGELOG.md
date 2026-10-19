# v1.3.1 — Poteitr support

- Added the full Poteitr cycle: PoteitrSeeds → Poteitr and Poteitr → PoteitrSeeds. Default batches consume 5 items and produce 15.
- Added localization-key fallback and resource diagnostics for Poteitr, using the existing seed/crop mapping and visuals.
- Upgrades existing 1.3.0 YAML configurations with only missing Poteitr conversions. Older installations also receive the Deep North defaults. Custom destinations, quantities and previously removed older crops are preserved.
- Creates a .pre-poteitr.bak backup before migrating, preserves earlier backups and retries failed migrations. Completed migrations respect later user removals.
- Requires version 1.3.1 on the server and every client. Poteitr was validated by the maintainer in a test world; the multiplayer validation below applies to 1.3.0.

# v1.3.0 — SeedBed Revamped

- Renamed to SeedBed Revamped, maintained by Tetus, with full credit to blacks7ar for the original SeedBed.
- Added Deep North crops (Kale/KaleSeeds and Oat/OatSeeds) and Lingonberries.
- Grants native Farming XP when planting and harvesting in the seed bed.
- Records harvests through native Valheim APIs for eligible achievement progress. Earning achievements in modded sessions requires an achievement-enabling mod such as Unshamed.
- Preserves the plugin ID, configuration names, assembly, prefab and Germination component for compatibility with existing beds. Replaces the original SeedBed; do not install both packages together.
- Includes a refreshed icon, English documentation, credits, license notices and a single merged DLL.
- Tested on a multiplayer server with all mod features validated by the maintainer. Planting, XP and The Farmer progress had also been validated in solo play with Unshamed enabled.

# Development cleanup — 1.2.10

- Replaced decompiled third-party sources with pinned official managers and YamlDotNet 16.0.0 from NuGet.
- Generates publicized game references during the build and distributes a single merged DLL using ILRepack.
- Refactored casts, names and visual loops while preserving mod contracts.
- Expanded localization and packaging regressions; backups are stored outside the versioned source tree.
- In-game validation completed by the maintainer, including multiplayer and all mod features in the 1.3.0 release.

# v1.2.10 — local fixes

- Added Kale/KaleSeeds, Oat/OatSeeds and Lingonberries with one-time YAML migration and backup.
- Detects seed/vegetable pairs generically and resolves new item names through localization keys when needed.
- Grants native Farming XP when planting and harvesting and records native harvest statistics.
- Confirms planting and harvesting with the seed bed owner before granting credit; rejected planting requests return seeds.
- Requires matching 1.2.10 versions on clients and server. Visuals and final integration still require in-game validation.

#
# v1.2.9
- updated for valheim 1.0 deepnorth
#
# v1.2.8
- updated for the latest bepinex build (5.4.2333).
- updated for the latest valheim build (221.4) call to arms update.
- updated unity project with the latest game rip.
#
# v1.2.7
- updated serversync.
#
# v1.2.6
- fixed visual bug for herbalist mod's plants and added the new plants on the example yml config.
#
# v1.2.5
- readded vulkan support
#
# v1.2.4
- updated piece manager
#
# v1.2.3
- updated for the latest valheim build (219.13) bog witch
#
# v1.2.2
- as requested disabled exp gain when planting for farming and herbalist to avoid exploit, you can only get exp when harvesting.
- minor code fixes
#
# v1.2.1
- added additional filter when adding data on the yml
- added more null checks to avoid null reference errors.
#
# v1.2.0
- reworked and rewritten to support other mods that adds their own pickable/plantable plants (currently tested supported mods: cookingadditions, herbalist and therzie's wizardry).
- you can now configure which plant is allowed in the seedbed through a yml file. (includes a yml file which contains 34 tested items vanilla and modded)
#
# v1.1.3
- fixed build menu tab scaling (updated piece manager).
#
# v1.1.2
- updated for the latest valheim build (218.15 ashlands)
#
# v1.1.1
- updated for the latest valheim build (217.46)
#
# v1.1.0
- added dandelion and thistle to be plantable at the seedbed.
- put seedbed into a custom build category called CollectorSeries (all of my collector mods will be put into this category) (configurable)
#
# v1.0.8
- added support to my cookingadditions mod, you can now plant chilipepper and tomato into the seedbed.
- added support to my herbalist mod, you can now plant herbal plants into the seedbed, you'll gain exp for herbalist skill if you plant and harvest herbal plants from the seedbed
#
# v1.0.3
- added support to Farming skill mod. You can now get an exp for farming skill when harvesting and planting at the seedbed (if farming skill is already learned).
#
# v1.0.2
- added snapping points to the seedbed.
- added mushrooms, yellowmushrooms, cloudberry, blueberry and rapsberry to be plantable.
- added config options for seedbeds required crafting materials.
#
# v1.0.0
- first release.
#
