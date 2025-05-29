using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitFunctionality : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Exiting Game");
        //Just to make sure its working?
    }
}
