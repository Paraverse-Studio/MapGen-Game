using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ModType
{
    Stats, Skill, Effects
}


//[CreateAssetMenu(fileName = "ModName", menuName = "SOs/Mods/SO_Mod")]
public class SO_Mod : ScriptableObject
{
    [System.Serializable]
    public struct Evolving
    {
        public bool canStack;
        public float valueGrowthFactor;
        public float costGrowthFactor;
    }

    [Header("——————  BASICS  —————")]
    [Space(10)]
    [SerializeField]
    protected string Title;
    public int ID;
    public ModType Type;
    public Sprite Image;

    [SerializeField]
    protected int Cost;
    [TextArea(1,2)]
    public string Lore;
    [SerializeField, TextArea(3, 4)]
    protected string Description;
    [Header("——————  DETAILS  —————")]
    [SerializeField]
    private bool _removeOnPurchase;

    [Header("——————  SPECIAL  —————")]
    [Tooltip("This mod is only available if all of prereqMods are purchased.")]
    public SO_Mod[] PrerequisiteMods;
    [Tooltip("This mod is removed if any of blockedByMods are purchased.")]
    public SO_Mod[] BlockedByMods;
    public Evolving evolve;

    /// <summary>
    /// in rare cases, this function could be needed to change
    /// for sub classes
    /// </summary>
    public virtual string GetTitle()
    {
        return Title;
    }

    /// <summary>
    /// in rare cases, this function could be needed to change
    /// for sub classes
    /// </summary>
    public virtual string GetDescription()
    {
        return Description;
    }

    /// <summary>
    /// in rare cases, this function could be needed to change
    /// for sub classes
    /// </summary>
    public virtual int GetCost()
    {
        return Cost;
    }

    /// <summary>
    /// takes in a user's currency, and existing mods in numerable, and returns
    /// whether this mod is purchaseable
    /// Also, this is where conditions such as prerequisite mods are checked
    /// </summary>
    public virtual bool CanPurchase(int userCurrencyAmount, IEnumerable<SO_Mod> userCurrentMods)
    {
        if (userCurrencyAmount < GetCost()) return false;

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
        if (null != BlockedByMods && BlockedByMods.Length > 0)
        {
            foreach (SO_Mod _blockingMod in BlockedByMods)
            {
                if (userCurrentMods.Contains(_blockingMod))
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
    public virtual void Activate(GameObject providedObject = null)
    {
        // implement in sub classes
    }

    /// <summary>
    /// Logic for after activated, how does this mod get consumed (used up)
    /// </summary>
    public virtual SO_Mod Consume()
    {
        if (_removeOnPurchase) return null;
        return this;
        // implement in sub classes
    }

    /// <summary>
    /// Logic for auto-filling description when applicable
    /// </summary>
    public virtual void AutofillDescription()
    {
        // implement in sub classes
    }

    /// <summary>
    /// Logic for special cases where SO data needs to be reset after un-playing
    /// so as to not retain changes made during gameplay
    /// </summary>
    public virtual void Reset()
    {
        // implement in sub classes (primarily only needed in stat mod)
    }


}
