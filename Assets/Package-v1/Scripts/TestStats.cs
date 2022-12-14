using Paraverse.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStats : MonoBehaviour
{
    public Stat MaxHealth;
    public Stat Attack;
    public Stat Armor;


    private void Start()
    {
        MaxHealth = new Stat(100f);
        Attack = new Stat(10f);
        Armor = new Stat(5f);
        Debug.Log("Player has " + MaxHealth.BaseValue + " points in max health, " + Attack.BaseValue + " points in attack damage, " + Armor.BaseValue + " points in armor.");
    }

    public void Update()
    {
        MaxHealth.TempStatModifierHandler();
        Attack.TempStatModifierHandler();
        Armor.TempStatModifierHandler();

        Debug.Log("Final Value for MaxHealth: " + MaxHealth.FinalValue + ".");
        Debug.Log("Final Value for Attack: " + Attack.FinalValue + ".");
        Debug.Log("Final Value for Armor: " + Armor.FinalValue + ".");

        Debug.Log("StatModValue for MaxHealth: " + MaxHealth.StatModValue + ".");
        Debug.Log("StatModValue for Attack: " + Attack.StatModValue + ".");
        Debug.Log("StatModValue for Armor: " + Armor.StatModValue + ".");

        Debug.Log("TempStatModValue for MaxHealth: " + MaxHealth.TempStatModValue + ".");
        Debug.Log("TempStatModValue for Attack: " + Attack.TempStatModValue + ".");
        Debug.Log("TempStatModValue Value for Armor: " + Armor.TempStatModValue + ".");


        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            MaxHealth.AddMod(new StatModifier(5f));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            Attack.AddMod(new StatModifier(2.5f));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Armor.AddMod(new StatModifier(1.5f));
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            MaxHealth.ClearAllMods();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Attack.ClearAllMods();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Armor.ClearAllMods();
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            MaxHealth.AddTempMod(new TempStatModifier(12f, 5f));
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            Attack.AddTempMod(new TempStatModifier(7f, 3f));
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            Armor.AddTempMod(new TempStatModifier(3f, 2f));
        }
    }
}
