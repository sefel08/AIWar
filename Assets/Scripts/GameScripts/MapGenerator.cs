using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MapGenerator
{
    Material mapMaterial;

    public MapGenerator(Material mapMaterial)
    {
        this.mapMaterial = mapMaterial;
    }

    public (GameMap, GameMapData) GenerateMap(float size, float unitSize, float elementSpacing, int maxNumberOfTrials, float minElementSize, float maxElementSize, Gradient mapColor, GameObject mapParent)
    {
        GameMap gameMap = new GameMap();
        GameMapData gameMapData = new GameMapData(mapParent);
        
        bool smallerCircle = false;

        int elementNumber = 1;
        int numberOfTrials = 0;
        //numberOfTrials < maxNumberOfTrials
        while (true)
        {
            if(numberOfTrials > maxNumberOfTrials)
            {
                if (smallerCircle)
                {
                    break;
                }
                smallerCircle = true;
                numberOfTrials = 0;
                size *= 0.45f;
                maxNumberOfTrials = 300;
            }

            Color objectColor = mapColor.Evaluate(Random.Range(0f, 1f));
            GameObject gameObject = CreateRandomGameObject(gameMapData.mapContainer.transform, minElementSize, maxElementSize, objectColor, out PolygonCollider2D collider);

            //move gameObject to a random position within the map size
            //gameObject.transform.position = new Vector3(Random.Range(-size / 2, size / 2), Random.Range(-size / 2, size / 2), 0);
            gameObject.transform.position = Random.insideUnitCircle * size;

            Transform transform = gameObject.transform;
            List<Vector2> points = new List<Vector2>();
            //make global positions from local
            foreach (Vector2 point in collider.points) {
                points.Add(transform.TransformPoint(point));
            }

            MapElement mapElement = new MapElement(elementNumber, points, (collider.points.Length == 3) ? MapElementType.triangle : MapElementType.box);
            float radius = collider.points.Max(p => Vector2.Distance(gameObject.transform.position, transform.TransformPoint(p)));
            MapElementData mapElementData = new MapElementData(mapElement, gameObject, radius);

            bool isValid = true;
            foreach (MapElementData element in gameMapData.elementsWithElementData.Values)
            {
                if (Vector2.Distance(mapElementData.position, element.position) < (mapElementData.radius + element.radius + unitSize + elementSpacing))
                {
                    isValid = false;
                    break;
                }
            }
            if (!isValid)
            {
                numberOfTrials++;
                Object.Destroy(gameObject);
                continue;
            }

            //add the MapElement to the gameMap
            gameMap.Elements.Add(elementNumber, mapElement);
            //add data to gameMapData
            gameMapData.elementsWithElementData.Add(mapElement, mapElementData);
            gameMapData.gameObjectWithElementDatas.Add(gameObject, mapElementData);

            elementNumber++;
            numberOfTrials = 0;
        }

        return (gameMap, gameMapData);
    }
    public GameObject CreateRandomGameObject(Transform parent, float minElementSize, float maxElementSize, Color objectColor, out PolygonCollider2D collider)
    {
        GameObject mapElementGameObject = new GameObject("MapElement");
        mapElementGameObject.transform.SetParent(parent, true);
        MeshFilter meshFilter = mapElementGameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = mapElementGameObject.AddComponent<MeshRenderer>();
        collider = mapElementGameObject.AddComponent<PolygonCollider2D>();
        Rigidbody2D rigidbody = mapElementGameObject.AddComponent<Rigidbody2D>();

        rigidbody.bodyType = RigidbodyType2D.Static;
        meshRenderer.material = new Material(mapMaterial);
        meshRenderer.material.color = objectColor;

        Mesh mesh = new Mesh();

        int numberOfVertices = Random.Range(3, 5); // Randomly choose between 3 and 4 vertices  

        Vector3[] vertices = new Vector3[numberOfVertices];
        if (numberOfVertices == 3)
        {
            // Generate a triangle  
            float[] minAngles = { 5f, 125f, 245f };
            float[] maxAngles = { 115f, 235f, 355f };
            for (int i = 0; i < 3; i++)
            {
                float angleDeg = Random.Range(minAngles[i], maxAngles[i]);
                float angleRad = angleDeg * Mathf.Deg2Rad;
                float radius = Random.Range(minElementSize, maxElementSize);
                vertices[i] = new Vector3(Mathf.Cos(angleRad) * radius, Mathf.Sin(angleRad) * radius, 0);
            }
        }
        else // numberOfVertices == 4  
        {
            // Generate a rectangle  
            float width = Random.Range(minElementSize, maxElementSize);
            float height = Random.Range(minElementSize, maxElementSize);

            vertices[0] = new Vector3(-width / 2, -height / 2, 0); // Bottom left  
            vertices[1] = new Vector3(width / 2, -height / 2, 0);  // Bottom right  
            vertices[2] = new Vector3(width / 2, height / 2, 0);   // Top right  
            vertices[3] = new Vector3(-width / 2, height / 2, 0);  // Top left  

            // Apply random rotation to the rectangle  
            Quaternion rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = rotation * vertices[i];
            }
        }

        mesh.vertices = vertices;
        collider.SetPath(0, vertices.Select(v => new Vector2(v.x, v.y)).ToArray());

        int[] triangles = new int[(numberOfVertices - 2) * 3];
        for (int i = 0; i < numberOfVertices - 2; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

        return mapElementGameObject;
    }
}
