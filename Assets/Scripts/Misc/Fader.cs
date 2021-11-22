using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour
{
    public CanvasGroup cg;
    public float currentSpeed = 1f;

    private bool _fadeIn = false;
    public bool FadeIn
    {
        get { return _fadeIn; }
        set { _fadeIn = value; }
    }

    private bool _fadeOut = false;
    public bool FadeOut
    {
        get { return _fadeOut; }
        set { _fadeOut = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetSpeed(float v) => currentSpeed = v;

    // Update is called once per frame
    void Update()
    {
        if (_fadeIn)
        {
            cg.alpha += Time.deltaTime * currentSpeed;
        }
        else if (_fadeOut)
        {
            cg.alpha -= Time.deltaTime * currentSpeed;
        }
    }
}
