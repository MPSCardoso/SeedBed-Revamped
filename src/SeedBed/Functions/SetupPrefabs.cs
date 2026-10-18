using PieceManager;
using UnityEngine;

namespace SeedBed.Functions;

public static class SetupPrefabs
{
    private static AssetBundle _seedBundle;

    public static void Init()
    {
        _seedBundle = PiecePrefabManager.RegisterAssetBundle("seedbedbundle");
        BuildPiece buildPiece = new BuildPiece(_seedBundle, "BSB_SeedBed");
        buildPiece.Crafting.Set(CraftingTable.Workbench);
        buildPiece.Usage.Set((UsageTagFlags)2);
        buildPiece.Category.Set("CollectorSeries");
        buildPiece.RequiredItems.Add("YmirRemains", 5, recover: true);
        buildPiece.RequiredItems.Add("BronzeNails", 8, recover: true);
        buildPiece.RequiredItems.Add("Wood", 18, recover: true);
        buildPiece.SpecialProperties = new SpecialProperties
        {
            AdminOnly = false,
            NoConfig = false
        };
        ShaderReplacer.Replace(buildPiece.Prefab);
        if (buildPiece.Prefab.GetComponent<Germination>() == null)
        {
            buildPiece.Prefab.AddComponent<Germination>();
        }
        PiecePrefabManager.RegisterPrefab(_seedBundle, "bsb_sfx_build_hammer_wood");
        PiecePrefabManager.RegisterPrefab(_seedBundle, "bsb_sfx_wood_destroyed");
        PiecePrefabManager.RegisterPrefab(_seedBundle, "bsb_vfx_Place_workbench");
        PiecePrefabManager.RegisterPrefab(_seedBundle, "bsb_vfx_SawDust");
    }

    public static void SetCollector(ZNetScene zNetScene)
    {
        if (Helper.ZNetSceneAwake())
        {
            Germination component = _seedBundle.LoadAsset<GameObject>("BSB_SeedBed").GetComponent<Germination>();
            component.m_name = "$bsb_seedbed";
            Plugin._germinationDuration.SettingChanged += delegate
            {
                component.m_germinationDuration = Plugin._germinationDuration.Value;
            };
            component.m_germinationDuration = Plugin._germinationDuration.Value;
            component.m_addEffects.m_effectPrefabs = new EffectData[2]
            {
                new EffectData
                {
                    m_prefab = zNetScene.GetPrefab("sfx_fermenter_add"),
                    m_enabled = true,
                    m_variant = -1
                },
                new EffectData
                {
                    m_prefab = zNetScene.GetPrefab("vfx_fermenter_add"),
                    m_enabled = true,
                    m_variant = -1
                }
            };
            component.m_harvestEffects.m_effectPrefabs = new EffectData[1]
            {
                new EffectData
                {
                    m_prefab = zNetScene.GetPrefab("sfx_pickable_pick"),
                    m_enabled = true,
                    m_variant = -1
                }
            };
            component.m_spawnEffects.m_effectPrefabs = new EffectData[2]
            {
                new EffectData
                {
                    m_prefab = zNetScene.GetPrefab("sfx_lootspawn"),
                    m_enabled = true,
                    m_variant = -1
                },
                new EffectData
                {
                    m_prefab = zNetScene.GetPrefab("vfx_lootspawn"),
                    m_enabled = true,
                    m_variant = -1
                }
            };
            Plugin._harvestDelay.SettingChanged += delegate
            {
                component.m_harvestDelay = Plugin._harvestDelay.Value;
            };
            component.m_harvestDelay = Plugin._harvestDelay.Value;
        }
    }
}
