using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BloodlinesController : MonoBehaviour
{
    public enum BloodlineType
    {
        Vagabond = 0, Harrier = 1, Pioneer = 2, Scholar
    }

    public BloodlineType chosenBloodline;
    public TextMeshProUGUI playAsText;
    public string playAsPhrase;
    public ModCard[] bloodlineCards;



    public void ChooseBloodline(int type)
    {
        chosenBloodline = (BloodlineType)type;
        playAsText.text = playAsPhrase.Replace("[BLOODLINE]", chosenBloodline.ToString());
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DeselectAll()
    {
        foreach (ModCard card in bloodlineCards)
        {
            card.ToggleSelect(false);
        }
    }
}
