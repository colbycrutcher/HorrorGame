using UnityEngine;

namespace ProjectileCurveVisualizerSystem
{
    public class Trampoline : MonoBehaviour
    {
        [Range(0.0f, 1.0f)]
        public float bounciness = 0.99f;

        private Transform trampolineTransform;

        private bool touched = false;
        private Transform objectTransform;
        private Rigidbody objectRigidbody;
        private float currentClosestDistance = 999999.0f;
        private float closestDistance = 999999.0f;
        private Vector3 objectVelocity;

        private float incidenceVectorLength;
        private Vector3 incidenceVector;
        private Vector3 reflectionVector;

        // Output variables of method VisualizeProjectileCurve
        private Vector3 updatedProjectileStartPosition;
        private RaycastHit hit;

        public float projectileCurveStartPositionYOffset = 0.1f;
        public ProjectileCurveVisualizer projectileCurveVisualizer;

        private Trampoline nextTrampoline;

        void Start()
        {
            trampolineTransform = this.transform;
        }

        void Update()
        {
            if (touched && objectRigidbody.linearVelocity.y < 0.0f)
            {
                currentClosestDistance = objectTransform.position.y - trampolineTransform.position.y;
                if (currentClosestDistance < closestDistance)
                {
                    closestDistance = currentClosestDistance;
                    objectVelocity = objectRigidbody.linearVelocity;
                }
            }
        }

        public void VisualizeOutgoingProjectileCurve(Vector3 hitPosition, Vector3 incidenceVelocity, float projectileRadius, float distanceOffsetAboveHitPosition, bool debugMode)
        {
            if (projectileCurveVisualizer)
            {
                projectileCurveVisualizer.VisualizeProjectileCurve(hitPosition + Vector3.up * projectileCurveStartPositionYOffset, 0.0f, CalculateReflectionVector(incidenceVelocity) * bounciness, projectileRadius, distanceOffsetAboveHitPosition, debugMode, out updatedProjectileStartPosition, out hit);

                if (projectileCurveVisualizer.hitObjectTransform)
                {
                    // Check if the hit object is a trampoline
                    if (projectileCurveVisualizer.hitObjectTransform.name == "Trampoline")
                    {
                        nextTrampoline = projectileCurveVisualizer.hitObjectTransform.GetComponent<Trampoline>();
                        nextTrampoline.VisualizeOutgoingProjectileCurve(projectileCurveVisualizer.hitPosition, projectileCurveVisualizer.projectileVelocityWhenHit, projectileRadius, 0.1f, true);
                    }
                }
            }
        }

        public void HideProjectileCurve()
        {
            if (projectileCurveVisualizer)
            {
                projectileCurveVisualizer.HideProjectileCurve();

                if (nextTrampoline)
                {
                    nextTrampoline.HideProjectileCurve();
                    nextTrampoline = null;
                }
            }
        }

        Vector3 CalculateReflectionVector(Vector3 incidenceVector)
        {
            incidenceVectorLength = incidenceVector.magnitude;
            incidenceVector = incidenceVector.normalized;

            return Vector3.Normalize(incidenceVector - 2f * Vector3.Dot(incidenceVector, trampolineTransform.forward) * trampolineTransform.forward) * incidenceVectorLength;
        }

        void OnTriggerEnter(Collider collider)
        {
            if (!touched)
            {
                touched = true;

                objectTransform = collider.transform;
                objectRigidbody = objectTransform.GetComponent<Rigidbody>();

                if (objectRigidbody)
                {
                    touched = true;

                    closestDistance = objectTransform.position.y - trampolineTransform.position.y;
                    objectVelocity = objectRigidbody.linearVelocity;
                }
            }
        }

        void OnTriggerExit(Collider collider)
        {
            touched = false;

            objectRigidbody = null;
            closestDistance = 999999.0f;
            objectVelocity = Vector3.zero;
        }

        void OnCollisionEnter(Collision collision)
        {
            closestDistance = 999999.0f;

            // Calculate reflection vector
            reflectionVector = CalculateReflectionVector(objectVelocity);
            objectRigidbody.linearVelocity = reflectionVector * bounciness;
        }
    }
}