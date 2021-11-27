using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressBar : MonoBehaviour
{
    [Header("References: ")]
    public Image bar;
    public Image wholeBar;
    public TextMeshProUGUI[] texts;

    [Header("Lerp speed (smoothStep):")]
    [SerializeField]
    private float _lerpSpeed;

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
        bar.fillAmount = Mathf.SmoothDamp(bar.fillAmount, _progress / _total, ref _velocity, _lerpSpeed);
        
        texts[0].text = !string.IsNullOrEmpty(_specificText) ? _specificText : "Loading...";

        float val = (_progress / _total);
        if (val != 0)
        {
            texts[1].text = (int)(Mathf.Clamp((val * 100.0f), 0.0f, 100.0f)) + "%";
        }
        else
        {
            texts[1].text = "";
        }

    }

    public void OnProgressStartBar()
    {
        wholeBar.gameObject.SetActive(true);
        ResetUI();
    }

    private void ResetUI()
    {
        _progress = 0f;
        _specificText = "";
        _total = 0.0001f;
        bar.fillAmount = 0f;
        texts[0].text = "";
        texts[1].text = "";
    }

    public void OnProgressEndBar()
    {
        wholeBar.gameObject.SetActive(false);
        ResetUI();
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
