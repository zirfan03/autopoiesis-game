using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("References")]
    public Transform swayPivot;
    public PlayerControls playerControls;
    public WeaponAim weaponAim;

    [Header("Position Sway")]
    public float positionAmount = 0.02f;
    public float maxPositionAmount = 0.06f;
    public float positionSmooth = 6f;

    [Header("Rotation Sway")]
    public float rotationAmount = 2f;
    public float maxRotationAmount = 5f;
    public float rotationSmooth = 8f;

    [Header("Aiming Modifier")]
    [Range(0f, 1f)] public float aimMultiplier = 0.1f;

    private Vector3 _targetPosition;
    private Quaternion _targetRotation;

    private void Update()
    {
        if (swayPivot == null || playerControls == null) return;

        Vector2 lookInput = playerControls.GetLookInput();

        float currentMultiplier = (weaponAim != null && weaponAim.IsAiming) ? aimMultiplier : 1f;

        float moveX = Mathf.Clamp(lookInput.x * -positionAmount, -maxPositionAmount, maxPositionAmount) * currentMultiplier;
        float moveY = Mathf.Clamp(lookInput.y * -positionAmount, -maxPositionAmount, maxPositionAmount) * currentMultiplier;

        _targetPosition = new Vector3(moveX, moveY, 0f);

        float tiltX = Mathf.Clamp(lookInput.y * rotationAmount, -maxRotationAmount, maxRotationAmount) * currentMultiplier;
        float tiltY = Mathf.Clamp(lookInput.x * -rotationAmount, -maxRotationAmount, maxRotationAmount) * currentMultiplier;

        _targetRotation = Quaternion.Euler(tiltX, tiltY, tiltY * 0.5f);

        swayPivot.localPosition = Vector3.Lerp(swayPivot.localPosition, _targetPosition, Time.deltaTime * positionSmooth);
        swayPivot.localRotation = Quaternion.Slerp(swayPivot.localRotation, _targetRotation, Time.deltaTime * rotationSmooth);
    }
}