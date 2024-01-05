using Paraverse.Combat;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;
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
    if (SkillState.Equals(SkillState.InUse))
    {
      if (skillCurTimer > 0)
        skillCurTimer -= Time.deltaTime;
      else
        OnSkillComplete();
    }
  }

  protected override void ExecuteSkillLogic()
  {
    base.ExecuteSkillLogic();
    skillCurTimer = skillStartTimer;
    projData.projOrigin.position = target.transform.position + new Vector3(0f, 10f, 0f);
  }

  protected override void OnSkillComplete()
  {
    base.OnSkillComplete();
    _curCooldown = _cooldown;
  }
  #endregion
}
