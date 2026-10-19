using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SeedBed.Functions;

public class Germination : MonoBehaviour, Hoverable, Interactable
{
    private class ItemConversion
    {
        public ItemDrop m_from;

        public ItemDrop m_to;

        public int m_seedAmount = 5;

        public int m_producedItems = 15;
    }

    private enum Status
    {
        Empty,
        Germinating,
        Ready
    }

    public string m_name = "Seed Bed";

    public float m_germinationDuration = 2400f;

    public GameObject m_sapling;

    public GameObject m_grown;

    public GameObject m_saplingT5x5;

    public GameObject m_grownT5x5;

    public GameObject m_saplingT3x1;

    public GameObject m_grownT3x1;

    public GameObject m_saplingVine;

    public GameObject m_grownVine;

    public GameObject m_saplingVineWall;

    public GameObject m_GrownVineWall;

    public EffectList m_addEffects = new EffectList();

    public EffectList m_harvestEffects = new EffectList();

    public EffectList m_spawnEffects = new EffectList();

    public Switch m_addSwitch;

    public Switch m_harvestSwitch;

    public float m_harvestDelay = 1.5f;

    public Transform m_spawnPoint;

    private List<ItemConversion> m_conversion = new List<ItemConversion>();

    public ZNetView m_nview;

    public string m_delayedTapItem = "";

    public float m_hoverOffset;

    private string m_pendingHarvest = "";

    private Humanoid m_pendingHarvester;

    private ItemData m_pendingPlant;

    private Humanoid m_pendingPlanter;

    private float m_harvestRequestTime;

    public void Awake()
    {
        m_nview = this.GetComponent<ZNetView>();
        SetTransforms();
        SetConversion();
        if (m_nview != null && m_nview.GetZDO() != null)
        {
            DisableGameObject();
            ReapplyVisuals();
            m_nview.Register<string>("RPC_AddItem", (Action<long, string>)RPC_AddItem);
            m_nview.Register<string>("RPC_Planted", RPC_Planted);
            m_nview.Register("RPC_Tap", (Action<long>)RPC_Tap);
            m_nview.Register<string>("RPC_Harvested", RPC_Harvested);
            this.InvokeRepeating("SlowUpdate", (GetStatus() == Status.Germinating) ? 2f : 0f, 2f);
            WearNTear component = this.GetComponent<WearNTear>();
            if ((component != null))
            {
                component.m_onDestroyed = (Action)Delegate.Combine(component.m_onDestroyed, new Action(OnDestroyed));
            }
        }
    }

    public void DisableGameObject()
    {
        m_sapling.SetActive(false);
        m_grown.SetActive(false);
    }

    public void SetTransforms()
    {
        if (m_nview != null && m_nview.GetZDO() != null)
        {
            m_sapling = this.transform.Find("_sapling").gameObject;
            m_grown = this.transform.Find("_grown").gameObject;
            m_saplingT5x5 = m_sapling.transform.Find("_sapling5x5").gameObject;
            m_grownT5x5 = m_grown.transform.Find("_grown5x5").gameObject;
            m_saplingT3x1 = m_sapling.transform.Find("_sapling3x1").gameObject;
            m_grownT3x1 = m_grown.transform.Find("_grown3x1").gameObject;
            m_saplingVine = m_sapling.transform.Find("_saplingvine").gameObject;
            m_saplingVineWall = m_sapling.transform.Find("deco").gameObject;
            m_grownVine = m_grown.transform.Find("_grownvine").gameObject;
            m_GrownVineWall = m_grown.transform.Find("deco").gameObject;
            m_addSwitch = this.transform.Find("add_seeds").GetComponent<Switch>();
            m_harvestSwitch = this.transform.Find("collect_produce").GetComponent<Switch>();
            m_spawnPoint = this.transform.Find("spawnpoint");
        }
    }

    public void SetConversion()
    {
        m_conversion.Clear();
        foreach (var data in Plugin._itemConversions)
        {
            ItemDrop from = ResolveConversionItem(data.m_from);
            ItemDrop to = ResolveConversionItem(data.m_to);
            bool accepted = from != null && to != null && data.m_seedAmount > 0 && data.m_producedItems > 0;
            Plugin.LogConversion(data.m_from, data.m_to, accepted);
            if (!accepted || m_conversion.Any(entry => entry.m_from == from)) continue;
            m_conversion.Add(new ItemConversion
            {
                m_from = from,
                m_to = to,
                m_seedAmount = data.m_seedAmount,
                m_producedItems = data.m_producedItems
            });
        }
        Logging.LogDebug("Added " + m_conversion.Count + " items to conversion list from " + Plugin._itemConversions.Count + " items in " + Plugin.ymlFileName + " file.");
    }

