using Paraverse.Combat;
using Paraverse.Mob.Combat;
using Paraverse.Mob.Stats;
using UnityEngine;

public class LaserSkill : MobSkill, IMobSkill
{
    [SerializeField]
    protected float laserRadius = 1.5f;
    [SerializeField]
    protected float laserLength = 100f; 
    [SerializeField]
    protected float laserWidth = 1f;                     
    [SerializeField]
    protected LayerMask targetLayer;                     
    [SerializeField]
    private bool isSticky = false;
    [SerializeField]
    protected GameObject chargeFX;
    [SerializeField]
    protected Transform chargeOrigin;
    [SerializeField]
    protected float skillStartTimer = 3f;
    protected float skillCurTimer = 3f;

    [SerializeField, Tooltip("Manually sets the projectile origin on to target. [Use offset variable to adjust offset from origin.]")]
    protected bool adjustProjOrigin = false;
    [SerializeField]
    protected Vector3 projOriginOffset = Vector3.zero;

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
        if (chargeFX != null)
            Instantiate(chargeFX, chargeOrigin);
    }

    public virtual void InstantiateBeamFX()
    {
        if (adjustProjOrigin)
        {
            projData.projOrigin.position = target.position + projOriginOffset;
        }

        GameObject go = Instantiate(projData.projPf, projData.projOrigin);
        BeamProjectile beam = go.GetComponentInChildren<BeamProjectile>();
        beam.Init(mob, target.position, scalingStatData, beam.gameObject, laserRadius, laserLength, laserWidth, targetLayer, isSticky);
        beam.SpawnBeam();
    }
    #endregion
}
