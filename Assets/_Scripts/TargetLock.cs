using UnityEngine;
using System.Collections;
using Paraverse.Mob.Controller;

public class TargetLock : MonoBehaviour
{
    [Header("References")]
    public GameObject source;
    public Transform targettingIcon;
    public GameObject Target;

    [Header("Target Outline")]
    public float outlineSize;
    public Color outlineColor;

    void Update()
    {
        if (Target)
        {
            targettingIcon.gameObject.SetActive(true);
            targettingIcon.position = Camera.main.WorldToScreenPoint(Target.transform.position + new Vector3(0, 0.5f, 0));
        }
        else
        {
            targettingIcon.gameObject.SetActive(false);
        }


        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!Target) SelectTarget();
        }
        else
        {
            DeselectTarget();
        }

    }

    public void SelectTarget()
    {
        MobController[] mobs = FindObjectsOfType<MobController>();
        float distance = Mathf.Infinity;
        GameObject targetSoFar = null;

        foreach (MobController mob in mobs)
        {
            float dist = Vector3.Distance(source.transform.position, mob.transform.position);
            if (dist < distance)
            {
                distance = dist;
                targetSoFar = mob.gameObject;
            }
        }

        Target = targetSoFar;

        if (Target != null)
        {
            Outline outline = Target.GetComponent<Outline>();

            if (!outline) outline = Target.AddComponent<Outline>();
            
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineColor = outlineColor;
            outline.OutlineWidth = outlineSize;
            outline.enabled = true;
        }
    }

    public void DeselectTarget()
    {
        if (Target)
        {
            Outline outline;
            if (Target.TryGetComponent(out outline))
            {
                outline.enabled = false;
            }
        }
        Target = null;
    }

}