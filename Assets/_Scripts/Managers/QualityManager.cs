using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QualityManager : MonoBehaviour
{
    public static int QualityLevel;

    [Range(1,5)]
    public int qualityLevel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetQualityLevel(int level)
    {
        qualityLevel = Mathf.Clamp(level, 1, 5);
        QualityLevel = qualityLevel;
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
