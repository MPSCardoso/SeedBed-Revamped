using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace SeedBed.Functions;

public static class ShaderReplacer
{
    private static readonly List<GameObject> ObjectsToSwap = new List<GameObject>();

    static ShaderReplacer()
    {
        var harmony = new Harmony("blacks7ar.utilities.ShaderReplacer");
        harmony.Patch(AccessTools.DeclaredMethod(typeof(FejdStartup), "Awake"),
            postfix: new HarmonyMethod(AccessTools.DeclaredMethod(typeof(ShaderReplacer), nameof(ReplaceShaderPatch))));
    }

    public static void Replace(GameObject gameObject) => ObjectsToSwap.Add(gameObject);

    [HarmonyPriority(700)]
    private static void ReplaceShaderPatch()
    {
        foreach (GameObject gameObject in ObjectsToSwap)
        {
            foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer == null) continue;
                foreach (Material material in renderer.sharedMaterials)
                {
                    if (material == null) continue;
                    foreach (Shader shader in Resources.FindObjectsOfTypeAll<Shader>())
                    {
                        if (material.shader.name == shader.name) material.shader = shader;
                    }
                }
            }
        }
    }
}
