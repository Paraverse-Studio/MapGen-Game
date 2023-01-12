using Paraverse;
using Paraverse.Combat;
using Paraverse.Helper;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Controller;
using Paraverse.Mob.Stats;
using Paraverse.Player;
using UnityEngine;

public class SlamDownPunch : MobSkill, IMobSkill
{
    #region Variables
    [SerializeField]
    protected GameObject mainHandAttackColliderGO;
    protected AttackCollider mainHandAttackCollider;
    #endregion

    #region Public Methods
    public void ChargePunch()
    {
        GetComponent<MobController>().ApplyFlyUp(mob.transform.position, transform.up);
    }

    public void ChargePunchCancel()
    {
        GetComponent<MobController>().isFlyingUp = false;
    }

    public void UnleashPunch()
    {
        GetComponent<MobController>().ApplyFlyDown(target.transform.position);
    }

    public void EnableOffAttackCollider()
    {
        if (mainHandAttackCollider != null)
            mainHandAttackColliderGO.SetActive(false);
    }

    public void DisableOffAttackCollider()
    {
        if (mainHandAttackCollider != null)
            mainHandAttackColliderGO.SetActive(false);
    }
    #endregion

    #region Private Methods
    private void RotateToTarget()
    {
        if (mob.Target)
        {
            Vector3 targetDir = ParaverseHelper.GetPositionXZ(mob.Target.position - transform.position).normalized;
            transform.forward = targetDir;
        }
    }

    /// <summary>
    /// Run this method everytime a skill is activated
    /// </summary>
    protected void MarkSkillAsEnabled()
    {
        skillOn = true;
        anim.SetBool(StringData.IsUsingSkill, true);
    }

    protected virtual void DisableSkill()
    {
        skillOn = false;
        anim.SetBool(StringData.IsUsingSkill, false);
    }

    /// <summary>
    /// Handles skill cooldown.
    /// </summary>
    protected virtual void CooldownHandler()
    {
        if (curCooldown > 0)
        {
            curCooldown -= Time.deltaTime;
        }
        curCooldown = Mathf.Clamp(curCooldown, 0f, cooldown);
    }

    /// <summary>
    /// Returns true if skill conditions are met. 
    /// </summary>
    /// <returns></returns>
    protected virtual bool CanUseSkill()
    {
        if (IsOffCooldown && HasEnergy && TargetWithinRange && mob.IsAttackLunging == false)
        {
            return true;
        }

        //Debug.Log(_skillName + " is on cooldown or don't have enough energy!");
        return false;
    }

    protected virtual bool IsInRange()
    {
        if (target == null) return true;

        float disFromTarget = ParaverseHelper.GetDistance(mob.transform.position, target.position);

        return disFromTarget >= _minRange && disFromTarget <= _maxRange;
    }
    #endregion
}
