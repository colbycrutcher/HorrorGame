using UnityEngine;

public class MonsterTrigger : MonoBehaviour
{
    public MonsterMover monster; // Drag the monster GameObject here in Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            monster.StartMoving();
            Destroy(gameObject); // Remove trigger after activation
        }
    }
}
