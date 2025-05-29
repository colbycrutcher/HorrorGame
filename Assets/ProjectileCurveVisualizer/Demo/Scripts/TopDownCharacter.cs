using UnityEngine;
using UnityEngine.UI;
using ProjectileCurveVisualizerSystem;

public class TopDownCharacter : MonoBehaviour
{
    private Transform characterTransform;

    private Transform springArmTransform;
    private Transform cameraTransform;
    private Camera characterCamera;

    private Ray ray;
    private RaycastHit mouseRaycastHit;

    private Vector3 targetCharacterPosition;

    public float characterMovementSpeed = 35.0f;

    public float launchSpeed = 15.0f;

    public LayerMask ignoredLayers;

    private Vector3 previousPosition;

    private bool canHitTarget = false;

    // Output variables of method VisualizeProjectileCurveWithTargetPosition
    private Vector3 updatedProjectileStartPosition;
    private Vector3 projectileLaunchVelocity;
    private Vector3 predictedTargetPosition;
    private RaycastHit hit;

    private int gettingHitTimes = 0;

    public ProjectileCurveVisualizer projectileCurveVisualizer;

    public GameObject projectileGameObject;

    public Text gettingHitTimesText;

    void Start()
    {
        characterTransform = this.transform;

        // Detach spring arm from prefab
        springArmTransform = this.transform.GetChild(0).transform;
        springArmTransform.parent = null;
        cameraTransform = springArmTransform.GetChild(0).transform;
        characterCamera = cameraTransform.GetComponent<Camera>();

        targetCharacterPosition = characterTransform.position;

        previousPosition = characterTransform.position;
    }

    void Update()
    {
        CameraControlLogic();
        CameraZoomingLogic();
        CharacterMovementLogic();

        Attributes.characterVelocity = (characterTransform.position - previousPosition) / Time.deltaTime;
        previousPosition = characterTransform.position;

        // Adjust launch speed
        if (Input.GetKeyDown(KeyCode.Z))
            launchSpeed = Mathf.Clamp(launchSpeed - 1.0f, 1.0f, 30.0f);
        else if (Input.GetKeyDown(KeyCode.X))
            launchSpeed = Mathf.Clamp(launchSpeed + 1.0f, 1.0f, 30.0f);

        if (Input.GetMouseButton(1))
        {
            ray = characterCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out mouseRaycastHit, Mathf.Infinity, ~ignoredLayers))
            {
                characterTransform.LookAt(new Vector3(mouseRaycastHit.point.x, characterTransform.position.y, mouseRaycastHit.point.z));

                // Visualize Projectile Curve With Target Position(Customizable)
                canHitTarget = projectileCurveVisualizer.VisualizeProjectileCurveWithTargetPosition(characterTransform.position, 1.5f, mouseRaycastHit.point, launchSpeed, Vector3.zero, Vector3.zero, 0.05f, 0.1f, false, out updatedProjectileStartPosition, out projectileLaunchVelocity, out predictedTargetPosition, out hit);

                if (!canHitTarget)
                {
                    projectileCurveVisualizer.HideProjectileCurve();

                    print("Too far, cannot throw to there");
                }
            }
            else
            {
                projectileCurveVisualizer.HideProjectileCurve();
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            projectileCurveVisualizer.HideProjectileCurve();

            if (canHitTarget)
            {
                canHitTarget = false;

                Projectile projectile = GameObject.Instantiate(projectileGameObject).GetComponent<Projectile>();
                projectile.transform.position = updatedProjectileStartPosition;

                // Trigger LateDestroy method in the script, for the performance reason
                projectile.GetComponent<PaperBall>().enabled = true;

                // Throw projectile
                projectile.Throw(projectileLaunchVelocity);
            }
        }
    }

    void CameraControlLogic()
    {
        springArmTransform.position = characterTransform.position;
    }

    void CameraZoomingLogic()
    {
        cameraTransform.localPosition = new Vector3(0.0f, 0.0f, Mathf.Clamp(cameraTransform.localPosition.z + Input.GetAxis("Mouse ScrollWheel") * 6.0f, -30.0f, -8.0f));
    }

    void CharacterMovementLogic()
    {
        if (Input.GetMouseButton(0))
        {
            ray = characterCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out mouseRaycastHit, Mathf.Infinity, ~ignoredLayers))
            {
                characterTransform.LookAt(new Vector3(mouseRaycastHit.point.x, characterTransform.position.y, mouseRaycastHit.point.z));
                targetCharacterPosition = characterTransform.position + characterTransform.forward * characterMovementSpeed * Time.deltaTime;
            }
        }

        characterTransform.position = Vector3.Lerp(characterTransform.position, new Vector3(targetCharacterPosition.x, characterTransform.position.y, targetCharacterPosition.z), 0.125f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.name == "Projectile(Clone)")
        {
            gettingHitTimes += 1;
            gettingHitTimesText.text = "Getting hit " + gettingHitTimes + " times";
        }
    }
}