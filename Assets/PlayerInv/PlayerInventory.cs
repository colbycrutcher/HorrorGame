using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public bool hasRedKey = false;
    public bool hasGreenKey = false;
    public bool hasBlueKey = false;

    public void PickUpKey(string keyColor)
    {
        switch (keyColor.ToLower())
        {
            case "red":
                hasRedKey = true;
                Debug.Log("Picked up Red Key!");
                break;
            case "green":
                hasGreenKey = true;
                Debug.Log("Picked up Green Key!");
                break;
            case "blue":
                hasBlueKey = true;
                Debug.Log("Picked up Blue Key!");
                break;
            default:
                Debug.LogWarning("Unknown key color: " + keyColor);
                break;
        }
    }

    public bool HasAllKeys()
    {
        return hasRedKey && hasGreenKey && hasBlueKey;
    }
}
