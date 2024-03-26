using Paraverse.Combat;
using ParaverseWebsite.Models;
using System;

[Serializable]
public class MobsKilledAchievement
{
    public int TotalMobsKilledCount = 0;
    public int Index = 0;   // To fetch the current active achievement
    public int[] UpdatedTotalMobsKilledTargetCount = new int[6];
    public bool[] UpdatedTotalMobsKilledCompletionStatus = new bool[6];


    public MobsKilledAchievement(int mobKills)
    {
        TotalMobsKilledCount = mobKills;
        Index = 0;

        UpdatedTotalMobsKilledTargetCount[0] = 10;
        UpdatedTotalMobsKilledTargetCount[1] = 100;
        UpdatedTotalMobsKilledTargetCount[2] = 250;
        UpdatedTotalMobsKilledTargetCount[3] = 500;
        UpdatedTotalMobsKilledTargetCount[4] = 750;
        UpdatedTotalMobsKilledTargetCount[5] = 1000;

        for (int i = 0; i < UpdatedTotalMobsKilledTargetCount.Length-1; i++)
        {
            UpdatedTotalMobsKilledCompletionStatus[i] = false;
        }
        TotalMobsKilledProgressHandler();
    }

    public MobsKilledAchievement(AchievementsModel oldModel, SessionDataModel sessionData)
    {
        TotalMobsKilledCount = oldModel.MobsKilledAchievement.TotalMobsKilledCount + sessionData.MobsDefeatedCount;
        Index = 0;

        UpdatedTotalMobsKilledTargetCount[0] = 10;
        UpdatedTotalMobsKilledTargetCount[1] = 100;
        UpdatedTotalMobsKilledTargetCount[2] = 250;
        UpdatedTotalMobsKilledTargetCount[3] = 500;
        UpdatedTotalMobsKilledTargetCount[4] = 750;
        UpdatedTotalMobsKilledTargetCount[5] = 1000;

        TotalMobsKilledProgressHandler();
    }

    public void TotalMobsKilledProgressHandler()
    {
        for (int i = 0; i < UpdatedTotalMobsKilledTargetCount.Length-1; i++)
        {
            // Check if achievement is completed!
            if (TotalMobsKilledCount >= UpdatedTotalMobsKilledTargetCount[i] && UpdatedTotalMobsKilledCompletionStatus[i] == false)
            {
                Index++;
                UpdatedTotalMobsKilledCompletionStatus[i] = true;
            }
        }
    }
}
