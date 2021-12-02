using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyMoveState
{
    idle,
    backToSpawn,
    inDetect,
    inChaseRange,
    inAttackRange,
    inRetreatRange
}

public enum EnemyMoveDirection
{
    Towards, Reverse
}

public class MobAI : MonoBehaviour
{
    [Header("Movement state radiuses: ")]
    public float detectRadius = 5;
    public float chaseRadius = 7; // longer than detect
    public float attackRadius = 1;
    public float createDistanceRadius = 0.6f;

    [Header("Movement rotation: ")]
    private float rotationSpeed = 8f;

    private EnemyMoveState enemyMoveState = EnemyMoveState.idle;

    private MobController _controller;
    private Transform _body;
    private Transform _playerBody;
    private Transform _target;
    public Transform Target => _target;

    private float _distanceToTarget = 0;
    public float DistanceToTarget => _distanceToTarget;

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

        DetectAhead();


        // Transitions
        if (!_target)
        {
            // Go back to original spot, and patrol around
            if (_distanceToOriginalPosition > 0.8f)
            {
                enemyMoveState = EnemyMoveState.backToSpawn;
            }
            else
            {
                enemyMoveState = EnemyMoveState.idle;
            }

            if (_distanceToPlayer <= detectRadius && _playerBody.gameObject.activeInHierarchy)
            {
                SetTarget(_playerBody);
            }
        }

        if (_target)
        {
            if (_distanceToTarget <= createDistanceRadius)
            {
                enemyMoveState = EnemyMoveState.inRetreatRange;
            }
            else if (_distanceToTarget <= attackRadius)
            {
                enemyMoveState = EnemyMoveState.inAttackRange;
            }
            else if (_distanceToTarget <= chaseRadius)
            {
                enemyMoveState = EnemyMoveState.inChaseRange;
                SendMovement(_target.position);
            }
            else if (_distanceToTarget > chaseRadius)
            {
                SetTarget(null);
                enemyMoveState = EnemyMoveState.backToSpawn;
            }

            if (Time.frameCount % 10 == 0 && _target && !_target.gameObject.activeInHierarchy)
            {
                SetTarget(null);
                enemyMoveState = EnemyMoveState.backToSpawn;
            }
        }


        switch (enemyMoveState)
        {
            case EnemyMoveState.idle:

                SendMovement(Vector3.zero);

                break;


            case EnemyMoveState.backToSpawn:

                SendMovement(_originalPosition);

                break;


            case EnemyMoveState.inDetect:

                break;


            case EnemyMoveState.inChaseRange:

                SendMovement(_target.position);

                break;


            case EnemyMoveState.inAttackRange:
                SendMovement(Vector3.zero);
                _controller.Attack();
                _controller.TurnTo(_target.position - _body.position, 8f);

                break;

            case EnemyMoveState.inRetreatRange:
                SendMovement(_target.position, EnemyMoveDirection.Reverse);

                break;
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


    private void SendMovement(Vector3 t, EnemyMoveDirection directionEnum = EnemyMoveDirection.Towards)
    {
        Vector3 forward;
        if (t != Vector3.zero)
        {
            Vector3 tPosition = new Vector3(t.x, _body.transform.position.y, t.z);

            Quaternion lookAngle; Vector3 lookDirection;
            if (directionEnum == EnemyMoveDirection.Towards)
            {
                lookDirection = tPosition - _body.transform.position;
                lookAngle = Quaternion.LookRotation(lookDirection);
            }
            else
            {
                lookDirection =  _body.transform.position - tPosition;
                lookAngle = Quaternion.LookRotation(lookDirection);
            }

            //_body.transform.rotation = Quaternion.Slerp(_body.transform.rotation, lookAngle, Time.deltaTime * rotationSpeed);


            Vector3 newDirection = Vector3.RotateTowards(_body.transform.forward, lookDirection, Time.deltaTime * rotationSpeed, 0.0f);
            _body.transform.rotation = Quaternion.LookRotation(newDirection);


            forward = (_body.transform.forward).normalized * (directionEnum == EnemyMoveDirection.Towards? 4f: 2.5f);
        }
        else forward = Vector3.zero;
        _controller.MoveDirection = new Vector3(forward.x, _controller.MoveDirection.y, forward.z);
    }

    public void DetectAhead()
    {
        if (Time.frameCount % 20 == 0)
        {
            Debug.DrawRay(_body.transform.position, _body.transform.forward * 1.25f, Color.red, 0.2f);
            RaycastHit[] hits;
            hits = Physics.RaycastAll(_body.transform.position, _body.transform.forward, 1.25f);

            for (int i = 0; i < hits.Length; ++i)
            {
                if (hits[i].collider.gameObject.layer == (int)LayerEnum.Solid)
                {
                    if (_controller.FinalDirection.sqrMagnitude > 2f)
                        _controller.Jump();
                }
            }
        }
    }
}
