using System.Collections.Generic;
using UnityEngine;

namespace ProjectileCurveVisualizerSystem
{
    public class ProjectileCurveVisualizer : MonoBehaviour
    {
        private LineRenderer lineRenderer;

        public Transform projectileTargetPlaneTransform;
        private MeshRenderer projectileTargetPlaneMeshRenderer;

        public LayerMask ignoredLayers;

        public int curveSubdivision = 32;
        public float maximumInAirTime = 6.0f;
        public float detectionInterval = 0.5f;

        public bool getHitObjectTransform = false;
        public bool calculateProjectileVelocityWhenHit = false;

        private Vector3 horizontalVector;
        private float horizontalDisplacement;
        private float horizontalTime;

        // Projectile variables
        private Vector3 previousDetectionPosition = Vector3.zero;
        private Vector3 nextDetectionPosition = Vector3.zero;
        private float t;
        public List<Vector3> detectionPositionList = new List<Vector3>();
        private Collider[] hitColliderArray = new Collider[1];
        private bool notHit = true;
        private RaycastHit defaultRaycastHit;
        private Vector3 rayDirection;
        private float rayLength;
        public Vector3 hitPosition;
        private Vector3 hitNormal;

        private float predictedProjectileTravelTime;
        private Vector3 horizontalDirection;
        private float horizontalDistance;
        private float deltaHeight;
        private float launchSpeedSquare;
        private float component;
        private float launchAngle;

        // Bézier curve variables
        private Vector3 startPoint = Vector3.zero;
        private Vector3 endPoint = Vector3.zero;
        private Vector3 controlPoint = Vector3.zero;

        public Transform hitObjectTransform;
        public Vector3 projectileVelocityWhenHit;
        public ProjectileCurveVisualizer projectileCurveVisualizer;

        void Awake()
        {
            // Initialize variables
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = curveSubdivision;

            projectileTargetPlaneMeshRenderer = projectileTargetPlaneTransform.GetComponent<MeshRenderer>();

            Collider[] hitColliderArray = new Collider[1];

            defaultRaycastHit = new RaycastHit();
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(hitPosition, 0.25f);
        }

