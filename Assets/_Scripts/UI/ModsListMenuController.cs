using Paraverse.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModsListMenuController : MonoBehaviour
{
    [SerializeField]
    private ItemDisplayCreator _displayCreator;

    private PlayerInputControls _playerInputs;

    private void Start()
    {
        _playerInputs = GlobalSettings.Instance.player.GetComponent<PlayerInputControls>();

        _playerInputs.OnEscapeEvent += PressedEscape;
    }


    public void UpdateDisplay()
    {
        _displayCreator.Display(ModsManager.Instance.PurchasedMods, null);
    }

    public void PressedEscape()
    {
        _displayCreator.gameObject.SetActive(false);
    }

}
