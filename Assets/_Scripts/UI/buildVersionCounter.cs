using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class buildVersionCounter : MonoBehaviour
{
    public TextMeshProUGUI text;
    public string prefixText;

    private void Start()
    {
        text.text = prefixText + Application.version;
    }
}
