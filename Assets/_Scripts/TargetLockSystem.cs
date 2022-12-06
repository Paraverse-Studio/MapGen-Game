using UnityEngine;
using System.Collections;
using Paraverse.Mob.Controller;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

public class TargetLockSystem : MonoBehaviour
{
    public static TargetLockSystem Instance;

    [Header("References")]
    public GameObject source;
    public Selectable Target;

    [Header("Target Outline")]
    public float outlineSize;
    public Color hostileOutlineColor;
    public Color neutralOutlineColor;

    [Header("Events")]
    public SelectableEvent OnTargetLocked = new SelectableEvent();
    public SelectableEvent OnTargetUnlocked = new SelectableEvent();

    private bool _continuousTargetting = false;

    private List<Selectable> _selectables = new List<Selectable>();

    public List<Selectable> Selectables
    {
        get { return _selectables; }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (_continuousTargetting && null == Target) SelectTarget();
    }

    public void Add(Selectable selectable)
    {
        if (!_selectables.Contains(selectable))
        {
            _selectables.Add(selectable);

            selectable.outline.OutlineMode = Outline.Mode.OutlineAll;
            selectable.outline.OutlineColor = hostileOutlineColor;
            selectable.outline.OutlineWidth = outlineSize;

            StartCoroutine(DoAfterDelay(0.2f, () => selectable.outline.enabled = false));
        }
    }

    public void Remove(Selectable selectable)
    {
        if (_selectables.Contains(selectable))
        {
            _selectables.Remove(selectable);
        }
    }
 
    private IEnumerator DoAfterDelay(float f, Action action)
    {
        yield return new WaitForSeconds(f);
        action?.Invoke();
    }

    public void ToggleSelect()
    {
        if (Target)
        {
            _continuousTargetting = false;
            DeselectTarget();
        }
        else
        {
            _continuousTargetting = true;
            SelectTarget();
        }
    }

    public void SelectTarget()
    {
        float distance = Mathf.Infinity;
        Selectable targetSoFar = null;

        foreach (Selectable obj in _selectables)
        {            
            float dist = Vector3.Distance(source.transform.position, obj.transform.position);
            if (obj.priority == Selectable.SelectablePriority.whenIsolated)
                dist *= 2f;

            if (dist < distance)
            {
                distance = dist;
                targetSoFar = obj;
            }
        }

        Target = targetSoFar;

        Target.Select();

        if (Target != null)
        {
            OnTargetLocked?.Invoke(Target);

            Target.outline.OutlineMode = Outline.Mode.OutlineAll;
            Target.outline.OutlineColor = hostileOutlineColor;
            if (Target.type != Selectable.SelectableType.hostile)
            {
                Target.outline.OutlineColor = neutralOutlineColor;
            }
            Target.outline.OutlineWidth = outlineSize;
            Target.outline.enabled = true;
        }
    }

    public void DeselectTarget()
    {
        if (!Target) return;

        Target.Deselect();

        Target.outline.enabled = false;

        OnTargetUnlocked?.Invoke(Target);

        Target = null;
    }

}