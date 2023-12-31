using Paraverse.Combat;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;
using Paraverse.Player;
using UnityEngine;

public class SpinSkill : MobSkill, IMobSkill
{
    #region Variables
    PlayerController controller;
    [SerializeField]
    protected float skillStartTimer = 3f;
    protected float skillCurTimer = 3f;
    [SerializeField]
    private float skillMoveSpeed = 3f;

    #endregion


    #region Inherited Methods
    public override void ActivateSkill(PlayerCombat mob, PlayerInputControls input, Animator anim, MobStats stats, Transform target = null)
    {
        base.ActivateSkill(mob, input, anim, stats, target);
        if (null == controller) controller = mob.GetComponent<PlayerController>();
    }

    /// <summary>
    /// Contains all methods required to run in Update within MobCombat script.
    /// </summary>
    public override void SkillUpdate()
    {
        base.SkillUpdate();
        SkillHander();
    }

    protected override void ExecuteSkillLogic()
    {
        _curCooldown = _cooldown;
        stats.UpdateCurrentEnergy(-cost);
        anim.Play(animName);
        EnableColldier();
    }

    protected void SkillHander()
    {
        if (skillOn)
        {
            if (skillCurTimer > 0)
            {
                skillCurTimer -= Time.deltaTime;
                controller.ControlMovement(skillMoveSpeed);
            }
            else
            {
                DisableColldier();
            }
        }
        else
        {
            DisableColldier();
        }
    }
    #endregion

    #region Animation Events
    protected void EnableColldier()
    {
        attackColliderGO.SetActive(true);
        skillCurTimer = skillStartTimer;
        skillOn = true;
    }

    protected void DisableColldier()
    {
        attackColliderGO.SetActive(false);
        skillCurTimer = skillStartTimer;
        skillOn = false;
    }
    #endregion
}
