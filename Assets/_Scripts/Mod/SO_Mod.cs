using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ModType
{
    StatMod, SkillMod, SkillUpgradeMod, EffectMod
}

[CreateAssetMenu(fileName = "ModName", menuName = "SOs/Mods/SO_Mod")]
public class SO_Mod : ScriptableObject
{
    [Header("——————  BASICS  —————")]
    [Space(10)]
    public string Title;
    public int ID;
    public ModType Type;
    public Sprite Image;
    public int Cost;
    [TextArea(1, 4)]
    public string Description;
    public SO_Mod[] PrerequisiteMods; // <mod ID, how many of that mod needed>

    /// <summary>
    /// takes in a user's currency, and existing mods in numerable, and returns
    /// whether this mod is purchaseable
    /// Also, this is where conditions such as prerequisite mods are checked
    /// </summary>
    public virtual bool CanPurchase(int userCurrencyAmount, IEnumerable<SO_Mod> userCurrentMods)
    {
        if (userCurrencyAmount < Cost) return false;

        if (null != PrerequisiteMods && PrerequisiteMods.Length > 0)
        {
            foreach (SO_Mod _modRequired in PrerequisiteMods)
            {
                if (!userCurrentMods.Contains(_modRequired))
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Logic for activating this mod to the user
    /// </summary>
    public virtual void Activate(System.Action callback = null)
    {

    }


}
