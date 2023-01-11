using Paraverse;
using Paraverse.Combat;
using Paraverse.Helper;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;
using Paraverse.Player;
using UnityEngine;

public class DualComboSkill : MobSkill, IMobSkill
{

    #region Variables
    [SerializeField]
    protected GameObject offHandAttackColliderGO;
    protected AttackCollider offHandAttackCollider;
    [SerializeField]
    protected float rotSpeed = 100f;
    #endregion

    #region Public Methods
    public override void ActivateSkill(MobCombat mob, Animator anim, IMobStats stats, Transform target = null)
    {
        this.mob = mob;
        this.target = target;
        this.anim = anim;
        this.stats = stats;
        curCooldown = 0f;
        if (mob.tag.Equals(StringData.PlayerTag))
            input.OnSkillOneEvent += Execute;

        if (null == attackCollider && null != attackColliderGO)
        {
            attackCollider = attackColliderGO.GetComponent<AttackCollider>();
            attackCollider.Init(mob, stats);
        }

        // Checks if melee users have basic attack collider script on weapon
        if (attackColliderGO == null)
        {
            Debug.LogWarning(gameObject.name + " needs to have a basic attack collider!");
            return;
        }
        attackColliderGO.SetActive(true);
        attackCollider = attackColliderGO.GetComponent<AttackCollider>();
        attackCollider.Init(mob, stats);
        attackColliderGO.SetActive(false);

        // Checks if melee users have basic attack collider script on weapon
        if (offHandAttackColliderGO == null)
        {
            Debug.LogWarning(gameObject.name + " needs to have a basic attack collider!");
            return;
        }
        offHandAttackColliderGO.SetActive(true);
        offHandAttackCollider = offHandAttackColliderGO.GetComponent<AttackCollider>();
        offHandAttackCollider.Init(mob, stats);
        offHandAttackColliderGO.SetActive(false);
    }

    /// <summary>
    /// Contains all methods required to run in Update within MobCombat script.
    /// </summary>
    public override void SkillUpdate()
    {
        base.SkillUpdate();
        RotateToTarget();
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
            stats.UpdateCurrentEnergy(-cost);
            anim.Play(animName);
            Debug.Log("Executing skill: " + _skillName + " which takes " + cost + " points of energy out of " + stats.CurEnergy + " point of current energy." +
                "The max cooldown for this skill is " + cooldown + " and the animation name is " + animName + ".");

            // depending on the skill, 
            // if it's a projectile, set its damage to:
            // int damage = (flatPower) + (mobStats.AttackDamage.FinalValue * attackScaling) + (mobStats.AbilityPower.FinalValue * abilityScaling);
        }
    }

    private void RotateToTarget()
    {
        transform.rotation = ParaverseHelper.FaceTarget(transform, target, rotSpeed);

        // Ensures mob is looking at the target before attacking
        Vector3 dir = (target.position - transform.position).normalized;
        dir = ParaverseHelper.GetPositionXZ(dir);
        Quaternion lookRot = Quaternion.LookRotation(dir);
        float angle = Quaternion.Angle(transform.rotation, lookRot);
    }

    public void EnableMainAttackCollider()
    {
        if (attackColliderGO != null)
            attackColliderGO.SetActive(true);
    }

    public void DisableMainAttackCollider()
    {

        if (attackColliderGO != null)
            attackColliderGO.SetActive(false);
    }

    public void EnableOffHandAttackCollider()
    {

        if (offHandAttackCollider != null)
            offHandAttackColliderGO.SetActive(true);
    }

    public void DisableOffHandAttackCollider()
    {

        if (offHandAttackCollider != null)
            offHandAttackColliderGO.SetActive(false);
    }

    public void DisableSkillAndCollider()
    {
        if (attackColliderGO != null)
            attackColliderGO.SetActive(false);
        if (offHandAttackCollider != null)
            offHandAttackColliderGO.SetActive(false);

        skillOn = false;
    }
    #endregion
}