using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobAI : MonoBehaviour
{
    public int detectRadius = 5;
    public int chaseRadius = 7; // longer than detect
    public int attackRadius = 1;

    [Header("Movement rotation: ")]
    public float rotationSpeed;

    private MobController _controller;
    private Transform _body;
    private Transform _playerBody;
    private Transform _target;

    private float _distanceToTarget = 0;
    private float _distanceToPlayer = 0;
    private float _distanceToOriginalPosition = 0;
    private Vector3 _originalPosition;

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<MobController>();
        _body = _controller.Body;
        _playerBody = GameObject.FindGameObjectWithTag("Player").GetComponent<MobController>().Body.transform;
        _originalPosition = _body.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount % 10 == 0)
        {
            UpdateDistances();            
        }

        if (!_target)
        {
            // Go back to original spot, and patrol around
            if (_distanceToOriginalPosition > 0.5f) SendMovement(_originalPosition);
            else _controller.MoveDirection = Vector3.zero;

            if (_distanceToPlayer <= detectRadius)
            {
                _target = _playerBody;
            }
        }

        if (_target)
        {
            if (_distanceToTarget <= attackRadius)
            {
                _controller.MoveDirection = Vector3.zero;
                _controller.Attack();
            }
            else if (_distanceToTarget <= chaseRadius)
            {
                SendMovement(_target.position);
            }
            else if (_distanceToTarget > chaseRadius)
            {
                _target = null;
            }
        }

    }

    private void UpdateDistances()
    {
        if (_target) _distanceToTarget = Vector3.Distance(_body.position, _target.position);        
        _distanceToPlayer = Vector3.Distance(_body.position, _playerBody.position);
        _distanceToOriginalPosition = Vector3.Distance(_body.position, _originalPosition);
    }

    private void SendMovement(Vector3 t)
    {
        Vector3 tPosition = new Vector3(t.x, _body.transform.position.y, t.z);
        Quaternion lookDirection = Quaternion.LookRotation(tPosition - _body.transform.position);
        _body.transform.rotation = Quaternion.Slerp(_body.transform.rotation, lookDirection, Time.deltaTime * rotationSpeed);

        Vector3 forward = (_body.transform.forward).normalized * 2.5f;
        forward.y = _controller.MoveDirection.y;
        _controller.MoveDirection = forward;
    }

}
