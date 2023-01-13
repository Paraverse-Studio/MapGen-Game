using Paraverse.Combat;
using Paraverse.Mob.Controller;
using Paraverse.Mob.Stats;
using Paraverse.Player;
using UnityEngine;

public class SummonSkill : MobSkill, IMobSkill
{
    #region Variables
    [SerializeField]
    private GameObject summonPf;
    [SerializeField]
    private int maxSummonCount = 3;
    [SerializeField]
    private int curSummonCount;
    #endregion


    #region Inherited Methods
    public override void ActivateSkill(EnhancedMobCombat mob, Animator anim, IMobStats stats, Transform target = null)
    {
        base.ActivateSkill(mob, anim, stats, target);
        mob.OnSummonSkillEvent += SummonSapling;
    }

    public override void DeactivateSkill(PlayerInputControls input)
    {
        base.DeactivateSkill(input);
        mob.OnSummonSkillEvent -= SummonSapling;
    }

    protected override bool CanUseSkill()
    {
        if (IsOffCooldown && HasEnergy && TargetWithinRange && CanSpawn())
        {
            return true;
        }

        Debug.Log(_skillName + " is on cooldown or don't have enough energy!");
        return false;
    }

    protected override bool IsInRange()
    {
        return true;
    }
    #endregion

    #region Private Methods
    private void DecrementSummonCount(Transform target)
    {
        if (curSummonCount <= 0)
        {
            Debug.LogError("curSummonCount is already at 0...");
            return;
        }

        --curSummonCount;
    }

    protected bool CanSpawn()
    {
        return curSummonCount < maxSummonCount;
    }
    #endregion

    #region Animation Events
    public void SummonSapling()
    {
        if (summonPf == null)
            Debug.LogError("Please add a summon prefab to the skill: " + _skillName);

        // Get random position around mob
        float posX = Random.Range(1, 2);
        float posY = Random.Range(1, 2);
        float posZ = Random.Range(1, 2);
        Vector3 spawnPos = mob.transform.position + new Vector3(posX, posY, posZ);
        ++curSummonCount;

        GameObject go = Instantiate(summonPf, spawnPos, transform.rotation);
        MobController summonedMob = go.GetComponentInChildren<MobController>();
        summonedMob.OnDeathEvent += DecrementSummonCount;
        skillOn = false;
    }
    #endregion
}
