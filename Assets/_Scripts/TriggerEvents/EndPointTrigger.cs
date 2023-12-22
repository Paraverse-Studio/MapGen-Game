using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPointTrigger : MonoBehaviour
{
    public GameObject portal;
    public Action OnInteractAction = null;

    private bool _activated = false; public bool IsActivated => _activated;

    private void Start()
    {
        portal.SetActive(false);
    }

    public void Activate(bool o)
    {
        portal.SetActive(o);
        _activated = true;
    }

    public void Interact()
    {
        if (!_activated)
        {
            AnnouncementManager.Instance.QueueAnnouncementUnique(new Announcement().AddText("Complete the objective to open the gate!"));
        }
        else
        {
            OnInteractAction?.Invoke();
            _activated = false;
        }
    }
}
