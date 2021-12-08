using UnityEngine;
using System.Collections;


public class PlaneController : MonoBehaviour
{
    [Header("Automotion ")]
    public bool isPlayer = false;
    public float speed = 6.0f;


    private CharacterController _characterController;
    private Renderer _renderer;

    private Vector3 _changeDirection = Vector3.zero;
    public Vector3 ChangeDirection
    {
        get { return _changeDirection; }
        set { _changeDirection = value; }
    }

    private Vector3 _moveVector = Vector3.zero;
    public Vector3 MoveDirection
    {
        get { return _moveVector; }
        set { _moveVector = value; }
    }

    private Vector3 _finalDirection;
    public Vector3 FinalDirection => _finalDirection;

    private Vector3 moveVector;

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

        _changeDirection = Vector3.Lerp(_changeDirection, Vector3.zero, Time.deltaTime * 2f);
        ///////////////////       


        // Conditions /////
        if (!_active) return;


        if (isPlayer)
        {
            GetMovement();
        }

        _characterController.Move(_moveVector * Time.deltaTime);

        //// Facing the direction you're moving
        //if ((Mathf.Abs(_moveDirection.x) + Mathf.Abs(_moveDirection.z)) > 0.1f)
        //{
        //    TurnTo(_moveDirection);
        //}

    }


    public void TurnTo(Vector3 direction, float lerpValue = 100f)
    {
        Vector3 _direction = new Vector3(direction.x, 0, direction.z);

        _body.transform.forward = Vector3.RotateTowards(_body.transform.forward, _direction, Time.deltaTime * lerpValue, 0.0f);
    }


    void GetMovement()
    {
        float moveX = Input.GetAxis("Vertical");
        float moveZ = Input.GetAxis("Horizontal");

        _moveVector = (transform.forward * moveX * speed) + (transform.forward * moveZ * speed);
        
    }


    public void TeleportPlayer(Vector3 pos)
    {
        _characterController.enabled = false;
        transform.position = pos;
        _characterController.enabled = true;
    }

    public void TeleportPlayer()
    {
        _moveVector.y = -1f;
        TeleportPlayer(MapGeneration.Instance.CenterPointWithY + new Vector3(0, 1f, 0));
    }

    public void TogglePlayer(bool onOrOff)
    {
        _active = onOrOff;
        _body.gameObject.SetActive(onOrOff);
    }


}
