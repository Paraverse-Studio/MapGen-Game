using Paraverse.Mob.Stats;
using Paraverse.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ModName", menuName = "SOs/Mods/Stat Mod")]
public class SO_StatMod : SO_Mod
{
    [System.Serializable]
    public enum StatType
    {
        Attack,
        Ability,
        Health,
        Energy,
        AttackSpeed,
        MoveSpeed
    }

    [System.Serializable]
    public struct StatPair
    {
        public StatType type;
        public float value;
    }

    [Header("Stats Change")]
    [SerializeField]
    public List<StatPair> addStats;

    private MobStats _player;

    public override void Activate(GameObject go)
    {
        base.Activate();

        // if the provided object can't be parsed into a player, something is wrong
        // activate() should be called from shop, and supplied with the player to act upon
        if (!go.TryGetComponent(out _player))
        {
            Debug.LogError("Skill Mod: Activate() called with a non-player parameter.");
            return;
        }

        // Add the stats to the player provided
        foreach (StatPair statPair in addStats)
        {
            // do something with entry.Value or entry.Key
            switch (statPair.type)
            {
                case StatType.Attack:
                    _player.UpdateAttackDamage(statPair.value);
                    break;
                case StatType.Ability:
                    _player.UpdateAbilityPower(statPair.value);
                    break;
                case StatType.Health:
                    _player.UpdateMaxHealth((int)statPair.value);
                    break;
                case StatType.Energy:
                    _player.UpdateMaxEnergy(statPair.value);
                    break;
                case StatType.AttackSpeed:
                    _player.UpdateAttackSpeed(statPair.value);
                    break;
                case StatType.MoveSpeed:
                    _player.UpdateMovementSpeed(statPair.value);
                    break;
            }
        }        

        Debug.Log($"Stat Mod: Mod \"{Title}\" (ID {ID}) activated for {_player.gameObject.name}!");
    }
}
