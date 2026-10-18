using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using ItemConversions = SeedBed.Plugin.ItemConversions;

namespace SeedBed;

internal static class ConversionDefaults
{
    internal static int MigrateFile(string filePath)
    {
        var existing = new DeserializerBuilder().Build().Deserialize<List<ItemConversions>>(File.ReadAllText(filePath)) ?? new List<ItemConversions>();
        var merged = MergeNewDefaults(existing);
        int added = merged.Count - existing.Count;
        if (added == 0) return 0;
        string contents = new SerializerBuilder().Build().Serialize(merged);
        string backup = filePath + ".pre-valheim1.bak";
        if (!File.Exists(backup)) File.Copy(filePath, backup);
        string temporaryPath = filePath + ".migration.tmp";
        try
        {
            File.WriteAllText(temporaryPath, contents);
            File.Replace(temporaryPath, filePath, null, ignoreMetadataErrors: true);
        }
        finally
        {
            if (File.Exists(temporaryPath)) File.Delete(temporaryPath);
        }
        return added;
    }

    internal static List<ItemConversions> GetDefaultConversions()
    {
        var conversions = new List<ItemConversions>
        {
            Create("Barley", "Barley"),
            Create("CarrotSeeds", "Carrot"),
            Create("Dandelion", "Dandelion"),
            Create("Fiddleheadfern", "Fiddleheadfern"),
            Create("Flax", "Flax"),
            Create("Mushroom", "Mushroom"),
            Create("MushroomMagecap", "MushroomMagecap"),
            Create("MushroomYellow", "MushroomYellow"),
            Create("MushroomJotunPuffs", "MushroomJotunPuffs"),
            Create("OnionSeeds", "Onion"),
            Create("Carrot", "CarrotSeeds"),
            Create("Onion", "OnionSeeds"),
            Create("Turnip", "TurnipSeeds"),
            Create("MushroomSmokePuff", "MushroomSmokePuff"),
            Create("Thistle", "Thistle"),
            Create("TurnipSeeds", "Turnip"),
            Create("Blueberries", "Blueberries"),
            Create("Cloudberry", "Cloudberry"),
            Create("Raspberry", "Raspberry"),
            Create("Vineberry", "Vineberry")
        };
        conversions.AddRange(GetNewDefaultConversions());
        return conversions;
    }

    internal static List<ItemConversions> GetNewDefaultConversions() => new List<ItemConversions>
    {
        Create("KaleSeeds", "Kale"),
        Create("Kale", "KaleSeeds"),
        Create("OatSeeds", "Oat"),
        Create("Oat", "OatSeeds"),
        Create("Lingonberries", "Lingonberries")
    };

    private static ItemConversions Create(string from, string to) => new ItemConversions
    {
        m_from = from,
        m_to = to,
        m_seedAmount = 5,
        m_producedItems = 15
    };

    internal static List<ItemConversions> MergeNewDefaults(List<ItemConversions> conversions)
    {
        var result = new List<ItemConversions>(conversions);
        var existing = new HashSet<string>(conversions.Select(entry => entry.m_from), StringComparer.Ordinal);
        foreach (var conversion in GetNewDefaultConversions())
        {
            if (existing.Add(conversion.m_from)) result.Add(conversion);
        }
        return result;
    }

}
