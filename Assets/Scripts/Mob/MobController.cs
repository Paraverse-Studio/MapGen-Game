using UnityEngine;
using System.Collections;

// This script moves the character controller forward
// and sideways based on the arrow keys.
// It also jumps when pressing space.
// Make sure to attach a character controller to the same game object.
// It is recommended that you make only one call to Move or SimpleMove per frame.

public class MobController : MonoBehaviour
{
    [Header("Automotion ")]
    public bool isPlayer = false;
    public float speed = 6.0f;

    [Header("Jump")]
    public float jumpSpeed = 8.0f;
    public float jumpCD = 0.5f;
    private float jumpTimer = 0.0f;

    [Header("Attack")]
    public float attackCD = 1f;
    private float attackTimer = 0.0f;

    public float gravity = 9.8f;

    private CharacterController _characterController;
    private Renderer _renderer;

    private Vector3 _changeDirection = Vector3.zero;
    public Vector3 ChangeDirection
    {
        get { return _changeDirection; }
        set { _changeDirection = value; }
    }

    private Vector3 _moveDirection = Vector3.zero;    
    public Vector3 MoveDirection
    {
        get { return _moveDirection; }
        set { _moveDirection = value; }
    }

    private Vector3 _finalDirection;
    public Vector3 FinalDirection => _finalDirection;

    private Vector3 moveVector;

    private readonly Vector3 referenceAngleVector = new Vector3(-1, 0, 1);

    private Vector3 _lastSafePosition = Vector3.zero;
    private Transform _body;
    public Transform Body => _body;




    // For disabling player movement, gravity, input, all
    private bool _active = true;
    public bool Active => _active;



    private void Awake()
    {
        _characterController = GetComponentInChildren<CharacterController>();
        _renderer = GetComponentInChildren<Renderer>();
        _body = GetComponentInChildren<CharacterController>().transform;
    }

    void Start()
    {
        if (MapGeneration.Instance && isPlayer) MapGeneration.Instance.OnMapGenerateEnd.AddListener(TeleportPlayer);

        // calculate the correct vertical position
        float correctHeight = _characterController.center.y + _characterController.skinWidth;
        // set the controller center vector
        _characterController.center = new Vector3(0, correctHeight, 0);
    }

    void Update()
    {
        //  TIMERS   //////
        jumpTimer += Time.deltaTime;

        _changeDirection = Vector3.Lerp(_changeDirection, Vector3.zero, Time.deltaTime * 2f);
        ///////////////////       


        // Conditions /////
        if (!_active) return;


        if (_characterController.isGrounded)
        {
            if (_moveDirection.y < 0) _moveDirection.y = 0;
        }

        if (isPlayer)
        {
            GetMovement();

            GetJump();

            GetAttack();
        }


        // Gravity
        _moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        _finalDirection = (_moveDirection + _changeDirection);

        // AI jitter prevention
        if (!isPlayer)
        {
            if (_finalDirection.sqrMagnitude < 1f) { _finalDirection.x = 0; _finalDirection.z = 0; }
        }

        _characterController.Move(_finalDirection * Time.deltaTime);
        
        // Facing the direction you're moving
        if ((Mathf.Abs(_moveDirection.x) + Mathf.Abs(_moveDirection.z)) > 0.1f)
        {
            TurnTo(_moveDirection);
        }               

    }


    public void TurnTo(Vector3 direction, float lerpValue = 100f)
    {
        Vector3 _direction = new Vector3(direction.x, 0, direction.z);        

        _body.transform.forward = Vector3.RotateTowards(_body.transform.forward, _direction, Time.deltaTime * lerpValue, 0.0f);
    }


    private void GetJump()
    {
        if (Input.GetButton("Jump"))
        {
            Jump();
        }
    }

    public void Jump()
    {
        if (jumpTimer >= jumpCD && _characterController.isGrounded)
        {
            _moveDirection.y = jumpSpeed;
            jumpTimer = 0.0f;
        }
    }

    void GetMovement()
    {
        float moveX = Input.GetAxis("Vertical");
        float moveZ = Input.GetAxis("Horizontal");

        Vector3 moveVectorNow = new Vector3(moveX, 0, moveZ);
        if (moveVectorNow.magnitude <= (moveVector.magnitude * 0.9f))
        {
            moveX = 0;
            moveZ = 0;
            moveVector = moveVectorNow;

            _moveDirection = new Vector3(0, _moveDirection.y, 0);
            return;
        }
        moveVector = moveVectorNow;


        float currentY = _moveDirection.y;

        Vector3 rightOfReference = new Vector3(referenceAngleVector.z, referenceAngleVector.y, -referenceAngleVector.x);
        _moveDirection = (referenceAngleVector * (moveX * 3f)) + (rightOfReference * (moveZ * 3f));

        _moveDirection.y = 0;

        _moveDirection *= speed;

        _moveDirection.y = currentY;
    }

    private void GetAttack()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Attack();
        }
    }

    public void Attack()
    {
        if (attackTimer < attackCD) return;
        attackTimer = 0f;

        _moveDirection = Vector3.zero;

        Debug.Log(gameObject.name + " attacked!");
        // wind up attack animation

        // perhaps use animation event to trigger the box on the sword for better accuracy 
    }





    void SafetyNet()
    {
        if (Time.frameCount % 60 == 0)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(_body.transform.position + new Vector3(0, 0.2f, 0), Vector3.down, out hitInfo, LayerMask.NameToLayer("Solid")))
            {
                if (_characterController.isGrounded) _lastSafePosition = _body.transform.position;
            }

            if (MapGeneration.Instance && _body.transform.position.y < (MapGeneration.Instance.YBoundary.y - 3.0f))
            {
                if (_lastSafePosition != Vector3.zero) TeleportPlayer(_lastSafePosition);
                else TeleportPlayer();
            }

        }
    }

    public void TeleportPlayer(Vector3 pos)
    {
        _characterController.enabled = false;
        _body.position = pos;
        _characterController.enabled = true;
    }

    public void TeleportPlayer()
    {
        _moveDirection.y = -1f;
        TeleportPlayer(MapGeneration.Instance.CenterPointWithY + new Vector3(0, 1f, 0));
    }

    public void TogglePlayer(bool onOrOff)
    {
        _active = onOrOff;
        _body.gameObject.SetActive(onOrOff);
    }


}