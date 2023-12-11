using UnityEngine;

[CreateAssetMenu(fileName = "SO_Condition", menuName = "SOs/Conditions/RandomChanceCondition")]
public class SO_RandomChanceCondition : SO_Condition
{
    [Header("% chance between 0 - 100")]
    public float chance;
    public override bool Evaluate()
    {
        return Random.Range(0.0f, 100.0f) <= chance;
    }
}
