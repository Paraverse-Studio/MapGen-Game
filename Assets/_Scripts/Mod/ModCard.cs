using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModCard : MonoBehaviour
{
    [Header("Mod Colours")]
    public Color StatModColor;
    public Color StatModColorGlow;
    public Color SkillModColor;
    public Color SkillModColorGlow;
    public Color EffectModColor;
    public Color EffectModColorGlow;

    [Header("Mod")]
    public SO_Mod Mod;
    public TextMeshProUGUI titleLabel;
    public Image imageHolder;
    public TextMeshProUGUI descriptionLabel;
    public TextMeshProUGUI loreLabel;
    public TextMeshProUGUI costLabel;
    public TextMeshProUGUI typeLabel;

    public Button purchaseButton;
    public GameObject cardLock;

    public Image CardBG;
    public Image CardOtherBG;
    public TextMeshProUGUI TypeLabel;
    public Image[] CardHighlights;

    private void Start()
    {
        if (Mod) UpdateDisplay();
    }

    private Color GetModColor(Image image = null, bool typeTwo = false)
    {
        float alphaRetain = (null != image)? image.color.a : -1f;        
        Color c;
        if (Mod.Type == ModType.Stats) c = StatModColor;
        else if (Mod.Type == ModType.Skill) c = SkillModColor;
        else c = EffectModColor;

        if (typeTwo)
        {
            if (Mod.Type == ModType.Stats) c = StatModColorGlow;
            else if (Mod.Type == ModType.Skill) c = SkillModColorGlow;
            else c = EffectModColorGlow;
        }

        if (alphaRetain != -1f) c.a = alphaRetain;
        return c;
    }

    public void UpdateDisplay(System.Action clickCallBack = null)
    {
        if (titleLabel) titleLabel.text = Mod.GetTitle();
        if (imageHolder) imageHolder.sprite = Mod.Image;
        if (descriptionLabel) descriptionLabel.text = Mod.GetDescription();
        if (loreLabel) loreLabel.text = Mod.Lore;
        if (costLabel) costLabel.text = Mod.GetCost().ToString();
        if (typeLabel) typeLabel.text = Mod.Type.ToString();

        if (CardBG) CardBG.color = GetModColor();
        if (CardOtherBG) CardOtherBG.color = GetModColor();
        if (TypeLabel) TypeLabel.color = GetModColor(typeTwo: true);
        if (null != CardHighlights && CardHighlights.Length > 0) 
        {
            foreach (Image highlight in CardHighlights)
            {
                Color c = GetModColor(highlight, typeTwo: true); highlight.color = c;
            }
        }

        if (null != clickCallBack) purchaseButton.onClick.AddListener(() => { clickCallBack.Invoke(); });
    }

    public void UpdateDescription()
    {
        if (descriptionLabel) descriptionLabel.text = Mod.GetDescription();
    }

}
