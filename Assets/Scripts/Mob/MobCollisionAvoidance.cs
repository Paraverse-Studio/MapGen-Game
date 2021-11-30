using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobCollisionAvoidance : MonoBehaviour, ITickElement
{
    private MobController _controller;
    private Transform _body;

    private float force = 0.4f;

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponentInParent<MobController>();
        if (_controller) _body = _controller.Body;

        TickManager.Instance.Subscribe(this, TickDelayOption.t4);
    }

    public void Tick()
    {
        force = Mathf.Clamp(force -= 0.025f, 0f, 1.4f);
    }


    private void OnTriggerStay(Collider other)
    {
        if (Time.frameCount % 2 != 0 || !_controller) return;

        if (other.gameObject.layer != (int)LayerEnum.MobNormal) return;

        Vector3 directionToMob = other.gameObject.transform.position - _body.position;

        force += 0.05f;

        _controller.ChangeDirection = (-directionToMob).normalized * force;

    }


}
