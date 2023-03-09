using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModsManager : MonoBehaviour
{
    public static ModsManager Instance;   

    [Header("Purchased/Obtained Mods")]
    public List<SO_Item> PurchasedMods;

    [Header("Available Mods")]
    public List<SO_Item> AvailableMods;

    private void Awake()
    {
        Instance = this;
    }

    public int GetMod(ModType type, out SO_Mod mod)
    {
        mod = null;
        int index = -1;

        List<SO_Item> filtered = new();
        int random;

        if (type == ModType.Stats) 
        {
            filtered = AvailableMods.Where(mod => null != mod && (mod is SO_Mod) && ((SO_Mod)mod).Type == type).ToList();            
        } 
        else if (type == ModType.Skill) 
        { 
            filtered = AvailableMods.Where(mod => null != mod && (mod is SO_Mod) && ((SO_Mod)mod).Type == type).ToList();            
        }
        else if (type == ModType.Effects) 
        { 
            filtered = AvailableMods.Where(mod => null != mod && (mod is SO_Mod) && ((SO_Mod)mod).Type == type).ToList();            
        }

        random = Random.Range(0, filtered.Count - 1);
        mod = (SO_Mod)filtered[random];
        index = AvailableMods.IndexOf(filtered[random]);

        return index;
    }

}
