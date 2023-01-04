using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleOscillator : MonoBehaviour
{
    public Vector3 offset;
    public Vector3 direction;
    public float speed;
    public float amplitude;


    public ParticleSystem ps;

    private void Start()
    {
        offset += transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (ps)
        {
            float newX = offset.x + (direction.x * amplitude * Mathf.Sin(Time.time * speed));
            float newY = offset.y + (direction.y * amplitude * Mathf.Sin(Time.time * speed));
            float newZ = offset.z + (direction.z * amplitude * Mathf.Sin(Time.time * speed));
            
            ParticleSystem.VelocityOverLifetimeModule mod = ps.velocityOverLifetime;
            mod.y = new ParticleSystem.MinMaxCurve(newY-2, newY);


            
        }
    }
}
