using UnityEngine;
using System;
using System.Collections.Generic;

namespace ParaverseWebsite.Models
{
  [Serializable]
  public class BloodlineOccurancesModel
  {
    public int Vagabond = 0;
    public int Harrier = 0;
    public int Pioneer = 0;
    public int Scholar = 0;

    public BloodlineOccurancesModel() { }

    /// <summary>
    /// For updating Match History 
    /// </summary>
    /// <param name="bloodLine"></param>
    public BloodlineOccurancesModel(BloodlineType bloodLine)
    {
      switch (bloodLine)
      {
        case BloodlineType.Vagabond:
          Vagabond++;
          break;
        case BloodlineType.Harrier:
          Harrier++; 
          break;
        case BloodlineType.Pioneer: 
          Pioneer++;
          break;
        case BloodlineType.Scholar:
          Scholar++; 
          break;
        default:
          Debug.Log(bloodLine + " doesn't exist in BloodlineType enum");
          break;
      }
    }

    /// <summary>
    /// For updating Leaderboards
    /// </summary>
    /// <param name="oldLeaderboards"></param>
    /// <param name="sessionDataModel"></param>
    public BloodlineOccurancesModel(LeaderboardsModel oldLeaderboards, SessionDataModel sessionDataModel)
    {
      Vagabond = oldLeaderboards.BloodLine.Vagabond;
      Harrier = oldLeaderboards.BloodLine.Harrier;
      Pioneer = oldLeaderboards.BloodLine.Pioneer;
      Scholar = oldLeaderboards.BloodLine.Scholar;

      switch (sessionDataModel.BloodLineEnum)
      {
        case BloodlineType.Vagabond:
          Vagabond++;
          break;
        case BloodlineType.Harrier:
          Harrier++;
          break;
        case BloodlineType.Pioneer:
          Pioneer++;
          break;
        case BloodlineType.Scholar:
          Scholar++;
          break;
        default:
          Debug.Log(sessionDataModel.BloodLineEnum + " doesn't exist in BloodlineType enum");
          break;
      }
    }

    public BloodlineType GetMostUsedBloodLine()
    {
      int occurances = 0;
      BloodlineType mostUsedBloodline = BloodlineType.Harrier;
      Dictionary<BloodlineType, int> result = new Dictionary<BloodlineType, int>
      {
        { BloodlineType.Vagabond, Vagabond },
        { BloodlineType.Harrier, Harrier },
        { BloodlineType.Pioneer, Pioneer },
        { BloodlineType.Scholar, Scholar }
      };

      foreach (KeyValuePair<BloodlineType, int> bloodline in result)
      {
        if (bloodline.Value >= occurances)
        {
          occurances = bloodline.Value;
          mostUsedBloodline = bloodline.Key;
        }
      }

      return mostUsedBloodline;
    }
  }
}