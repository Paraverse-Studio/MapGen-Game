using Paraverse.Mob.Stats;
using Paraverse.Player;
using UnityEngine;

public interface IMobSkill
{
    public bool TargetWithinRange { get; }
    public bool IsOffCooldown { get; }
    public bool HasEnergy { get; }
    public virtual void ActivateSkill(Transform mob, PlayerInputControls input, Animator anim, IMobStats stats, Transform target = null)
    {
    }

    public virtual void DeactivateSkill(PlayerInputControls input)
    {
    }

    /// <summary>
    /// Contains all methods required to run in Update within MobCombat script.
    /// </summary>
    public virtual void SkillUpdate()
    {
    }

    /// <summary>
    /// Handles skill cooldown.
    /// </summary>
    protected virtual void CooldownHandler()
    {
    }

    /// <summary>
    /// Responsible for executing skill on button press.
    /// </summary>
    public virtual void Execute()
    {

    }

    /// <summary>
    /// Returns true if skill conditions are met. 
    /// </summary>
    /// <returns></returns>
    protected virtual bool CanUseSkill()
    {
        return true;
    }
}
