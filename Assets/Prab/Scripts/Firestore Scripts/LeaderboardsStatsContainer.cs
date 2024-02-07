using Paraverse.Helper;
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
  public TextMeshProUGUI bossKilledText;
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
    bossKilledText.text = model.CumulativeBossesDefeatedCount.ToString();
    //Debug.Log($"bloodline for {model.Username}");
    mostUsedBloodlineImage.sprite = GetMostUsedBloodlineSprite(model.BloodLine, bloodlineMapper);
    mostUsedBloodlineImage.name = ParaverseHelper.GetBloodlineName(model.BloodLine.GetMostUsedBloodLine());
    //Debug.Log($"Most Used Bloodline: {mostUsedBloodlineImage}");

    mostUsedSkillImage.sprite = GetMostUsedSkillSprite(model.SkillUsed, skillMapper);
    mostUsedSkillImage.name = ParaverseHelper.GetSkillName(model.SkillUsed.GetMostUsedSkillEnumName());
    //Debug.Log($"Most Used Skill: {mostUsedSkillImage}");

    for (int i = 0; i < GetMostUsedEffectsSprite(model.EffectsObtained, effectMapper).Count; i++)
    {
      if (i == 0)
      {
        mostUsedEffectsImageOne.sprite = GetMostUsedEffectsSprite(model.EffectsObtained, effectMapper)[i];
        mostUsedEffectsImageOne.name = ParaverseHelper.GetEffectName(model.EffectsObtained.GetMostUsedEffects()[i]);
        //Debug.Log($"Most Used Effects: {mostUsedEffectsImageOne}");
      }
      if (i == 1)
      {
        mostUsedEffectsImageTwo.sprite = GetMostUsedEffectsSprite(model.EffectsObtained, effectMapper)[i];
        mostUsedEffectsImageTwo.name = ParaverseHelper.GetEffectName(model.EffectsObtained.GetMostUsedEffects()[i]);
        //Debug.Log($"Most Used Effects: {mostUsedEffectsImageTwo}");
      }
      if (i == 2)
      {
        mostUsedEffectsImageThree.sprite = GetMostUsedEffectsSprite(model.EffectsObtained, effectMapper)[i];
        mostUsedEffectsImageThree.name = ParaverseHelper.GetEffectName(model.EffectsObtained.GetMostUsedEffects()[i]);
        //Debug.Log($"Most Used Effects: {mostUsedEffectsImageThree}");
      }
    }

    mostUsedBloodlineImage.type = Image.Type.Sliced;
    mostUsedSkillImage.type = Image.Type.Sliced;
    mostUsedEffectsImageOne.type = Image.Type.Sliced;
    mostUsedEffectsImageTwo.type = Image.Type.Sliced;
    mostUsedEffectsImageThree.type = Image.Type.Sliced;
  }

  private Sprite GetMostUsedBloodlineSprite(BloodlineOccurancesModel model, Dictionary<BloodlineType, Sprite> bloodlineMapper)
  {
    return bloodlineMapper[model.GetMostUsedBloodLine()];
  }

  public Sprite GetMostUsedSkillSprite(SkillsUsedOccurancesModel model, Dictionary<SkillName, Sprite> skillMapper)
  {
    //Debug.Log($"Most Used Skill is {skillMapper[model.GetMostUsedSkillEnumName()]}");
    return skillMapper[model.GetMostUsedSkillEnumName()];
  }

  public List<Sprite> GetMostUsedEffectsSprite(EffectsObtainedOccurancesModel model, Dictionary<EffectName, Sprite> effectMapper)
  {
    List<Sprite> sprites = new List<Sprite>();

    foreach (EffectName effect in model.GetMostUsedEffects())
    {
      sprites.Add(effectMapper[effect]);
    }
    return sprites;
  }
}
