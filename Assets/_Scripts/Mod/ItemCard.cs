using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemCard : MonoBehaviour
{
    [Header("Item")]
    public SO_Item Item;
    public TextMeshProUGUI titleLabel;
    public Image imageHolder;
    public TextMeshProUGUI descriptionLabel;
    public TextMeshProUGUI loreLabel;
    public TextMeshProUGUI costLabel;
    public GameObject costHolder;
    public TextMeshProUGUI typeLabel;
    public GameObject typeHolder;
    public TextMeshProUGUI quantityLabel;

    public ItemCardEvent OnClickCard = new();

    public virtual void UpdateDisplay(System.Action clickCallBack = null, int modLevel = -1)
    {
    }

    public virtual void Lock(bool onOrOff)
    {
        if (costLabel)
        {
            costLabel.color = onOrOff? Color.red : Color.white;
        }
    }

    public void OnClickCardEvent()
    {
        OnClickCard?.Invoke(this);
    }

}
