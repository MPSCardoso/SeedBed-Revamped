using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using LocalizationManager;
using SeedBed.Functions;
using ServerSync;
using UnityEngine;
using YamlDotNet.Serialization;

namespace SeedBed;

[BepInPlugin(modGUID, modName, modVersion)]
[BepInDependency("blacks7ar.Herbalist", BepInDependency.DependencyFlags.SoftDependency)]
public class Plugin : BaseUnityPlugin
{
    public struct ItemConversions
    {
        [YamlMember(Alias = "From", ApplyNamingConventions = false)]
        public string m_from { get; set; } = "";

        [YamlMember(Alias = "To", ApplyNamingConventions = false)]
        public string m_to { get; set; } = "";

        [YamlMember(Alias = "SeedAmount", ApplyNamingConventions = false)]
        public int m_seedAmount { get; set; } = 0;

        [YamlMember(Alias = "ProducedItems", ApplyNamingConventions = false)]
        public int m_producedItems { get; set; } = 0;

        public ItemConversions()
        {
        }
    }

    [HarmonyPatch]
    private static class Patches
    {
        [HarmonyPriority(0)]
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ZNetScene), "Awake")]
        private static void Awake_Postfix(ZNetScene __instance)
        {
            GetResourcesFromZNetScene(__instance);
            SetupPrefabs.SetCollector(__instance);
        }
    }

    // Retain the original ID so existing configuration and replacement installs remain compatible.
    private const string modGUID = "blacks7ar.SeedBed";

    public const string modName = "SeedBed Revamped";

    public const string modAuthor = "Tetus";

    public const string originalAuthor = "blacks7ar";

    public const string modVersion = "1.3.1";

    public static string ymlFileName = "blacks7ar.SeedBed.yml";

    private static string ymlFullPath = Path.Combine(Paths.ConfigPath, ymlFileName);

    private static readonly Harmony _harmony = new Harmony(modGUID);

    private static readonly ConfigSync _configSync = new ConfigSync(modGUID)
    {
        DisplayName = modName,
        CurrentVersion = modVersion,
        MinimumRequiredVersion = modVersion
    };

    private static ConfigEntry<Toggle> _serverConfigLocked;

    public static ConfigEntry<float> _germinationDuration;

    public static ConfigEntry<float> _harvestDelay;

    public static ConfigEntry<float> _expAmountFarming;

    private const int CurrentDefaultsVersion = ConversionDefaults.CurrentVersion;

    private ConfigEntry<int> _defaultsVersion;

    public static ConfigEntry<float> _expAmountHerbalist;

    public static List<ItemConversions> _itemConversions = new List<ItemConversions>();

    private static readonly CustomSyncedValue<string> _ItemConversionsSyncedValue = new CustomSyncedValue<string>(_configSync, "Item Conversions", "");

    public static bool _hasHerbalistMod;

    public static readonly IDictionary<string, GameObject> _ResourceDB = new Dictionary<string, GameObject>();

    public static readonly IDictionary<string, Piece> _PlantDB = new Dictionary<string, Piece>();

    private static readonly HashSet<string> _conversionDiagnostics = new HashSet<string>();

    internal static void LogConversion(string from, string to, bool accepted)
    {
        string message = $"Conversion {from} -> {to}: {(accepted ? "accepted" : "unavailable or invalid")}";
        if (_conversionDiagnostics.Add(message)) Logging.LogInfo(message);
    }

    public static readonly HashSet<string> _herbalPlants = new HashSet<string> { "$bh_boswellia", "$bh_chamomile", "$bh_chicory", "$bh_daisy", "$bh_echinacea", "$bh_lavender", "$item_dandelion", "$item_thistle" };

    public static readonly HashSet<string> _notAllowedItems = new HashSet<string>
    {
        "$item_grausten", "$item_blackcore", "$item_ironore", "$item_wood", "$item_charredskull", "$item_dragonegg", "$item_lantern", "$item_coins", "$item_dvergrtankard", "$item_fishingrod",
        "$item_flint", "$item_bonefragments", "$item_wolfhairbundle", "$item_entrails", "$item_moltencore", "$item_crystal", "$item_obsidian", "$item_pot_shard_green", "$item_royaljelly", "$item_stone",
        "$item_sulfurstone", "$item_surtlingcore", "$item_Dyrnwyn_hilt", "$item_Dyrnwyn_blade", "$item_tar", "$item_tinore", "$item_voltureegg", "$item_Dyrnwyn_tip", "$item_goblintotem", "$item_wisp"
    };

    private ConfigEntry<T> BindConfig<T>(string group, string name, T value, ConfigDescription description, bool synchronizedConfig = true)
    {
        ConfigDescription syncedDescription = new ConfigDescription(description.Description + (synchronizedConfig ? " [Synced with Server]" : " [Not Synced with Server]"), description.AcceptableValues, description.Tags);
        ConfigEntry<T> entry = Config.Bind<T>(group, name, value, syncedDescription);
        _configSync.AddConfigEntry<T>(entry).SynchronizedConfig = synchronizedConfig;
        return entry;
    }

    private static void YmlDataWatcher()
    {
        FileSystemWatcher fileSystemWatcher = new FileSystemWatcher(Paths.ConfigPath, ymlFileName);
        fileSystemWatcher.Changed += YmlDataChanged;
        fileSystemWatcher.Created += YmlDataChanged;
        fileSystemWatcher.Renamed += YmlDataChanged;
        fileSystemWatcher.IncludeSubdirectories = true;
        fileSystemWatcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
        fileSystemWatcher.EnableRaisingEvents = true;
    }

    private static void YmlDataChanged(object sender, FileSystemEventArgs e)
    {
        if (!File.Exists(ymlFullPath))
        {
            return;
        }
        try
        {
            Logging.LogDebug("YmlDataWatcher called..");
            _ItemConversionsSyncedValue.AssignLocalValue(File.ReadAllText(ymlFullPath));
        }
        catch
        {
            Logging.LogError("There was an issue loading your " + ymlFileName);
            Logging.LogError("Please check your entries for spelling and format!");
        }
    }

    public void Awake()
    {
        Logging.LogInfo($"{modName} {modVersion} by {modAuthor}. Based on SeedBed by {originalAuthor}.");
        Localizer.Load();
        Config.SaveOnConfigSet = false;
        _serverConfigLocked = BindConfig("1- ServerSync", "Lock Configuration", Toggle.On, new ConfigDescription("If On, the configuration is locked and can be changed by server admins only.", null, Array.Empty<object>()));
        _configSync.AddLockingConfigEntry<Toggle>(_serverConfigLocked);
        _germinationDuration = BindConfig("2- General", "Germination Duration", 4000f, new ConfigDescription("How many seconds would it take to finished germination.", new AcceptableValueRange<float>(60f, 5000f), Array.Empty<object>()));
        _harvestDelay = BindConfig("2- General", "Harvest Delay", 1.5f, new ConfigDescription("Delay in seconds before the produced items spawns.", new AcceptableValueRange<float>(0.1f, 3f), Array.Empty<object>()));
        _expAmountFarming = BindConfig("3- Farming", "Exp Amount", 5f, new ConfigDescription("Exp amount given to the native Farming skill per seedbed planting or harvest.", new AcceptableValueRange<float>(0.01f, 25f), Array.Empty<object>()));
        // Local migration state must never be synchronized from the server.
        _defaultsVersion = Config.Bind("9- Internal", "Defaults Version", 0, "Last applied item conversion defaults migration.");
        _hasHerbalistMod = Helper.CheckHerbalistMod();
        if (_hasHerbalistMod)
        {
            _expAmountHerbalist = BindConfig("4- Herbalist", "Exp Amount", 1f, new ConfigDescription("Exp amount given to Herbalist skill when harvesting herbal plants from the seedbed.", new AcceptableValueRange<float>(0.01f, 5f), Array.Empty<object>()));
        }
        Assembly executingAssembly = Assembly.GetExecutingAssembly();
        _harmony.PatchAll(executingAssembly);
        Config.SaveOnConfigSet = true;
        Config.Save();
        if (!File.Exists(ymlFullPath))
        {
            CreateItemConversionYml();
            Logging.LogWarning("File not found - " + ymlFileName + "!");
            Logging.LogWarning("Creating a new one..");
        }
        MigrateItemConversions();
        _ItemConversionsSyncedValue.ValueChanged += ItemConversionsSyncedValueOnValueChanged;
        _ItemConversionsSyncedValue.AssignLocalValue(File.ReadAllText(ymlFullPath));
        SetupPrefabs.Init();
        YmlDataWatcher();
    }

    public static void ItemConversionsSyncedValueOnValueChanged()
    {
        Logging.LogDebug("ItemConversionsSyncedValueOnChanged called..");
        try
        {
            var conversions = new DeserializerBuilder().Build().Deserialize<List<ItemConversions>>(_ItemConversionsSyncedValue.Value);
            _itemConversions = conversions ?? new List<ItemConversions>();
        }
        catch (Exception error)
        {
            Logging.LogError($"Failed to deserializer {ymlFileName}: {error}");
        }
    }

    private void OnDestroy()
    {
        Config.Save();
    }

    private static void GetResourcesFromZNetScene(ZNetScene zNetScene)
    {
        if (!Helper.ZNetSceneAwake())
        {
            return;
        }
        _ResourceDB.Clear();
        _PlantDB.Clear();
        var rawResources = new Dictionary<string, GameObject>();
        var itemNames = new HashSet<string>();
        foreach (GameObject prefab in zNetScene.m_prefabs)
        {
            ItemDrop item = prefab.GetComponent<ItemDrop>();
            if (item != null) itemNames.Add(item.m_itemData.m_shared.m_name);
            string prefabName = prefab.name.ToLowerInvariant();
            Pickable component = prefab.GetComponent<Pickable>();
            if (component == null || prefabName.EndsWith("_wild") || prefabName.Contains("dvergerthing") || prefabName.Contains("_wild_"))
            {
                continue;
            }
            if (component.m_itemPrefab == null || component.m_itemPrefab.GetComponent<ItemDrop>() == null)
            {
                continue;
            }
            string name = component.m_itemPrefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_name;
            if (!_notAllowedItems.Contains(name) && !rawResources.ContainsKey(name))
            {
                rawResources.Add(name, prefab);
            }
        }
        foreach (var resource in ResourceMapping.Create(rawResources, itemNames)) _ResourceDB[resource.Key] = resource.Value;
        foreach (GameObject prefab in zNetScene.m_prefabs)
        {
            Plant plant = prefab.GetComponent<Plant>();
            Piece piece = prefab.GetComponent<Piece>();
            if (plant == null || piece == null || plant.m_grownPrefabs == null) continue;
            foreach (var resource in _ResourceDB)
            {
                if (!_PlantDB.ContainsKey(resource.Key) && plant.m_grownPrefabs.Contains(resource.Value)) _PlantDB[resource.Key] = piece;
            }
        }
        foreach (var resource in _ResourceDB.Where(entry => entry.Key.Contains("kale") || entry.Key.Contains("oat") || entry.Key.Contains("lingon") || entry.Key.Contains("poteitr")))
        {
            Logging.LogInfo($"Resource {resource.Key} -> {resource.Value.name} (item: {resource.Value.GetComponent<Pickable>().m_itemPrefab.name})");
        }
    }

    private static void CreateItemConversionYml()
    {
        File.WriteAllText(ymlFullPath, new SerializerBuilder().Build().Serialize(ConversionDefaults.GetDefaultConversions()));
    }

    private void MigrateItemConversions()
    {
        if (_defaultsVersion.Value >= CurrentDefaultsVersion) return;
        try
        {
            int added = ConversionDefaults.MigrateFile(ymlFullPath, _defaultsVersion.Value);
            if (added > 0)
            {
                Logging.LogInfo($"Added {added} new default conversions to {ymlFileName}. Backup: {ymlFullPath}{ConversionDefaults.BackupSuffix}");
            }
            _defaultsVersion.Value = CurrentDefaultsVersion;
        }
        catch (Exception error)
        {
            Logging.LogError($"Could not migrate {ymlFileName}; migration will retry on next start: {error}");
        }
    }

    static Plugin()
    {
    }
}
