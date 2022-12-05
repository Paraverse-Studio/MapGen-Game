using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Selectable : MonoBehaviour
{
    private bool _isSelected;

    public bool IsSelected
    {
        get { return _isSelected; }
    }

    public UnityEvent OnSelected = new UnityEvent();

    public UnityEvent OnDeselected = new UnityEvent();

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
