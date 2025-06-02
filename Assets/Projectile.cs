using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public AudioClip boltSound;
    private Rigidbody rb;
    private SoundEmitter m_SoundEmitter;
    private AudioSource audioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        m_SoundEmitter = GetComponent<SoundEmitter>();
    }

    public void Throw(Vector3 velocity)
    {
        if (rb != null)
        {
            rb.linearVelocity = velocity;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Hit: " + other.gameObject.name);
        if (m_SoundEmitter != null)
        {
            m_SoundEmitter.EmitSound();
        }

        audioSource.PlayOneShot(boltSound);
    }

}
