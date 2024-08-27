using System.Collections;
using System.Collections.Generic;
using Live17Game;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _particleSystem = null;

    void Awake()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) Play();
        if (Input.GetKeyDown(KeyCode.Alpha2)) Stop();
    }

    private void Play()
    {
        _particleSystem.Play();
    }

    private void Stop()
    {
        _particleSystem.Stop();
        _particleSystem.Clear();
    }

    /* void OnDrawGizmos()
    {
        Vector3 direction = MathUtility.GetClosestDirection(cubeA, (cubeB.position - cubeA.position).normalized);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(cubeA.position, direction * 10f);
    } */
}