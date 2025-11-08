using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using MinAttribute = UnityEngine.MinAttribute;

public class DebugVariablesManager : MonoBehaviour 
{
    [Header("Static Variables")]
    [SerializeField] int debugInt1;
    [SerializeField] int debugInt2;
    [SerializeField] int debugInt3;
    [SerializeField] float debugFloat1;
    [SerializeField] float debugFloat2;
    [Range(0, 1)]
    [SerializeField] float debugFloat3;
    [SerializeField] bool test1;
    [SerializeField] bool test2;
    [SerializeField] bool test3;
    [SerializeField] Vector2 debugVector1;
    [SerializeField] Vector2 debugVector2;
    [SerializeField] Vector2 debugVector3;

    private void OnValidate()
    {
        DebugVariables.testInt1 = debugInt1;
        DebugVariables.testInt2 = debugInt2;
        DebugVariables.testInt3 = debugInt3;
        DebugVariables.testFloat1 = debugFloat1;
        DebugVariables.testFloat2 = debugFloat2;
        DebugVariables.testFloat3 = debugFloat3;
        DebugVariables.test1 = test1;
        DebugVariables.test2 = test2;
        DebugVariables.test3 = test3;
        DebugVariables.debugVector1 = debugVector1;
        DebugVariables.debugVector2 = debugVector2;
        DebugVariables.debugVector3 = debugVector3;
    }
}