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

    #region Inherited Methods
    public override void ActivateSkill(MobCombat mob, Animator anim, MobStats stats, Transform target = null)
    {
        base.ActivateSkill(mob, anim, stats, target);
        skillCurTimer = skillStartTimer;
    }

    public override void SkillUpdate()
    {
        base.SkillUpdate();
        SkillHander();
    }

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

    protected override void ExecuteSkillLogic()
    {
        mob.IsUsingSkilling = true;
        skillOn = true;
        anim.SetBool(StringData.IsUsingSkill, true);
        skillCurTimer = skillStartTimer;
        stats.UpdateCurrentEnergy(-cost);
        anim.Play(animName);
        projData.projOrigin.position = target.transform.position + new Vector3(0f, 10f, 0f);
    }

    protected override void DisableSkill()
    {
        base.DisableSkill();
        curCooldown = cooldown;
    }
    #endregion
}
