using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEmitter : MonoBehaviour
{
    // Attach this script to objects that can emit sounds, call in event scripts
    // For small noises (i.e., footsteps) - small baseNoiseRadius - (use a higher intensityMultiplier for running)
    // For large noises (i.e., door slam) - high baseNoiseRadius - (moderate intensityMultiplier)
    public float baseNoiseRadius = 5f;
    public float maxNoiseRadius = 15f;
    public float radiusDecayRate = 2f;
    public float noiseCooldown = 1f;
    public LayerMask enemyLayer;

    [Header("Sound Intensity Settings")] 
    [Range(0f, 2f)] public float intensityMultiplier = 1f;
    [Range(0f, 1f)] public float randomRadiusVariation = 0.2f;
    
    public bool showGizmos = true;
    public Color gizmoColor = Color.red;
    
    private float lastNoiseTime;
    
    public void EmitSound()
    {
        if (Time.time - lastNoiseTime < noiseCooldown)
            return;

        float randomVariation = Random.Range(-randomRadiusVariation, randomRadiusVariation);
        float actualRadius = Mathf.Clamp(baseNoiseRadius * intensityMultiplier * (1 + randomVariation), baseNoiseRadius, maxNoiseRadius);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, actualRadius, enemyLayer);
        foreach (var hitCollider in hitColliders)
        {
            BaseEnemyAI enemyAI = hitCollider.GetComponent<BaseEnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.OnHeardSound(transform.position);
            }
        }

        lastNoiseTime = Time.time;
    }

    private void OnDrawGizmosSelected()
    {
        if (showGizmos)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, baseNoiseRadius);
            Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.3f);
            Gizmos.DrawWireSphere(transform.position, maxNoiseRadius);
        }
    }
}