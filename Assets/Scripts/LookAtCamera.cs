using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera _camera;

    private Plane _plane;
    private Vector3 _closestPoint;
    private bool _positiveOfPlane;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;

        Invoke("UpdateMeasurementsTowardsCamera", 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(_closestPoint - transform.position);
    }


    private void UpdateMeasurementsTowardsCamera()
    {        
        _plane.normal = _camera.transform.forward;
        _plane = Plane.Translate(_plane, new Vector3(0, 100000f, 0));
        _closestPoint = _plane.ClosestPointOnPlane(transform.position);        
    }


}
