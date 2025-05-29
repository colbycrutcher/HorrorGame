using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenuFromGame : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Scenes/MainMenuScene");
        Debug.Log("Returning To MainMenu");
    }
}