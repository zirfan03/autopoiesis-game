using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [Header("Configuration")]
    public bool enableHeadBob = true;
    public float bobSpeed = 7f;
    public float bobAmount = 0.05f;
    public float bobMultiplierSprint = 1.5f;

    [Header("References")]
    public Transform cameraTransform;
    public PlayerControls playerControls;

    private float _defaultCameraY;
    private float _bobTimer;

    private void Start()
    {
        if (cameraTransform != null)
        {
            _defaultCameraY = cameraTransform.localPosition.y;
        }
    }

    private void Update()
    {
        if (!enableHeadBob || cameraTransform == null || playerControls == null) return;

        Vector3 velocity = playerControls.GetVelocity();
        float speed = new Vector3(velocity.x, 0, velocity.z).magnitude;
        bool isGrounded = playerControls.IsGrounded();

        Vector3 targetCameraPosition = cameraTransform.localPosition;

        if (speed > 0.1f && isGrounded)
        {
            float currentSpeedMultiplier = speed > playerControls.walkSpeed + 1f ? bobMultiplierSprint : 1f;

            _bobTimer += Time.deltaTime * bobSpeed * currentSpeedMultiplier;

            targetCameraPosition.y = _defaultCameraY + Mathf.Sin(_bobTimer) * bobAmount;
            targetCameraPosition.x = Mathf.Cos(_bobTimer / 2f) * bobAmount;
        }
        else
        {
            _bobTimer = 0f;
            targetCameraPosition.y = _defaultCameraY;
            targetCameraPosition.x = 0f;
        }

        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetCameraPosition, Time.deltaTime * 10f);
        
        
    }
}