    private static ItemDrop ResolveConversionItem(string prefabName)
    {
        if (string.IsNullOrEmpty(prefabName)) return null;
        ItemDrop item = ObjectDB.instance.GetItemPrefab(prefabName)?.GetComponent<ItemDrop>();
        if (item != null) return item;
        // New prefab names are not confirmed; localization keys were verified in the game assets.
        if (prefabName != "Kale" && prefabName != "KaleSeeds" && prefabName != "Oat" && prefabName != "OatSeeds" && prefabName != "Lingonberries" && prefabName != "Poteitr" && prefabName != "PoteitrSeeds") return null;
        string itemKey = "$item_" + prefabName.ToLowerInvariant();
        return ObjectDB.instance.m_items.Select(prefab => prefab.GetComponent<ItemDrop>())
            .FirstOrDefault(candidate => candidate != null && candidate.m_itemData.m_shared.m_name == itemKey);
    }

    public void DropAllItems()
    {
        string content = GetContent();
        if (string.IsNullOrEmpty(content)) return;
        ItemConversion conversion = GetItemConversion(content);
        if (conversion != null)
        {
            bool ready = GetStatus() == Status.Ready;
            ItemDrop item = ready ? conversion.m_to : conversion.m_from;
            int amount = ready ? conversion.m_producedItems : conversion.m_seedAmount;
            for (int index = 0; index < amount; index++)
            {
                Vector3 position = transform.position + Vector3.up + Random.insideUnitSphere * 0.3f;
                Quaternion rotation = Quaternion.Euler(0f, Random.Range(0, 360), 0f);
                ItemDrop.OnCreateNew(Object.Instantiate(item.gameObject, position, rotation), false);
            }
        }
        m_nview.GetZDO().Set(ZDOVars.s_content, "");
        m_nview.GetZDO().Set(ZDOVars.s_startTime, 0, false);
    }

    public void OnDestroyed()
    {
        m_nview.IsOwner();
        DropAllItems();
    }

    public string GetHoverName()
    {
        return m_name;
    }

    public string GetHoverText()
    {
        if (!PrivateArea.CheckAccess(this.transform.position, 0f, false, false))
        {
            return Localization.instance.Localize(m_name + "\n$bsb_noaccess");
        }
        switch (GetStatus())
        {
        case Status.Ready:
        {
            string contentName2 = GetContentName();
            return Localization.instance.Localize(m_name + "\n( " + contentName2 + ", $bsb_germentation_ready )\n[<color=yellow><b>$KEY_Use</b></color>] $bsb_seedbed_harvest");
        }
        case Status.Germinating:
        {
            string contentName = GetContentName();
            return Localization.instance.Localize(m_name + " <color=orange><b>" + TimeLeft(this) + "</b></color>\n( " + contentName + ", $bsb_seedbed_germinating )");
        }
        case Status.Empty:
            return Localization.instance.Localize(m_name + " ( $bsb_seedbed_empty )\n[<color=yellow><b>$KEY_Use</b></color>] $bsb_seedbed_add");
        default:
            return m_name;
        }
    }

    public bool Interact(Humanoid user, bool hold, bool alt)
    {
        if (hold)
        {
            return false;
        }
        if (!PrivateArea.CheckAccess(this.transform.position, 0f, true, false))
        {
            return false;
        }
        switch (GetStatus())
        {
        case Status.Empty:
        {
            ItemData seed = FindPlantableSeed(user.GetInventory());
            if (seed == null)
            {
                user.Message((MessageType)2, "$bsb_notenoughseeds", 0, null, false);
                return false;
            }
            ItemConversion conversion = GetItemConversion(seed.m_dropPrefab.name);
            return AddItem(user, seed, conversion.m_seedAmount);
        }
        case Status.Ready:
        {
            // Capture content on the interacting client before the owner clears the ZDO.
            ItemConversion itemConversion = GetItemConversion(GetContent());
            if (itemConversion == null || (!string.IsNullOrEmpty(m_pendingHarvest) && Time.time - m_harvestRequestTime < 5f)) return false;
            m_pendingHarvest = GetContent();
            m_pendingHarvester = user;
            m_harvestRequestTime = Time.time;
            m_nview.InvokeRPC("RPC_Tap", Array.Empty<object>());
            return true;
        }
        default:
            return false;
        }
    }

