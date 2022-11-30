using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobCollisionAvoidance : MonoBehaviour, ITickElement
{
    public float radius = 1.5f;
    public LayerMask layerMask;

    private OldMobController _controller;
    private MobAI _ai;
    private Transform _body;
    private float force = 0.4f;
    private CapsuleCollider _thisCapsule;

    private List<Collider> _contacts;
    private readonly int maxColliders = 10;
    Collider[] hitColliders;
    private Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        _contacts = new List<Collider>();
        _controller = GetComponentInParent<OldMobController>();
        _ai = GetComponentInParent<MobAI>();
        if (_controller) _body = _controller.Body;

        _thisCapsule = GetComponent<CapsuleCollider>();
        _thisCapsule.radius = radius;

        TickManager.Instance.Subscribe(this, gameObject, TickDelayOption.t0);

        hitColliders = new Collider[maxColliders];
    }

    public void Tick()
    {
        force = Mathf.Clamp(force, 0f, 1.5f);

        ApplyPhysicsAvoidanceForce();

    }


    //private void OnTriggerStay(Collider other)
    //{
    //    if (Time.frameCount % 2 != 0 || !_controller) return;

    //    if (other.gameObject.layer != (int)LayerEnum.MobNormal) return;

    //    ApplyAvoidanceForce(other);
    //}



    private void ApplyAvoidanceForce(Collider other)
    {
        Vector3 directionToMob = other.gameObject.transform.position - _body.position;

        if (directionToMob == Vector3.zero) directionToMob = Random.insideUnitSphere;
        force += 0.06f;

        _controller.ChangeDirection = (-directionToMob).normalized * force;
    }


    private void ApplyPhysicsAvoidanceForce()
    {
        Vector3 averageVector = Vector3.zero;

        int numColliders = Physics.OverlapBoxNonAlloc(transform.position, new Vector3(radius, radius, radius), hitColliders, Quaternion.identity, layerMask);
        for (int i = 0; i < numColliders; i++)
        {
            if (hitColliders[i].gameObject.layer != (int)LayerEnum.MobNormal) continue;
            if (hitColliders[i].gameObject == _body.gameObject) continue;

            Vector3 thisDifference = hitColliders[i].gameObject.transform.position - _body.position;
            thisDifference.Normalize();
            averageVector += thisDifference;

        }

        averageVector /= numColliders;

        force += 0.01f;

        if (averageVector != Vector3.zero)
        {
            float factor = 1f;
            if (_ai && _ai.Target && _ai.DistanceToTarget < 8)
            {
                factor = 8f - _ai.DistanceToTarget;

                factor = 1.0f + (factor / 15f);

                if (_ai.DistanceToTarget < _ai.createDistanceRadius) averageVector = ((_ai.Target.position - _body.transform.position).normalized);
            }
            Vector3 goalDirection = (-averageVector).normalized * force * (factor);
            _controller.ChangeDirection = goalDirection;

            //Vector3 moveDir = Vector3.zero;
            //_controller.MoveDirection = new Vector3(moveDir.x, _controller.MoveDirection.y, moveDir.z);
        }
        else
        {
            force = Mathf.Clamp(force -= 0.1f, 0f, 1.0f);
        }

    }

}
