using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Transform cameraTs;
    public Transform targetTs;

    // private float duration = 100f;
    private Vector3 targetPoint => targetTs.transform.position;
    private Vector3 cameraPoint => cameraTs.transform.position;
    private float height => cameraPoint.y - targetPoint.y;
    private float distance = 0f;
    private Vector3 planeOnNormal = Vector3.up;
    private Vector3 vector => -cameraTs.transform.forward * distance;
    private Vector3 result => Vector3.ProjectOnPlane(vector, planeOnNormal);

    void Awake()
    {
        distance = Vector3.Distance(cameraPoint, targetPoint);


        Run();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) Run();
        // Run();
    }

    private void Run()
    {
        // Debug.DrawRay(targetPoint, planeOnNormal, Color.green, duration);
        // Debug.DrawRay(targetPoint, vector, Color.blue, duration);
        // Debug.DrawRay(targetPoint + planeOnNormal, result, Color.yellow, duration);
        // Debug.DrawRay(targetPoint, result, Color.red, duration);


        // Camera.main.transform.position = targetPoint + result;
        // Camera.main.transform.position = targetPoint + (vector - result);

        // var h = vector - result;
        // Camera.main.transform.position = targetPoint + result + h;
        Camera.main.transform.position = targetPoint + vector;
    }

    void OnDrawGizmos()
    {
        // Gizmos.color = Color.green;
        // Gizmos.DrawRay(targetPoint, planeOnNormal);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(targetPoint, vector);

        // Gizmos.color = Color.yellow;
        // Gizmos.DrawRay(targetPoint, result);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(targetPoint + planeOnNormal * height, result);
    }

    /* void OnGUI()
    {
        //Output the angle found above
        // GUI.Label(new Rect(25, 25, 200, 40), "Angle Between Objects" + m_Angle);
    } */
}