    public bool UseItem(Humanoid user, ItemData item)
    {
        if (!PrivateArea.CheckAccess(this.transform.position, 0f, true, false))
        {
            return false;
        }
        ItemConversion itemConversion = GetItemConversion(item.m_dropPrefab.name);
        if (itemConversion == null)
        {
            return false;
        }
        if (item.m_stack < itemConversion.m_seedAmount)
        {
            user.Message((MessageType)2, "$bsb_notenoughseeds", 0, null, false);
            return false;
        }
        if (!AddItem(user, item, itemConversion.m_seedAmount))
        {
            return false;
        }
        return true;
    }

    public void ReapplyVisuals()
    {
        string fromName = GetFromName();
        SetVisuals(fromName);
    }

    public void SlowUpdate()
    {
        switch (GetStatus())
        {
        case Status.Empty:
            DisableGameObject();
            break;
        case Status.Germinating:
            m_sapling.SetActive(true);
            m_grown.SetActive(false);
            break;
        case Status.Ready:
            m_sapling.SetActive(false);
            m_grown.SetActive(true);
            break;
        }
    }

    private Status GetStatus()
    {
        if (string.IsNullOrEmpty(GetContent()))
        {
            return Status.Empty;
        }
        if (GetGerminationTime() > (double)m_germinationDuration)
        {
            return Status.Ready;
        }
        return Status.Germinating;
    }

    private bool AddItem(Humanoid user, ItemData item, int amount)
    {
        if (GetStatus() != Status.Empty || m_pendingPlant != null)
        {
            return false;
        }
        if (!IsItemAllowed(item))
        {
            return false;
        }
        ItemData plantedItem = item.Clone();
        plantedItem.m_stack = amount;
        if (!user.GetInventory().RemoveItem(item, amount))
        {
            return false;
        }
        m_pendingPlant = plantedItem;
        m_pendingPlanter = user;
        m_nview.InvokeRPC("RPC_AddItem", new object[1] { item.m_dropPrefab.name });
        SetVisuals(item.m_shared.m_name);
        return true;
    }

    private static Pickable GetPickable(ItemConversion conversion)
    {
        return Plugin._ResourceDB.TryGetValue(conversion.m_from.m_itemData.m_shared.m_name, out var prefab)
            ? prefab.GetComponent<Pickable>() : null;
    }

    private static void RecordHarvest(Humanoid user, ItemConversion conversion)
    {
        if (user is not Player player || player != Player.m_localPlayer) return;
        Pickable pickable = GetPickable(conversion);
        player.RaiseSkill(SkillType.Farming, Plugin._expAmountFarming.Value);
        if (Plugin._hasHerbalistMod && Plugin._herbalPlants.Contains(conversion.m_to.m_itemData.m_shared.m_name))
        {
            player.GetSkills().m_skillData.TryGetValue((SkillType)Mathf.Abs(StringExtensionMethods.GetStableHashCode("Herbalist")), out var herbalist);
            if (herbalist != null) player.RaiseSkill(herbalist.m_info.m_skill, Plugin._expAmountHerbalist.Value);
        }
        if (Game.instance == null) return;
        if (pickable != null && pickable.m_harvestStat != PlayerStatType.None)
        {
            Game.instance.IncrementPlayerStat(pickable.m_harvestStat, conversion.m_seedAmount);
        }
        string itemName = pickable != null && pickable.m_itemPrefab != null ? pickable.m_itemPrefab.name : conversion.m_to.gameObject.name;
        Game.instance.GetPlayerProfile().IncrementStatPickable(itemName, conversion.m_seedAmount);
    }

    private static void RecordPlanting(Humanoid user, ItemConversion conversion, int amount)
    {
        if (conversion == null || user is not Player player || player != Player.m_localPlayer) return;
        player.RaiseSkill(SkillType.Farming, Plugin._expAmountFarming.Value);
        if (Game.instance == null) return;
        Game.instance.IncrementPlayerStat(PlayerStatType.Builds, amount);
        if (Plugin._PlantDB.TryGetValue(conversion.m_from.m_itemData.m_shared.m_name, out var piece))
        {
            Game.instance.GetPlayerProfile().IncrementStatBuildPiecePlaced(piece.m_name, amount);
        }
    }

    public void ResetVisuals()
    {
        foreach (GameObject container in new[] { m_saplingT5x5, m_grownT5x5, m_saplingT3x1, m_grownT3x1, m_saplingVine, m_grownVine })
        {
            foreach (Transform slot in container.transform)
            {
                foreach (Transform visual in slot) Object.Destroy(visual.gameObject);
            }
        }
    }

