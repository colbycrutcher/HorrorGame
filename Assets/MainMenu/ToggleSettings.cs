using UnityEngine;

public class ToggleSettings : MonoBehaviour
{
    public GameObject targetObject;  // Assign the object to toggle in the Inspector

    public void ToggleVisibility()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(!targetObject.activeSelf);
        }
    }
}
