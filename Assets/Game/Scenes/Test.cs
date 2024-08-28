using System.Collections;
using System.Collections.Generic;
using Live17Game;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _particleSystem = null;

    [SerializeField]
    private AudioSource _audioSource = null;

    void Awake()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) Play();
        if (Input.GetKeyDown(KeyCode.Alpha2)) Stop();

        // _audioSource.pitch
    }

    private void Play()
    {
        _audioSource.Play();
    }

    private void Stop()
    {

    }

    /* void OnDrawGizmos()
    {
        Vector3 direction = MathUtility.GetClosestDirection(cubeA, (cubeB.position - cubeA.position).normalized);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(cubeA.position, direction * 10f);
    } */
}