using UnityEngine;
using System.Collections;

// This script moves the character controller forward
// and sideways based on the arrow keys.
// It also jumps when pressing space.
// Make sure to attach a character controller to the same game object.
// It is recommended that you make only one call to Move or SimpleMove per frame.

public class PlayerController : MonoBehaviour
{
    [Header("Automotion: ")]
    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 9.8f;

    private CharacterController _characterController;    

    private Vector3 _moveDirection = Vector3.zero;
    private GameObject _simulatedCamera;

    private Vector3 _lastSafePosition = Vector3.zero;
    private float _failingSafePositionCounter = 0f;
    private Transform _body;

    void Start()
    {
        _characterController = GetComponentInChildren<CharacterController>();
        _simulatedCamera = new GameObject();
        _body = _characterController.transform;
        MapGeneration.Instance.OnMapGenerateEnd.AddListener(TeleportPlayer);

        // calculate the correct vertical position
        float correctHeight = _characterController.center.y + _characterController.skinWidth;
        // set the controller center vector
        _characterController.center = new Vector3(0, correctHeight, 0);
    }

    void Update()
    {
        //  TIMERS   //////
        _failingSafePositionCounter = Mathf.Max(0, _failingSafePositionCounter - Time.deltaTime);

        ///////////////////


        if (_characterController.isGrounded)
        {
            _moveDirection.y = 0;
        }

        GetMovement();

        GetJump();

        // Gravity
        _moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        _characterController.Move(_moveDirection * Time.deltaTime);

        if ((Mathf.Abs(_moveDirection.x) + Mathf.Abs(_moveDirection.z)) > 0.1f)
        {
            _body.transform.forward = Vector3.Lerp(_body.transform.forward, _moveDirection, Time.deltaTime * 2f);
        }


        SafetyNet();
    }


    void GetJump()
    {
        if (Input.GetButton("Jump") && _characterController.isGrounded)
        {
            _moveDirection.y = jumpSpeed;
        }
    }

    void GetMovement()
    {
        // Using camera's forward
        _simulatedCamera.transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);

        float currentY = _moveDirection.y;
        _moveDirection = _simulatedCamera.transform.forward * Input.GetAxis("Vertical") + _simulatedCamera.transform.right * Input.GetAxis("Horizontal");

        _moveDirection *= speed;

        _moveDirection.y = currentY;
    }

    void SafetyNet()
    {        
        if (Time.frameCount % 50 == 0)
        {            
            RaycastHit hitInfo;
            if (Physics.Raycast(_body.transform.position + new Vector3(0, 0.2f, 0), Vector3.down, out hitInfo, LayerMask.NameToLayer("Solid")))
            {
                if (_characterController.isGrounded) _lastSafePosition = _body.transform.position;
            }
            else if (_moveDirection.y < 0)
            {
                _failingSafePositionCounter += 1.2f;

                if (_failingSafePositionCounter >= 1.6f )
                {
                    _moveDirection.y = 0;
                    if (_lastSafePosition != Vector3.zero) TeleportPlayer(_lastSafePosition);                    
                    else TeleportPlayer(MapGeneration.Instance.CenterPoint + new Vector3(0, 2, 0));                    
                }
            }            
        }        
    }

    private void TeleportPlayer(Vector3 pos)
    {
        _characterController.enabled = false;
        _body.position = pos;
        _characterController.enabled = true;
    }
    private void TeleportPlayer()
    {
        _moveDirection.y = 0;
        TeleportPlayer(MapGeneration.Instance.CenterPoint + new Vector3(0, 4, 0));
    }


}