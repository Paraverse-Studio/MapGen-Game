using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// planned for use later if the returned object from this script should give more than just float score
public class ScoreData
{
    public float Score;

}


public class ScoreFormula 
{
    /*
    *   So far, criteria to determine score:
    *   - Speed (Time Taken to complete round)
    *   - Evasion (Damage Taken throughout round)
    *   - Accuracy (How many skillshots launched vs how many hit)
    */

    // SCORE FORMULA equations experimented here:
    // https://docs.google.com/spreadsheets/d/1YmY8MhHaiasEs3mNR-olTqkzsJOxXCbKX6hNTioxm4c/edit#gid=0

    // Time Taken resources:

    // Damage Taken resources graph:
    // https://www.desmos.com/calculator/wuaxtd2vef (shows the graph)
    // https://www.mathcelebrity.com/3ptquad.php?p1=50%2C+75&p2=0%2C+107&p3=100%2C+35&pl=Calculate+Equation (helped find the equation)


    public static float CalculateScore(float expectedTime, float timeTaken, int maxHealth, int damageTaken, out int goldToReward)
    {
        float score = -1f;

        // Time Taken ///////////////
        float extraTimeTaken = timeTaken - expectedTime;
        
        float timeScore = (1.0f - (extraTimeTaken / expectedTime)) * 35f + 45f; // individual aspect scores should be out of 100 (but can go beyond 100)

        timeScore *= timeScore > 100? 1.1f : 1f;  // amplifies good score if they're above 100

        timeScore = Mathf.Max(timeScore, 50); // Safety, 50 so players can explore the map a bit


        // Damage Taken /////////////
        float damageTakenScore = DamageTakenScore2(maxHealth, damageTaken); 

        damageTakenScore *= damageTakenScore > 100f? 1.1f : 1f; // amplifies good score if they're above 100

        damageTakenScore = Mathf.Max(damageTakenScore, 0); // Safety

        // Accuracy /////////////////


        // Summary //////////////////
        score = (timeScore + damageTakenScore) / 2f;

        Debug.Log($"Score Summary: Time taken {timeTaken} (score {(int)timeScore}%), " +
            $"and damage taken {damageTaken} [given max HP: {maxHealth}] (score {(int)damageTakenScore}%): Score {(int)score}%");


        if (score >= 90)
        {
            goldToReward = (int)(score * 1.2f);
        }
        else if (score >= 80)
        {
            goldToReward = (int)(score * 1.1f);
        }
        else
        {
            goldToReward = (int)score;
        }

        return score;
    }



    // Version 1
    private static float DamageTakenScore(int maxHealth, int damageTaken)
    {
        float relativeDamageTaken = maxHealth - damageTaken;

        float damageTakenScore = (relativeDamageTaken / (maxHealth * 0.90f)) * 80f + 20f; // check google sheets to see why this equation is such

        damageTakenScore *= damageTakenScore > 100f ? 1.1f : 1f; // amplifies good score if they're above 100

        damageTakenScore = Mathf.Max(damageTakenScore, 0); // Safety

        return damageTakenScore;
    }

    // Version 2
    private static float DamageTakenScore2(int maxHealth, int damageTaken)
    {
        float percentDamageTaken = ((float)damageTaken / (float)maxHealth) * 100.0f;
        return (-0.0026f * Mathf.Pow(percentDamageTaken, 2f)) - (0.75f * percentDamageTaken) + 107f; 
    }

    public static string GetScoreRank(int score)
    {
        if (score > 100) return "Cube Hero!";
        else if (score >= 95) return "God-Like!";
        else if (score >= 90) return "Legendary!";
        else if (score >= 80) return "Excellent";
        else if (score >= 70) return "Great";
        else if (score >= 60) return "Very Good";
        else return "Good";
    }

}
