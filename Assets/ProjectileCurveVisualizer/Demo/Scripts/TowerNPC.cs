using UnityEngine;
using ProjectileCurveVisualizerSystem;

public class TowerNPC : MonoBehaviour
{
    private float backupTime;

    private bool alerted;

    private Transform npcTransform;

    private Transform characterTransform;

    private Vector3 throwerVelocity;
    private Vector3 previousPosition;

    public float launchSpeed = 10.0f;

    public float throwFrequency = 2.0f;

    public float dizzyDuration = 5.0f;

    private bool dizzy = false;

    bool canHitTarget = false;

    // Output variables of method VisualizeProjectileCurveWithTargetPosition
    private Vector3 updatedProjectileStartPosition;
    private Vector3 projectileLaunchVelocity;
    private Vector3 predictedTargetPosition;
    private RaycastHit hit;

    public ProjectileCurveVisualizer projectileCurveVisualizer;

    public GameObject projectileGameObject;

    public Transform alertTextTransform;

    public ParticleSystem dizzyParticleSystem;

    private Transform dizzyParticleSystemTransform;

    void Start()
    {
        npcTransform = this.transform;

        previousPosition = npcTransform.position;

        dizzyParticleSystemTransform = dizzyParticleSystem.transform;
    }

    void Update()
    {
        // Calculate velocity for self
        throwerVelocity = (npcTransform.position - previousPosition) / Time.deltaTime;
        previousPosition = npcTransform.position;

        // Floating alert text
        if (alerted && !dizzy)
            alertTextTransform.position = npcTransform.position + new Vector3(0.0f, 2.0f, 0.0f);

        if (dizzy)
            dizzyParticleSystemTransform.position = npcTransform.position + new Vector3(-1.4f, 1.35f, 0.0f);

        if (Time.realtimeSinceStartup - backupTime > throwFrequency)
        {
            backupTime = Time.realtimeSinceStartup;

            if (alerted && !dizzy)
            {
                canHitTarget = projectileCurveVisualizer.VisualizeProjectileCurveWithTargetPosition(npcTransform.position, 1.5f, characterTransform.position, launchSpeed, throwerVelocity, Attributes.characterVelocity, 0.05f, 0.1f, false, out updatedProjectileStartPosition, out projectileLaunchVelocity, out predictedTargetPosition, out hit);

                if (throwerVelocity.magnitude > 0.1f)
                    projectileCurveVisualizer.HideProjectileCurve();

                npcTransform.LookAt(updatedProjectileStartPosition);

                Projectile projectile = GameObject.Instantiate(projectileGameObject).GetComponent<Projectile>();
                projectile.transform.position = updatedProjectileStartPosition;

                // Trigger LateDestroy method in the script, for the performance reason
                projectile.GetComponent<PaperBall>().enabled = true;

                // Throw projectile, if NPC is standing on a moving platform, the actual launch velocity = projectile launch velocity + moving platform velocity
                projectile.Throw(projectileLaunchVelocity + throwerVelocity);

                Invoke("LateHideProjectileCurve", 4.0f);
            }
        }
    }

    void LateHideProjectileCurve()
    {
        projectileCurveVisualizer.HideProjectileCurve();
    }

    void LateEndDizzy()
    {
        dizzy = false;

        dizzyParticleSystem.Stop();
        dizzyParticleSystem.Clear();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "TopDownCharacter")
        {
            alerted = true;

            if (!characterTransform)
                characterTransform = other.transform;
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.name == "TopDownCharacter")
        {
            alerted = false;

            alertTextTransform.position = new Vector3(0.0f, -999.0f, 0.0f);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!dizzy && collision.transform.name == "Projectile(Clone)")
        {
            dizzy = true;

            Invoke("LateEndDizzy", dizzyDuration);

            alertTextTransform.position = new Vector3(0.0f, -999.0f, 0.0f);

            dizzyParticleSystem.Play();
        }
    }
}