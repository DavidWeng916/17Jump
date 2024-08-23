using System.Collections;
using System.Collections.Generic;
using Live17Game;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform cubeA;
    public Transform cubeB;
    public Transform cubeC;

    void Awake()
    {
        Debug.Log($"cubeA localScale:{cubeA.localScale} lossyScale:{cubeA.lossyScale}");
        Debug.Log($"cubeB localScale:{cubeB.localScale} lossyScale:{cubeB.lossyScale}");
        Debug.Log($"cubeC localScale:{cubeC.localScale} lossyScale:{cubeC.lossyScale}");

        Vector3 localScale = cubeC.localScale;
        Vector3 lossyScale = cubeC.lossyScale;
        localScale.x /= lossyScale.x;
        localScale.y /= lossyScale.y;
        localScale.z /= lossyScale.z;


        cubeC.localScale = localScale;
    }

    void Update()
    {

    }

    /* void OnDrawGizmos()
    {
        Vector3 direction = MathUtility.GetClosestDirection(cubeA, (cubeB.position - cubeA.position).normalized);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(cubeA.position, direction * 10f);
    } */
}