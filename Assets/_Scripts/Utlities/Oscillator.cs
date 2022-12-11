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
        transform.localPosition = 
            new Vector3(offset.x + (direction.x * amplitude * Mathf.Sin(Time.time * speed)), 
                        offset.y + (direction.y * amplitude * Mathf.Sin(Time.time * speed)),
                        offset.z + (direction.z * amplitude * Mathf.Sin(Time.time * speed)));
    }
}
