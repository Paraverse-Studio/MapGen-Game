using UnityEngine;
using System.Collections;

public class Billboard : MonoBehaviour
{
    [Header("Camera to look at")]
    private Camera cameraObject;

    private void Start()
    {
        cameraObject = Camera.main;
    }

    void Update()
    {
        transform.LookAt(transform.position + cameraObject.transform.rotation * Vector3.forward,
              cameraObject.transform.rotation * Vector3.up);
        Vector3 eulerAngles = transform.eulerAngles;
        eulerAngles.z = 0;
        transform.eulerAngles = eulerAngles;
    }
}
