using UnityEngine;
using UnityEngine.UI; // or TMPro
using UnityEngine.Video; // required for VideoPlayer
using UnityEngine.SceneManagement;


public class DoorUnlock : MonoBehaviour, IInteractable
{
    public AudioClip unlockSound;
    public AudioClip lockedSound;
    public GameObject doorObject;
    public bool requireAllKeys = true;

    public GameObject videoOverlay;        // 🔁 RawImage GameObject (disabled by default)
    public VideoPlayer videoPlayer;  // 🔁 Assign the VideoPlayer component
   
    public GameObject exitVideoPrompt;      // The "Press ESC to exit" UI
    private bool isVideoReadyToExit = false; // replaces isVideoPlaying


    public Text lockedMessageText;
    public float messageDisplayTime = 2f;
    private bool isMessageShowing = false;




    public void Interact(PlayerInventory playerInventory)
    {
        if (playerInventory == null) return;

        if (requireAllKeys && playerInventory.HasAllKeys())
        {
            Debug.Log("Unlocked door!");

            if (unlockSound != null)
                AudioSource.PlayClipAtPoint(unlockSound, transform.position);

            // Remove door
            if (doorObject != null)
                Destroy(doorObject);
            else
                Destroy(gameObject);

            // ✅ Trigger video
            if (videoOverlay != null && videoPlayer != null)
            {
                videoOverlay.SetActive(true);
                videoPlayer.Play();

                // Only trigger once
                videoPlayer.loopPointReached -= OnVideoEnd;
                videoPlayer.loopPointReached += OnVideoEnd;
            }


        }
        else
        {
            // 🔊 Play locked sound
            if (lockedSound != null)
                AudioSource.PlayClipAtPoint(lockedSound, transform.position);

            // 🧾 Show locked UI message
            if (!isMessageShowing && lockedMessageText != null)
                ShowLockedMessage("The door is locked.");
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        Debug.Log("Video ended, waiting for E to exit...");
        isVideoReadyToExit = true;

        if (exitVideoPrompt != null)
            exitVideoPrompt.SetActive(true);
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

    void Update()
    {
        if (isVideoReadyToExit && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E pressed — loading MainMenuScene...");
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenuScene");
        }
    }





}
