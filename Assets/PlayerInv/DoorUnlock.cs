using UnityEngine;

public class DoorUnlock : MonoBehaviour, IInteractable
{
    public void Interact(PlayerInventory playerInventory)
    {
        if (playerInventory.HasAllKeys())
        {
            Debug.Log("Unlocked door!");
            Destroy(gameObject); // Door disappears
        }
        else
        {
            Debug.Log("You need all 3 keys to unlock the door.");
        }
    }

    public string GetPromptText()
    {
        return "Unlock door";
    }
}
