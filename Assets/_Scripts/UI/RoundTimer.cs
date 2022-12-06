using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundTimer : MonoBehaviour
{
    TextMeshProUGUI text;
    float timer;
    float originalSize;

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
        timer += Time.deltaTime;

        int minutes = Mathf.FloorToInt(timer / 60f);
        int seconds = Mathf.FloorToInt(timer - minutes * 60f);
        text.text = "Time " + string.Format("{0:0}:{1:00}", minutes, seconds);

        if (text.fontSize > originalSize)
        {
            text.fontSize -= (Time.deltaTime*70f);
            if (text.fontSize < originalSize) text.fontSize = originalSize;
        }
    }

    public void RestartTimer()
    {
        timer = 0;
        text.fontSize = (originalSize * 2f);
    }

    public void HideTimer()
    {
        // implement
    }

}
