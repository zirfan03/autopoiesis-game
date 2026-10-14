using UnityEngine;

public class ElevatorReturnSequence : MonoBehaviour
{
    [Header("Elevator Reference")]
    public WaypointElevator targetElevator;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;

            if (targetElevator != null)
            {
                targetElevator.StartReturnSequence();
            }
            else
            {
                Debug.LogWarning("Target Elevator not assigned in the Return Trigger");
            }
        }
    }
}
