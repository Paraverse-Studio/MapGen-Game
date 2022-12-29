using Paraverse.Stats;
using UnityEngine;

public class SkillOneEffect : Effect
{
    [SerializeField]
    private float healAmount = 5f;



    public SkillOneEffect(string name, string description) : base(name, description)
    {
    }

    public override void Execute()
    {
        
        Debug.Log("Execute Skill One Effect");
    }
}
