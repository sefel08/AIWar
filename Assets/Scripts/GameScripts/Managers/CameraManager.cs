using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraManager
{
    private UnitManager unitManager;
    private Camera camera;
    private float mapSize;

    Vector3 targetPosition;
    float targetSize;

    Vector2 positionOffset;
    float sizeOffset;

    const float minCameraSize = 35f; // Minimum camera size to prevent zooming out too far
    const float UIPercentage = 0.15f; // Percentage of camera size reserved for UI
    const float edgeDistanceFactor = 0.1f; // Factor to determine distance from edges to seen units
    const float lerpSpeed = 0.05f; // Speed of camera movement and size adjustment

    public CameraManager(UnitManager unitManager, Camera camera, float mapSize)
    {
        this.unitManager = unitManager;
        this.camera = camera;
        this.mapSize = mapSize;
    }

    public void UpdateCameraPosition()
    {
        float cameraSize = camera.orthographicSize;
        float edgeDistance = edgeDistanceFactor * cameraSize;
        float UISize = cameraSize * UIPercentage;


        // Camera movement and zoom logic
        if (Input.GetKey(KeyCode.LeftShift))
        {
            positionOffset += new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * (cameraSize * 0.5f * Time.deltaTime);
            sizeOffset += -Input.mouseScrollDelta.y * (cameraSize * 0.1f);
        }
        else
        {
            positionOffset = Vector2.zero;
            sizeOffset = 0f;
        }

        float maxY;
        float minY;
        float maxX;
        float minX;

        List<UnitData> allUnits = unitManager.GetAllUnitData();

        if (allUnits.Count <= 0)
        {
            targetPosition = new Vector3(0, 0 - UISize, -10f);
            targetSize = mapSize;
            MoveCamera();
            return;
        }
        if (allUnits.Count == 1) 
        {
            Vector2 unitPosition = allUnits[0].Position;
            targetPosition = new Vector3(unitPosition.x, unitPosition.y, -10f);
            MoveCamera();
            return;
        }

        maxY = allUnits[0].Position.y;
        minY = allUnits[0].Position.y;
        maxX = allUnits[0].Position.x;
        minX = allUnits[0].Position.x;

        for (int i = 1; i < allUnits.Count; i++)
        {
            Vector2 unitPosition = allUnits[i].Position;
            if (unitPosition.y > maxY) maxY = unitPosition.y;
            if (unitPosition.y < minY) minY = unitPosition.y;
            if (unitPosition.x > maxX) maxX = unitPosition.x;
            if (unitPosition.x < minX) minX = unitPosition.x;
        }

        float centerY = ((maxY + minY) / 2f) - UISize;
        float centerX = (maxX + minX) / 2f;

        targetPosition = new Vector3(centerX, centerY, -10f) + new Vector3(positionOffset.x, positionOffset.y, 0f);

        // Adjust camera size based on the distance between the furthest units
        float distanceY = maxY - minY;
        targetSize = Mathf.Max((distanceY / 2f) + UISize + (edgeDistance * 2f), minCameraSize) + sizeOffset;

        MoveCamera();
    }
    private void MoveCamera()
    {
        float cameraSize = camera.orthographicSize;
        camera.transform.position = Vector3.Lerp(camera.transform.position, targetPosition, Time.deltaTime * (cameraSize * lerpSpeed));
        camera.orthographicSize = Mathf.Lerp(cameraSize, targetSize, Time.deltaTime * (cameraSize * lerpSpeed));
    }
}
