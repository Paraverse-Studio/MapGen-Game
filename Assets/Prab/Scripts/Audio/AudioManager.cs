using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
  [SerializeField] private AudioSource audioSource;

  [Header("Music Settings:")]
  public TextMeshProUGUI musicLabel;
  public Slider musicSlider;

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
  }

  private void Start()
  {
    if (audioSource == null) audioSource = GetComponent<AudioSource>();
    audioSource.Play();
  }

  private float GetAudioVolume()
  {
    return audioSource.volume * 100;
  }

  public void UpdateMusicDisplay()
  {
    musicLabel.text = GetAudioVolume() + "%";
    musicSlider.value = GetAudioVolume();
  }

  public void UpdateMusicLevel()
  {
    int val = Mathf.RoundToInt(musicSlider.value * 100);
    musicLabel.text = val + "%";
    audioSource.volume = musicSlider.value;
  }
}
