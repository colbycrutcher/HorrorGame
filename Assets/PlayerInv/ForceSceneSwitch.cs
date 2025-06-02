using UnityEngine;
using UnityEngine.SceneManagement;

public class ForceSceneSwitch : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            Debug.Log("F12 pressed — forcing scene load");
            Time.timeScale = 1f; // just in case time is frozen
            SceneManager.LoadScene("MainMenuScene");
        }
    }
}
