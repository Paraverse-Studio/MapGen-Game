using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressBar : MonoBehaviour
{
    public Image bar;
    public Image wholeBar;
    public TextMeshProUGUI[] texts;

    private float _total;
    private float _progress;
    private float _velocity;

    private string _specificText = "";

    // Start is called before the first frame update
    void Start()
    {
         _progress = 0f;
        _total = 0.001f;
        bar.fillAmount = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        bar.fillAmount = Mathf.SmoothDamp(bar.fillAmount, _progress / _total, ref _velocity, 0.2f);

        if (Time.frameCount % 2 == 0)
        {
            if (_progress != _total && wholeBar.gameObject.activeSelf == false) wholeBar.gameObject.SetActive(true);                    
        }

        if (texts.Length > 1)
        {
            texts[0].text = !string.IsNullOrEmpty(_specificText) ? _specificText : "Loading . . .";
            texts[1].text = ((int)((_progress / _total) * 100.0f)) + "%";

            if (_progress == _total && bar.fillAmount >= 0.99f)
            {
                texts[0].text = "";
                texts[1].text = "";
                _specificText = "";
                wholeBar.gameObject.SetActive(false);

            }
        }


    }


    public void OnProgressSetText(string progressText)
    {
        _specificText = progressText;
    }

    public void OnProgressIncrementText(string progressText)
    {
        _specificText += progressText + (string.IsNullOrEmpty(progressText) ? "" : "\n");
    }

    public void OnProgressChange(float progress, float total)
    {
        _progress = progress; _total = total;
    }

}
