using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QualityManager : MonoBehaviour, ITickElement
{
    public static int QualityLevel;
    public static QualityManager Instance;

    [Range(1, 5), SerializeField]
    private int _qualityLevel = 5;

    [SerializeField]
    private TickDelayOption _checkDistanceDelay;

    private void Awake()
    {
        Instance = this;
        SetQualityLevel(_qualityLevel);
    }

    // Start is called before the first frame update
    void Start()
    {
        //TickManager.Instance?.Subscribe(this, gameObject, _checkDistanceDelay);
        //DetectQualitySetting();

#if UNITY_ANDROID
        SetQualityLevel(1);
        Application.targetFrameRate = 60;
#endif
    }

    public void SetQualityLevel(int level)
    {
        _qualityLevel = Mathf.Clamp(level, 1, 5);
        QualityLevel = _qualityLevel;
    }

    public void DetectQualitySetting()
    {
        StartCoroutine(IQualitySettingsCheck());
    }

    private IEnumerator IQualitySettingsCheck()
    {
        yield return new WaitForSecondsRealtime(1f);
        Debug.Log("Quality Manager: Detecting game performance...");
        float sum = 0;
        int sampleCount = 10;
        string sampleMessage = "Samples: ";
        for (int i = 0; i < sampleCount; ++i)
        {
            sum += FPSCounter.FPS;
            sampleMessage += (int)FPSCounter.FPS +", ";
            yield return new WaitForSecondsRealtime(0.2f);
        }
        float averageFPS = sum / sampleCount;
        if (averageFPS >= 60) SetQualityLevel(5);
        else if (averageFPS >= 30) SetQualityLevel(3);
        else SetQualityLevel(1);
        Debug.Log("(" + sampleMessage+")");
        Debug.Log($"Quality Manager: Average FPS: {averageFPS} ({sum} over {sampleCount} samples)  =>  Quality applied: {QualityLevel}!");

        
    }

    public void Tick()
    {
        //DetectQualitySetting();
    }
}
