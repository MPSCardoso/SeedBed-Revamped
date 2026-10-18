using System;
using System.Collections.Generic;
using System.Linq;

namespace SeedBed;

internal static class ResourceMapping
{
    internal static Dictionary<string, T> Create<T>(IDictionary<string, T> rawResources, IEnumerable<string> itemNames)
    {
        var result = new Dictionary<string, T>(rawResources);
        // Always read the original map: a swap must not overwrite its other half.
        foreach (string seedName in itemNames.Concat(rawResources.Keys).Where(name => name.EndsWith("seeds", StringComparison.Ordinal)).Distinct())
        {
            string cropName = seedName.Substring(0, seedName.Length - "seeds".Length);
            if (rawResources.TryGetValue(cropName, out var crop)) result[seedName] = crop;
            if (rawResources.TryGetValue(seedName, out var seeds)) result[cropName] = seeds;
        }
        foreach (string name in new[] { "$bca_chilipepper", "$bca_tomato" })
        {
            if (rawResources.TryGetValue(name, out var crop)) result[name + "seeds"] = crop;
        }
        return result;
    }
}
