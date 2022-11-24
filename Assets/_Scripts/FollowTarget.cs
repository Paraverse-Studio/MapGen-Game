using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [Header("SmoothStep lerp: ")]
    public float smoothStepLerp = 0;
    public Transform target;
    public Vector3 _offset;

    [Header("Snap to long distance?")]
    public bool snapToFarDistance = false;

    private Vector3 velocity;
    

    // Start is called before the first frame update
    void Awake()
    {
        if (!target) return;

        _offset = transform.position - target.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!target) 
        {
            Destroy(gameObject);
            return;
        }

        Vector3 goalPosition = _offset + target.position;

        if (smoothStepLerp > 0)
        {
            transform.position = Vector3.SmoothDamp(transform.position, goalPosition, ref velocity, smoothStepLerp);
        }
        else
        {
            transform.position = goalPosition;
        }

        if (snapToFarDistance && Vector3.Distance(transform.position, goalPosition) > 10f) 
            transform.position = goalPosition;

    }


}
