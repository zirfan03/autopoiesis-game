using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class TeleporterPad : MonoBehaviour
{
    [Header("Destination")]
    [SerializeField] private string destinationScene;
    [SerializeField] private string destinationSpawnID;

    private bool isTeleporting;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerTeleportation player = other.GetComponentInParent<PlayerTeleportation>();

        if (player != null)
            player.EnterPad(this);
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerTeleportation player = other.GetComponentInParent<PlayerTeleportation>();

        if (player != null)
            player.ExitPad(this);
    }

    public void Teleport()
    {
        if (isTeleporting) return;

        isTeleporting = true;
        TeleportDestination.PendingSpawnID = destinationSpawnID;

        SceneManager.LoadSceneAsync(destinationScene, LoadSceneMode.Single);
    }
}