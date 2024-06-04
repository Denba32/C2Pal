using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;

    [Header("Movement")]
    public float moveSpeed;
    public float jumpPower;
    public LayerMask groundLayer;

    private Vector2 curMovementInput;

    [Header("Look")]
    public Transform cameraContainer;
    public float limitedXLook;
    private float camCurXRot;
    public float lookSensitivity;
    private Vector2 mouseDelta;
    public bool canLook = true;

    public Action inventory;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        ActiveCursor(true);
    }

    private void OnEnable()
    {
        AddListener(playerInput.actions["Move"], OnMove);
        AddListener(playerInput.actions["Look"], OnLook);
        AddListener(playerInput.actions["Jump"], OnJump);
        AddListener(playerInput.actions["Inventory"], OnInventory);
    }

    private void OnDisable()
    {
        RemoveListener(playerInput.actions["Move"], OnMove);
        RemoveListener(playerInput.actions["Look"], OnLook);
        RemoveListener(playerInput.actions["Jump"], OnJump);
        RemoveListener(playerInput.actions["Inventory"], OnInventory);
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        if(canLook)
        {
            CameraLook();
        }
    }

    /// <summary>
    /// 
    /// TRUE일 경우 커서 활성화
    /// FALSE일 경우 커서 비활성화
    /// </summary>q
    /// <param name="active"></param>
    private void ActiveCursor(bool active)
    {
        Cursor.lockState = active ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = active ? false : true;
    }

    void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;

        dir *= moveSpeed;
        dir.y = _rigidbody.velocity.y;

        _rigidbody.velocity = dir;
    }

    void CameraLook()
    {
        // 상하 회전 제한
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, -limitedXLook, limitedXLook);

        // 상하 회전
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        // 좌우 회전
        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);

    }


    public void OnMove(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }

    public void OnLook(InputAction.CallbackContext callback)
    {
        mouseDelta = callback.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started && IsGrounded())
        {
            _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
        }
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            inventory?.Invoke();
            ToggleCursor();
        }
    }    

    void ToggleCursor()
    {
        canLook = !canLook;
        ActiveCursor(canLook);
    }

    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
        };

        for(int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 1f, groundLayer))
            {
                return true;
            }
        }
        return false;
    }

    #region ========== Function ==========
    private void AddListener(InputAction action, Action<InputAction.CallbackContext> callback)
    {
        action.started += callback;
        action.performed += callback;
        action.canceled += callback;
    }

    private void RemoveListener(InputAction action, Action<InputAction.CallbackContext> callback)
    {
        action.started -= callback;
        action.performed -= callback;
        action.canceled -= callback;
    }
    #endregion
}