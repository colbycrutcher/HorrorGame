using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameScript : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Scenes/EricSandbox");
        Debug.Log("Starting Game");
        //Just to make sure its working?
    }
}
