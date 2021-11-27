using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPointTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Huh?1");
        if (other.CompareTag("Player"))
        {
            Debug.Log("HUH");
            GameLoopManager.Instance.CompleteLevel();
        }
    }
}
