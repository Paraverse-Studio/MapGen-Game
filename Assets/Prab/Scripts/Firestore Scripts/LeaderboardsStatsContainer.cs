using ParaverseWebsite.Models;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardsStatsContainer : MonoBehaviour
{
  public TextMeshProUGUI idxText;
  public TextMeshProUGUI usernameText;
  public TextMeshProUGUI totalScoreText;
  public TextMeshProUGUI roundReachedText;
  public TextMeshProUGUI gamesPlayedText;
  public TextMeshProUGUI averageLengthText;
  public Image mostUsedBloodlineImage;
  public Image mostUsedSkillImage;
  public Image mostUsedEffectsImage;

  public void Init(int idx, LeaderboardsModel model, Dictionary<BloodlineType, Sprite> bloodlineMapper, Dictionary<SkillName, Sprite> skillMapper, Dictionary<EffectName, Sprite> effectMapper)
  {
    idxText.text = idx.ToString();
    usernameText.text = model.Username.ToString();
    totalScoreText.text = model.CumulativeTotalScore.ToString();
    roundReachedText.text = model.HighestRoundNumberReached.ToString();
    gamesPlayedText.text = model.CumulativeGamesPlayed.ToString();
    averageLengthText.text = model.CumulativeSessionLength.ToString();
    Debug.Log($"bloodline for {model.Username}");
    mostUsedBloodlineImage.sprite = GetMostUsedBloodline(model.BloodLine, bloodlineMapper);
    //mostUsedBloodlineImage.type = Image.Type.Sliced;
    mostUsedSkillImage.sprite = GetMostUsedSkill(model.SkillUsed, skillMapper);
    mostUsedEffectsImage.sprite = GetMostUsedEffects(model.EffectsObtained, effectMapper);
  }

  private Sprite GetMostUsedBloodline(BloodlineOccurancesModel model, Dictionary<BloodlineType, Sprite> bloodlineMapper)
  {
    return bloodlineMapper[model.GetMostUsedBloodLine()];
  }

  public Sprite GetMostUsedSkill(SkillsUsedOccurancesModel model, Dictionary<SkillName, Sprite> skillMapper)
  {
    Debug.Log($"Most Used Skill is {skillMapper[model.GetMostUsedSkill()]}");
    return skillMapper[model.GetMostUsedSkill()];
  }

  public Sprite GetMostUsedEffects(EffectsObtainedOccurancesModel model, Dictionary<EffectName, Sprite> effectMapper)
  {
    Debug.Log($"Most Used Effect is {effectMapper[model.GetMostUsedEffects()]}");
    return effectMapper[model.GetMostUsedEffects()];
  }
}
