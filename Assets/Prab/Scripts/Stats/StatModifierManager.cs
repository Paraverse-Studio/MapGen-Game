using Paraverse.Stats;
using System.Collections.Generic;
using UnityEngine;

public class StatModifierManager : MonoBehaviour
{
    public List<StatModifier> statModifiers = new List<StatModifier>();


    private void Update()
    {

    }

    public void AddTempStatMod(StatModifier tempMod)
    {
        if (statModifiers.Count > 0)
            StartCoroutine(StringData.TempStatModifierHandler);
        else
            statModifiers.Add(tempMod);
            
    }


}