        public void VisualizeProjectileCurve(Vector3 projectileStartPosition, float projectileStartPositionForwardOffset, Vector3 launchVelocity, float projectileRadius, float distanceOffsetAboveHitPosition, bool debugMode, out Vector3 updatedProjectileStartPosition, out RaycastHit hit)
        {
            hit = defaultRaycastHit;

            hitObjectTransform = null;

            if (!lineRenderer.enabled)
                lineRenderer.enabled = true;

            t = 0.0f;

            updatedProjectileStartPosition = projectileStartPosition + new Vector3(launchVelocity.x, 0.0f, launchVelocity.z).normalized * projectileStartPositionForwardOffset;

            previousDetectionPosition = updatedProjectileStartPosition;

            detectionPositionList = new List<Vector3>();

            notHit = true;

            while (t < maximumInAirTime)
            {
                // Move the detection sphere along the projectile curve
                nextDetectionPosition = updatedProjectileStartPosition + new Vector3(launchVelocity.x * t, launchVelocity.y * t - 4.9f * t * t, launchVelocity.z * t);
                t += detectionInterval;

                detectionPositionList.Add(nextDetectionPosition);

                // Perform ray physics detection for each detection position pair, check whether there is obstacle blocked between them
                rayDirection = (nextDetectionPosition - previousDetectionPosition).normalized;
                rayLength = Vector3.Distance(previousDetectionPosition, nextDetectionPosition);

                if (debugMode)
                    Debug.DrawLine(previousDetectionPosition, previousDetectionPosition + rayDirection * rayLength, Color.green);

                if (Physics.Raycast(previousDetectionPosition, rayDirection, out hit, rayLength, ~ignoredLayers, QueryTriggerInteraction.Ignore))
                {
                    notHit = false;

                    hitPosition = hit.point;
                    hitNormal = hit.normal;

                    if (getHitObjectTransform)
                        hitObjectTransform = hit.transform;

                    if (calculateProjectileVelocityWhenHit)
                    {
                        horizontalVector = new Vector3(launchVelocity.x, 0.0f, launchVelocity.z);
                        horizontalDisplacement = Vector3.Distance(new Vector3(previousDetectionPosition.x, 0.0f, previousDetectionPosition.z), new Vector3(hitPosition.x, 0.0f, hitPosition.z));
                        horizontalTime = horizontalDisplacement / horizontalVector.magnitude;

                        projectileVelocityWhenHit = (hitPosition - previousDetectionPosition) / horizontalTime;
                    }

                    if (debugMode)
                        Debug.DrawLine(hitPosition, hitPosition + hitNormal, Color.red);

                    break;
                }
                else
                {
                    // Perform sphere physics detection at current position, check whether there is obstacle on either side of the curve
                    if (Physics.OverlapSphereNonAlloc(nextDetectionPosition, projectileRadius, hitColliderArray, ~ignoredLayers, QueryTriggerInteraction.Ignore) > 0)
                    {
                        notHit = false;

                        hitPosition = hitColliderArray[0].ClosestPoint(nextDetectionPosition);
                        hitNormal = Vector3.Normalize(nextDetectionPosition - hitPosition);

                        break;
                    }
                }

                previousDetectionPosition = nextDetectionPosition;
            }

            startPoint = updatedProjectileStartPosition;

            if (notHit)
            {
                endPoint = detectionPositionList[detectionPositionList.Count - 1];

                if (projectileTargetPlaneMeshRenderer.enabled)
                    projectileTargetPlaneMeshRenderer.enabled = false;
            }
            else
            {
                endPoint = hitPosition;

                projectileTargetPlaneTransform.position = hitPosition;
                projectileTargetPlaneTransform.LookAt(projectileTargetPlaneTransform.position + hitNormal);
                projectileTargetPlaneTransform.rotation = Quaternion.Euler(projectileTargetPlaneTransform.rotation.eulerAngles.x + 90.0f, projectileTargetPlaneTransform.rotation.eulerAngles.y, projectileTargetPlaneTransform.rotation.eulerAngles.z);

                projectileTargetPlaneTransform.position += hitNormal * distanceOffsetAboveHitPosition;

                if (!projectileTargetPlaneMeshRenderer.enabled)
                    projectileTargetPlaneMeshRenderer.enabled = true;
            }

            if (detectionPositionList.Count > 2)
                controlPoint = 2.0f * detectionPositionList[detectionPositionList.Count / 2] - startPoint * 0.5f - endPoint * 0.5f;
            else if (detectionPositionList.Count == 2)
                controlPoint = (startPoint + endPoint) / 2.0f;

            // Render Bézier curve
            lineRenderer.SetPosition(0, startPoint);
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, endPoint);

