using UnityEngine;

public class LoreFloppy : MonoBehaviour
{
    public AudioClip voiceLine;
    public string floppyID;
    public GameObject interactionPromptUI;

    private AudioSource audioSource;
    private Transform player;
    private bool isInRange = false;
    private bool hasBeenCollected = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = voiceLine;
        audioSource.spatialBlend = 1f;
        audioSource.playOnAwake = false;
        audioSource.volume = 0.8f;

        if (interactionPromptUI != null)
            interactionPromptUI.SetActive(false);
    }

    void Update()
    {
        if (isInRange && Input.GetKeyDown(KeyCode.E) && !hasBeenCollected)
        {
            PlayVoiceLine();
            hasBeenCollected = true;

            if (interactionPromptUI != null)
                interactionPromptUI.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
            if (!hasBeenCollected && interactionPromptUI != null)
                interactionPromptUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
            if (interactionPromptUI != null)
                interactionPromptUI.SetActive(false);
        }
    }

    void PlayVoiceLine()
    {
        if (audioSource && voiceLine != null && !audioSource.isPlaying)
        {
            audioSource.Play();
            Debug.Log($"Lore floppy collected: {floppyID}");
        }
    }
}