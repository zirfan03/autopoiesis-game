using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerControls : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 15f;
    public float sprintSpeed = 25f;
    public float acceleration = 15f;
    public float deceleration = 15f;

    [Header("Movement Directional Multipliers")]
    public float strafeMultiplier = 0.8f;
    public float backwardMultiplier = 0.6f;

    [Header("Jump")]
    public float jumpHeight = 3f;
    public float gravity = -30f;

    [Header("Dodge")]
    public float dodgeDistance = 5f;
    public float dodgeDuration = 0.15f;
    public float dodgeCooldown = 5f;

    [Header("Look")]
    public float mouseSensitivity = 0.2f;
    public float maxLookAngle = 90f;

    [Header("References")]
    public Transform cameraTransform;

    private CharacterController _cc;
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private Vector3 _horizontalVelocity;
    private float _verticalVelocity;
    private float _cameraPitch = 0f;

    private bool _isSprinting;
    private bool _isDodging;
    private float _dodgeCooldownTimer;


    private PlayerControlsInputSystem _inputActions;

    void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _inputActions = new PlayerControlsInputSystem();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    void OnEnable()
    {
        _inputActions.Player.Enable();
        PauseControls.OnGamePaused += HandleGamePaused;
    }

    void OnDisable()
    {
        _inputActions.Player.Disable();
        PauseControls.OnGamePaused -= HandleGamePaused;
    }



    private void OnDestroy()
    {
        _inputActions.Dispose();
    }
    private void HandleGamePaused(bool isPaused)
    {
        if (isPaused)
        {
            _inputActions.Player.Disable();
        }
        else
        {
            _inputActions.Player.Enable();
        }
    }

    public void OnMove(InputAction.CallbackContext ctx)
        => _moveInput = ctx.ReadValue<Vector2>();

    public void OnSprint(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) _isSprinting = true;
        if (ctx.canceled) _isSprinting = false;
    }

    public void OnLook(InputAction.CallbackContext ctx)
        => _lookInput = ctx.ReadValue<Vector2>();

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && _cc.isGrounded && !_isDodging)
        {
            _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    public void OnDodge(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !_isDodging && _dodgeCooldownTimer <= 0f && _cc.isGrounded)
            StartCoroutine(DodgeCoroutine());
    }

    private IEnumerator DodgeCoroutine()
    {
        _isDodging = true;
        _dodgeCooldownTimer = dodgeCooldown;

        Vector2 input = _moveInput != Vector2.zero ? _moveInput : new Vector2(0, -1);
        Vector3 direction = (transform.right * input.x + transform.forward * input.y).normalized;
        if (direction == Vector3.zero) direction = transform.forward;

        float elapsed = 0f;
        Vector3 start = transform.position;
        Vector3 target = start + direction * dodgeDistance;

        while (elapsed < dodgeDuration)
        {
            float t = elapsed / dodgeDuration;
            t = t * t * (3f - 2f * t);

            Vector3 nextPos = Vector3.Lerp(start, target, t);
            _cc.Move((nextPos - transform.position));

            elapsed += Time.deltaTime;
            yield return null;
        }

        _isDodging = false;
    }

    void Update()
    {
        if (PauseControls.Instance != null && PauseControls.Instance.IsPaused) return;

        if (_dodgeCooldownTimer > 0f)
            _dodgeCooldownTimer -= Time.deltaTime;

        HandleMovement();
        HandleLook();
    }

    void HandleMovement()
    {
        if (_isDodging) return;

        if (_cc.isGrounded && _verticalVelocity < 0f)
            _verticalVelocity = -2f;

        _verticalVelocity += gravity * Time.deltaTime;

        Vector3 moveDir = transform.right * _moveInput.x + transform.forward * _moveInput.y;

        float targetSpeed = walkSpeed;

        if (_moveInput.y > 0.1f)
        {
            targetSpeed = _isSprinting ? sprintSpeed : walkSpeed;
        }
        else if (_moveInput.y < -0.1f)
        {
            targetSpeed = walkSpeed * backwardMultiplier;
        }
        else if (Mathf.Abs(_moveInput.x) > 0.1f)
        {
            targetSpeed = walkSpeed * strafeMultiplier;
        }

        if (moveDir.magnitude > 0.1f)
        {
            Vector3 targetVelocity = moveDir.normalized * targetSpeed;
            _horizontalVelocity = Vector3.Lerp(_horizontalVelocity, targetVelocity, acceleration * Time.deltaTime);
        }
        else
        {
            _horizontalVelocity = Vector3.Lerp(_horizontalVelocity, Vector3.zero, deceleration * Time.deltaTime);
        }

        Vector3 totalVelocity = _horizontalVelocity + new Vector3(0f, _verticalVelocity, 0f);
        _cc.Move(totalVelocity * Time.deltaTime);
    }

    void HandleLook()
    {
        transform.Rotate(Vector3.up, _lookInput.x * mouseSensitivity);

        _cameraPitch -= _lookInput.y * mouseSensitivity;
        _cameraPitch = Mathf.Clamp(_cameraPitch, -maxLookAngle, maxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(_cameraPitch, 0f, 0f);
    }

    public Vector3 GetVelocity()
    {
        return _horizontalVelocity + new Vector3(0f, _verticalVelocity, 0f);
    }

    public bool IsGrounded()
    {
        return _cc.isGrounded;
    }

    public Vector2 GetLookInput()
    {
        return _lookInput;
    }
}