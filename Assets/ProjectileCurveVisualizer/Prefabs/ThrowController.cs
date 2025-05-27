using ProjectileCurveVisualizerSystem;
using UnityEngine;

public class ThrowController : MonoBehaviour
{
    public ProjectileCurveVisualizer projectileCurveVisualizer;
    public GameObject projectilePrefab;
    public float launchSpeed = 15f;

    private Vector3 updatedProjectileStartPosition;
    private Vector3 projectileLaunchVelocity;
    private Vector3 predictedTargetPosition;
    private RaycastHit hit;
    private bool canHitTarget = false;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Hold right-click to aim
        if (Input.GetMouseButton(1))
        {
            if (Physics.Raycast(ray, out RaycastHit mouseRaycastHit))
            {
                canHitTarget = projectileCurveVisualizer.VisualizeProjectileCurveWithTargetPosition(
                    transform.position,
                    1.5f,
                    mouseRaycastHit.point,
                    launchSpeed,
                    Vector3.zero,
                    Vector3.zero,
                    0.05f,
                    0.1f,
                    true,
                    out updatedProjectileStartPosition,
                    out projectileLaunchVelocity,
                    out predictedTargetPosition,
                    out hit
                );
            }
        }
        else
        {
            projectileCurveVisualizer.HideProjectileCurve();
            canHitTarget = false;
        }

        // Left-click to throw
        if (Input.GetMouseButtonDown(0) && canHitTarget)
        {
            ThrowProjectile();
        }
    }

    void ThrowProjectile()
    {
        if (projectilePrefab != null)
        {
            var projectile = Instantiate(projectilePrefab);
            projectile.transform.position = updatedProjectileStartPosition;

            Projectile projScript = projectile.GetComponent<Projectile>();
            if (projScript != null)
            {
                projScript.Throw(projectileLaunchVelocity);
            }
        }
    }
}
