using ParaverseWebsite.Models;
using System;

[Serializable]
public class RoundsReachedAchievement
{
    public int CompletedCount = 0;
    public int Index = 0;   // To fetch the current active achievement
    public int[] TargetCounts = new int[6];
    public bool[] CompletionStatuses = new bool[6];


    public RoundsReachedAchievement(int currValue)
    {
        CompletedCount = currValue;
        Index = 0;

        InitAchievementValues();
        AchievementProgressHandler();
    }

    public RoundsReachedAchievement(AchievementsModel oldModel, SessionDataModel sessionData)
    {
        int RoundNumberCompleted = sessionData.RoundNumberReached - 1;
        CompletedCount = oldModel.RoundsReachedAchievement.CompletedCount + RoundNumberCompleted;
        Index = 0;

        InitAchievementValues();
        AchievementProgressHandler();
    }

    private void InitAchievementValues()
    {
        TargetCounts[0] = 10;
        TargetCounts[1] = 100;
        TargetCounts[2] = 250;
        TargetCounts[3] = 500;
        TargetCounts[4] = 750;
        TargetCounts[5] = 1000;

        CompletionStatuses[0] = false;
        CompletionStatuses[1] = false;
        CompletionStatuses[2] = false;
        CompletionStatuses[3] = false;
        CompletionStatuses[4] = false;
        CompletionStatuses[5] = false;
    }

    public void AchievementProgressHandler()
    {
        for (int i = 0; i < TargetCounts.Length - 1; i++)
        {
            // Check if achievement is completed!
            if (CompletedCount >= TargetCounts[i] && CompletionStatuses[i] == false)
            {
                Index++;
                CompletionStatuses[i] = true;
            }
        }
    }
}
