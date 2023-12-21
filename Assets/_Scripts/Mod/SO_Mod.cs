using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ModType
{
    Stat, Skill, Effect
}


//[CreateAssetMenu(fileName = "ModName", menuName = "SOs/Mods/SO_Mod")]
public class SO_Mod : SO_Item
{
    [System.Serializable]
    public struct Evolving
    {
        public bool canStack;
        public float valueGrowthFactor;
        public float costGrowthFactor;
    }

    // This is here because items themselves won't be evolvable, just specific mods
    [Header("——————  SPECIAL  —————")]
    public ModType Type;
    public Evolving evolve;

}
