using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPointTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameLoopManager.Instance.EndRound(successfulRound: true);
            this.enabled = false;

        }
    }
}
