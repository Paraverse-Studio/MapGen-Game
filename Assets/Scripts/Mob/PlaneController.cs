using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    public CharacterController _controller;
    public Transform _camera;
    public float gravity = 20f;
    public bool isActive = false;

    [Header("Movement")]
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;

    [Header("Jump")]
    public float jumpSpeed = 8.0f;
    public float jumpCD = 0.5f;
    private float jumpTimer = 0.0f;


    private Vector3 _moveDirection;
    private float _currentYvalue = 0f;
    float turnSmoothVelocity;

    void Start()
    {
        // calculate the correct vertical position
        float correctHeight = _controller.center.y + _controller.skinWidth;
        // set the controller center vector
        _controller.center = new Vector3(0, correctHeight, 0);
                
    }

    void Update()
    {
        jumpTimer += Time.deltaTime;

        if (!isActive) return;

        // Apply gravity
        _currentYvalue -= gravity * Time.deltaTime;
        if (_controller.isGrounded)
        {
            if (_currentYvalue < 0) _currentYvalue = 0;
        }


        if (true)
        {
            GetMovement();

            GetJump();
        }


        // Do the move, and rotate the camera
        float targetAngle = 0;
        if (_moveDirection.magnitude >= 0.1f)
        {
            targetAngle = Mathf.Atan2(_moveDirection.x, _moveDirection.z) * Mathf.Rad2Deg + _camera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }

        Vector3 moveDir = Vector3.zero;
        if (_moveDirection.magnitude >= 0.1f)
        {
            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }          
        moveDir.Normalize();
        moveDir *= speed;
        moveDir.y = _currentYvalue;
        _controller.Move(moveDir * Time.deltaTime);
        
    }


    private void GetMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        _moveDirection = new Vector3(horizontal, _moveDirection.y, vertical).normalized;
        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
    }

    private void GetJump()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Jump();
        }
    }

    public void Jump()
    {
        if (jumpTimer >= jumpCD && _controller.isGrounded)
        {
            _currentYvalue = jumpSpeed;
            jumpTimer = 0.0f;
        }
    }

    public void Active(bool o)
    {
        isActive = o;
        if (isActive)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void TeleportPlayer(Vector3 pos)
    {
        _controller.enabled = false;
        transform.position = pos;
        _controller.gameObject.transform.position = pos;
        _controller.enabled = true;
    }

    public void TeleportPlayer()
    {
        _currentYvalue = 0f;
        TeleportPlayer(MapGeneration.Instance.CenterPointWithY + new Vector3(0, 5f, 0));
    }
}