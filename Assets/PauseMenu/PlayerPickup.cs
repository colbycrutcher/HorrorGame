using UnityEngine;
using UnityEngine.UI;

public class PlayerPickup : MonoBehaviour
{
    public float interactRange = 3f;
    public KeyCode interactKey = KeyCode.E;
    public Text pickupPromptText; // Assign in inspector

    private GameObject currentPickup;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactRange))
        {
            if (hit.collider.CompareTag("Pickup"))
            {
                currentPickup = hit.collider.gameObject;
                pickupPromptText.gameObject.SetActive(true);

                if (Input.GetKeyDown(interactKey))
                {
                    PickUp(currentPickup);
                }
                return;
            }
        }

        // If not looking at pickup
        pickupPromptText.gameObject.SetActive(false);
        currentPickup = null;
    }

    void PickUp(GameObject obj)
    {
        Debug.Log("Picked up: " + obj.name);
        // Example: destroy or disable the object
        Destroy(obj);
        pickupPromptText.gameObject.SetActive(false);
    }
}
