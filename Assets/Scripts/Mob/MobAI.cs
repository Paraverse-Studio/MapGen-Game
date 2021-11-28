using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobAI : MonoBehaviour
{
    public int detectRadius = 5;
    public int chaseRadius = 7; // longer than detect
    public int attackRadius = 1;
    private MobController _controller;
    private Transform _body;
    private Transform _playerBody;
    private Transform _target;

    private float _distanceToTarget = 0;
    private float _distanceToPlayer = 0;
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
        if (Time.frameCount % 3 == 0)
        {
            if (_target) _distanceToTarget = Vector3.Distance(_body.position, _target.position);
            if (Time.frameCount % 6 == 0)
            {
                _distanceToPlayer = Vector3.Distance(_body.position, _playerBody.position);
            }
        }

        if (!_target)
        {
            // Go back to original spot, and patrol around
            _controller.MoveDirection = (_originalPosition - _body.position).normalized * 2.5f; // *2 to match player

            if (_distanceToPlayer <= detectRadius)
            {
                _target = _playerBody;
                UpdateDistances();
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
                _controller.MoveDirection = (_target.position - _body.position).normalized * 2.5f;
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
    }


}
