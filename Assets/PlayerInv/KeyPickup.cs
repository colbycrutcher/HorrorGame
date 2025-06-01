using UnityEngine;

public class KeyPickup : MonoBehaviour, IInteractable
{
    public string keyColor = "red";

    public void Interact(PlayerInventory playerInventory)
    {
        Debug.Log($"Picked up {keyColor} key");
        playerInventory.PickUpKey(keyColor);
        Destroy(gameObject);
    }

    public string GetPromptText()
    {
        return $"Pick up {keyColor} key";
    }

}
