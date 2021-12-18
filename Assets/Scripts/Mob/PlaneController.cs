using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    public CharacterController _controller;
    public Transform _camera;
    public float gravity = 20f;
    public bool isActive = false;
    public CinemachineFreeLook vcam;

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

    private bool boost = false;

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
        _currentYvalue -= gravity * Time.deltaTime * (boost? 0.2f:1);
        if (_controller.isGrounded)
        {
            if (_currentYvalue < 0) _currentYvalue = 0;
        }
        else
        {
            if (_currentYvalue < -20) _currentYvalue = -20;

            if (boost) _currentYvalue = Mathf.Max(_currentYvalue, -2);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) vcam.gameObject.SetActive(!vcam.gameObject.activeSelf);


        if (true)
        {
            GetMovement();

            GetJump();

            boost = false;
            if (Input.GetKey(KeyCode.LeftShift)) boost = true;
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
        moveDir *= speed * (boost ? 5 : 1);
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
            _currentYvalue = jumpSpeed * (boost ? 2 : 1);
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

    public void TeleportPlayer(Vector3 pos, bool alsoMoveCamera = false)
    {
        _controller.enabled = false;
        transform.position = pos;
        _controller.gameObject.transform.position = pos;
        _controller.enabled = true;

        if (alsoMoveCamera)
        {
            Camera.main.transform.position = pos;
        }
    }

    public void TeleportPlayer(bool alsoMoveCamera = false)
    {
        _currentYvalue = 0f;
        TeleportPlayer(MapGeneration.Instance.CenterPointWithY + new Vector3(0, 5f, 0), alsoMoveCamera);        
    }
}