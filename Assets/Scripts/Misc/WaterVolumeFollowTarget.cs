using UnityEngine;
using Bitgem.VFX.StylisedWater;

public class WaterVolumeFollowTarget : MonoBehaviour
{
    public Transform target;
    public Vector3 _offset;

    [Header("Time Delay (if any): ")]
    public float timeDelay = 0;
    private float timer = 0;

    [Header("Distance delay (player delta): ")]
    public float distanceDelay = 0;
    private Vector3 playerLastDeltaPosition;

    private Vector3 lastPosition;
    private WaterVolumeTransforms waterVolume;

    // Start is called before the first frame update
    void Awake()
    {
        waterVolume = GetComponent<WaterVolumeTransforms>();

        _offset = transform.position - target.position;
        lastPosition = GetIntVector(transform.position, -0.7f);
        playerLastDeltaPosition = target.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Method 1 of delay - time passed
        if (timeDelay > 0)
        {
            timer += Time.deltaTime;
            if (timer < timeDelay) return;
            timer = 0f;
        }

        // Method 2 of delay - distance difference
        if (distanceDelay > 0)
        {
            float dist = (target.position - playerLastDeltaPosition).sqrMagnitude;
            if (dist < (distanceDelay * distanceDelay)) return;
            playerLastDeltaPosition = target.position;
        }

        Vector3 goalPosition = _offset + target.position;

        lastPosition = transform.position;

        transform.position = GetIntVector(goalPosition, -0.7f);

        MoveChildrenOppositeDirection();

    }

    private Vector3 GetIntVector(Vector3 v, float overrideY = 0)
    {
        int x = (int)v.x;
        float y = overrideY;
        int z = (int)v.z;
        return new Vector3(x, y, z);
    }

    private void MoveChildrenOppositeDirection()
    {
        Vector3 oppositeDirection = lastPosition - transform.position;
        foreach (Transform child in transform)
        {
            child.transform.position += oppositeDirection;
        }

        UpdateWater();
    }

    public void UpdateWater()
    {
        waterVolume.Rebuild();
        waterVolume.Validate();
    }

}
