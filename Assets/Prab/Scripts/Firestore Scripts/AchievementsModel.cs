using System;
using UnityEngine;

namespace ParaverseWebsite.Models
{
    [Serializable]
    public class AchievementsModel
    {
        public string Username;
        // ROUNDS REACHED
        public RoundsReachedAchievement RoundsReachedAchievement;
        // MOBS KILLED
        public MobsKilledAchievement MobsKilledAchievement;
        // BOSSES KILLED
        public BossesKilledAchievement BossesKilledAchievement;
        // MOBS DROPPED
        //public MobsDroppedAchievement MobsDroppedAchievement;
        // ROUNDS COMPLETED WITH VAGABOND BLOODLINE
        public VagabondRoundsCompletedAchievement VagabondRoundsCompletedAchievement;
        // ROUNDS COMPLETED WITH HARRIER BLOODLINE
        public HarrierRoundsCompletedAchievement HarrierRoundsCompletedAchievement;
        // ROUNDS COMPLETED WITH PIONEER BLOODLINE
        public PioneerRoundsCompletedAchievement PioneerRoundsCompletedAchievement;
        // ROUNDS COMPLETED WITH SCHOLAR BLOODLINE
        public ScholarRoundsCompletedAchievement ScholarRoundsCompletedAchievement;

        public AchievementsModel()
        {
        }

        public AchievementsModel(
            string username,
            RoundsReachedAchievement roundsReachedAchievement,
            MobsKilledAchievement mobsKilledAchievement,
            BossesKilledAchievement bossesKilledAchievement,
            //MobsDroppedAchievement mobsDroppedAchievement,
            VagabondRoundsCompletedAchievement vagabondRoundsCompletedAchievement,
            HarrierRoundsCompletedAchievement harrierRoundsCompletedAchievement,
            PioneerRoundsCompletedAchievement pioneerRoundsCompletedAchievement,
            ScholarRoundsCompletedAchievement scholarRoundsCompletedAchievement
            )
        {
            Username = username;
            RoundsReachedAchievement = roundsReachedAchievement;
            MobsKilledAchievement = mobsKilledAchievement;
            BossesKilledAchievement = bossesKilledAchievement;
            //MobsDroppedAchievement = mobsDroppedAchievement;
            VagabondRoundsCompletedAchievement = vagabondRoundsCompletedAchievement;
            HarrierRoundsCompletedAchievement = harrierRoundsCompletedAchievement;
            PioneerRoundsCompletedAchievement = pioneerRoundsCompletedAchievement;
            ScholarRoundsCompletedAchievement = scholarRoundsCompletedAchievement;
        }

        public AchievementsModel(AchievementsModel oldAchievements, SessionDataModel sessionDataModel)
        {
            Username = sessionDataModel.Username;
            RoundsReachedAchievement = new RoundsReachedAchievement(oldAchievements, sessionDataModel);
            MobsKilledAchievement = new MobsKilledAchievement(oldAchievements, sessionDataModel);
            BossesKilledAchievement = new BossesKilledAchievement(oldAchievements, sessionDataModel);
            //MobsDroppedAchievement = new MobsDroppedAchievement(oldAchievements, sessionDataModel);
            VagabondRoundsCompletedAchievement = new VagabondRoundsCompletedAchievement(oldAchievements, sessionDataModel);
            HarrierRoundsCompletedAchievement = new HarrierRoundsCompletedAchievement(oldAchievements, sessionDataModel);
            PioneerRoundsCompletedAchievement = new PioneerRoundsCompletedAchievement(oldAchievements, sessionDataModel);
            ScholarRoundsCompletedAchievement = new ScholarRoundsCompletedAchievement(oldAchievements, sessionDataModel);

            AchievementProgressHandler();
        }

        public void AchievementProgressHandler()
        {
            RoundsReachedAchievement.AchievementProgressHandler();
            MobsKilledAchievement.AchievementProgressHandler();
            BossesKilledAchievement.AchievementProgressHandler();
            //MobsDroppedAchievement.AchievementProgressHandler();
            VagabondRoundsCompletedAchievement.AchievementProgressHandler();
            HarrierRoundsCompletedAchievement.AchievementProgressHandler();
            PioneerRoundsCompletedAchievement.AchievementProgressHandler();
            ScholarRoundsCompletedAchievement.AchievementProgressHandler();
        }
    }
}