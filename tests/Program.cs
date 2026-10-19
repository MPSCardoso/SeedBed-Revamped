using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using SeedBed;
using YamlDotNet.Serialization;
using ItemConversions = SeedBed.Plugin.ItemConversions;

int checks = 0;
void Check(bool condition, string description)
{
    if (!condition) throw new Exception(description);
    checks++;
}

var defaults = ConversionDefaults.GetDefaultConversions();
Check(defaults.Count == 27, "20 original conversions, five Deep North conversions and two Poteitr conversions must be present.");
Check(defaults.Select(entry => entry.m_from).Distinct().Count() == 27, "Defaults must have unique sources.");
Check(defaults.All(entry => entry.m_seedAmount == 5 && entry.m_producedItems == 15), "Default yield remains 5 -> 15.");
Check(defaults.Single(entry => entry.m_from == "Carrot").m_to == "CarrotSeeds", "Preserve the old crop-to-seed cycle.");
Check(defaults.Single(entry => entry.m_from == "Kale").m_to == "KaleSeeds", "Kale produces its seeds.");
Check(defaults.Single(entry => entry.m_from == "OatSeeds").m_to == "Oat", "Oat seeds produce oats.");
Check(defaults.Single(entry => entry.m_from == "Lingonberries").m_to == "Lingonberries", "Lingonberries reproduce without a seed item.");
Check(defaults.Single(entry => entry.m_from == "PoteitrSeeds").m_to == "Poteitr", "Poteitr seeds produce the vegetable.");
Check(defaults.Single(entry => entry.m_from == "Poteitr").m_to == "PoteitrSeeds", "Poteitr produces seeds for the full seedbed cycle.");

var custom = new List<ItemConversions>
{
    new() { m_from = "KaleSeeds", m_to = "Thistle", m_seedAmount = 2, m_producedItems = 7 },
    new() { m_from = "ModdedSeed", m_to = "ModdedCrop", m_seedAmount = 3, m_producedItems = 9 }
};
var merged = ConversionDefaults.MergeNewDefaults(custom, 0);
Check(custom.Count == 2 && merged.Count == 8, "Version 0 must receive only absent Deep North and Poteitr defaults, preserving the input.");
var retained = merged.Single(entry => entry.m_from == "KaleSeeds");
Check(retained.m_to == "Thistle" && retained.m_seedAmount == 2 && retained.m_producedItems == 7, "Preserve user overrides, including amounts.");
Check(ConversionDefaults.MergeNewDefaults(merged, 0).Count == merged.Count, "Repeated merge must be idempotent.");

var versionOne = defaults.Where(entry => entry.m_from != "Poteitr" && entry.m_from != "PoteitrSeeds").ToList();
Check(ConversionDefaults.MergeNewDefaults(versionOne, 1).Count == 27, "An unmodified 1.3.0 configuration must receive both Poteitr conversions.");
var customizedVersionOne = versionOne.Where(entry => entry.m_from != "Oat" && entry.m_from != "Lingonberries").ToList();
customizedVersionOne.Add(new ItemConversions { m_from = "PoteitrSeeds", m_to = "Thistle", m_seedAmount = 2, m_producedItems = 7 });
customizedVersionOne.Add(new ItemConversions { m_from = "Poteitr", m_to = "ModdedCrop", m_seedAmount = 3, m_producedItems = 9 });
var upgraded = ConversionDefaults.MergeNewDefaults(customizedVersionOne, 1);
Check(!upgraded.Any(entry => entry.m_from == "Oat" || entry.m_from == "Lingonberries"), "Version 1 migration must not restore older defaults deliberately removed by the user.");
Check(upgraded.Count == customizedVersionOne.Count && upgraded.Single(entry => entry.m_from == "PoteitrSeeds").m_to == "Thistle"
    && upgraded.Single(entry => entry.m_from == "PoteitrSeeds").m_seedAmount == 2 && upgraded.Single(entry => entry.m_from == "PoteitrSeeds").m_producedItems == 7,
    "Preserve existing Poteitr seed destinations and amounts without duplicates.");
