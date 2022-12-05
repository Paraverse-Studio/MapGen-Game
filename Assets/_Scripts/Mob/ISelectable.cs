using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface ISelectable
{

    public bool IsSelected { get; }

    public void Select();

    public void Deselect();

    public UnityEvent OnSelected { get; }

    public UnityEvent OnDeslected { get; }

}
