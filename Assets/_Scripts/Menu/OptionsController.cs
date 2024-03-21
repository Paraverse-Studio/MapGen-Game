using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
  [Header("Quality Settings:")]
  public TextMeshProUGUI qualityLabel;
  public Slider qualitySlider;

  [Header("Water:")]
  public Toggle waterToggle;
  public bool WaterEnabled = true;

  private void OnEnable()
  {
    UpdateDisplay();
  }

  public void UpdateDisplay()
  {
    qualityLabel.text = QualityManager.QualityLevel + "";
    qualitySlider.value = (QualityManager.QualityLevel - 1f) / 4f;
    waterToggle.isOn = GlobalSettings.Instance.waterVolume.gameObject.activeSelf;
  }

  public void UpdateQualityLevel()
  {
    int val = Mathf.RoundToInt(qualitySlider.value * 4f) + 1;
    qualityLabel.text = val + "";
    QualityManager.Instance.SetQualityLevel(val);
  }

  public void UpdateWater()
  {
    WaterEnabled = waterToggle.isOn;
    if (GlobalSettings.Instance.waterVolume) GlobalSettings.Instance.waterVolume.gameObject.SetActive(waterToggle.isOn);
  }
}
