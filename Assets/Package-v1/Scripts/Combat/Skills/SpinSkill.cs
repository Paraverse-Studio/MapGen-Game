using Paraverse.Combat;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;
using Paraverse.Player;
using UnityEngine;

public class SpinSkill : MobSkill, IMobSkill
{
    PlayerController controller;
    [SerializeField]
    protected float skillStartTimer = 3f;
    protected float skillCurTimer = 3f;
    [SerializeField]
    private float skillMoveSpeed = 3f;

    #region Public Methods
    public override void ActivateSkill(MobCombat mob, PlayerInputControls input, Animator anim, IMobStats stats, Transform target = null)
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

    /// <summary>
    /// Responsible for executing skill on button press.
    /// </summary>
    public override void Execute()
    {
        if (CanUseSkill())
        {
            curCooldown = cooldown;
            stats.UpdateCurrentEnergy(-cost);
            anim.Play(animName);
            EnableColldier();
            Debug.Log("Executing skill: " + _skillName + " which takes " + cost + " points of energy out of " + stats.CurEnergy + " point of current energy." +
                "The max cooldown for this skill is " + cooldown + " and the animation name is " + animName + ".");
        }
    }
    #endregion

    #region Private Methods
    protected void SkillHander()
    {
        if (_skillOn)
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

    protected void EnableColldier()
    {
        attackColliderGO.SetActive(true);
        skillCurTimer = skillStartTimer;
        controller.IsInvulnerable = true;
        _skillOn = true;
    }

    protected void DisableColldier()
    {
        attackColliderGO.SetActive(false);
        skillCurTimer = skillStartTimer;
        controller.IsInvulnerable = false;
        _skillOn = false;
    }
    #endregion
}
