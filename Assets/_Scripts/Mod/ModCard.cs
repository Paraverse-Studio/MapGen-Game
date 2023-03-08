using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModCard : ItemCard
{
    [Header("Mod Colours")]
    public Color StatModColor;
    public Color StatModColorGlow;
    public Color SkillModColor;
    public Color SkillModColorGlow;
    public Color EffectModColor;
    public Color EffectModColorGlow;

    public Button purchaseButton;
    public GameObject cardLock;

    public Image CardBG;
    public Image CardOtherBG;
    public TextMeshProUGUI TypeLabel;
    public Image[] CardHighlights;

    private void Start()
    {
        if (Item) UpdateDisplay();
    }

    private Color GetModColor(SO_Mod mod, Image image = null, bool typeTwo = false)
    {
        float alphaRetain = (null != image)? image.color.a : -1f;        
        Color c;
        if (mod.Type == ModType.Stats) c = StatModColor;
        else if (mod.Type == ModType.Skill) c = SkillModColor;
        else c = EffectModColor;

        if (typeTwo)
        {
            if (mod.Type == ModType.Stats) c = StatModColorGlow;
            else if (mod.Type == ModType.Skill) c = SkillModColorGlow;
            else c = EffectModColorGlow;
        }

        if (alphaRetain != -1f) c.a = alphaRetain;
        return c;
    }

    public override void UpdateDisplay(System.Action clickCallBack = null)
    {
        if (null == Item) return;

        // All items can have these elements
        if (titleLabel) titleLabel.text = Item.GetTitle();
        if (imageHolder) imageHolder.sprite = Item.Image;
        if (descriptionLabel) descriptionLabel.text = Item.GetDescription();
        if (loreLabel) loreLabel.text = Item.Lore;
        if (costLabel) costLabel.text = Item.GetCost().ToString();

        // Mod specific
        if (typeLabel && Item is SO_Mod) typeLabel.text = ((SO_Mod)Item).Type.ToString();
        if (CardBG && Item is SO_Mod) CardBG.color = GetModColor((SO_Mod)Item, CardBG);
        if (CardOtherBG && Item is SO_Mod) CardOtherBG.color = GetModColor((SO_Mod)Item);
        if (TypeLabel && Item is SO_Mod) TypeLabel.color = GetModColor((SO_Mod)Item, typeTwo: true);
        if (null != CardHighlights && CardHighlights.Length > 0 && Item is SO_Mod) 
        {
            foreach (Image highlight in CardHighlights)
            {
                Color c = GetModColor((SO_Mod)Item, highlight, typeTwo: true); highlight.color = c;
            }
        }

        // Callback to when this item card is clicked
        if (null != clickCallBack) purchaseButton.onClick.AddListener(() => { clickCallBack.Invoke(); });
    }

    public void UpdateDescription()
    {
        if (descriptionLabel) descriptionLabel.text = Item.GetDescription();
    }

}