            // The following t is a parameter Bézier curve, not the time t above
            t = 0.0f;
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                lineRenderer.SetPosition(i, (1 - t) * (1 - t) * startPoint + 2 * (1 - t) * t * controlPoint + t * t * endPoint);
                t += (1 / (float)lineRenderer.positionCount);
            }
        }

        public bool VisualizeProjectileCurveWithTargetPosition(Vector3 projectileStartPosition, float projectileStartPositionForwardOffset, Vector3 projectileEndPosition, float launchSpeed, Vector3 throwerVelocity, Vector3 targetObjectVelocity, float projectileRadius, float distanceOffsetAboveHitPosition, bool debugMode, out Vector3 updatedProjectileStartPosition, out Vector3 projectileLaunchVelocity, out Vector3 predictedTargetPosition, out RaycastHit hit)
        {
            hit = defaultRaycastHit;

            // Use the target object velocity to predict the end position
            predictedProjectileTravelTime = (Vector3.Distance(projectileStartPosition, projectileEndPosition) - projectileStartPositionForwardOffset) / launchSpeed;
            predictedTargetPosition = projectileEndPosition + (targetObjectVelocity - throwerVelocity) * predictedProjectileTravelTime;

            horizontalDirection = predictedTargetPosition - projectileStartPosition;
            horizontalDirection.y = 0.0f;
            horizontalDirection = horizontalDirection.normalized;

            updatedProjectileStartPosition = projectileStartPosition + horizontalDirection * projectileStartPositionForwardOffset;
            projectileLaunchVelocity = Vector3.zero;

            horizontalDistance = Vector3.Distance(new Vector3(updatedProjectileStartPosition.x, 0.0f, updatedProjectileStartPosition.z), new Vector3(predictedTargetPosition.x, 0.0f, predictedTargetPosition.z));
            deltaHeight = predictedTargetPosition.y - updatedProjectileStartPosition.y;

            launchSpeedSquare = launchSpeed * launchSpeed;
            component = launchSpeedSquare * launchSpeedSquare - 9.8f * (9.8f * horizontalDistance * horizontalDistance + 2.0f * deltaHeight * launchSpeedSquare);

            if (component < 0.0f)
                return false;

            component = Mathf.Sqrt(component);

            // Only calculate the result with lower angle
            launchAngle = Mathf.Atan2(launchSpeedSquare - component, 9.8f * horizontalDistance);

            projectileLaunchVelocity = horizontalDirection * Mathf.Cos(launchAngle) * launchSpeed + Vector3.up * Mathf.Sin(launchAngle) * launchSpeed;

            // Perform physics detection after obtaining the launch velocity
            if (!lineRenderer.enabled)
                lineRenderer.enabled = true;

            t = 0.0f;

            previousDetectionPosition = updatedProjectileStartPosition;

            detectionPositionList = new List<Vector3>();

            while (t < maximumInAirTime)
            {
                // Move the detection sphere along the projectile curve
                nextDetectionPosition = updatedProjectileStartPosition + new Vector3(projectileLaunchVelocity.x * t, projectileLaunchVelocity.y * t - 4.9f * t * t, projectileLaunchVelocity.z * t);
                t += detectionInterval;

                detectionPositionList.Add(nextDetectionPosition);

                // Perform ray physics detection for each detection position pair, check whether there is obstacle blocked between them
                rayDirection = (nextDetectionPosition - previousDetectionPosition).normalized;
                rayLength = Vector3.Distance(previousDetectionPosition, nextDetectionPosition);

                if (debugMode)
                    Debug.DrawLine(previousDetectionPosition, previousDetectionPosition + rayDirection * rayLength, Color.green);

                if (Physics.Raycast(previousDetectionPosition, rayDirection, out hit, rayLength, ~ignoredLayers, QueryTriggerInteraction.Ignore))
                {
                    notHit = false;

                    hitPosition = hit.point;
                    hitNormal = hit.normal;

                    if (debugMode)
                        Debug.DrawLine(hitPosition, hitPosition + hitNormal, Color.red);

                    break;
                }
                else
                {
                    // Perform sphere physics detection at current position, check whether there is obstacle on either side of the curve
                    if (Physics.OverlapSphereNonAlloc(nextDetectionPosition, projectileRadius, hitColliderArray, ~ignoredLayers, QueryTriggerInteraction.Ignore) > 0)
                    {
                        notHit = false;

                        hitPosition = hitColliderArray[0].ClosestPoint(nextDetectionPosition);
                        hitNormal = Vector3.Normalize(nextDetectionPosition - hitPosition);

                        break;
                    }
                }

                previousDetectionPosition = nextDetectionPosition;
            }

            startPoint = updatedProjectileStartPosition;

            endPoint = hitPosition;

            projectileTargetPlaneTransform.position = hitPosition;
            projectileTargetPlaneTransform.LookAt(projectileTargetPlaneTransform.position + hitNormal);
            projectileTargetPlaneTransform.rotation = Quaternion.Euler(projectileTargetPlaneTransform.rotation.eulerAngles.x + 90.0f, projectileTargetPlaneTransform.rotation.eulerAngles.y, projectileTargetPlaneTransform.rotation.eulerAngles.z);

            projectileTargetPlaneTransform.position += hitNormal * distanceOffsetAboveHitPosition;

            if (!projectileTargetPlaneMeshRenderer.enabled)
                projectileTargetPlaneMeshRenderer.enabled = true;

            if (detectionPositionList.Count > 2)
                controlPoint = 2.0f * detectionPositionList[detectionPositionList.Count / 2] - startPoint * 0.5f - endPoint * 0.5f;
            else if (detectionPositionList.Count == 2)
                controlPoint = (startPoint + endPoint) / 2.0f;

            // Render Bézier curve
            lineRenderer.SetPosition(0, startPoint);
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, endPoint);

            // The following t is a parameter Bézier curve, not the time t above
            t = 0.0f;
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                lineRenderer.SetPosition(i, (1 - t) * (1 - t) * startPoint + 2 * (1 - t) * t * controlPoint + t * t * endPoint);
                t += (1 / (float)lineRenderer.positionCount);
            }

            return true;
        }

        public void HideProjectileCurve()
        {
            if (lineRenderer.enabled)
            {
                lineRenderer.enabled = false;
                projectileTargetPlaneMeshRenderer.enabled = false;
            }
        }
    }
}