Check(upgraded.Single(entry => entry.m_from == "Poteitr").m_to == "ModdedCrop" && upgraded.Single(entry => entry.m_from == "Poteitr").m_seedAmount == 3
    && upgraded.Single(entry => entry.m_from == "Poteitr").m_producedItems == 9, "Preserve existing Poteitr vegetable destinations and amounts.");
var removedPoteitr = defaults.Where(entry => entry.m_from != "Poteitr").ToList();
Check(!ConversionDefaults.MergeNewDefaults(removedPoteitr, 2).Any(entry => entry.m_from == "Poteitr"), "Completed migration must respect subsequent Poteitr removals.");
Check(ConversionDefaults.MergeNewDefaults(removedPoteitr, 3).Count == removedPoteitr.Count, "Future migration versions must not reapply old defaults.");

string directory = Path.Combine(Directory.GetCurrentDirectory(), "tests", ".tmp", "SeedBed-checks-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(directory);
try
{
    string file = Path.Combine(directory, "conversions.yml");
    string original = "# preserve this exact backup\n" + new SerializerBuilder().Build().Serialize(custom);
    File.WriteAllText(file, original);
    File.WriteAllText(file + ".pre-valheim1.bak", "historical backup");
    Check(ConversionDefaults.MigrateFile(file, 0) == 6, "Version 0 YAML must receive only six missing entries.");
    Check(File.ReadAllText(file + ConversionDefaults.BackupSuffix) == original, "Poteitr backup must preserve exact bytes/text including comments.");
    Check(File.ReadAllText(file + ".pre-valheim1.bak") == "historical backup", "The historical Deep North backup must remain untouched.");
    var roundtrip = new DeserializerBuilder().Build().Deserialize<List<ItemConversions>>(File.ReadAllText(file));
    Check(roundtrip.Count == 8 && roundtrip[0].m_to == "Thistle" && roundtrip[1].m_from == "ModdedSeed", "Migrated YAML must retain custom conversions.");
    string migrated = File.ReadAllText(file);
    Check(ConversionDefaults.MigrateFile(file, 0) == 0 && File.ReadAllText(file) == migrated, "Repeated file migration must not rewrite the YAML.");
    Check(!File.Exists(file + ".migration.tmp"), "Atomic migration must clean its temporary file.");
    File.WriteAllText(file, "From: [unterminated");
    bool rejected = false;
    try { ConversionDefaults.MigrateFile(file, 0); } catch { rejected = true; }
    Check(rejected && File.ReadAllText(file) == "From: [unterminated", "Invalid YAML must remain untouched.");
    Check(File.ReadAllText(file + ConversionDefaults.BackupSuffix) == original, "Later migrations must not overwrite the first Poteitr backup.");

    string upgradeFile = Path.Combine(directory, "version-one.yml");
    var missingPoteitr = customizedVersionOne.Where(entry => entry.m_from != "Poteitr").ToList();
    string upgradeOriginal = "# version 1 with overrides and removed crops\n" + new SerializerBuilder().Build().Serialize(missingPoteitr);
    File.WriteAllText(upgradeFile, upgradeOriginal);
    Check(ConversionDefaults.MigrateFile(upgradeFile, 1) == 1, "Version 1 must add only the missing half of the Poteitr cycle.");
    var upgradeRoundtrip = new DeserializerBuilder().Build().Deserialize<List<ItemConversions>>(File.ReadAllText(upgradeFile));
    Check(upgradeRoundtrip.Count == missingPoteitr.Count + 1 && !upgradeRoundtrip.Any(entry => entry.m_from == "Oat" || entry.m_from == "Lingonberries"),
        "Version 1 file migration must preserve removed older defaults.");
    var seedOverride = upgradeRoundtrip.Single(entry => entry.m_from == "PoteitrSeeds");
    Check(seedOverride.m_to == "Thistle" && seedOverride.m_seedAmount == 2 && seedOverride.m_producedItems == 7, "Poteitr overrides must survive YAML migration.");
    Check(File.ReadAllText(upgradeFile + ConversionDefaults.BackupSuffix) == upgradeOriginal, "Version 1 must receive its own exact pre-Poteitr backup.");
    string upgradedYaml = File.ReadAllText(upgradeFile);
    Check(ConversionDefaults.MigrateFile(upgradeFile, 1) == 0 && File.ReadAllText(upgradeFile) == upgradedYaml, "Retrying a successful version 1 migration must be idempotent.");
    File.WriteAllText(upgradeFile, upgradeOriginal);
    Check(ConversionDefaults.MigrateFile(upgradeFile, 2) == 0 && File.ReadAllText(upgradeFile) == upgradeOriginal, "Version 2 restarts must leave deliberately removed Poteitr entries untouched.");
    File.WriteAllText(upgradeFile, upgradedYaml);
    Check(ConversionDefaults.MigrateFile(upgradeFile, 1) == 0 && File.ReadAllText(upgradeFile) == upgradedYaml, "A configuration with all Poteitr sources needs no rewrite.");

    string invalidFile = Path.Combine(directory, "invalid-version-one.yml");
    File.WriteAllText(invalidFile, "From: [unterminated");
    rejected = false;
    try { ConversionDefaults.MigrateFile(invalidFile, 1); } catch { rejected = true; }
    Check(rejected && File.ReadAllText(invalidFile) == "From: [unterminated" && !File.Exists(invalidFile + ConversionDefaults.BackupSuffix)
        && !File.Exists(invalidFile + ".migration.tmp"), "Invalid version 1 YAML must remain untouched without backup or temporary files.");
    File.WriteAllText(invalidFile, new SerializerBuilder().Build().Serialize(versionOne));
    Check(ConversionDefaults.MigrateFile(invalidFile, 1) == 2, "A failed migration must be retryable after the YAML is corrected.");
    Check(File.ReadAllText(file + ConversionDefaults.BackupSuffix) == original, "Retries must never overwrite an existing backup.");
}
finally
{
    // Only files explicitly created by this test are removed; no recursive deletion.
    foreach (string file in Directory.GetFiles(directory)) File.Delete(file);
    Directory.Delete(directory);
}

var raw = new Dictionary<string, string>
{
    ["$item_carrot"] = "carrot-crop",
    ["$item_carrotseeds"] = "carrot-seeds",
    ["$item_kale"] = "kale-crop",
    ["$item_kaleseeds"] = "kale-seeds",
    ["$item_oat"] = "oat-crop",
    ["$item_poteitr"] = "poteitr-crop",
    ["$item_lingonberries"] = "berry-bush",
    ["$bca_tomato"] = "tomato-plant"
};
var map = ResourceMapping.Create(raw, new[] { "$item_oatseeds", "$item_poteitrseeds" });
Check(map["$item_carrotseeds"] == "carrot-crop" && map["$item_carrot"] == "carrot-seeds", "Preserve existing seed/crop inversion.");
Check(map["$item_kaleseeds"] == "kale-crop" && map["$item_kale"] == "kale-seeds", "New pairs must invert without depending on prefab names.");
Check(map["$item_oatseeds"] == "oat-crop" && map["$item_oat"] == "oat-crop", "Oats must work when only the crop Pickable exists.");
Check(map["$item_poteitrseeds"] == "poteitr-crop" && map["$item_poteitr"] == "poteitr-crop", "Both Poteitr inputs must map when only the vegetable Pickable exists.");
Check(map["$item_lingonberries"] == "berry-bush" && !map.ContainsKey("$item_lingonberriesseeds"), "Do not invent berry seeds.");
Check(map["$bca_tomatoseeds"] == "tomato-plant" && map["$bca_tomato"] == "tomato-plant", "Retain BCA compatibility.");
Check(raw["$item_carrot"] == "carrot-crop" && raw.Count == 8 && !raw.ContainsKey("$item_poteitrseeds"), "Mapping must not mutate the raw prefab inventory.");
var oneSided = ResourceMapping.Create(new Dictionary<string, string> { ["$item_kaleseeds"] = "seed-plant" }, Array.Empty<string>());
Check(oneSided["$item_kale"] == "seed-plant", "A seed-only Pickable must map to its vegetable input.");
var poteitrPair = new Dictionary<string, string>
{
    ["$item_poteitr"] = "poteitr-crop",
    ["$item_poteitrseeds"] = "poteitr-seeds"
};
var poteitrMap = ResourceMapping.Create(poteitrPair, poteitrPair.Keys);
Check(poteitrMap["$item_poteitrseeds"] == "poteitr-crop" && poteitrMap["$item_poteitr"] == "poteitr-seeds", "Poteitr resources must invert correctly when both Pickables exist.");
Check(poteitrPair["$item_poteitr"] == "poteitr-crop" && poteitrPair["$item_poteitrseeds"] == "poteitr-seeds", "Poteitr mapping must leave its raw pair unchanged.");
var poteitrSeedOnly = ResourceMapping.Create(new Dictionary<string, string> { ["$item_poteitrseeds"] = "poteitr-seeds" }, Array.Empty<string>());
Check(poteitrSeedOnly["$item_poteitr"] == "poteitr-seeds", "Poteitr seed-only resources must map to the vegetable input.");

var assembly = typeof(ConversionDefaults).Assembly;
using (var english = assembly.GetManifestResourceStream("SeedBed.translations.English.yml"))
{
    Check(english != null, "English localization must retain its logical resource name.");
    using var reader = new StreamReader(english!);
    var translations = new DeserializerBuilder().Build().Deserialize<Dictionary<string, string>>(reader.ReadToEnd());
    Check(new[] { "bsb_seedbed", "bsb_seedbed_empty", "bsb_seedbed_add", "bsb_seedbed_harvest", "bsb_notenoughseeds", "bsb_noaccess", "bsb_germentation_ready", "bsb_seedbed_germinating" }.All(key => translations.TryGetValue(key, out var text) && !string.IsNullOrWhiteSpace(text)), "Every interaction message must have a nonempty English translation.");
}
using (var bundle = assembly.GetManifestResourceStream("SeedBed.assets.seedbedbundle"))
{
    Check(bundle != null, "Asset bundle must retain its logical resource name.");
    using var contents = new MemoryStream();
    bundle!.CopyTo(contents);
    byte[] expected = File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "src", "assets", "seedbedbundle"));
    Check(SHA256.HashData(contents.ToArray()).SequenceEqual(SHA256.HashData(expected)), "Packaging must preserve the exact asset bundle bytes.");
}
using (var binary = File.OpenRead(assembly.Location))
using (var pe = new PEReader(binary))
{
    var metadata = pe.GetMetadataReader();
    var names = metadata.TypeDefinitions.Select(handle => metadata.GetTypeDefinition(handle))
        .Select(type => metadata.GetString(type.Namespace) + "." + metadata.GetString(type.Name)).ToList();
    Check(names.Contains("SeedBed.Functions.Germination"), "The serialized Unity component must keep its namespace and type name.");
    Check(new[] { "PieceManager.BuildPiece", "ServerSync.ConfigSync", "LocalizationManager.Localizer", "YamlDotNet.Serialization.DeserializerBuilder" }.All(names.Contains), "The distribution DLL must embed all four dependencies.");
    var references = metadata.AssemblyReferences.Select(handle => metadata.GetString(metadata.GetAssemblyReference(handle).Name)).ToList();
    Check(!references.Any(name => new[] { "PieceManager", "ServerSync", "LocalizationManager", "YamlDotNet" }.Contains(name)), "The merged DLL must not require separate manager or YAML assemblies.");
    Check(references.Contains("BepInEx") && references.Contains("assembly_valheim"), "Game and BepInEx assemblies must remain external.");
    Check(names.Count(name => name == "YamlDotNet.Serialization.DeserializerBuilder") == 1, "Only one YAML implementation must be bundled.");
}
Console.WriteLine($"PASS: {checks} regression checks (defaults, migration, resource mapping, localization and merged DLL).");
