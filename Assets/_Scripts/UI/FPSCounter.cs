using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class FPSCounter : MonoBehaviour
{
    public static float FPS;

    public TextMeshProUGUI text;
    private float fps;
    private float timer = 0;


    // Update is called once per frame
    void Update()
    {
        fps = (int)(1f / Time.unscaledDeltaTime);
        FPS = fps;
        timer += Time.deltaTime;

        if (timer > 0.5f)
        {
            timer = 0f;
            text.text = $"{fps} FPS";            
        }
    }
}