using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Live17Game
{
    public class ForwardToCamera : MonoBehaviour
    {
        void Update()
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}