    public void SetVisuals(string itemName)
    {
        if (!Plugin._ResourceDB.TryGetValue(itemName, out var prefab)) return;
        string prefabName = prefab.name.ToLowerInvariant();
        GameObject saplings;
        GameObject grownPlants;
        bool vines = false;
        bool removeEffects = false;
        if (prefabName.Contains("pickable_"))
        {
            saplings = m_saplingT5x5;
            grownPlants = m_grownT5x5;
            removeEffects = true;
        }
        else if (prefabName.Contains("blueberrybush") || prefabName.Contains("raspberrybush") || (prefabName.EndsWith("plant") && !prefabName.Contains("lingonberry")))
        {
            saplings = m_saplingT3x1;
            grownPlants = m_grownT3x1;
        }
        else if (prefabName.Contains("cloudberrybush") || prefabName.Contains("lingonberry"))
        {
            saplings = m_saplingT5x5;
            grownPlants = m_grownT5x5;
        }
        else if (prefabName.Contains("vineash"))
        {
            saplings = m_saplingVine;
            grownPlants = m_grownVine;
            vines = true;
        }
        else return;

        foreach (Transform source in prefab.transform)
        {
            CreateVisuals(source.gameObject, saplings, prefab.name, growing: true, vines, removeEffects);
            CreateVisuals(source.gameObject, grownPlants, prefab.name, growing: false, vines, removeEffects: false);
        }
        if (vines)
        {
            m_saplingVineWall.SetActive(true);
            m_GrownVineWall.SetActive(true);
        }
    }

    private void CreateVisuals(GameObject source, GameObject container, string prefabName, bool growing, bool vines, bool removeEffects)
    {
        foreach (Transform slot in container.transform)
        {
            GameObject visual = Object.Instantiate(source, slot);
            visual.name = prefabName;
            if (!vines) visual.transform.localPosition = Vector3.zero;
            else if (growing) visual.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            if (growing) container.ChangeColor(new Color(0f, 1f, 0.45f));
            if (!vines) (growing ? m_saplingVineWall : m_GrownVineWall).SetActive(false);
            if (removeEffects) RemoveVisualEffects(visual);
        }
    }

    private static void RemoveVisualEffects(GameObject visual)
    {
        foreach (Transform child in visual.transform)
        {
            child.gameObject.RemoveComponent(typeof(ParticleSystem));
            child.gameObject.RemoveComponent(typeof(Light));
            child.gameObject.RemoveComponent(typeof(LightLod));
        }
        visual.RemoveComponent(typeof(Light));
        visual.RemoveComponent(typeof(ParticleSystem));
        visual.RemoveComponent(typeof(LightLod));
    }

    public void RPC_AddItem(long sender, string name)
    {
        if (m_nview.IsOwner() && GetStatus() == Status.Empty)
        {
            if (!IsItemAllowed(name))
            {
                ZLog.DevLog((object)"Item not allowed");
                m_nview.InvokeRPC(sender, "RPC_Planted", new object[] { "" });
                return;
            }
            m_addEffects.Create(this.transform.position, this.transform.rotation, null, 1f, -1, default(ZDOID));
            m_nview.GetZDO().Set(ZDOVars.s_content, name);
            m_nview.GetZDO().Set(ZDOVars.s_startTime, ZNet.instance.GetTime().Ticks);
            m_nview.InvokeRPC(sender, "RPC_Planted", new object[] { name });
        }
        else if (m_nview.IsOwner())
        {
            m_nview.InvokeRPC(sender, "RPC_Planted", new object[] { "" });
        }
    }

    private void RPC_Planted(long sender, string itemName)
    {
        if (sender != m_nview.GetZDO().GetOwner() || m_pendingPlant == null) return;
        ItemData item = m_pendingPlant;
        Humanoid user = m_pendingPlanter;
        m_pendingPlant = null;
        m_pendingPlanter = null;
        if (user == null) return;
        if (itemName == item.m_dropPrefab.name)
        {
            RecordPlanting(user, GetItemConversion(itemName), item.m_stack);
        }
        else
        {
            // A competing client may have filled the bed before this request arrived.
            if (!user.GetInventory().AddItem(item)) ItemDrop.DropItem(item, item.m_stack, user.transform.position + Vector3.up, Quaternion.identity);
            ResetVisuals();
        }
    }

