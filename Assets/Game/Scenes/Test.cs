using System.Collections;
using System.Collections.Generic;
using Live17Game;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform cubeA;
    public Transform cubeB;

    void Awake()
    {

    }

    void Update()
    {

    }

    void OnDrawGizmos()
    {
        Vector3 direction = MathUtility.GetClosestDirection(cubeA, (cubeB.position - cubeA.position).normalized);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(cubeA.position, direction * 10f);
    }
}