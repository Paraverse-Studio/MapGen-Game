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
    public TextMeshProUGUI costLabel;
    public TextMeshProUGUI typeLabel;

    public Button purchaseButton;
    public GameObject cardLock;

    private void Start()
    {
        UpdateDisplay();
    }
    public void UpdateDisplay()
    {
        titleLabel.text = Mod.Title;
        imageHolder.sprite = Mod.Image;
        descriptionLabel.text = Mod.Description;
        costLabel.text = Mod.GetCost().ToString();
        typeLabel.text = Mod.Type.ToString();
    }
}
