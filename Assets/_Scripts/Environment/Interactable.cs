using Paraverse.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour, ITickElement
{
    public bool interactable = true;
    public float proximityRange;

    public UnityEvent OnEnterProximity = new UnityEvent();
    public UnityEvent OnExitProximity = new UnityEvent();

    public UnityEvent OnInteract = new UnityEvent();

    private bool _userWithinProximity = false;
    private PlayerInputControls _player;
    private string _interactableMessage;
    private Color _interactableColor;

    private void Awake()
    {
        TickManager.Instance.Subscribe(this, gameObject, TickDelayOption.t10);
    }

    private void Start()
    {
        _player = GlobalSettings.Instance.player.GetComponent<PlayerInputControls>();

        _player.OnInteractEvent += PressedInteract;
        _interactableColor = GlobalSettings.Instance.interactableColor;
        _interactableMessage = 
            $"Press [E] to interact with <b><color=#{ColorUtility.ToHtmlStringRGB(_interactableColor)}>{gameObject.name}</color></b>";

    }

    public void Tick()
    {
        if (UtilityFunctions.IsDistanceLessThan(_player.gameObject.transform.position, transform.position, proximityRange))
        {
            if (!_userWithinProximity)
            {
                ToggleProximityShow(true);
            }
        }
        else if (_userWithinProximity)
        {
            ToggleProximityShow(false);
        }
    }

    private void ToggleProximityShow(bool show)
    {
        _userWithinProximity = show;
        if (show) 
            OnEnterProximity?.Invoke();
        else 
            OnExitProximity?.Invoke();

        if (show)                  
            CaptionTextController.Instance.SetText(_interactableMessage, gameObject);    
        else
            CaptionTextController.Instance.RemoveText(_interactableMessage, gameObject);
    }

    public void PressedInteract()
    {
        if (_userWithinProximity) Interact();
    }

    public void Interact()
    {
        OnInteract?.Invoke();
    }

    private void OnDestroy()
    {
        ToggleProximityShow(false);
        _player.OnInteractEvent -= PressedInteract;
        TickManager.Instance.Unsubscribe(this);
    }

    
}
