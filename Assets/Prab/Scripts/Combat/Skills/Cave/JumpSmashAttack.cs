using Paraverse.Combat;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Controller;
using Paraverse.Mob.Stats;
using Paraverse.Player;
using UnityEngine;

public class JumpSmashAttack : MobSkill, IMobSkill
{
    #region Variables
    protected MobController controller;
    [SerializeField]
    protected string landAnimName = "LandAnim";
    [SerializeField]
    protected int layerIdx = 1;
    [SerializeField]
    protected float layerWeight = 1;
    #endregion


    #region Inherited Methods
    public override void ActivateSkill(MobCombat mob, Animator anim, MobStats stats, Transform target = null)
    {
        base.ActivateSkill(mob, anim, stats, target);
        if (controller == null) controller = mob.GetComponent<MobController>();
        SubscribeAnimationEventListeners();
    }

    public override void DeactivateSkill(PlayerInputControls input)
    {
        base.DeactivateSkill(input);
        UnsubscribeAnimationEventListeners();
    }

    public override void SubscribeAnimationEventListeners()
    {
        controller.OnLandEvent += OnLand;
        mob.OnEnableSkillColliderSOneEvent += EnableSmashAttackCollider;
        mob.OnDisableSkillOneEvent += DisableSkill;
    }

    public override void UnsubscribeAnimationEventListeners()
    {
        controller.OnLandEvent -= OnLand;
        mob.OnDisableSkillColliderSOneEvent += DisableSmashAttackCollider;
        mob.OnDisableSkillOneEvent -= DisableSkill;
    }

    protected override void ExecuteSkillLogic()
    {
        mob.IsSkilling = true;
        skillOn = true;
        anim.SetBool(StringData.IsUsingSkill, true);
        curCooldown = cooldown;
        stats.UpdateCurrentEnergy(-cost);
        anim.Play(animName);
        controller.ApplyJump(target.transform.position);
    }

    protected override void DisableSkill()
    {
        base.DisableSkill();
    }
    #endregion

    #region Private Methods
    private void OnLand()
    {
        anim.Play(landAnimName);
        anim.SetLayerWeight(layerIdx, layerWeight);
        anim.SetBool(StringData.IsGrounded, true);
    }

    private void EnableSmashAttackCollider()
    {
        attackColliderGO.SetActive(true);
    }

    private void DisableSmashAttackCollider()
    {
        attackColliderGO.SetActive(false);  
    }
    #endregion
}
