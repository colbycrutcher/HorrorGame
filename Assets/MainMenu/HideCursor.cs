using UnityEngine;

public class HideCursor : MonoBehaviour
{
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideCursorAgain()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}