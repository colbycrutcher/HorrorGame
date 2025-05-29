using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private Transform movingPlatformTransform;

    public Vector3 destination1, destination2;

    public float speed = 4.0f;

    private bool toDestination2;

    void Start()
    {
        movingPlatformTransform = this.transform;
    }

    void Update()
    {
        if (toDestination2)
        {
            movingPlatformTransform.position += (destination2 - movingPlatformTransform.position).normalized * speed * Time.deltaTime;

            if (Vector3.Distance(movingPlatformTransform.position, destination2) < 1.0f)
                toDestination2 = false;
        }
        else
        {
            movingPlatformTransform.position += (destination1 - movingPlatformTransform.position).normalized * speed * Time.deltaTime;

            if (Vector3.Distance(movingPlatformTransform.position, destination1) < 1.0f)
                toDestination2 = true;
        }
    }
}