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

    public struct Events
    {
        public UnityEvent OnSelected;
        public UnityEvent OnDeselected;
    }

    [Header("Selectable Settings")]
    public SelectablePriority priority;
    public SelectableType type;
    public float rangeOverride = -1;

    [HideInInspector]
    public Outline outline;
    [HideInInspector]
    public float Range;

    private bool _isSelected;

    public bool IsSelected
    {
        get { return _isSelected; }
    }

    [Header("Events")]
    public Events events;

    private void Awake()
    {
        events.OnSelected = new();
        events.OnDeselected = new();
        outline = GetComponent<Outline>();
        if (!outline) outline = gameObject.AddComponent<Outline>();
        outline.enabled = false;

        Range = 5f;
        switch (type)
        {
            case SelectableType.hostile:
                Range = 9f;
                break;
            case SelectableType.interactive:
                Range = 5f;
                break;
            case SelectableType.informational:
                Range = 5f;
                break;
        }
        if (rangeOverride > 0) Range = rangeOverride;
    }

    private void Start()
    {
        if (SelectableSystem.Instance) SelectableSystem.Instance.Add(this);
    }

    private void OnEnable()
    {
        if (SelectableSystem.Instance) SelectableSystem.Instance.Add(this);
    }

    private void OnDisable()
    {
        if (SelectableSystem.Instance) SelectableSystem.Instance.Remove(this);
    }

    private void OnDestroy()
    {
        if (SelectableSystem.Instance) SelectableSystem.Instance.Remove(this);        
    }

    public void Select()
    {
        if (_isSelected) return;

        _isSelected = true;
        events.OnSelected?.Invoke();
    }

    public void Deselect()
    {
        if (!_isSelected) return;

        _isSelected = false;
        events.OnDeselected?.Invoke();
    }

}
