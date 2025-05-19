using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

// used as reference material
// https://www.youtube.com/watch?v=YFhr-pPAkkI 

public class BaseEnemyAI : MonoBehaviour
{
    public NavMeshAgent ai;
    public List<Transform> destinations;
    public Animator aiAnim;
    public float walkSpeed, chaseSpeed, minIdleTime, maxIdleTime, idleTime;
    public bool walking, chasing;
    public Transform player;
    private Transform currentDestination;
    private Vector3 destination;
    private int randNum, randNum2;
    public int destinationAmount;

    private void Start()
    {
        walking = true;
        randNum = Random.Range(0, destinations.Count + 1);
        currentDestination = destinations[randNum];
    }

    private void Update()
    {
        if (walking)
        {
            destination = currentDestination.position;
            ai.destination = destination;
            ai.speed = walkSpeed;

            if (ai.remainingDistance <= ai.stoppingDistance)
            {
                randNum2 = Random.Range(0, 2);

                if (randNum2 == 0)
                {
                    randNum = Random.Range(0, destinationAmount);
                    currentDestination = destinations[randNum];
                }

                if (randNum2 == 1)
                {
                    // Add these in when animations are set
                    // aiAnim.ResetTrigger("Walk");
                    // aiAnim.SetTrigger("Idle");
                    ai.isStopped = true;
                    StopCoroutine(nameof(StayIdle));
                    StartCoroutine(nameof(StayIdle));
                    walking = false;
                }
            }
        }
    }

    private IEnumerator StayIdle()
    {
        idleTime = Random.Range(minIdleTime, maxIdleTime);
        yield return new WaitForSeconds(idleTime);
        walking = true;
        randNum = Random.Range(0, destinationAmount);
        currentDestination = destinations[randNum];
        // Add these back in when animations are set
        // aiAnim.ResetTrigger("Idle");
        // aiAnim.SetTrigger("Walk");
        ai.isStopped = false;
    }
}