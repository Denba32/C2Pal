using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class PlayerController : MonoBehaviour
{
    public PlayerCondition condition;
    public float staminaRecoveryDelay = 10f;
    public float rollDuration = 1f;

    [Header("Attack")]
    [SerializeField] private float damage;
    [SerializeField] private BoxCollider meleeArea;
    [SerializeField] private TrailRenderer trailEffect;
    [SerializeField] private float force;
    [SerializeField] private float UseSpecialAttackStamina;

    [Header("Animation")]
    [SerializeField] private Animator anim;

    [Header("Roll")]
    [SerializeField] private BoxCollider playerBody;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpPower;
    [SerializeField] private float sprintMultiplier = 2f;
    [SerializeField] private float groundCheckDistance = 0.1f;
    private float moveSpeedRestorer;
    private Vector2 curMovementInput;

    [Header("Look")]
    [SerializeField] private Transform cameraContainer;
    [SerializeField] private float minXlook;
    [SerializeField] private float maxXlook;
    [SerializeField] private float lookSensitivity;
    [SerializeField] private LayerMask groundLayerMask;
    private float camCurXRot;
    private Vector2 mouseDelta;

    private Rigidbody _rb;
    private Vector2 _movementInput;
    public bool _isSprinting = false;
    public bool _isGrounded = true;
    private bool _isAttacking = false;

    private bool isPause = false;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        moveSpeedRestorer = moveSpeed;
        Cursor.lockState = CursorLockMode.Locked;

        GameManager.Instance.onGamePause += PausePlayer;
        GameManager.Instance.onGameStart += PlayPlayer;


    }
    private void FixedUpdate()
    {
        if (!isPause)
        {
            Move();
        }
    }

    private void Update()
    {
        _isGrounded = IsGrounded();

        Debug.Log(_isGrounded + "바닥 판단 여부");

        if (_isGrounded && _movementInput.magnitude > 0)
        {
            if (_isSprinting)
            {
                anim.SetBool("IsRunning", true);
                if(condition.uiconditions.stamina.curValue <= 1f)
                {
                    condition.uiconditions.health.Substract(10f * Time.deltaTime);
                }
                condition.uiconditions.stamina.Substract(3f * Time.deltaTime);
            }
            else
            {
                anim.SetBool("IsWalking", true);
            }
        }
        else
        {
            anim.SetBool("IsWalking", false);
            anim.SetBool("IsRunning", false);
        }
    }

    private void LateUpdate()
    {
        if (!isPause)
        {
            CameraLook();
        }

    }
  
    private void Move()
    {
        Vector3 moveDirection = transform.forward * _movementInput.y + transform.right * _movementInput.x;
        moveDirection *= _isSprinting ? moveSpeed * sprintMultiplier : moveSpeed;
        moveDirection.y = _rb.velocity.y;

        _rb.velocity = moveDirection;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector2>();

        if (_movementInput != Vector2.zero)
        {
            if (Keyboard.current.leftShiftKey.isPressed)
            {
                _isSprinting = true;
                anim.SetBool("IsRunning", true);
                anim.SetBool("IsWalking", false);
            }
            else
            {
                _isSprinting = false;
                anim.SetBool("IsRunning", false);
                anim.SetBool("IsWalking", true);
            }
        }
        else
        {
            _isSprinting = false;
            anim.SetBool("IsRunning", false);
            anim.SetBool("IsWalking", false);
        }
    }
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            _isSprinting = true;
            anim.SetBool("IsRunning", true);
            anim.SetBool("IsWalking", false);
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            _isSprinting = false;
            anim.SetBool("IsRunning", false);
            anim.SetBool("IsWalking", true);
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

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && _isGrounded)
        {
            _rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            _isGrounded = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        _isGrounded = CheckGrounded();
    }

    private bool CheckGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayerMask);
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            UIManager.Instance.ShowInventory();
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
        if (!_isAttacking && context.phase == InputActionPhase.Performed)
        {
            if (Keyboard.current.leftShiftKey.isPressed && condition.uiconditions.stamina.curValue > UseSpecialAttackStamina)
            {
                StopCoroutine(SpecialAttackCoroutine());
                StartCoroutine(SpecialAttackCoroutine());
                condition.uiconditions.stamina.Substract(UseSpecialAttackStamina);
                
            }
            else
            {
                StartCoroutine(AttackCoroutine());
            }
        }
    }
    public void OnRoll(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            StartCoroutine(Rolling());
            condition.uiconditions.stamina.Substract(UseSpecialAttackStamina);
        }
    }
    IEnumerator Rolling()
    {

        float originalMoveSpeed = moveSpeed;


        Vector3 originalPosition = transform.position;
        Vector3 targetPosition = originalPosition + transform.forward * 3f; // 이동 거리 조절 가능


        float duration = 0.3f;
        float elapsedTime = 0f;


        Vector3 startPosition = originalPosition;


        anim.SetBool("IsRoll", true);


        while (elapsedTime < duration)
        {

            float distanceCovered = (elapsedTime / duration) * Vector3.Distance(startPosition, targetPosition);
            transform.position = startPosition + transform.forward * distanceCovered;


            playerBody.transform.position = transform.position;

            elapsedTime += Time.deltaTime;
            yield return null;
        }


        anim.SetBool("IsRoll", false);

        transform.position = targetPosition;

        playerBody.transform.position = transform.position;


        moveSpeed = moveSpeedRestorer;
    }
    IEnumerator Swing()
    {
        moveSpeed = 0f;
        yield return new WaitForSeconds(0.1f); // 칼을 든다.
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        Vector3 originalPosition = transform.position;
        Vector3 targetPosition = originalPosition + transform.forward * 0.5f; // 약간 앞으로간다.
        float elapsedTime = 0f;
        float duration = 0.3f; // 내려칠 때의 시간

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        meleeArea.enabled = false;
        _isAttacking = false;

        yield return new WaitForSeconds(0.3f); // 다시든다.
        trailEffect.enabled = false;

        moveSpeed = moveSpeedRestorer;
    }

    IEnumerator Swing2()
    {
        moveSpeed = 0f;
        yield return new WaitForSeconds(0.06f); // 칼을 든다.
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        Vector3 originalPosition = transform.position;
        Vector3 targetPosition = originalPosition + transform.forward * 1f; //약간 앞으로(일반공격보다 조금더)간다.
        float elapsedTime = 0f;
        float duration = 0.44f; // 내려칠 때의 시간

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _isAttacking = false;

        yield return new WaitForSeconds(0.3f); //다시든다.
        meleeArea.enabled = false;
        trailEffect.enabled = false;

        moveSpeed = moveSpeedRestorer;
    }

    IEnumerator AttackCoroutine()
    {
        _isAttacking = true;
        anim.SetBool("IsAttack", true);
        StopCoroutine("Swing");
        StartCoroutine("Swing");

        yield return new WaitForSeconds(0f);

        anim.SetBool("IsAttack", false);
    }

    IEnumerator SpecialAttackCoroutine()
    {
        _isAttacking = true;
        anim.SetBool("IsSpecialAttack", true);
        StopCoroutine("Swing2");
        StartCoroutine("Swing2");

        yield return new WaitForSeconds(0f);
        anim.SetBool("IsSpecialAttack", false);
    }

    public void OnPickUp(InputAction.CallbackContext context)
    {

    }

    public void OnOpenInventory(InputAction.CallbackContext context)
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

    private void PausePlayer()
    {
        isPause = true;
        anim.SetBool("IsMove", false);
        anim.SetBool("IsRun", false);
        _rb.velocity = Vector3.zero;
    }

    private void PlayPlayer() => isPause = false;

}
