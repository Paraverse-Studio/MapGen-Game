using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Selectable : MonoBehaviour
{
    [System.Serializable]
    public enum SelectablePriority
    {
        aboveAllOthers,
        whenIsolated
    }

    public enum SelectableType
    {
        hostile,
        interactive,
        informational
    }

    [Header("Selectable Settings")]
    public SelectablePriority priority;
    public SelectableType type;
    public float rangeOverride = -1;

    [HideInInspector]
    public Outline outline;
    [HideInInspector]
    public float range;

    private bool _isSelected;

    public bool IsSelected
    {
        get { return _isSelected; }
    }

    [Header("Events")]
    public UnityEvent OnSelected = new UnityEvent();

    public UnityEvent OnDeselected = new UnityEvent();

    private void Awake()
    {
        outline = GetComponent<Outline>();
        if (!outline) outline = gameObject.AddComponent<Outline>();

        range = 5f;
        switch (type)
        {
            case SelectableType.hostile:
                range = 8f;
                break;
            case SelectableType.interactive:
                range = 4f;
                break;
            case SelectableType.informational:
                range = 4f;
                break;
        }
        if (rangeOverride > 0) range = rangeOverride;
    }

    private void Start()
    {
        if (TargetLockSystem.Instance) TargetLockSystem.Instance.Add(this);
    }

    private void OnEnable()
    {
        if (TargetLockSystem.Instance) TargetLockSystem.Instance.Add(this);
    }

    private void OnDisable()
    {
        if (TargetLockSystem.Instance) TargetLockSystem.Instance.Remove(this);
    }

    private void OnDestroy()
    {
        if (TargetLockSystem.Instance) TargetLockSystem.Instance.Remove(this);
    }

    public void Select()
    {
        if (_isSelected) return;

        _isSelected = true;
        OnSelected?.Invoke();
    }

    public void Deselect()
    {
        if (!_isSelected) return;

        _isSelected = false;
        OnDeselected?.Invoke();
    }

}
