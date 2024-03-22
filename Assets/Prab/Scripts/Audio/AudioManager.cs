using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
  [SerializeField] private AudioSource audioSource;

  [Header("Music Settings:")]
  public TextMeshProUGUI musicLabel;
  public Slider musicSlider;

  [Header("Sound Settings:")]
  public TextMeshProUGUI soundLabel;
  public Slider soundSlider;
  public List<AudioSource> soundSources = new List<AudioSource>();

  #region Singleton
  public static AudioManager Instance;

  private void Awake()
  {
    if (Instance == null)
      Instance = this;
    else
      Destroy(this);
  }
  #endregion

  private void OnEnable()
  {
    UpdateMusicDisplay();
    UpdateMusicLevel();
    UpdateSoundDisplay();
    UpdateSoundLevel();
  }

  private void Start()
  {
    if (audioSource == null) audioSource = GetComponent<AudioSource>();
    audioSource.Play();
  }

  public void UpdateMusicDisplay()
  {
    musicLabel.text = musicSlider.value * 100 + "%";
    musicSlider.value = musicSlider.value * 100;
  }

  public void UpdateMusicLevel()
  {
    int val = Mathf.RoundToInt(musicSlider.value * 100);
    musicLabel.text = val + "%";
    audioSource.volume = musicSlider.value;
  }

  public void UpdateSoundDisplay()
  {
    soundLabel.text = soundSlider.value * 100 + "%";
    soundSlider.value = soundSlider.value * 100;
  }

  public void UpdateSoundLevel()
  {
    int val = Mathf.RoundToInt(soundSlider.value * 100);
    soundLabel.text = val + "%";
    for (int i = 0; i < soundSources.Count; i++)
    {
      soundSources[i].volume = soundSlider.value;
    }
  }
}
