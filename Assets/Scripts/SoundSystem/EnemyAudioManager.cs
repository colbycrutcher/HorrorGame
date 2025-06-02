using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyAudioManager : MonoBehaviour
{
    public AudioClip clip1;
    public AudioClip clip2;
    public float intervalSeconds = 7f;

    private AudioSource audioSource;
    private Coroutine soundRoutine;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
    }

    private void OnEnable()
    {
        soundRoutine = StartCoroutine(PlayPeriodicSounds());
    }

    private void OnDisable()
    {
        if (soundRoutine != null)
            StopCoroutine(soundRoutine);
    }

    private IEnumerator PlayPeriodicSounds()
    {
        while (true)
        {
            yield return new WaitForSeconds(intervalSeconds);

            AudioClip chosenClip = (Random.value < 0.5f) ? clip1 : clip2;
            if (chosenClip != null)
            {
                audioSource.PlayOneShot(chosenClip);
            }
        }
    }
}