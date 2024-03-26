using ParaverseWebsite.Models;
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
    public int MusicVolume;

    [Header("Sound Settings:")]
    public TextMeshProUGUI soundLabel;
    public Slider soundSlider;
    public List<AudioSource> soundSources = new List<AudioSource>();
    public int SoundVolume;


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

    // Need to fix update sound upon enabling 
    private void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    public void UpdateOptionsSettingFromDatabase()
    {
        Debug.Log($"MainMenuController.Instance.Username: {MainMenuController.Instance.Username}");

        FirebaseDatabaseManager.Instance.GetUser(MainMenuController.Instance.Username,
        //  IF USER IS FOUND!!
          (updatedUserModel) => UpdateOptionsSettings(updatedUserModel),
          //  IF USER IS NOT FOUND!!
          () => DefaultSettings());
    }

    private void UpdateOptionsSettings(UserModel model)
    {

        UpdateMusicDisplayFromDatabase(model.MusicVolume);
        UpdateMusicLevelFromDatabase(model.MusicVolume);
        UpdateSoundDisplayFromDatabase(model.SoundVolume);
        UpdateSoundLevelFromDatabase(model.SoundVolume);
    }

    public void DefaultSettings()
    {
        UpdateMusicDisplay();
        UpdateMusicLevel();
        UpdateSoundDisplay();
        UpdateSoundLevel();
    }

    public void UpdateMusicDisplay()
    {
        musicLabel.text = musicSlider.value + "%";
        musicSlider.value = musicSlider.value;
    }

    public void UpdateMusicDisplayFromDatabase(int databaseVal)
    {
        musicLabel.text = databaseVal + "%";
        musicSlider.value = databaseVal;
    }

    public void UpdateMusicLevel()
    {
        int val = Mathf.RoundToInt(musicSlider.value);
        musicLabel.text = val + "%";
        audioSource.volume = musicSlider.value / 100;
        MusicVolume = val;
    }

    public void UpdateMusicLevelFromDatabase(int databaseVal)
    {
        musicLabel.text = databaseVal + "%";
        audioSource.volume = musicSlider.value / 100;
        MusicVolume = databaseVal;
    }



    public void UpdateSoundDisplay()
    {
        soundLabel.text = soundSlider.value + "%";
        soundSlider.value = soundSlider.value;
    }

    public void UpdateSoundDisplayFromDatabase(int databaseVal)
    {
        soundLabel.text = databaseVal + "%";
        soundSlider.value = databaseVal;
    }

    public void UpdateSoundLevel()
    {
        int val = Mathf.RoundToInt(soundSlider.value);
        soundLabel.text = val + "%";
        for (int i = 0; i < soundSources.Count; i++)
        {
            soundSources[i].volume = soundSlider.value / 100;
        }
        SoundVolume = val;
    }

    public void UpdateSoundLevelFromDatabase(int databaseVal)
    {
        soundLabel.text = databaseVal + "%";
        for (int i = 0; i < soundSources.Count; i++)
        {
            soundSources[i].volume = soundSlider.value / 100;
        }
        SoundVolume = databaseVal;
    }
}
