using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModCard : MonoBehaviour
{
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

    private void Start()
    {
        if (Mod) UpdateDisplay();
    }

    public void UpdateDisplay(System.Action clickCallBack = null)
    {
        if (titleLabel) titleLabel.text = Mod.GetTitle();
        if (imageHolder) imageHolder.sprite = Mod.Image;
        if (descriptionLabel) descriptionLabel .text = Mod.GetDescription();
        if (loreLabel) loreLabel.text = Mod.Lore;
        if (costLabel) costLabel.text = Mod.GetCost().ToString();
        if (typeLabel) typeLabel.text = Mod.Type.ToString();

        if (null != clickCallBack) purchaseButton.onClick.AddListener(() => { clickCallBack.Invoke(); });
    }

    public void UpdateDescription()
    {
        if (descriptionLabel) descriptionLabel.text = Mod.GetDescription();
    }

}
