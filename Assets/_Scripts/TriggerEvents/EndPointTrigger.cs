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
        portal.SetActive(o);
        _activated = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_activated) return;

        if (other.CompareTag("Player"))
        {
            GameLoopManager.Instance.EndRound(successfulRound: true);
            _activated = false;
        }
    }
}
