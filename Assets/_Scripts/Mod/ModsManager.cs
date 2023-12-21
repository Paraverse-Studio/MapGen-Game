using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModsManager : MonoBehaviour
{
    public static ModsManager Instance;

    [Header("Non-Mods")]
    public SO_Consumable GoldItem;

    [Header("Purchased/Obtained Mods")]
    public List<SO_Item> PurchasedMods;

    [Header("Available Mods")]
    public List<SO_Item> AvailableMods;

    private Object[] loadedObjects;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        loadedObjects = Resources.LoadAll("ITEMS", typeof(SO_Item));

        ResetMods();
    }

    public void ResetMods()
    {
        AvailableMods.Clear();
        PurchasedMods.Clear();

        // Reset all the mods to their default state (so that Attack or Ability aren't at mod lv 5 for example)
        foreach (Object obj in loadedObjects)
        {
            AvailableMods.Add((SO_Item)obj);
            AvailableMods[AvailableMods.Count - 1].Reset();
        }
    }

    public int GetMod(ModType type, out SO_Mod mod, List<SO_Mod> avoidMods = null)
    {
        mod = null;
        int index = -1;

        List<SO_Item> filtered = new();
        int random = -1;

        if (type == ModType.Stats) 
        {
            filtered = AvailableMods.Where(mod => null != mod && (mod is SO_StatMod statMod) && !avoidMods.Contains(mod) && statMod.ultraTier == false).ToList();            
        } 
        else if (type == ModType.Skill) 
        { 
            filtered = AvailableMods.Where(mod => null != mod && (mod is SO_SkillMod skillMod) && !avoidMods.Contains(mod)).ToList();            
        }
        else if (type == ModType.Effects) 
        { 
            filtered = AvailableMods.Where(mod => null != mod && (mod is SO_EffectMod effectMod) && !avoidMods.Contains(mod)).ToList();            
        }

        if (filtered.Count > 0) // if it's 0, then no mod was found, list is empty of any available mod
        {
            random = Random.Range(0, filtered.Count);
            mod = (SO_Mod)filtered[random];
            index = AvailableMods.IndexOf(filtered[random]);
        }

        return index;
    } 

    public void PurchaseMod(SO_Item item)
    {
        if (item is not SO_Mod) return;

        int indexOfMod = AvailableMods.IndexOf(item);

        if (!PurchasedMods.Contains(item) && item is not SO_StatMod) // stat mods get re-added
            PurchasedMods.Add(item);

        // if a mod (stat mod), then keep it in that in the spot,
        // otherwise, remove this entry in available mods
        if (((SO_Mod)item).Type != ModType.Stats)
            AvailableMods.RemoveAt(indexOfMod);
    }

}
