using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("References")]
    public Transform swayPivot;
    public PlayerControls playerControls;
    public WeaponAim weaponAim;

    [Header("Position Look Sway")]
    public float positionAmount = 0.0625f;
    public float maxPositionAmount = 0.125f;
    public float positionSmooth = 6f;

    [Header("Rotation Look Sway")]
    public float rotationAmount = 1f;
    public float maxRotationAmount = 2f;
    public float rotationSmooth = 8f;

    [Header("Weapon Bob Movement Sway")]
    public float bobSpeed = 10f;
    public float bobAmountX = 0.125f;
    public float bobAmountY = 0.0625f;

    [Header("Aiming Modifier")]
    [Range(0f, 1f)] public float aimMultiplier = 0.25f;

    private Vector3 _targetPosition;
    private Quaternion _targetRotation;
    private float _bobTimer;

    private void Update()
    {
        if (swayPivot == null || playerControls == null) return;

        Vector2 lookInput = playerControls.GetLookInput();

        Vector3 velocity = playerControls.GetVelocity();
        float speed = new Vector3(velocity.x, 0, velocity.z).magnitude;
        bool isGrounded = playerControls.IsGrounded();

        float currentMultiplier = (weaponAim != null && weaponAim.IsAiming) ? aimMultiplier : 1f;

        float moveX = Mathf.Clamp(lookInput.x * -positionAmount, -maxPositionAmount, maxPositionAmount) * currentMultiplier;
        float moveY = Mathf.Clamp(lookInput.y * -positionAmount, -maxPositionAmount, maxPositionAmount) * currentMultiplier;


        float tiltX = Mathf.Clamp(lookInput.y * rotationAmount, -maxRotationAmount, maxRotationAmount) * currentMultiplier;
        float tiltY = Mathf.Clamp(lookInput.x * -rotationAmount, -maxRotationAmount, maxRotationAmount) * currentMultiplier;

        float bobX = 0f;
        float bobY = 0f;

        if (speed > 1f && isGrounded)
        {
            float speedMultiplier = speed > playerControls.walkSpeed + 1f ? 1.5f : 1f;
            _bobTimer += Time.deltaTime * bobSpeed * speedMultiplier;

            bobX = Mathf.Cos(_bobTimer / 2f) * bobAmountX * currentMultiplier;
            bobY = Mathf.Sin(_bobTimer) * bobAmountY * currentMultiplier;
        }
        else
        {
            _bobTimer = 0f;
        }

        _targetPosition = new Vector3(moveX + bobX, moveY + bobY, 0f);
        _targetRotation = Quaternion.Euler(tiltX, tiltY, tiltY * 0.5f);

        swayPivot.localPosition = Vector3.Lerp(swayPivot.localPosition, _targetPosition, Time.deltaTime * positionSmooth);
        swayPivot.localRotation = Quaternion.Slerp(swayPivot.localRotation, _targetRotation, Time.deltaTime * rotationSmooth);
    }
}