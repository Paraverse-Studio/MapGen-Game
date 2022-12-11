using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 vector;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(Time.deltaTime * vector.x, Time.deltaTime * vector.y, Time.deltaTime * vector.z));
    }
}