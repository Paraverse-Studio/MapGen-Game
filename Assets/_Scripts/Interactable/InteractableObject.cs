using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [SerializeField]
    private GameObject objectModel;

    [SerializeField]
    private InteractableObjects thisInteractable;

    private Interactable _interactable;
    private Selectable _selectable;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        // Add Selectable for making this object display an outline and its name
        _selectable = objectModel.AddComponent<Selectable>();
        _selectable.priority = Selectable.SelectablePriority.whenIsolated;
        _selectable.type = Selectable.SelectableType.informational;

        // Add an Interactable for making this object be interacted with user's Interact press and from a distance
        _interactable = objectModel.AddComponent<Interactable>();
        _interactable.interactable = true;
        _interactable.proximityRange = 3f;
        _interactable.OnInteract.AddListener(InteractWithObject);
    }

    public void InteractWithObject()
    {
        InteractableObjectsManager.Instance.windows.Find(x => x.type == thisInteractable).obj.SetActive(true);
    }

}
