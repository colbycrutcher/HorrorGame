using UnityEngine;

public class PaperBall : MonoBehaviour
{
    void Start()
    {
        Invoke("LateDestroy", 8.0f);
    }

    void LateDestroy()
    {
        Destroy(this.gameObject);
    }
}