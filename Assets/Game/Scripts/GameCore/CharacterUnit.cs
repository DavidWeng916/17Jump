using UnityEngine;

namespace Live17Game
{
    public class CharacterUnit : MonoBehaviour
    {
        [SerializeField]
        public Transform _rotateXTs;
        public Transform RotateXTs => _rotateXTs;

        [SerializeField]
        public Transform _trailContainer;
        public Transform TrailContainer => _trailContainer;

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}