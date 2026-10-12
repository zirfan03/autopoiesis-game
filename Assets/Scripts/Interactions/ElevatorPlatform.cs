using UnityEngine;

[DefaultExecutionOrder(-100)]
public class ElevatorPlatform : MonoBehaviour
{
    private Transform _playerTransform;

    private Vector3 _lastPosition;
    private Vector3 _lastEulerAngles;

    private void Start()
    {
        _lastPosition = transform.position;
        _lastEulerAngles = transform.eulerAngles;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerTransform = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerTransform = null;
        }
    }

    private void LateUpdate()
    {
        if (Time.timeScale == 0f) return;

        Vector3 positionDelta = transform.position - _lastPosition;
        Vector3 currentEuler = transform.eulerAngles;
        float yRotationDelta = Mathf.DeltaAngle(_lastEulerAngles.y, currentEuler.y);

        if (_playerTransform != null)
        {
            _playerTransform.position += positionDelta;

            if (Mathf.Abs(yRotationDelta) > 0.001f)
            {
                _playerTransform.Rotate(0f, yRotationDelta, 0f);
            }
        }

        if (positionDelta.sqrMagnitude > 0f || Mathf.Abs(yRotationDelta) > 0.001f)
        {
            Physics.SyncTransforms();
        }

        _lastPosition = transform.position;
        _lastEulerAngles = currentEuler;
    }
}