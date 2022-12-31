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
    public class StatPair
    {
        public StatType type;
        public float value;

        public StatPair(StatType t, float f)
        {
            this.type = t;
            this.value = f;
        }
    }

    [Header("Stats Change")]
    [SerializeField]
    public List<StatPair> addStats; // reference for original values

    private List<StatPair> _addStatsMutable = null; // mutable
    private int _CostMutable; // mutable version

    private MobStats _player;


    private void OnValidate()
    {
        Description = "<Stat Mods are autofilled at run-time>";
    }

    public override int GetCost()
    {
        return _CostMutable;
    }

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

        if (null == _addStatsMutable) _addStatsMutable = new (addStats);

        // Add the stats to the player provided
        foreach (StatPair statPair in _addStatsMutable)
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

    public override SO_Mod Consume()
    {
        if (evolve.canStack)
        {
            for (int i = 0; i < _addStatsMutable.Count; ++i)
            {
                _addStatsMutable[i] = new StatPair(_addStatsMutable[i].type, _addStatsMutable[i].value * evolve.valueGrowthFactor);
            }
            _CostMutable = (int)(GetCost() * evolve.costGrowthFactor);

            AutofillDescription();
            return this;
        }
        else
        {
            return base.Consume();
        }
    }

    public override void AutofillDescription()
    {
        base.AutofillDescription();
        string message = string.Empty;

        foreach (StatPair p in _addStatsMutable)
        {
            message += ((p.value > 0)? "Gain " : "Lose ") + p.value + " ";
            switch (p.type)
            {
                case StatType.Attack:
                    message += "Attack";
                    break;
                case StatType.Ability:
                    message += "Ability";
                    break;
                case StatType.Health:
                    message += "Health";
                    break;
                case StatType.Energy:
                    message += "Energy";
                    break;
                case StatType.AttackSpeed:
                    message += "Attack Speed";
                    break;
                case StatType.MoveSpeed:
                    message += "Movement Speed";
                    break;
            }

            message += ". ";
            Description = message;
        }
    }

    public override void Reset()
    {
        base.Reset();
        _CostMutable = Cost;
        _addStatsMutable = new (addStats);
        AutofillDescription();
    }

}
