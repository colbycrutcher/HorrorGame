using UnityEngine;
using System.Collections;

public class MonsterMover : MonoBehaviour
{
    public Transform targetPosition;
    public float moveSpeed = 3f;

    private bool shouldMove = false;

    void Update()
    {
        if (shouldMove)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, moveSpeed * Time.deltaTime);
        }
    }

    public void StartMoving()
    {
        shouldMove = true;
    }
}
