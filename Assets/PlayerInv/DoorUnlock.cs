using UnityEngine;

public class DoorUnlock : MonoBehaviour, IInteractable
{
    public AudioClip unlockSound;
    public GameObject doorObject; // Optional: assign specific part of the door
    public bool requireAllKeys = true;

    public void Interact(PlayerInventory playerInventory)
    {
        if (playerInventory == null)
        {
            Debug.LogWarning("No player inventory!");
            return;
        }

        if (requireAllKeys && playerInventory.HasAllKeys())
        {
            Debug.Log("Unlocked door!");

            if (unlockSound != null)
                AudioSource.PlayClipAtPoint(unlockSound, transform.position);

            if (doorObject != null)
                Destroy(doorObject); // remove the door mesh
            else
                Destroy(gameObject); // or remove whole door

        }
        else
        {
            Debug.Log("Door locked. You need all 3 keys.");
        }
    }

    public string GetPromptText()
    {
        return "Unlock Door";
    }
}
