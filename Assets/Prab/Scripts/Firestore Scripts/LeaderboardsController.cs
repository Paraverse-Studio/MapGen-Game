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

  public Dictionary<BloodlineType, Sprite> BloodlineSpriteMapper = new Dictionary<BloodlineType, Sprite>();
  public Dictionary<SkillName, Sprite> SkillSpriteMapper = new Dictionary<SkillName, Sprite>();
  public Dictionary<EffectName, Sprite> EffectSpriteMapper = new Dictionary<EffectName, Sprite>();

  private void Awake()
  {
    BloodlineSpriteMapper.Add(BloodlineType.Vagabond, VagabondSprite);
    BloodlineSpriteMapper.Add(BloodlineType.Harrier, HarrierSprite);
    BloodlineSpriteMapper.Add(BloodlineType.Pioneer, PioneerSprite);
    BloodlineSpriteMapper.Add(BloodlineType.Scholar, ScholarSprite);

    SkillSpriteMapper.Add(SkillName.None, NoSkillSprite);
    SkillSpriteMapper.Add(SkillName.RegalCrescent, RegalCrescentSprite);
    SkillSpriteMapper.Add(SkillName.MoonlightSlash, MoonlightSlashSprite);
    SkillSpriteMapper.Add(SkillName.DescendingThrust, DescendingThrustSprite);
    SkillSpriteMapper.Add(SkillName.AzuriteInfusion, AzuriteInfusionSprite);
    SkillSpriteMapper.Add(SkillName.BladeWhirl, BladeWhirlSprite);
    SkillSpriteMapper.Add(SkillName.StealthStep, StealthStepSprite);
    SkillSpriteMapper.Add(SkillName.LightningBolt, LightningBoltSprite);
    SkillSpriteMapper.Add(SkillName.AvatarState, AvatarStateSprite);

    EffectSpriteMapper.Add(EffectName.None, NoSkillSprite);
    EffectSpriteMapper.Add(EffectName.EmpoweredAttack, EmpoweredAttackSprite);
    EffectSpriteMapper.Add(EffectName.Sunfire, SunfireSprite);
    EffectSpriteMapper.Add(EffectName.CooldownRefund, CooldownRefundSprite);
    EffectSpriteMapper.Add(EffectName.Lichbane, LichbaneSprite);
    EffectSpriteMapper.Add(EffectName.RepearKill, RepearKillSprite);
    EffectSpriteMapper.Add(EffectName.SweepingDash, SweepingDashSprite);
  }

  private void OnEnable()
  {
    ClearAll();
    FirebaseDatabaseManager.Instance.GetLeaderboards(GetLeaderboardsStatsFromDatabase);
  }

  private void GetLeaderboardsStatsFromDatabase(Dictionary<string, LeaderboardsModel> model)
  {
    

    int idx = 0;
    foreach (KeyValuePair<string, LeaderboardsModel> stat in model.OrderByDescending(key => key.Value.CumulativeTotalScore))
    {
      CreateLeaderboardStatContainer(idx, stat.Value);
      idx++;
    }
  }

  private void CreateLeaderboardStatContainer(int idx, LeaderboardsModel model)
  {
    GameObject obj = Instantiate(leaderboardsStatContainerPrefab, leaderboardsParentGO);
    obj.GetComponent<LeaderboardsStatsContainer>().Init(idx, model, BloodlineSpriteMapper, SkillSpriteMapper, EffectSpriteMapper);
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
