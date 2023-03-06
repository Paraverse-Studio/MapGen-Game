using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPointTrigger : MonoBehaviour
{
    public GameObject portal;

    private bool _activated = false;

    private void Start()
    {
        portal.SetActive(false);
    }

    public void Activate(bool o)
    {
        if (!_activated)
        {
            AnnouncementManager.Instance.QueueAnnouncement(new Announcement().AddType(1).AddText("All enemies are defeated — gate is open!"));
        }
        portal.SetActive(o);
        _activated = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_activated)
        {
            if (other.CompareTag("Player")) 
                AnnouncementManager.Instance.QueueAnnouncementUnique(new Announcement().AddText("Defeat all enemies to open the gate!"));
            return;
        }

        if (other.CompareTag("Player"))
        {
            GameLoopManager.Instance.EndRound(successfulRound: true);
            _activated = false;
        }
    }
}
