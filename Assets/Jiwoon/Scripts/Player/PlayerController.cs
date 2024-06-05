using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class PlayerController : MonoBehaviour
{
    [Header("Attack")]
    public float damage;
    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;
    public float force;

    [Header("Animation")]
    public Animator anim;

    [Header("Movement")]
    public float moveSpeed;
    private float moveSpeedRestorer;
    public float jumppower;
    private Vector2 curMovementInput;

    [Header("Look")]
    public Transform cameraContainer;
    public float minXlook;
    public float maxXlook;
    private float camCurXRot;
    public float lookSensitivity;
    private Vector2 mouseDelta;
    public LayerMask groundLayerMask;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        moveSpeedRestorer = moveSpeed;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void FixedUpdate()
    {
        Move();
    }

    private void Update()
    {

    }

    private void LateUpdate()
    {
        if (Time.timeScale > 0f)
        {
            CameraLook();
        }
    }

    private void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= moveSpeed;
        dir.y = _rb.velocity.y;

        _rb.velocity = dir;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
            anim.SetBool("IsMove", true);
            
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
            anim.SetBool("IsMove", false);
        }
    }


    void CameraLook()
    {
        if (Time.timeScale > 0f)
        {
            camCurXRot += mouseDelta.y * lookSensitivity;
            camCurXRot = Mathf.Clamp(camCurXRot, minXlook, maxXlook);
            cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

            transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
        }
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void Onjump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            _rb.AddForce(Vector2.up * jumppower, ForceMode.Impulse);
        }
    }

    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down)
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.5f, groundLayerMask))
            {
                return true;
            }
        }
        return false;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            anim.SetBool("IsAttack", true);
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if (context.phase == InputActionPhase.Performed)
        {
            anim.SetBool("IsAttack", false);
        }
    }
    IEnumerator Swing()
    {
        moveSpeed = 0f;
        yield return new WaitForSeconds(0.1f);
        meleeArea.enabled = true;
        trailEffect.enabled = true;
        yield return new WaitForSeconds(0.3f);
        _rb.AddForce(transform.forward * 10f * Time.timeScale, ForceMode.VelocityChange );
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;

        yield return new WaitForSeconds(0.1f);
        moveSpeed = moveSpeedRestorer;
    }

    public void OnPickUp(InputAction.CallbackContext context)
    {

    }

    public void OnOpenInventory(InputAction.CallbackContext context)
    {

    }

    public void OnOpenOptions(InputAction.CallbackContext context)
    {
        if (Time.timeScale == 0f)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1f;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            Time.timeScale = 0f;
        }
    }
    public void GamePause()
    {
        if (Time.timeScale == 1f)
        {
            CameraLook();
        }
    }



}
