using System;
using BepInEx.Bootstrap;
using UnityEngine;

namespace SeedBed.Functions;

public static class Helper
{
    public static bool ZNetSceneAwake()
    {
        if (ZNetScene.instance != null && ZNetScene.instance.m_prefabs.Count != 0)
        {
            return ZNetScene.instance.GetPrefab("piece_workbench") != null;
        }
        return false;
    }

    public static bool CheckHerbalistMod()
    {
        if (!Chainloader.PluginInfos.ContainsKey("blacks7ar.Herbalist"))
        {
            return false;
        }
        Logging.LogInfo("Herbalist mod detected! Applying patch..");
        return true;
    }

    public static void ChangeColor(this GameObject gameObject, Color color)
    {
        if (gameObject == null)
        {
            return;
        }
        Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>(true);
        if (componentsInChildren == null || componentsInChildren.Length == 0)
        {
            return;
        }
        for (int i = 0; i < componentsInChildren.Length; i++)
        {
            Material material = componentsInChildren[i].material;
            if (material != null)
            {
                material.SetColor("_Color", color);
            }
        }
    }

    public static void RemoveComponent(this GameObject gameObject, Type type)
    {
        Component component = gameObject.GetComponent(type);
        if (component != null)
        {
            Object.Destroy(component);
        }
        if (gameObject.GetComponentInChildren(type) != null)
        {
            Object.Destroy(component);
        }
    }
}
