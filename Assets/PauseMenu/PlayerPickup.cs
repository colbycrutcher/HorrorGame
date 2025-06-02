using UnityEngine;
using UnityEngine.UI; // Use TextMeshPro if using TMP

public class PlayerPickup : MonoBehaviour
{
    [Header("Interaction")]
    public float interactRange = 3f;
    public KeyCode interactKey = KeyCode.E;

    [Header("UI Prompt")]
    public Text pickupPromptText; // Assign in Inspector (or TMP if using TextMeshProUGUI)

    private PlayerInventory playerInventory;
    private IInteractable currentInteractable;

    void Start()
    {
        playerInventory = GetComponent<PlayerInventory>();
        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventory component not found on " + gameObject.name);
        }

        if (pickupPromptText != null)
            pickupPromptText.gameObject.SetActive(false);
    }

    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * interactRange, Color.green);

        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != null)
            {
                currentInteractable = interactable;
                pickupPromptText.text = currentInteractable.GetPromptText();
                pickupPromptText.gameObject.SetActive(true);

                if (Input.GetKeyDown(interactKey))
                {
                    currentInteractable.Interact(playerInventory);
                }

                return;
            }
        }

        // Hide prompt if not looking at anything interactable
        pickupPromptText.gameObject.SetActive(false);
        currentInteractable = null;
    }
}
