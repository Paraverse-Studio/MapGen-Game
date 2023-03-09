using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModsListMenuController : MonoBehaviour
{
    [SerializeField]
    private ItemDisplayCreator _displayCreator;


    private void OnEnable()
    {
        _displayCreator.Display(ModsManager.Instance.PurchasedMods, null);
    }

    public void UpdateDisplay()
    {
        _displayCreator.Display(ModsManager.Instance.PurchasedMods, null);
    }

}
