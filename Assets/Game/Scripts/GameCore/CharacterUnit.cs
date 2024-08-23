using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Live17Game
{
    public class CharacterUnit : MonoBehaviour
    {
        public Transform RotateXTs;
        public Transform TrailContainer;

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}