using UnityEngine;
using UnityEngine.UI; // or TMPro

public class DoorUnlock : MonoBehaviour, IInteractable
{
    public AudioClip unlockSound;
    public AudioClip lockedSound;            // 🔊 locked sound
    public GameObject doorObject;
    public bool requireAllKeys = true;

    public Text lockedMessageText;           // 🧾 assign this in inspector
    public float messageDisplayTime = 2f;    // ⏱ duration message stays

    private bool isMessageShowing = false;

    public void Interact(PlayerInventory playerInventory)
    {
        if (playerInventory == null) return;

        if (requireAllKeys && playerInventory.HasAllKeys())
        {
            if (unlockSound != null)
                AudioSource.PlayClipAtPoint(unlockSound, transform.position);

            if (doorObject != null)
                Destroy(doorObject);
            else
                Destroy(gameObject);
        }
        else
        {
            // 🔊 Play locked sound
            if (lockedSound != null)
                AudioSource.PlayClipAtPoint(lockedSound, transform.position);

            // 🧾 Show UI message
            if (!isMessageShowing && lockedMessageText != null)
                ShowLockedMessage("The door is locked.");
        }
    }

    void ShowLockedMessage(string message)
    {
        isMessageShowing = true;
        lockedMessageText.text = message;
        lockedMessageText.gameObject.SetActive(true);
        Invoke(nameof(HideLockedMessage), messageDisplayTime);
    }

    void HideLockedMessage()
    {
        lockedMessageText.gameObject.SetActive(false);
        isMessageShowing = false;
    }

    public string GetPromptText()
    {
        return "Unlock Door";
    }
}
