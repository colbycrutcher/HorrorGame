using UnityEngine;
using ProjectileCurveVisualizerSystem;

public class ThirdPersonCharacter : MonoBehaviour
{
    private Transform characterTransform;

    private Transform springArmTransform;
    private Transform cameraTransform;

    private Vector3 cameraZoomInPosition = new Vector3(1.5f, 0.0f, -3f);
    private Vector3 cameraNormalPosition = new Vector3(1.5f, 0.0f, -4f);

    private Rigidbody characterRigidbody;

    private Vector3 targetCharacterPosition;

    public float cameraSpeed = 350.0f;
    public float characterMovementSpeed = 5.0f;

    private bool arrowKeyDown = false;

    private bool inProjectileMode = false;

    private bool bounceOnTrampoline = false;

    private float launchSpeed = 10.0f;
    private Vector3 launchVelocity;

    // Output variables of method VisualizeProjectileCurve
    private Vector3 updatedProjectileStartPosition;
    private RaycastHit hit;

    public ProjectileCurveVisualizer projectileCurveVisualizer;

    public GameObject projectileGameObject;

    void Start()
    {
        characterTransform = this.transform;

        // Detach spring arm from prefab
        springArmTransform = this.transform.GetChild(0).transform;
        springArmTransform.parent = null;
        cameraTransform = springArmTransform.GetChild(0).transform;

        characterRigidbody = this.GetComponent<Rigidbody>();

        targetCharacterPosition = characterTransform.position;
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
            characterTransform.LookAt(lookAtPosition);
            targetCharacterPosition += characterTransform.forward * characterMovementSpeed * Time.deltaTime;
        }

        characterTransform.position = Vector3.Lerp(characterTransform.position, new Vector3(targetCharacterPosition.x, characterTransform.position.y, targetCharacterPosition.z), 0.125f);
    }

    void ProjectileModeLogic() {
        if (inProjectileMode)
        {
            // Adjust launch speed
            launchSpeed = Mathf.Clamp(launchSpeed + Input.GetAxis("Mouse ScrollWheel") * 6.0f, 0.5f, 1000.0f);

            launchVelocity = new Vector3(characterTransform.forward.x, characterTransform.forward.y + Mathf.Clamp(springArmTransform.forward.y * 2.0f, -0.25f, 4.0f), characterTransform.forward.z).normalized * launchSpeed;

            // Visualize Projectile Curve(Customizable)
            projectileCurveVisualizer.VisualizeProjectileCurve(characterTransform.position, 1.0f, launchVelocity, 0.25f, 0.1f, true, out updatedProjectileStartPosition, out hit);

            if (Input.GetMouseButtonUp(0))
            {
                inProjectileMode = false;

                projectileCurveVisualizer.HideProjectileCurve();

                // Spawn a projectile and move to the start position
                Projectile projectile = GameObject.Instantiate(projectileGameObject).GetComponent<Projectile>();
                projectile.transform.position = updatedProjectileStartPosition;

                // Throw projectile
                projectile.Throw(launchVelocity);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (bounceOnTrampoline)
            characterRigidbody.AddForce(new Vector3(0.0f, 180.0f, 0.0f));
    }
}