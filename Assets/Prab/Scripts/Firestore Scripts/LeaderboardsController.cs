using ParaverseWebsite.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LeaderboardsController : MonoBehaviour
{
  public Transform leaderboardsParentGO;
  public GameObject leaderboardsStatContainerPrefab;

  public List<GameObject> leaderboardStats = new List<GameObject>();

  public Sprite VagabondSprite;
  public Sprite HarrierSprite;
  public Sprite PioneerSprite;
  public Sprite ScholarSprite;

  public Sprite NoSkillSprite;
  public Sprite RegalCrescentSprite;
  public Sprite MoonlightSlashSprite;
  public Sprite DescendingThrustSprite;
  public Sprite AzuriteInfusionSprite;
  public Sprite BladeWhirlSprite;
  public Sprite StealthStepSprite;
  public Sprite LightningBoltSprite;
  public Sprite AvatarStateSprite;

  public Sprite EmpoweredAttackSprite;
  public Sprite SunfireSprite;
  public Sprite CooldownRefundSprite;
  public Sprite LichbaneSprite;
  public Sprite RepearKillSprite;
  public Sprite SweepingDashSprite;


  private void OnEnable()
  {
    ClearAll();
    FirebaseDatabaseManager.Instance.GetLeaderboards(GetLeaderboardsStatsFromDatabase);
  }

  private void GetLeaderboardsStatsFromDatabase(Dictionary<string, LeaderboardsModel> model)
  {
    

    int idx = 1;
    foreach (KeyValuePair<string, LeaderboardsModel> stat in model.OrderByDescending(key => key.Value.CumulativeTotalScore))
    {
      CreateLeaderboardStatContainer(idx, stat.Value);
      idx++;
    }
  }

  private void CreateLeaderboardStatContainer(int idx, LeaderboardsModel model)
  {
    GameObject obj = Instantiate(leaderboardsStatContainerPrefab, leaderboardsParentGO);
    obj.GetComponent<LeaderboardsStatsContainer>().Init(idx, model, DataMapper.BloodlineSpriteMapper, DataMapper.SkillSpriteMapper, DataMapper.EffectSpriteMapper);
    leaderboardStats.Add(obj);
  }

  private void ClearAll()
  {
    foreach (GameObject obj in leaderboardStats)
    {
      Destroy(obj);
    }
    leaderboardStats.Clear();
  }
}
