using System;
using UnityEngine;
public class Team
{
    public int points = 0;
    public bool hasRedCard = false;

    public int teamId;
    public string teamName;
    public Type unitType;
    public Type commandType;
    public Gradient teamColor;

    public Vector2 spawnPoint;
    public int unitCount;
    public GameObject parentGameObject = new GameObject("TeamParent");
}