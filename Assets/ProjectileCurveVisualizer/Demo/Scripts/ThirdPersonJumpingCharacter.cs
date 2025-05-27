using UnityEngine;
using ProjectileCurveVisualizerSystem;

public class ThirdPersonJumpingCharacter : MonoBehaviour
{
    private Transform characterTransform;

    private Transform springArmTransform;
    private Transform cameraTransform;

    private Vector3 cameraZoomInPosition = new Vector3(1.5f, 0.0f, -3f);
    private Vector3 cameraNormalPosition = new Vector3(1.5f, 0.0f, -4f);

    private Rigidbody characterRigidbody;
    public Collider characterCollider;

    public float cameraSpeed = 350.0f;
    public float characterMovementSpeed = 5.0f;

    private bool arrowKeyDown = false;

    private bool inProjectileMode = false;

    private bool jumping = false;
    private bool groundCheck = false;

    private bool bounceOnTrampoline = false;

    [Header("Projectile Attributes")]
    public float viewLookUpOffset = 1.0f;
    public float viewLookUpScale = 2.0f;

    public float projectileStartPositionYOffset = -0.875f;
    public float projectileRadius = 0.01f;

    private float launchSpeed = 5.0f;
    private Vector3 launchVelocity;

    // Output variables of method VisualizeProjectileCurve
    private Vector3 updatedProjectileStartPosition;
    private RaycastHit hit;

    public ProjectileCurveVisualizer projectileCurveVisualizer;

    private bool hitTrampoline = false;
    private Trampoline trampoline;

    void Start()
    {
        characterTransform = this.transform;

        // Detach spring arm from prefab
        springArmTransform = this.transform.GetChild(0).transform;
        springArmTransform.parent = null;
        cameraTransform = springArmTransform.GetChild(0).transform;

        characterRigidbody = this.GetComponent<Rigidbody>();
        characterCollider = this.GetComponent<Collider>();
    }

    void Update()
    {
        if (Settings.mouseControl)
        {
            CameraControlLogic();
            CameraZoomingLogic();
            CharacterMovementLogic();

            // Jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                bounceOnTrampoline = true;
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                bounceOnTrampoline = false;

                if (Mathf.Abs(characterRigidbody.linearVelocity.y) < 0.05f)
                    characterRigidbody.AddForce(new Vector3(0.0f, 180.0f, 0.0f));
            }

            // Enter/quit projectile mode
            if (Input.GetKeyDown(KeyCode.R))
            {
                inProjectileMode = !inProjectileMode;

                if (!inProjectileMode)
                    projectileCurveVisualizer.HideProjectileCurve();
                else
                    SetCollider(false);
            }

            ProjectileModeLogic();
        }
    }

    void CameraControlLogic()
    {
        springArmTransform.position = characterTransform.position;
        springArmTransform.rotation = Quaternion.Euler(springArmTransform.rotation.eulerAngles.x + -Input.GetAxis("Mouse Y") * cameraSpeed * Time.deltaTime, springArmTransform.rotation.eulerAngles.y + Input.GetAxis("Mouse X") * cameraSpeed * Time.deltaTime, 0.0f);
    }

    void CameraZoomingLogic()
    {
        if (inProjectileMode)
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, cameraZoomInPosition, 0.1f);
        else
            cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, cameraNormalPosition, 0.1f);
    }

    void CharacterMovementLogic()
    {
        if (!jumping)
        {
            arrowKeyDown = false;

            Vector3 lookAtPosition = characterTransform.position;

            if (Input.GetKey(KeyCode.W))
            {
                arrowKeyDown = true;
                lookAtPosition += new Vector3(springArmTransform.forward.x, 0.0f, springArmTransform.forward.z);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                arrowKeyDown = true;
                lookAtPosition -= new Vector3(springArmTransform.forward.x, 0.0f, springArmTransform.forward.z);
            }

            if (Input.GetKey(KeyCode.A))
            {
                arrowKeyDown = true;
                lookAtPosition -= new Vector3(springArmTransform.right.x, 0.0f, springArmTransform.right.z);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                arrowKeyDown = true;
                lookAtPosition += new Vector3(springArmTransform.right.x, 0.0f, springArmTransform.right.z);
            }

            if (arrowKeyDown)
            {
                if (inProjectileMode)
                {
                    projectileCurveVisualizer.HideProjectileCurve();
                    SetCollider(true);
                }

                characterTransform.LookAt(lookAtPosition);
                characterTransform.position += characterTransform.forward * characterMovementSpeed * Time.deltaTime;
            }
        }
    }

    void ProjectileModeLogic()
    {
        if (inProjectileMode && !arrowKeyDown)
        {
            if (!arrowKeyDown)
                SetCollider(false);

            // Adjust launch speed
            launchSpeed = Mathf.Clamp(launchSpeed + Input.GetAxis("Mouse ScrollWheel") * 2.0f, 0.5f, 1000.0f);

            launchVelocity = new Vector3(characterTransform.forward.x, characterTransform.forward.y + Mathf.Clamp((springArmTransform.forward.y + viewLookUpOffset) * viewLookUpScale, 0.0f, 10.0f), characterTransform.forward.z).normalized * launchSpeed;

            // Visualize Projectile Curve(Customizable)
            projectileCurveVisualizer.VisualizeProjectileCurve(characterTransform.position + new Vector3(0.0f, projectileStartPositionYOffset, 0.0f), 0.0f, launchVelocity, projectileRadius, 0.1f, true, out updatedProjectileStartPosition, out hit);

            if (projectileCurveVisualizer.hitObjectTransform)
            {
                // Check if the hit object is a trampoline
                if (projectileCurveVisualizer.hitObjectTransform.name == "Trampoline")
                {
                    hitTrampoline = true;

                    trampoline = projectileCurveVisualizer.hitObjectTransform.GetComponent<Trampoline>();

                    // Visualize outgoing projectile curve for trampoline
                    trampoline.VisualizeOutgoingProjectileCurve(projectileCurveVisualizer.hitPosition, projectileCurveVisualizer.projectileVelocityWhenHit, projectileRadius, 0.1f, true);
                }
                else if (hitTrampoline)
                {
                    hitTrampoline = false;

                    trampoline.HideProjectileCurve();

                    trampoline = null;
                }
            }
            else
            {
                if (hitTrampoline)
                {
                    hitTrampoline = false;

                    trampoline.HideProjectileCurve();

                    trampoline = null;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                inProjectileMode = false;

                SetCollider(true);

                projectileCurveVisualizer.HideProjectileCurve();

                if (hitTrampoline)
                {
                    hitTrampoline = false;

                    trampoline.HideProjectileCurve();

                    trampoline = null;
                }

                // Jump(throw character itself)
                jumping = true;
                characterRigidbody.linearVelocity = launchVelocity;

                Invoke("LateEnableGroundCheck", 0.1f);
            }
        }
    }

    void SetCollider(bool enabled)
    {
        if (characterCollider.enabled != enabled)
        {
            characterCollider.enabled = enabled;
            characterRigidbody.useGravity = enabled;
        }
    }

    void LateEnableGroundCheck()
    {
        groundCheck = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (groundCheck)
        {
            groundCheck = false;
            jumping = false;
        }

        if (bounceOnTrampoline)
            characterRigidbody.AddForce(new Vector3(0.0f, 180.0f, 0.0f));
    }
}