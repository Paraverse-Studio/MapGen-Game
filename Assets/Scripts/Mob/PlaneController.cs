using UnityEngine;
using System.Collections;


public class PlaneController : MonoBehaviour
{
    [Header("Automotion ")]
    public bool isPlayer = false;
    public float speed = 6.0f;
    public Camera camera;

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


    private Vector2 screenCenter;

    public float lookRateSpeed = 50f;
    private Vector2 lookInput;
    private Vector2 mouseDistance;

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

        screenCenter.x = Screen.width * 0.5f;
        screenCenter.y = Screen.height * 0.5f;
    }


    void Update()
    {
        // Conditions /////
        if (!_active) return;


        if (isPlayer)
        {
            GetCameraMovement();
            GetMovement();
        }

        _characterController.Move(_moveVector * Time.deltaTime);

        //_characterController.transform.rotation = Quaternion.Slerp(
        //    _characterController.transform.rotation,
        //    Quaternion.LookRotation(new Vector3(camera.transform.forward.x, 0, camera.transform.forward.z)),
        //    Time.deltaTime * 1f);

        transform.Rotate(-mouseDistance.y * lookRateSpeed * Time.deltaTime, mouseDistance.x * lookRateSpeed * Time.deltaTime, 0f, Space.Self);

    }


    public void TurnTo(Vector3 direction, float lerpValue = 100f)
    {
        Vector3 _direction = new Vector3(direction.x, 0, direction.z);

        _body.transform.forward = Vector3.RotateTowards(_body.transform.forward, _direction, Time.deltaTime * lerpValue, 0.0f);
    }


    void GetCameraMovement()
    {
        lookInput.x = Input.mousePosition.x;
        lookInput.y = Input.mousePosition.y;

        mouseDistance.x = (lookInput.x - screenCenter.x) / screenCenter.y;
        mouseDistance.y = (lookInput.y - screenCenter.y) / screenCenter.y;

        mouseDistance = Vector2.ClampMagnitude(mouseDistance, 1f);
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
