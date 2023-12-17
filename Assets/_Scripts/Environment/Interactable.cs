using Paraverse.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

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


    private string GetInteractPhrase(InputAction action)
    {
        var lastCompositeIndex = -1;
        var isFirstControl = true;

        var controls = "";
        foreach (var control in action.controls)
        {
            var bindingIndex = action.GetBindingIndexForControl(control);
            var binding = action.bindings[bindingIndex];
            if (binding.isPartOfComposite)
            {
                if (lastCompositeIndex != -1)
                    continue;
                lastCompositeIndex = action.ChangeBinding(bindingIndex).PreviousCompositeBinding().bindingIndex;
                bindingIndex = lastCompositeIndex;
            }
            else
            {
                lastCompositeIndex = -1;
            }
            if (!isFirstControl)
                controls += " or ";

            controls += action.GetBindingDisplayString(bindingIndex);
            isFirstControl = false;
        }
        //return controls;

        var interactKey = "E";

        var interactPhrase = $"Press [<b>{interactKey}</b>]";

#if UNITY_ANDROID
        interactPhrase = "Tap ";
#endif

        return interactPhrase;
    }

    private void Start()
    {
        _player = GlobalSettings.Instance.player.GetComponent<PlayerInputControls>();

        _player.OnInteractEvent += PressedInteract;
        _interactableColor = GlobalSettings.Instance.interactableColor;

        var interactPhrase = GetInteractPhrase(_player.Input.Player.Interact);

        _interactableMessage = 
        $"{interactPhrase} to interact with <b><color=#{ColorUtility.ToHtmlStringRGB(_interactableColor)}>{gameObject.name}</color></b>";
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
