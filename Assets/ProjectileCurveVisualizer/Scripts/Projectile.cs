using System;
using Unity.VisualScripting;
using UnityEngine;

namespace ProjectileCurveVisualizerSystem
{
    public class Projectile : MonoBehaviour
    {
        public Rigidbody projectileRigidbody;
        public MeshCollider projectileMeshCollider;
        private SoundEmitter m_SoundEmitter;

        private void Start()
        {
            m_SoundEmitter = GetComponent<SoundEmitter>();
        }

        public void Throw(Vector3 velocity)
        {
            projectileMeshCollider.enabled = true;
            projectileRigidbody.useGravity = true;

            projectileRigidbody.linearVelocity = velocity;
        }

        private void OnCollisionEnter(Collision other)
        {
            Debug.Log("Hit: " + other.gameObject.name);
            if (m_SoundEmitter != null)
            {
                m_SoundEmitter.EmitSound();
            }
            // play audio file sound here
        }
    }
}