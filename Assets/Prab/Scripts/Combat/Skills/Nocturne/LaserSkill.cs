using Paraverse.Combat;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;
using PolygonArsenal;
using UnityEngine;

public class LaserSkill : MobSkill, IMobSkill
{
    [SerializeField]
    protected GameObject chargeFX;
    [SerializeField]
    protected Transform chargeOrigin;
    [SerializeField]
    protected float skillStartTimer = 3f;
    protected float skillCurTimer = 3f;

    #region Inherited Methods
    public override void ActivateSkill(MobCombat mob, Animator anim, MobStats stats, Transform target = null)
    {
        base.ActivateSkill(mob, anim, stats, target);
        skillCurTimer = skillStartTimer;
    }

    public override void SubscribeAnimationEventListeners()
    {
        base.SubscribeAnimationEventListeners();

        mob.OnInstantiateFXOneEvent += InstantiateChargeFX;
        mob.OnInstantiateFXTwoEvent += InstantiateBeamFX;
    }

    public override void UnsubscribeAnimationEventListeners()
    {
        base.UnsubscribeAnimationEventListeners();

        mob.OnInstantiateFXOneEvent -= InstantiateChargeFX;
        mob.OnInstantiateFXTwoEvent -= InstantiateBeamFX;
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
        mob.IsSkilling = true;
        skillOn = true;
        anim.SetBool(StringData.IsUsingSkill, true);
        skillCurTimer = skillStartTimer;
        stats.UpdateCurrentEnergy(-cost);
        anim.Play(animName);
    }

    protected override void DisableSkill()
    {
        base.DisableSkill();
        curCooldown = cooldown;
        UnsubscribeAnimationEventListeners();
    }

    public void InstantiateChargeFX()
    {
        Instantiate(chargeFX, chargeOrigin);
    }

    public virtual void InstantiateBeamFX()
    {
        GameObject go = Instantiate(projData.projPf, projData.projOrigin);
        BeamProjectile beam = go.GetComponent<BeamProjectile>();
        beam.SpawnBeam();
    }
    #endregion
}
