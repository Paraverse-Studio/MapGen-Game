using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobCollisionAvoidance : MonoBehaviour
{
    private MobController _controller;
    private Transform _body;

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponentInParent<MobController>();
        if (_controller) _body = _controller.Body;
    }


    private void OnTriggerStay(Collider other)
    {
        if (Time.frameCount % 2 != 0 || !_controller) return;

        if (other.gameObject.layer != (int)LayerEnum.MobCollision) return;

        Vector3 directionToMob = other.gameObject.transform.position - _body.position;

        _controller.ChangeDirection = (-directionToMob).normalized * 1.4f;




    }


}
