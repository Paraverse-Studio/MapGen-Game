using UnityEngine;
using System.Collections;
using Paraverse.Mob.Controller;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

public class TargetLock : MonoBehaviour
{
    public static TargetLock Instance;

    [Header("References")]
    public GameObject source;
    public MobController Target;

    [Header("Target Outline")]
    public float outlineSize;
    public Color outlineColor;

    [Header("Events")]
    public MobControllerEvent OnTargetLocked = new MobControllerEvent();
    public MobControllerEvent OnTargetUnlocked = new MobControllerEvent();

    private List<MobController> selectableMobs;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!Target) SelectTarget();
        }
        else
        {
            DeselectTarget();
        }

    }

    public void UpdateSelectableMobsList(List <MobController> list)
    {
        selectableMobs = list;

        foreach (MobController mob in selectableMobs)
        {
            Outline outline = mob.gameObject.GetComponent<Outline>();
            if (!outline) outline = mob.gameObject.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = outlineColor;
            outline.OutlineWidth = outlineSize;

            StartCoroutine(DoAfterDelay(0.2f, () => outline.enabled = false));
        }        
    }

    private IEnumerator DoAfterDelay(float f, Action action)
    {
        yield return new WaitForSeconds(f);
        action?.Invoke();
    }

    public void SelectTarget()
    {
        float distance = Mathf.Infinity;
        MobController targetSoFar = null;

        foreach (MobController mob in selectableMobs)
        {
            float dist = Vector3.Distance(source.transform.position, mob.transform.position);
            if (dist < distance)
            {
                distance = dist;
                targetSoFar = mob;
            }
        }

        Target = targetSoFar;

        if (Target != null)
        {
            OnTargetLocked?.Invoke(Target);

            Outline outline = Target.GetComponent<Outline>();
            
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = outlineColor;
            outline.OutlineWidth = outlineSize;
            outline.enabled = true;
        }
    }

    public void DeselectTarget()
    {
        if (!Target) return;
        
        Outline outline;
        if (Target.TryGetComponent(out outline))
        {
            outline.enabled = false;
        }        

        OnTargetUnlocked?.Invoke(Target);

        Target = null;
    }

}