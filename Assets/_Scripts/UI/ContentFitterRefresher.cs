using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContentFitterRefresher : MonoBehaviour
{
    private void Awake()
    {
        RefreshContentFitters();
    }

    private void Start()
    {
        RefreshContentFitters();
    }

    public void RefreshContentFitters()
    {
        var rectTransform = (RectTransform)transform;
        if (rectTransform) RefreshContentFitter(rectTransform);
    }

    private void RefreshContentFitter(RectTransform transform)
    {
        if (transform == null || !transform.gameObject.activeSelf)
        {
            return;
        }

        foreach (RectTransform child in transform)
        {
            RefreshContentFitter(child);
        }

        var layoutGroup = transform.GetComponent<LayoutGroup>();
        var contentSizeFitter = transform.GetComponent<ContentSizeFitter>();
        if (layoutGroup != null)
        {
            layoutGroup.SetLayoutHorizontal();
            layoutGroup.SetLayoutVertical();
        }

        if (contentSizeFitter != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform);
        }
    }
}