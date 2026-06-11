using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 15f;
    public float jumpHeight = 5f;
    public float gravity = -19.62f;

    [Header("Sprinting")]
    public float sprintSpeed = 45f;
    private bool _isSprinting;

    [Header("Dodge")]
    public float dodgeSpeed = 100f;
    public float dodgeDuration = 0.1f;
    public float dodgeCooldown = 2.5f;

    private bool _isDodging;
    private float _dodgeCooldownTimer;
    private Vector3 _dodgeDirection;

    [Header("Look")]
    public float mouseSensitivity = 0.2f;
    public float maxLookAngle = 90f;

    [Header("References")]
    public Transform cameraTransform;

    private CharacterController _cc;
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private Vector3 _velocity;
    private float _cameraPitch = 0f;

    void Awake()
    {
        _cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
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
        if (ctx.performed && _cc.isGrounded)
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    public void OnDodge(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && !_isDodging && _dodgeCooldownTimer <= 0f)
            StartCoroutine(DodgeCoroutine());
    }

    private IEnumerator DodgeCoroutine()
    {
        _isDodging = true;
        _dodgeCooldownTimer = dodgeCooldown;

        Vector2 input = _moveInput != Vector2.zero ? _moveInput : new Vector2(0, -1);
        _dodgeDirection = transform.right * input.x + transform.forward * input.y;

        float elapsed = 0f;
        while (elapsed < dodgeDuration)
        {
            _cc.Move(_dodgeDirection * dodgeSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        _isDodging = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_dodgeCooldownTimer > 0f)
            _dodgeCooldownTimer -= Time.deltaTime;

        HandleMovement();
        HandleLook();
    }

    void HandleMovement()
    {
        if (_isDodging) return;

        if (_cc.isGrounded && _velocity.y < 0f)
            _velocity.y = -2f;

        _velocity.y += gravity * Time.deltaTime;

        Vector3 move = transform.right * _moveInput.x + transform.forward * _moveInput.y;

        float currentSpeed = _isSprinting ? sprintSpeed : moveSpeed;
        _cc.Move((move * currentSpeed + _velocity) * Time.deltaTime);
    }

    void HandleLook()
    {
        transform.Rotate(Vector3.up, _lookInput.x * mouseSensitivity);

        _cameraPitch -= _lookInput.y * mouseSensitivity;
        _cameraPitch = Mathf.Clamp(_cameraPitch, -maxLookAngle, maxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(_cameraPitch, 0f, 0f);
    }
}
