using UnityEngine;

public class MonsterTrigger : MonoBehaviour
{
    public MonsterMover monster;         // Assign in Inspector
    public AudioSource triggerSound;     // Assign in Inspector (AudioSource component)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            monster.StartMoving();

            if (triggerSound != null)
                triggerSound.Play();

            Destroy(gameObject, triggerSound.clip.length); // Optional: delay destroy until sound finishes
        }
    }
}
