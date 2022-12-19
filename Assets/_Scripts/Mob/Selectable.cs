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
    public float Range;

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

        Range = 5f;
        switch (type)
        {
            case SelectableType.hostile:
                Range = 8f;
                break;
            case SelectableType.interactive:
                Range = 4f;
                break;
            case SelectableType.informational:
                Range = 4f;
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
        OnSelected?.Invoke();
    }

    public void Deselect()
    {
        if (!_isSelected) return;

        _isSelected = false;
        OnDeselected?.Invoke();
    }

}
