using UnityEngine;
using ProjectileCurveVisualizerSystem;

public class ProjectileThrower : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject projectilePrefab;
    public Transform throwOrigin;
    public float throwForce = 30f;
    public float projectileRadius = 0.1f;

    public ProjectileCurveVisualizer curveVisualizer;

    private GameObject currentProjectile;

    void Update()
    {
        // Show curve prediction
        if (Input.GetMouseButton(1)) // Right-click to visualize
        {
            Vector3 launchVelocity = playerCamera.transform.forward * throwForce;

            curveVisualizer.VisualizeProjectileCurve(
                throwOrigin.position,
                0f,
                launchVelocity,
                projectileRadius,
                0.1f,
                false,
                out Vector3 updatedStartPos,
                out RaycastHit hit
            );
        }
        else
        {
            curveVisualizer.HideProjectileCurve();
        }

        // Throw projectile
        if (Input.GetMouseButtonDown(0)) // Left-click to throw
        {
            ThrowProjectile();
        }
    }

    void ThrowProjectile()
    {
        Vector3 direction = playerCamera.transform.forward;
        GameObject projectile = Instantiate(projectilePrefab, throwOrigin.position, Quaternion.identity);

        if (projectile.TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = direction * throwForce;
        }
    }
}
