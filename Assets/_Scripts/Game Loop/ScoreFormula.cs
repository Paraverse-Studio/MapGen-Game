using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreFormula 
{

    /*
    *   So far, criteria to determine score:
    *   - Speed (Time Taken to complete round)
    *   - Evasion (Damage Taken throughout round)
    *   - Accuracy (How many skillshots launched vs how many hit)
    */

    public static float CalculateScore(float expectedTime, float timeTaken, int maxHealth, int damageTaken)
    {
        float score = -1f;

        // Time Taken ///////////////
        float extraTimeTaken = timeTaken - expectedTime;
        
        float timeScore = (1.0f - (extraTimeTaken / expectedTime)) * 50f + 30f; // individual aspect scores should be out of 100 (but can go beyond 100)

        //timeScore *= timeScore > 100? 1.1f : 1f;  // amplifies good score if they're above 100

        timeScore = Mathf.Max(timeScore, 0); // Safety


        // Damage Taken /////////////
        float relativeDamageTaken = maxHealth - damageTaken;

        float damageTakenScore = (relativeDamageTaken / (maxHealth * 0.90f)) * 100f; // 85% is considered 100 score in dmg taken

        //damageTakenScore *= damageTakenScore > 100f? 1.1f : 1f; // amplifies good score if they're above 100

        damageTakenScore = Mathf.Max(damageTakenScore, 0); // Safety

        // Accuracy /////////////////


        // Summary //////////////////
        score = (timeScore + damageTakenScore) / 2f;

        Debug.Log($"Score Summary: Time taken {timeTaken} (score {(int)timeScore}%), " +
            $"and damage taken {damageTaken} (score {(int)damageTakenScore}%): Score {(int)score}%");

        return score;
    }


}
