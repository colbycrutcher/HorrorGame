using UnityEngine;

public class Settings : MonoBehaviour
{
    public static bool mouseControl = false;

    void Update()
    {
        if (!mouseControl && Input.GetMouseButtonUp(0))
        {
            LockMouse();

            Destroy(GameObject.Find("Canvas"));
        }
    }

    void LockMouse()
    {
        mouseControl = true;

        // Lock mouse cursor in the view
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}