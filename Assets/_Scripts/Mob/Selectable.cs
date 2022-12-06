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
    public float range;

    [HideInInspector]
    public Outline outline;

    private bool _isSelected;

    public bool IsSelected
    {
        get { return _isSelected; }
    }

    public UnityEvent OnSelected = new UnityEvent();

    public UnityEvent OnDeselected = new UnityEvent();

    private void Awake()
    {
        outline = GetComponent<Outline>();
        if (!outline) outline = gameObject.AddComponent<Outline>();
    }

    private void Start()
    {
        TargetLockSystem.Instance.Add(this);
    }

    private void OnEnable()
    {
        TargetLockSystem.Instance.Add(this);
    }

    private void OnDisable()
    {
        TargetLockSystem.Instance.Remove(this);
    }

    private void OnDestroy()
    {
        TargetLockSystem.Instance.Remove(this);
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
