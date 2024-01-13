using ParaverseWebsite.Models;
using System;

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
    if (bloodLine.Equals(BloodlineType.Vagabond))
      Vagabond++;
    else if (bloodLine.Equals(BloodlineType.Harrier))
      Harrier++;
    else if (bloodLine == BloodlineType.Pioneer)
      Pioneer++;
    else if (bloodLine == BloodlineType.Scholar)
      Scholar++;
  }

  /// <summary>
  /// For updating Leaderboards
  /// </summary>
  /// <param name="oldLeaderboards"></param>
  /// <param name="sessionDataModel"></param>
  public BloodlineOccurancesModel(LeaderboardsModel oldLeaderboards, SessionDataModel sessionDataModel)
  {
    if (sessionDataModel.BloodLineEnum.Equals(BloodlineType.Vagabond))
      Vagabond = ++oldLeaderboards.BloodLine.Vagabond;
    else if (sessionDataModel.BloodLineEnum.Equals(BloodlineType.Harrier))
      Harrier = ++oldLeaderboards.BloodLine.Harrier;
    else if (sessionDataModel.BloodLineEnum.Equals(BloodlineType.Pioneer))
      Pioneer = ++oldLeaderboards.BloodLine.Pioneer;
    else if (sessionDataModel.BloodLineEnum.Equals(BloodlineType.Scholar))
      Scholar = ++oldLeaderboards.BloodLine.Scholar;
  }
}

