using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour
{
    public Vector3 offset;
    public Vector3 direction;
    public float speed;
    public float amplitude;

    private void Start()
    {
        offset += transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        float newX = offset.x + (direction.x * amplitude * Mathf.Sin(Time.time * speed));
        float newY = offset.y + (direction.y * amplitude * Mathf.Sin(Time.time * speed));
        float newZ = offset.z + (direction.z * amplitude * Mathf.Sin(Time.time * speed));

        transform.localPosition = new Vector3(newX, newY, newZ);
    }
}
