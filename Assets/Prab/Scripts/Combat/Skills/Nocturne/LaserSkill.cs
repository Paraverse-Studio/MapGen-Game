using Paraverse.Combat;
using Paraverse.Mob.Stats;
using Paraverse.Player;
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
    public override void ActivateSkill(EnhancedMobCombat mob, Animator anim, IMobStats stats, Transform target = null)
    {
        base.ActivateSkill(mob, input, anim, stats, target);
        skillCurTimer = skillStartTimer;

        mob.OnInstantiateFXOneEvent += InstantiateChargeFX;
        mob.OnInstantiateFXTwoEvent += InstantiateBeamFX;
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
        projData.projOrigin.position = target.transform.position + new Vector3(0f, 10f, 0f);
    }

    protected override void DisableSkill()
    {
        base.DisableSkill();
        curCooldown = cooldown;
    }
    
    public void InstantiateChargeFX()
    {
        Instantiate(chargeFX, chargeOrigin.position, chargeOrigin.rotation);
    }

    public virtual void InstantiateBeamFX()
    {
        GameObject go = Instantiate(projData.projPf, projData.projOrigin, projData.projOrigin);
        PolygonBeamStatic beam = go.GetComponent<PolygonBeamStatic>();
        beam.SpawnBeam();
    }
    #endregion
}
