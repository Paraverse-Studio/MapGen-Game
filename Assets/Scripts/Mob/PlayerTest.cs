using UnityEngine;
using System.Collections;

// This script moves the character controller forward
// and sideways based on the arrow keys.
// It also jumps when pressing space.
// Make sure to attach a character controller to the same game object.
// It is recommended that you make only one call to Move or SimpleMove per frame.

public class PlayerTest : MonoBehaviour
{
    CharacterController characterController;

    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 9.8f;

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
        failingSafePositionCounter = Mathf.Max(0, failingSafePositionCounter - Time.deltaTime);

        if (characterController.isGrounded)
        {
            // We are grounded, so recalculate
            // move direction directly from axes
            moveDirection.y = 0;
        }

        _simulatedCamera.transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, Camera.main.transform.eulerAngles.z);

        float currentY = moveDirection.y;
        moveDirection = _simulatedCamera.transform.forward * Input.GetAxis("Vertical") + _simulatedCamera.transform.right * Input.GetAxis("Horizontal");

        moveDirection *= speed;

        moveDirection.y = currentY;

        if (Input.GetButton("Jump") && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);


        if (Time.frameCount % 60 == 0)
        {
            if (_body.position.y >= 0f)
            {
                RaycastHit hitInfo;
                if (Physics.Raycast(_body.transform.position, Vector3.down, out hitInfo, LayerMask.NameToLayer("Ground")))
                {
                    lastSafePosition = _body.transform.position;
                }
            }
        }

        if (_body.position.y < -10)
        {
            failingSafePositionCounter += 1f;
            _body.position = lastSafePosition;

            if (failingSafePositionCounter > 1) _body.position = MapGeneration.Instance.CenterPoint + new Vector3(0, 10, 0);
        }

    }
}