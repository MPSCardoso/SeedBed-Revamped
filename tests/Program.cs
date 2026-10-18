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
Check(defaults.Count == 25, "20 previous conversions and five new conversions must be present.");
Check(defaults.Select(entry => entry.m_from).Distinct().Count() == 25, "Defaults must have unique sources.");
Check(defaults.All(entry => entry.m_seedAmount == 5 && entry.m_producedItems == 15), "Default yield remains 5 -> 15.");
Check(defaults.Single(entry => entry.m_from == "Carrot").m_to == "CarrotSeeds", "Preserve the old crop-to-seed cycle.");
Check(defaults.Single(entry => entry.m_from == "Kale").m_to == "KaleSeeds", "Kale produces its seeds.");
Check(defaults.Single(entry => entry.m_from == "OatSeeds").m_to == "Oat", "Oat seeds produce oats.");
Check(defaults.Single(entry => entry.m_from == "Lingonberries").m_to == "Lingonberries", "Lingonberries reproduce without a seed item.");

var custom = new List<ItemConversions>
{
    new() { m_from = "KaleSeeds", m_to = "Thistle", m_seedAmount = 2, m_producedItems = 7 },
    new() { m_from = "ModdedSeed", m_to = "ModdedCrop", m_seedAmount = 3, m_producedItems = 9 }
};
var merged = ConversionDefaults.MergeNewDefaults(custom);
Check(custom.Count == 2 && merged.Count == 6, "Merge must add only absent new defaults and preserve the input.");
var retained = merged.Single(entry => entry.m_from == "KaleSeeds");
Check(retained.m_to == "Thistle" && retained.m_seedAmount == 2 && retained.m_producedItems == 7, "Preserve user overrides, including amounts.");
Check(ConversionDefaults.MergeNewDefaults(merged).Count == merged.Count, "Repeated merge must be idempotent.");

string directory = Path.Combine(Directory.GetCurrentDirectory(), "tests", ".tmp", "SeedBed-checks-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(directory);
try
{
    string file = Path.Combine(directory, "conversions.yml");
    string original = "# preserve this exact backup\n" + new SerializerBuilder().Build().Serialize(custom);
    File.WriteAllText(file, original);
    Check(ConversionDefaults.MigrateFile(file) == 4, "Existing YAML must receive only four missing entries.");
    Check(File.ReadAllText(file + ".pre-valheim1.bak") == original, "Backup must preserve exact bytes/text including comments.");
    var roundtrip = new DeserializerBuilder().Build().Deserialize<List<ItemConversions>>(File.ReadAllText(file));
    Check(roundtrip.Count == 6 && roundtrip[0].m_to == "Thistle" && roundtrip[1].m_from == "ModdedSeed", "Migrated YAML must retain custom conversions.");
    string migrated = File.ReadAllText(file);
    Check(ConversionDefaults.MigrateFile(file) == 0 && File.ReadAllText(file) == migrated, "Repeated file migration must not rewrite the YAML.");
    Check(!File.Exists(file + ".migration.tmp"), "Atomic migration must clean its temporary file.");
    File.WriteAllText(file, "From: [unterminated");
    bool rejected = false;
    try { ConversionDefaults.MigrateFile(file); } catch { rejected = true; }
    Check(rejected && File.ReadAllText(file) == "From: [unterminated", "Invalid YAML must remain untouched.");
    Check(File.ReadAllText(file + ".pre-valheim1.bak") == original, "Later migrations must not overwrite the first backup.");
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
    ["$item_lingonberries"] = "berry-bush",
    ["$bca_tomato"] = "tomato-plant"
};
var map = ResourceMapping.Create(raw, new[] { "$item_oatseeds" });
Check(map["$item_carrotseeds"] == "carrot-crop" && map["$item_carrot"] == "carrot-seeds", "Preserve existing seed/crop inversion.");
Check(map["$item_kaleseeds"] == "kale-crop" && map["$item_kale"] == "kale-seeds", "New pairs must invert without depending on prefab names.");
Check(map["$item_oatseeds"] == "oat-crop" && map["$item_oat"] == "oat-crop", "Oats must work when only the crop Pickable exists.");
Check(map["$item_lingonberries"] == "berry-bush" && !map.ContainsKey("$item_lingonberriesseeds"), "Do not invent berry seeds.");
Check(map["$bca_tomatoseeds"] == "tomato-plant" && map["$bca_tomato"] == "tomato-plant", "Retain BCA compatibility.");
Check(raw["$item_carrot"] == "carrot-crop" && raw.Count == 7, "Mapping must not mutate the raw prefab inventory.");
var oneSided = ResourceMapping.Create(new Dictionary<string, string> { ["$item_kaleseeds"] = "seed-plant" }, Array.Empty<string>());
Check(oneSided["$item_kale"] == "seed-plant", "A seed-only Pickable must map to its vegetable input.");

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
