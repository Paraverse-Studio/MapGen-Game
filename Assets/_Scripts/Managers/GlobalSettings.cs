using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum LayerEnum {
    def = 0,
    TransparentFX = 1,
    IgnoreRaycast = 2,
    Water = 4,
    UI = 5, 
    Ground = 6,
    Wall = 7,
    Solid = 8,
    Breakable = 9,
    MobNormal = 10,
    MobCollision = 11
}


public class GlobalSettings : MonoBehaviour
{
    public static GlobalSettings Instance;
    private void Awake() => Instance = this;

    [Header("Quality Level:")]
    [Range(1, 5)]
    public int QualityLevel;

    [Header("Folders for objects ")]
    public Canvas ScreenSpaceCanvas;
    public Transform uiFolder;
    public Transform healthBarFolder;

    public Transform waterVolume;

    public bool recordBlockHistory = true;
    public GameObject playerPrefab;
    public GameObject player;

    [Header("Combat")]
    public GameObject healthBarPrefab;
    public Material FlashMaterial;
    public GameObject popupTextPrefab;
    public Color damageColour;
    public Color healColour;

    [Header("Backup Safe Position: ")]
    [Space(10)]
    public Vector3 backupSafePosition;
    public GameObject testGameObject;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (GlobalSettings.Instance.testGameObject)
                Instantiate(GlobalSettings.Instance.testGameObject, player.transform.position + new Vector3(0, 0.5f, 0), player.transform.rotation);
        }
    }

    public void DetectQualitySetting()
    {
        StartCoroutine(IQualitySettingsCheck());
    }

    private IEnumerator IQualitySettingsCheck()
    {
        yield return new WaitForSecondsRealtime(1f);
        Debug.Log("Global Settings: Detecting game performance...");
        float sum = 0;
        int sampleCount = 30;
        for (int i = 0; i < sampleCount; ++i)
        {
            sum += FPSCounter.FPS;
            Debug.Log("Sample: " + FPSCounter.FPS);
            yield return new WaitForSecondsRealtime(0.1f);
        }
        float averageFPS = sum / sampleCount;
        if (averageFPS >= 60) QualityLevel = 5;
        else if (averageFPS >= 30) QualityLevel = 3;
        else QualityLevel = 1;
        Debug.Log($"Global Settings: Average FPS: {averageFPS} ({sum} over {sampleCount} samples)  =>  Quality applied: {QualityLevel}!");
    }

}
