using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [Header("Target & following")]
    public Transform target;
    public bool usePlayerAsTarget;
    public bool useStartingOffset;
    public Vector3 offset;

    [Header("SmoothStep lerp: ")]
    public float smoothStepLerp = 0;

    [Header("Snap to long distance?")]
    public bool snapToFarDistance = false;

    [Header("Lerp on y-value?")]
    public bool lerpY;
    public bool dontFollowY;
    public float lerpValue;

    [Header("Follow Rotation")]
    public bool followRotation;

    private Vector3 velocity;
    

    // Start is called before the first frame update
    void Start()
    {
        if (usePlayerAsTarget)
        {
            target = GlobalSettings.Instance.player.transform;
        }

        if (!target) return;

        if (useStartingOffset) offset = transform.position - target.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!target) 
        {
            Destroy(gameObject);
            return;
        }

        Vector3 goalPosition = offset + target.position;
        Vector3 goalPositionOriginal = goalPosition;

        if (lerpY) goalPosition.y = Mathf.Lerp(transform.position.y, goalPosition.y, Time.deltaTime * lerpValue);
        if (dontFollowY) goalPosition.y = transform.position.y;

        if (smoothStepLerp > 0)
        {
            transform.position = Vector3.SmoothDamp(transform.position, goalPosition, ref velocity, smoothStepLerp);
        }
        else
        {
            transform.position = goalPosition;
        }

        if (snapToFarDistance && (transform.position - goalPositionOriginal).sqrMagnitude > (10f * 10f)) 
            transform.position = goalPositionOriginal;

        if (followRotation) transform.rotation = target.rotation;

    }


}