    public void RPC_Tap(long sender)
    {
        if (m_nview.IsOwner() && GetStatus() == Status.Ready)
        {
            m_delayedTapItem = GetContent();
            this.Invoke("DelayedTap", m_harvestDelay);
            m_harvestEffects.Create(this.transform.position, this.transform.rotation, null, 1f, -1, default(ZDOID));
            m_nview.GetZDO().Set(ZDOVars.s_content, "");
            m_nview.GetZDO().Set(ZDOVars.s_startTime, 0, false);
            ResetVisuals();
            // Only the client whose request the owner accepted receives XP/stat credit.
            m_nview.InvokeRPC(sender, "RPC_Harvested", new object[] { m_delayedTapItem });
        }
        else if (m_nview.IsOwner())
        {
            m_nview.InvokeRPC(sender, "RPC_Harvested", new object[] { "" });
        }
    }

    private void RPC_Harvested(long sender, string itemName)
    {
        if (sender != m_nview.GetZDO().GetOwner() || string.IsNullOrEmpty(m_pendingHarvest)) return;
        string pending = m_pendingHarvest;
        Humanoid user = m_pendingHarvester;
        m_pendingHarvest = "";
        m_pendingHarvester = null;
        if (itemName != pending) return;
        ItemConversion conversion = GetItemConversion(itemName);
        if (conversion != null && user != null) RecordHarvest(user, conversion);
    }

    public void DelayedTap()
    {
        m_spawnEffects.Create(this.transform.position, Quaternion.identity, null, 1f, -1, default(ZDOID));
        ItemConversion itemConversion = GetItemConversion(m_delayedTapItem);
        if (itemConversion != null)
        {
            for (int i = 0; i < itemConversion.m_producedItems; i++)
            {
                Vector3 position = m_spawnPoint.position + Vector3.up * 0.3f;
                ItemDrop.OnCreateNew(Object.Instantiate<ItemDrop>(itemConversion.m_to, position, Quaternion.identity), false);
            }
        }
    }

    public void ResetGerminationTimer()
    {
        if (GetStatus() == Status.Germinating)
        {
            m_nview.GetZDO().Set(ZDOVars.s_startTime, ZNet.instance.GetTime().Ticks);
        }
    }

    private double GetGerminationTime()
    {
        DateTime dateTime = new DateTime(m_nview.GetZDO().GetLong(ZDOVars.s_startTime, 0L));
        if (dateTime.Ticks == 0L)
        {
            return -1.0;
        }
        return (ZNet.instance.GetTime() - dateTime).TotalSeconds;
    }

    private string GetFromName()
    {
        string content = GetContent();
        if (string.IsNullOrEmpty(content))
        {
            return "";
        }
        ItemConversion itemConversion = GetItemConversion(content);
        if (itemConversion == null)
        {
            return "Invalid";
        }
        return itemConversion.m_from.m_itemData.m_shared.m_name;
    }

    private string GetContentName()
    {
        string content = GetContent();
        if (string.IsNullOrEmpty(content))
        {
            return "";
        }
        ItemConversion itemConversion = GetItemConversion(content);
        if (itemConversion == null)
        {
            return "Invalid";
        }
        return itemConversion.m_to.m_itemData.m_shared.m_name;
    }

    private string GetContent()
    {
        if (m_nview.GetZDO() != null)
        {
            return m_nview.GetZDO().GetString(ZDOVars.s_content, "");
        }
        return "";
    }

    private bool IsItemAllowed(ItemData item)
    {
        return IsItemAllowed(item.m_dropPrefab.name);
    }

    private bool IsItemAllowed(string itemName)
    {
        return m_conversion.Any((ItemConversion item) => item.m_from.gameObject.name == itemName);
    }

    private ItemData FindPlantableSeed(Inventory inventory)
    {
        return (from item2 in m_conversion
            let item = inventory.GetItem(item2.m_from.m_itemData.m_shared.m_name, -1, false)
            where item != null
            where item.m_stack >= item2.m_seedAmount
            select item).FirstOrDefault();
    }

    private ItemConversion GetItemConversion(string itemName)
    {
        return m_conversion.FirstOrDefault((ItemConversion item) => item.m_from.gameObject.name == itemName);
    }

    private static string TimeLeft(Germination germination)
    {
        int secondsLeft = (int)(germination.m_germinationDuration - germination.GetGerminationTime());
        return $"({secondsLeft / 60:00}:{secondsLeft % 60:00})";
    }

    public float GetHoverOffset()
    {
        return m_hoverOffset;
    }

}
