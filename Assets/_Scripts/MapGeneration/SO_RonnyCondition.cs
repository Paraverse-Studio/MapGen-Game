using UnityEngine;

[CreateAssetMenu(fileName = "SO_Condition", menuName = "SOs/Conditions/RonnyCondition")]
public class SO_RonnyCondition : SO_Condition
{
    public override bool Evaluate()
    {
        return MapCreator.Instance.mapType == MapType.reward && GlobalSettings.Instance.playerCombat.ActiveSkill == null;
    }
}