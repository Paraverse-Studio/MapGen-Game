using System;

namespace ParaverseWebsite.Models
{
    [Serializable]
    public class AchievementsModel
    {
        // ROUNDS REACHED
        public int TotalRoundsReachedCount { get; set; }
        private bool TotalRoundReachedTen = false;
        private bool TotalRoundReachedTwentyFive = false;
        private bool TotalRoundReachedFifty = false;
        private bool TotalRoundReachedOneHundred = false;
        private bool TotalRoundReachedFiveHundred = false;

        // MOBS KILLED
        public int TotalMobsKilledCount { get; set; }
        private bool TotalMobsKilledTen = false;
        private bool TotalMobsKilledOneHundred = false;
        private bool TotalMobsKilledTwoHundredFifty = false;
        private bool TotalMobsKilledFiveHundred = false;
        private bool TotalMobsKilledSevenHundredFifty = false;
        private bool TotalMobsKilledOneThousand = false;
        // BOSSES KILLED
        public int TotalBossesKilledCount { get; set; }
        private bool TotalBossesKilledFive = false;
        private bool TotalBossesKilledTen = false;
        private bool TotalBossesKilledTwentyFive = false;
        private bool TotalBossesKilledFifty = false;
        private bool TotalBossesKilledSeventyFive = false;
        private bool TotalBossesKilledOneHundred = false;
        // MOBS DROPPED
        public int TotalMobsDroppedCount { get; set; }
        // ROUNDS COMPLETED WITH NO DAMAGE
        public int TotalRoundsCompletedWithNoDamageTakenCount { get; set; }
        // ROUNDS COMPLETED WITH VAGABOND BLOODLINE
        public int TotalRoundsCompletedWithVagabondCount { get; set; }
        // ROUNDS COMPLETED WITH HARRIER BLOODLINE
        public int TotalRoundsCompletedWithHarrierCount { get; set; }
        // ROUNDS COMPLETED WITH PIONEER BLOODLINE
        public int TotalRoundsCompletedWithPioneerCount { get; set; }
        // ROUNDS COMPLETED WITH SCHOLAR BLOODLINE
        public int TotalRoundsCompletedWithScholarCount { get; set; }

        public AchievementsModel()
        {
            TotalRoundsReachedCount = 0;
            TotalMobsKilledCount = 0;
            TotalBossesKilledCount = 0;
            TotalMobsDroppedCount = 0;
            TotalRoundsCompletedWithNoDamageTakenCount = 0;
            TotalRoundsCompletedWithVagabondCount = 0;
            TotalRoundsCompletedWithHarrierCount = 0;
            TotalRoundsCompletedWithPioneerCount = 0;
            TotalRoundsCompletedWithScholarCount = 0;
        }

        public AchievementsModel(
            int totalRndsReached,
            int totalMobsKilled,
            int totalMobsDropped,
            int totalRndsCmpltedNoDmg,
            int totalRndsWithVagabond,
            int totalRndsWithHarrier,
            int totalRndsWithPioneer,
            int totalRndsWithScholar
            )
        {
            TotalRoundsReachedCount += totalRndsReached;
            TotalMobsKilledCount = totalMobsKilled;
            TotalMobsDroppedCount += totalMobsDropped;
            TotalRoundsCompletedWithNoDamageTakenCount += totalRndsCmpltedNoDmg;
            TotalRoundsCompletedWithVagabondCount += totalRndsWithVagabond;
            TotalRoundsCompletedWithHarrierCount += totalRndsWithHarrier;
            TotalRoundsCompletedWithPioneerCount += totalRndsWithPioneer;
            TotalRoundsCompletedWithScholarCount += totalRndsWithScholar;
        }

        public void TotalRoundReachedProgressHandler()
        {
            if (TotalRoundsReachedCount >= 500 && !TotalRoundReachedFiveHundred)
                TotalRoundReachedFiveHundred = true;
            else if (TotalRoundsReachedCount >= 100 && !TotalRoundReachedOneHundred)
                TotalRoundReachedOneHundred = true;
            else if (TotalRoundsReachedCount >= 50 && !TotalRoundReachedFifty)
                TotalRoundReachedFifty = true;
            else if (TotalRoundsReachedCount >= 25 && !TotalRoundReachedTwentyFive)
                TotalRoundReachedTwentyFive = true;
            else if (TotalRoundsReachedCount >= 10 && !TotalRoundReachedTen)
                TotalRoundReachedTen = true;
        }

        public void TotalMobsKilledProgressHandler()
        {
            if (TotalMobsKilledCount >= 1000 && !TotalMobsKilledOneThousand)
                TotalMobsKilledOneThousand = true;
            else if (TotalMobsKilledCount >= 750 && !TotalMobsKilledSevenHundredFifty)
                TotalMobsKilledSevenHundredFifty = true;
            else if (TotalMobsKilledCount >= 500 && !TotalMobsKilledFiveHundred)
                TotalMobsKilledFiveHundred = true;
            else if (TotalMobsKilledCount >= 250 && !TotalMobsKilledTwoHundredFifty)
                TotalMobsKilledTwoHundredFifty = true;
            else if (TotalMobsKilledCount >= 100 && !TotalMobsKilledOneHundred)
                TotalMobsKilledOneHundred = true;
            else if (TotalMobsKilledCount >= 10 && !TotalMobsKilledTen)
                TotalMobsKilledTen = true;
        }

        public void TotalBossesKilledProgressHandler()
        {
            if (TotalBossesKilledCount >= 100 && !TotalBossesKilledOneHundred)
                TotalBossesKilledOneHundred = true;
            else if (TotalBossesKilledCount >= 75 && !TotalBossesKilledSeventyFive)
                TotalBossesKilledSeventyFive = true;
            else if (TotalBossesKilledCount >= 50 && !TotalBossesKilledFifty)
                TotalBossesKilledFifty = true;
            else if (TotalBossesKilledCount >= 25 && !TotalBossesKilledTwentyFive)
                TotalBossesKilledTwentyFive = true;
            else if (TotalBossesKilledCount >= 10 && !TotalBossesKilledTen)
                TotalBossesKilledTen = true;
            else if (TotalBossesKilledCount >= 5 && !TotalBossesKilledFive)
                TotalBossesKilledFive = true;
        }
    }
}