using Paraverse.Combat;
using UnityEngine;


public class RootSkill : MobSkill, IMobSkill
{
  #region Variables
  [SerializeField]
  [Header("Root Skill")]
  private float rootDuration = 3f;
  private float curRootDuration;
  #endregion


  #region Inherited Methods
  public override void SkillUpdate()
  {
    base.SkillUpdate();
    RootHandler();
  }

  protected override void ExecuteSkillLogic()
  {
    SetSkillState(SkillState.InUse);
    anim.SetBool(StringData.IsUsingSkill, true);
    curRootDuration = rootDuration;
    stats.UpdateCurrentEnergy(-cost);
    anim.Play(animName);
  }
  #endregion

  #region Private Methods
  private void RootHandler()
  {
    if (SkillState.Equals(SkillState.InUse))
    {
      if (curRootDuration <= 0)
      {
        OnSkillComplete();
        _curCooldown = _cooldown;
      }
      else
      {
        curRootDuration -= Time.deltaTime;
      }
    }
  }
  #endregion
}
