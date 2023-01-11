using Paraverse.Combat;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;
using Paraverse.Player;
using UnityEngine;

public class SkyBarrageSkill : MobSkill, IMobSkill
{
    [SerializeField]
    protected float skillStartTimer = 3f;
    protected float skillCurTimer = 3f;

    #region Public Methods
    public override void ActivateSkill(MobCombat mob, PlayerInputControls input, Animator anim, IMobStats stats, Transform target = null)
    {
        base.ActivateSkill(mob, input, anim, stats, target);
    }

    /// <summary>
    /// Contains all methods required to run in Update within MobCombat script.
    /// </summary>
    public override void SkillUpdate()
    {
        base.SkillUpdate();
        SkillHander();
    }

    /// <summary>
    /// Responsible for executing skill on button press.
    /// </summary>
    public override void Execute()
    {
        if (CanUseSkill())
        {
            mob.IsSkilling = true;
            MarkSkillAsEnabled();
            curCooldown = cooldown;
            skillCurTimer = skillStartTimer;
            stats.UpdateCurrentEnergy(-cost);
            anim.Play(animName);
            projData.projOrigin.position = target.transform.position + new Vector3(0f, 10f, 0f);
            Debug.Log("Executing skill: " + _skillName + " which takes " + cost + " points of energy out of " + stats.CurEnergy + " point of current energy." +
                "The max cooldown for this skill is " + cooldown + " and the animation name is " + animName + ".");
        }
    }
    #endregion

    #region Private Methods
    protected void SkillHander()
    {
        if (skillOn)
        {
            if (skillCurTimer > 0)
            {
                skillCurTimer -= Time.deltaTime;
            }
            else
            {
                DisableSkill();
            }
        }
    }

    protected override void DisableSkill()
    {
        base.DisableSkill();
        skillCurTimer = skillStartTimer;
    }
    #endregion
}
