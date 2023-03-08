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
    public TextMeshProUGUI typeLabel;

    public virtual void UpdateDisplay(System.Action clickCallBack = null)
    {

    }

}
