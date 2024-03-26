using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ContextMessageHandler : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private int defaultIndex = -1;

    public List<string> MessageOptions;

    // Start is called before the first frame update
    void Awake()
    {
        if (!text) text = GetComponent<TextMeshProUGUI>();
        DisplayMessage(defaultIndex);
    }

    //  0 - 9  for all the option messages
    //   -1    for clearing the message
    public void DisplayMessage(int i = 0)
    {
        if (null == MessageOptions || MessageOptions.Count < 1)
        {
            text.text = "";
            Debug.Log("ContextMessageHandler: Error! ContextMessageHandler doesn't have proper Message Options.");
            return;
        }

        text.text = i < 0 ? "" : MessageOptions[i];
    }


}
