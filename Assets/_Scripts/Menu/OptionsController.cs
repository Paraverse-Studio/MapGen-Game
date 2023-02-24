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
        qualityLabel.text = QualityManager.QualityLevel + "";
        qualitySlider.value = QualityManager.QualityLevel / 5f;
    }

    public void UpdateQualityLevel()
    {
        int val = Mathf.RoundToInt(qualitySlider.value * 4f) + 1;
        qualityLabel.text = val + "";
        QualityManager.Instance.SetQualityLevel(val);
    }
}
