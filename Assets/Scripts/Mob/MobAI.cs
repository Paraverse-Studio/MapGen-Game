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
        _playerBody = GameObject.FindGameObjectWithTag("Player").GetComponentInParent<MobComponents>().body;
        _originalPosition = _body.transform.position;
    }

    public void OnEnable()
    {
        _originalPosition = transform.position;
        GetComponent<MobComponents>().body.transform.localPosition = Vector3.zero;
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
            if (_distanceToOriginalPosition > 0.8f) SendMovement(_originalPosition);
            else _controller.MoveDirection = Vector3.zero;

            if (_distanceToPlayer <= detectRadius && _playerBody.gameObject.activeInHierarchy)
            {
                SetTarget(_playerBody);
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
                SetTarget(null);
            }

            if (Time.frameCount % 10 == 0 && _target && !_target.gameObject.activeInHierarchy) 
                SetTarget(null);
        }

    }

    public void SetTarget(Transform t)
    {
        _target = t;
        UpdateDistances();
    }

    private void UpdateDistances()
    {
        Vector3 bodyPosition = _body.position; bodyPosition.y = 0;
        Vector3 playerBodyPosition = _playerBody.position; playerBodyPosition.y = 0;
        Vector3 originalPosition = _originalPosition; originalPosition.y = 0;

        _distanceToPlayer = Vector3.Distance(bodyPosition, playerBodyPosition);
        _distanceToOriginalPosition = Vector3.Distance(bodyPosition, originalPosition);

        if (_target)
        {
            Vector3 targetPosition = _target.position; targetPosition.y = 0;
            _distanceToTarget = Vector3.Distance(bodyPosition, targetPosition);
        }        
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
