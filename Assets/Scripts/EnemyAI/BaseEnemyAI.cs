using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

// used as reference material, huge thanks to Omogonix for the core logic
// https://www.youtube.com/watch?v=YFhr-pPAkkI 

public class BaseEnemyAI : MonoBehaviour
{
    public NavMeshAgent ai;
    public List<Transform> destinations;
    public Animator aiAnim;
    public float walkSpeed, chaseSpeed, minIdleTime, maxIdleTime, idleTime, catchDistance;
    public bool walking, chasing;
    public Transform player;
    private Transform currentDestination;
    private Vector3 destination, lastHeardPosition;
    private int randNum, randNum2;
    public int destinationAmount;
    private Coroutine activeChaseRoutine;

    private void Start()
    {
        walking = true;
        chasing = false;
        randNum = Random.Range(0, destinations.Count + 1);
        currentDestination = destinations[randNum];
    }

    private void Update()
    {
        // Player is caught logic
        if (chasing && Vector3.Distance(transform.position, player.position) <= catchDistance)
        {
            player.gameObject.SetActive(false);
            chasing = false;
            ai.isStopped = true;
            // TODO Add animation triggers back in
            // aiAnim.SetTrigger("Idle");
        }

        //if (searching)
        //{
        //    TODO logic that handles how the enemy searches for the player
        //}

        //if (chasing)
        //{
        //    destination = player.position;
        //    ai.destination = destination;
        //    ai.speed = chaseSpeed;
        //    if (ai.remainingDistance <= catchDistance)
        //    {
        //        player.gameObject.SetActive(false);
        //        //TODO reset animation triggers here

        //        StartCoroutine(deathRoutine());
        //        chasing = false;
        //    }
        //}
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
                    // TODO Add these in when animations are set
                    
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
//-------------------------------- Routines --------------------------------
    // The idle routine for the enemy character
    private IEnumerator StayIdle()
    {
        idleTime = Random.Range(minIdleTime, maxIdleTime);
        yield return new WaitForSeconds(idleTime);
        walking = true;
        randNum = Random.Range(0, destinationAmount);
        currentDestination = destinations[randNum];
        
        // TODO Add animation triggers back in
        // aiAnim.ResetTrigger("Idle");
        // aiAnim.SetTrigger("Walk");
        
        ai.isStopped = false;
    }

    private IEnumerator ChaseRoutine(Vector3 soundPosition)
    {
        chasing = true;
        walking = false;
        ai.isStopped = false;
        ai.speed = chaseSpeed;

        lastHeardPosition = soundPosition;
        ai.destination = lastHeardPosition;

        // TODO Add animation triggers back in
        // aiAnim.SetTrigger("Chase");

        while (chasing)
        {
            ai.destination = lastHeardPosition;
            // player is caught
            if (Vector3.Distance(transform.position, player.position) <= catchDistance)
            {
                player.gameObject.SetActive(false);
                chasing = false;
                ai.isStopped = true;
                // TODO Add animation triggers back in
                // aiAnim.SetTrigger("Idle");
                yield break;
            }

            // If the AI reaches the last known sound position, stop chasing
            if (Vector3.Distance(transform.position, lastHeardPosition) <= ai.stoppingDistance)
            {
                chasing = false;
                walking = true;
                StartCoroutine(nameof(StayIdle));
                yield break;
            }

            yield return null;
        }
    }

    // The routine that allows the player to escape a death
    //private IEnumerator EscapeRoutine()
    //{
    //    TODO write escape routine
    //}

    // The game over routine when a player can no longer escape
    //private IEnumerator DeathRoutine()
    //{
    //    TODO write death routine
    //}

    // The routine where the enemy listens for and searches for the player
    //private IEnumerator SearchRoutine()
    //{
    //    TODO write search routine
    //}

    // The routine where the enemy hears a sound under a threshold and listens for confirmation
    //private IEnumerator ListeningRoutine()
    //{
    //    TODO write listening routing
    //}

    //-------------------------------- Helpers --------------------------------
    public void OnHeardSound(Vector3 soundPosition)
    {
        // Update the last heard position
        lastHeardPosition = soundPosition;

        // Interrupt any existing chase
        if (!chasing)
        {
            chasing = true;
            walking = false;
            StopCoroutine(nameof(StayIdle));
            StartCoroutine(ChaseRoutine(soundPosition));
        }
    }
}