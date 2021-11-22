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

    private CharacterController characterController;    

    private Vector3 moveDirection = Vector3.zero;
    private GameObject _simulatedCamera;

    private Vector3 lastSafePosition = Vector3.zero;
    private float failingSafePositionCounter = 0f;
    private Transform _body;

    void Start()
    {
        characterController = GetComponentInChildren<CharacterController>();
        _simulatedCamera = new GameObject();
        _body = characterController.transform;
    }

    void Update()
    {
        //  TIMERS   //////
        failingSafePositionCounter = Mathf.Max(0, failingSafePositionCounter - Time.deltaTime);

        ///////////////////


        if (characterController.isGrounded)
        {
            moveDirection.y = 0;
        }

        GetMovement();

        GetJump();

        // Gravity
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);


        SafetyNet();
    }


    void GetJump()
    {
        if (Input.GetButton("Jump") && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
    }

    void GetMovement()
    {
        // Using camera's forward
        _simulatedCamera.transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);

        float currentY = moveDirection.y;
        moveDirection = _simulatedCamera.transform.forward * Input.GetAxis("Vertical") + _simulatedCamera.transform.right * Input.GetAxis("Horizontal");

        moveDirection *= speed;

        moveDirection.y = currentY;
    }

    void SafetyNet()
    {        
        if (Time.frameCount % 50 == 0)
        {            
            RaycastHit hitInfo;
            if (Physics.Raycast(_body.transform.position, Vector3.down, out hitInfo, LayerMask.NameToLayer("Solid")))
            {
                lastSafePosition = _body.transform.position;
            }
            else
            {
                failingSafePositionCounter += 1.2f;

                if (failingSafePositionCounter >= 1.6f && lastSafePosition != Vector3.zero)
                {
                    _body.position = lastSafePosition;
                }
                else
                {
                    _body.position = MapGeneration.Instance.CenterPoint + new Vector3(0, 3, 0);
                }
            }            
        }        
    }



}