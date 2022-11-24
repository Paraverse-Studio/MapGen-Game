using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget2D : MonoBehaviour
{
    [Header("SmoothStep lerp: ")]
    public float smoothStepLerp = 0;
    public Transform target;
    public Vector3 _offset;

    private Vector3 velocity;

    // Update is called once per frame
    void Update()
    {
        if (!target)
        {
            return;
        }

        Vector3 goalPosition = Camera.main.WorldToScreenPoint(target.position + _offset);
        goalPosition.z = 0;

        if (smoothStepLerp > 0)
        {
            transform.position = Vector3.SmoothDamp(transform.position, goalPosition, ref velocity, smoothStepLerp);
        }
        else
        {
            transform.position = goalPosition;
        }

    }

}
