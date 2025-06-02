using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Vector3 = UnityEngine.Vector3;

// used as reference material, huge thanks to Omogonix for the core logic
// https://www.youtube.com/watch?v=YFhr-pPAkkI 

// Usage:
// Attach to an enemy object. Set Destinations and adjust

public class BaseEnemyAI : MonoBehaviour
{
    public NavMeshAgent ai;
    public List<Transform> destinations;
    public Animator aiAnim;
    public float walkSpeed, chaseSpeed, minIdleTime, maxIdleTime, idleTime, catchDistance, jumpScareTime;
    public bool walking, chasing, listening;
    public Transform player;
    private Transform currentDestination;
    private Vector3 destination, lastHeardPosition;
    private int randNum, randNum2;
    public int destinationAmount;
    private Coroutine activeRoutine;
    public string deathScene;

    //Enum for Animation states
    private enum AIAnimState { Idle = 0, Walk = 1, Chase = 2, Listen = 3 }


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
            if (aiAnim) aiAnim.SetInteger("State", (int)AIAnimState.Idle);
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
                    if (aiAnim) aiAnim.SetInteger("State", (int)AIAnimState.Idle);
                    if (aiAnim) aiAnim.SetInteger("State", (int)AIAnimState.Walk);

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
        if (aiAnim) aiAnim.SetInteger("State", (int)AIAnimState.Idle);

        if (aiAnim) aiAnim.SetInteger("State", (int)AIAnimState.Walk);

        ai.isStopped = false;
    }

    private IEnumerator ChaseRoutine(Vector3 initialSoundPosition)
    {
        chasing = true;
        walking = false;
        ai.isStopped = false;
        ai.speed = chaseSpeed;

        lastHeardPosition = initialSoundPosition;
        ai.destination = lastHeardPosition;
        Vector3 previousPlayerPosition = player.position;
        Vector3 lastKnownPlayerDirection = (player.position - transform.position).normalized;

        // TODO Add animation triggers back in
        // aiAnim.SetTrigger("Chase");
        if (aiAnim) aiAnim.SetInteger("State", (int)AIAnimState.Chase);

        Debug.Log("AI started chasing towards " + lastHeardPosition);

        while (chasing)
        {
            // Calculate player trajectory
            Vector3 playerMovement = player.position - previousPlayerPosition;
            previousPlayerPosition = player.position;

            // Bias the chase towards the player's current movement
            //Vector3 predictiveOffset = playerMovement.normalized * 5f;
            //Vector3 pursuitPoint = player.position + predictiveOffset;

            // Add some randomness
            Vector3 randomOffset = Random.insideUnitSphere * 2f;
            randomOffset.y = 0;
            //pursuitPoint += randomOffset;

            //ai.destination = pursuitPoint;

            // player is caught
            if (Vector3.Distance(transform.position, player.position) <= catchDistance)
            {
                player.gameObject.SetActive(false);
                chasing = false;
                ai.isStopped = true;
                // TODO Add animation triggers back in
                // aiAnim.SetTrigger("Idle");
                if (aiAnim) aiAnim.SetInteger("State", (int)AIAnimState.Chase);
                Debug.Log("You have Been caught");
                StartCoroutine(nameof(DeathRoutine));
                yield break;
            }

            // If the AI reaches the last known sound position, stop chasing
            if (Vector3.Distance(transform.position, lastHeardPosition) <= ai.stoppingDistance)
            {
                chasing = false;
                walking = true;
                Debug.Log("AI reached last known sound position. Returning to patrol.");
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
    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(jumpScareTime);
        SceneManager.LoadScene(deathScene);
    }

    // The routine where the enemy listens for and searches for the player
    //private IEnumerator SearchRoutine()
    //{
    //    TODO write search routine
    //}

    //The routine where the enemy hears a sound under a threshold and listens for confirmation
    private IEnumerator ListenRoutine()
    {
        Debug.Log("AI is listening...");
        // Give player some time to stop moving before listening begins
        yield return new WaitForSeconds(0.6f);

        ai.isStopped = true;
        float listenDuration = Random.Range(minIdleTime, maxIdleTime);
        float elapsedTime = 0f;

        // TODO Add animation triggers back in
        // aiAnim.SetTrigger("Listen");
        if (aiAnim) aiAnim.SetInteger("State", (int)AIAnimState.Listen);

        Vector3 initialHeardPosition = lastHeardPosition;
        bool noiseConfirmed = false;

        while (elapsedTime < listenDuration)
        {
            // Check to confirm if noise source has moved significantly
            if (Vector3.Distance(initialHeardPosition, lastHeardPosition) > 0.5f)
            {
                Debug.Log("AI has confirmed noise... Switching to chase.");
                noiseConfirmed = true;
                break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        listening = false;
        if (noiseConfirmed)
        {
            chasing = true;
            walking = false;
            Debug.Log("AI is chasing sound at " + lastHeardPosition);
            activeRoutine = StartCoroutine(ChaseRoutine(lastHeardPosition));
        }
        else
        {
            walking = true;
            Debug.Log("AI no longer is interested in the noise. Returning to patrol routine.");
            activeRoutine = StartCoroutine(nameof(StayIdle));
        }
    }

    //-------------------------------- Helpers --------------------------------
    public void OnHeardSound(Vector3 soundPosition)
    {
        if (listening)
        {
            lastHeardPosition = soundPosition;
            Debug.Log("AI already listening, updated sound position to: " + lastHeardPosition);
            return;
        }

        // Update the last heard position
        lastHeardPosition = soundPosition;
        chasing = false;
        walking = false;
        listening = true;

        // Interrupt any existing chase
        if (activeRoutine != null)
        {
            StopCoroutine(activeRoutine);
            Debug.Log("AI stopped active routine from OnHeardSound.");
        }

        Debug.Log("AI is entering into listening state from OnHeardSound");
        activeRoutine = StartCoroutine(ListenRoutine());
    }
}