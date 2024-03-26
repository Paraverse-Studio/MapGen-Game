using System;

namespace ParaverseWebsite.Models
{
    [Serializable]
    public class AchievementsModel
    {
        public string Username;
        // ROUNDS REACHED
        //public int TotalRoundsReachedCount = 0;
        //public bool TotalRoundReachedTen = false;
        //public bool TotalRoundReachedTwentyFive = false;
        //public bool TotalRoundReachedFifty = false;
        //public bool TotalRoundReachedOneHundred = false;
        //public bool TotalRoundReachedFiveHundred = false;
        // MOBS KILLED
        public MobsKilledAchievement MobsKilledAchievement;
        // BOSSES KILLED
        //public int TotalBossesKilledCount = 0;
        //public bool TotalBossesKilledFive = false;
        //public bool TotalBossesKilledTen = false;
        //public bool TotalBossesKilledTwentyFive = false;
        //public bool TotalBossesKilledFifty = false;
        //public bool TotalBossesKilledSeventyFive = false;
        //public bool TotalBossesKilledOneHundred = false;
        //// MOBS DROPPED
        //public int TotalMobsDroppedCount = 0;
        //// ROUNDS COMPLETED WITH NO DAMAGE
        //public int TotalRoundsCompletedWithNoDamageTakenCount = 0;
        //// ROUNDS COMPLETED WITH VAGABOND BLOODLINE
        //public int TotalRoundsCompletedWithVagabondCount = 0;
        //// ROUNDS COMPLETED WITH HARRIER BLOODLINE
        //public int TotalRoundsCompletedWithHarrierCount = 0;
        //// ROUNDS COMPLETED WITH PIONEER BLOODLINE
        //public int TotalRoundsCompletedWithPioneerCount = 0;
        //// ROUNDS COMPLETED WITH SCHOLAR BLOODLINE
        //public int TotalRoundsCompletedWithScholarCount = 0;

        public AchievementsModel()
        {
        }

        public AchievementsModel(
            string username,
            MobsKilledAchievement mobsKilledAchievement
            )
        {
            Username = username;
            MobsKilledAchievement = mobsKilledAchievement;
            //AchievementProgressHandler();
        }

        public AchievementsModel(AchievementsModel oldAchievements, SessionDataModel sessionDataModel)
        {
            Username = sessionDataModel.Username;
            MobsKilledAchievement = new MobsKilledAchievement(oldAchievements, sessionDataModel);
            //TotalBossesKilledCount = oldAchievements.TotalBossesKilledCount + sessionDataModel.BossesDefeatedCount;
            //if (sessionDataModel.BloodLineEnum.Equals(BloodlineType.Vagabond))
            //    TotalRoundsCompletedWithVagabondCount = oldAchievements.TotalRoundsCompletedWithVagabondCount + sessionDataModel.RoundNumberReached;
            //else if (sessionDataModel.BloodLineEnum.Equals(BloodlineType.Harrier))
            //    TotalRoundsCompletedWithHarrierCount = oldAchievements.TotalRoundsCompletedWithHarrierCount + sessionDataModel.RoundNumberReached;
            //else if (sessionDataModel.BloodLineEnum.Equals(BloodlineType.Pioneer))
            //    TotalRoundsCompletedWithPioneerCount = oldAchievements.TotalRoundsCompletedWithPioneerCount + sessionDataModel.RoundNumberReached;
            //else if (sessionDataModel.BloodLineEnum.Equals(BloodlineType.Scholar))
            //    TotalRoundsCompletedWithScholarCount = oldAchievements.TotalRoundsCompletedWithScholarCount + sessionDataModel.RoundNumberReached;

            AchievementProgressHandler();
        }

        public void AchievementProgressHandler()
        {
            MobsKilledAchievement.TotalMobsKilledProgressHandler();
        }
    }
}