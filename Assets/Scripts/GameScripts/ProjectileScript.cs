using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProjectileScript : MonoBehaviour
{
    private const float POINT_DISTANCE_MODIFIER = 8f; // Modifier for distance between points in the path
    private const int PROJECTILE_SPEED = 500; // Speed of the projectile

    private List<Vector2> path = new List<Vector2>();
    private Action returnProjectile;

    public void Init(Vector2 startPosition, Vector2 finalPoint, Action returnProjectile)
    {
        this.returnProjectile = returnProjectile;

        Vector2 direction = (finalPoint - startPosition).normalized;
        path.Clear();

        float distance = Vector2.Distance(startPosition, finalPoint) * Random.Range(0.8f, 0.95f);

        // Calculate the path points based on the distance
        int pointsCount = Mathf.Max(2, Mathf.RoundToInt(distance / POINT_DISTANCE_MODIFIER));
        for (int i = 0; i < pointsCount; i++)
        {
            float t = (float)i / (pointsCount - 1);
            Vector2 point = Vector2.Lerp(startPosition, finalPoint, t);
            path.Add(point);
        }

        // Calculate wait time based on distance
        float waitTime = distance / (pointsCount * PROJECTILE_SPEED); // Adjust speed as needed

        StartCoroutine(MoveProjectile(waitTime));
    }

    IEnumerator MoveProjectile(float waitTime)
    {
        foreach (Vector2 point in path)
        {
            transform.position = point;
            yield return new WaitForSeconds(waitTime);
        }

        returnProjectile();
    }
}
