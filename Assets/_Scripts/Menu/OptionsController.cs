using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    [Header("Quality Settings:")]
    public TextMeshProUGUI qualityLabel;
    public Slider qualitySlider;

    private void OnEnable()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        qualityLabel.text = GlobalSettings.Instance.QualityLevel + "";
        qualitySlider.value = GlobalSettings.Instance.QualityLevel / 5f;
    }

    public void UpdateQualityLevel()
    {
        int val = Mathf.RoundToInt(qualitySlider.value * 4f) + 1;
        qualityLabel.text = val + "";
        GlobalSettings.Instance.QualityLevel = val;
    }
}
