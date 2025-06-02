using System.Collections; // ← this line fixes your error!
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDelayLoader : MonoBehaviour
{
    public string sceneToLoad = "NextScene"; // set in Inspector
    public float delaySeconds = 5f;

    void Start()
    {
        StartCoroutine(LoadSceneAfterDelay());
    }

    IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(delaySeconds);
        SceneManager.LoadScene(sceneToLoad);
    }
}
