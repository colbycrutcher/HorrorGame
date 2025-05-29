using System.Collections.Generic;
using UnityEngine;
using ProjectileCurveVisualizerSystem;

public class ProjectileCurveSpawner : MonoBehaviour
{
    public List<Transform> projectileCurveVisualizerTransformList = new List<Transform>();
    private List<ProjectileCurveVisualizer> projectileCurveVisualizerList = new List<ProjectileCurveVisualizer>();

    private Transform projectileCurveSpawnerTransform;
    private bool movingUp;

    private float launchVelocityY = 0.1f;

    // Output variables of method VisualizeProjectileCurve
    private Vector3 updatedProjectileStartPosition;
    private RaycastHit hit;

    void Start()
    {
        projectileCurveSpawnerTransform = this.transform;

        for (int index = 0; index < projectileCurveVisualizerTransformList.Count; index++)
            projectileCurveVisualizerList.Add(projectileCurveVisualizerTransformList[index].GetComponent<ProjectileCurveVisualizer>());
    }

    void Update()
    {
        ProjectileCurveVisualizerMovementLogic();

        for (int index = 0; index < projectileCurveVisualizerTransformList.Count; index++)
            projectileCurveVisualizerList[index].VisualizeProjectileCurve(projectileCurveVisualizerTransformList[index].position, 1.0f, new Vector3(0.0f, launchVelocityY, -5.0f), 0.1f, 0.01f, true, out updatedProjectileStartPosition, out hit);
    }

    void ProjectileCurveVisualizerMovementLogic()
    {
        if (movingUp)
        {
            launchVelocityY = Mathf.Clamp(launchVelocityY + 0.02f * Time.deltaTime, 0.1f, 1f);

            projectileCurveSpawnerTransform.position += new Vector3(0.0f, 1.0f, 0.0f) * Time.deltaTime;

            if (projectileCurveSpawnerTransform.position.y > 3.0f)
                movingUp = false;
        }
        else
        {
            launchVelocityY = Mathf.Clamp(launchVelocityY - 0.02f * Time.deltaTime, 0.1f, 1f);

            projectileCurveSpawnerTransform.position += new Vector3(0.0f, -1.0f, 0.0f) * Time.deltaTime;

            if (projectileCurveSpawnerTransform.position.y < 1.5f)
            {
                movingUp = true;
            }
        }
    }
}