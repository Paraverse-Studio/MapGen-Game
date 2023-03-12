using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundTimer : MonoBehaviour
{
    public TextMeshProUGUI text;
    float timer;
    float originalSize;
    bool paused = false;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        text = GetComponentInChildren<TextMeshProUGUI>();
        originalSize = text.fontSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (!paused) timer += Time.deltaTime;

        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer - minutes * 60f);
        text.text = "Time " + UtilityFunctions.GetFormattedTime(timer);

        if (text.fontSize > originalSize)
        {
            text.fontSize -= (Time.deltaTime*70f);
            if (text.fontSize < originalSize) text.fontSize = originalSize;
        }
    }

    public float GetTime() => timer;

    public void PauseTimer()
    {
        paused = true;
        text.fontSize = originalSize * 1.25f;
    }

    public void RestartTimer()
    {
        paused = false;
        timer = 0;
        text.fontSize = (originalSize * 2f);
    }

    public void HideTimer()
    {
        // implement
    }

}
