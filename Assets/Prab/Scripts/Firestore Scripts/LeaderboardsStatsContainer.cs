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
  public Image mostUsedEffectsImageOne;
  public Image mostUsedEffectsImageTwo;
  public Image mostUsedEffectsImageThree;


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
    mostUsedSkillImage.sprite = GetMostUsedSkill(model.SkillUsed, skillMapper);

    for (int i = 0; i < GetMostUsedEffects(model.EffectsObtained, effectMapper).Count; i++)
    {
      if (i == 0) mostUsedEffectsImageOne.sprite = GetMostUsedEffects(model.EffectsObtained, effectMapper)[i];
      if (i == 1) mostUsedEffectsImageTwo.sprite = GetMostUsedEffects(model.EffectsObtained, effectMapper)[i];
      if (i == 2) mostUsedEffectsImageThree.sprite = GetMostUsedEffects(model.EffectsObtained, effectMapper)[i];
    }


    mostUsedBloodlineImage.type = Image.Type.Sliced;
    mostUsedSkillImage.type = Image.Type.Sliced;
    mostUsedEffectsImageOne.type = Image.Type.Sliced;
    mostUsedEffectsImageTwo.type = Image.Type.Sliced;
    mostUsedEffectsImageThree.type = Image.Type.Sliced;
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

  public List<Sprite> GetMostUsedEffects(EffectsObtainedOccurancesModel model, Dictionary<EffectName, Sprite> effectMapper)
  {
    List<Sprite> sprites = new List<Sprite>();

    foreach (EffectName effect in model.GetMostUsedEffects())
    {
      sprites.Add(effectMapper[effect]);
    }
    return sprites;
  }
}
