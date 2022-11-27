using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ContextMessageHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public string[] messageOptions;

    // Start is called before the first frame update
    void Awake()
    {
        if (!text) text = GetComponent<TextMeshProUGUI>();
    }

    public void DisplayMessage(int i = 0)
    {
        text.text = i < 0 ? "" : messageOptions[i];
    }


}
