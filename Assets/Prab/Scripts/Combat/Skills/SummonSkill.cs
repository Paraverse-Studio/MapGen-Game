using Paraverse.Combat;
using Paraverse.Mob.Controller;
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


    #region Public Methods

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
    }

    private void DecrementSummonCount(Transform target)
    {
        if (curSummonCount <= 0)
        {
            Debug.LogError("curSummonCount is already at 0...");
            return;
        }

        --curSummonCount;
    }
    #endregion

    /// <summary>
    /// Returns true if skill conditions are met. 
    /// </summary>
    /// <returns></returns>
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

    protected bool CanSpawn()
    {
        return curSummonCount < maxSummonCount;
    }
}
