using UnityEngine;
using System;

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
  }
}