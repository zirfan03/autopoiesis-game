using UnityEngine;

public class TeleportDestination : MonoBehaviour
{
    public static string PendingSpawnID;

    [SerializeField] private string spawnID;

    private void Start()
    {
        if (PendingSpawnID != spawnID) return;

        PlayerTeleportation player = FindFirstObjectByType<PlayerTeleportation>();

        if (player != null)
        {
            CharacterController controller = player.GetComponent<CharacterController>();

            if (controller != null)
                controller.enabled = false;

            player.transform.SetPositionAndRotation(
                transform.position,
                transform.rotation
            );

            if (controller != null)
                controller.enabled = true;
        }

        PendingSpawnID = null;
    }
}