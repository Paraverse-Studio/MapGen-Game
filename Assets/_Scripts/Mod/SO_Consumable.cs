using Paraverse.Mob.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConsumableType
{
    Gold, Heal, Revive, RedPot, Essence
}

[CreateAssetMenu(fileName = "ItemName", menuName = "SOs/Items/Consumable")]
public class SO_Consumable : SO_Item
{
    [Header("Consumable Type:")]
    public ConsumableType Type;
    public float strength;

    private MobStats _player;

    public override void Activate(GameObject go, int level = -1)
    {
        base.Activate();

        // if the provided object can't be parsed into a player, something is wrong
        // activate() should be called from shop, and supplied with the player to act upon
        if (!go.TryGetComponent(out _player))
        {
            Debug.LogError("Consumable Item: Activate() called with a non-player parameter.");
            return;
        }

        // Add this skill to the player's list of skills, and also activate this one
        switch (Type)
        {
            case ConsumableType.Gold:
                _player.UpdateGold(Quantity);
                break;
            case ConsumableType.Heal:
                _player.UpdateCurrentHealth((int)(_player.MaxHealth.FinalValue * strength));
                break;
            case ConsumableType.Revive:
                break;
            case ConsumableType.RedPot:
                break;
            case ConsumableType.Essence:
                break;
        }

        Debug.Log($"Consumable Item: Item \"{Title}\" (ID {ID}) activated for {_player.gameObject.name}!");
    }
}
