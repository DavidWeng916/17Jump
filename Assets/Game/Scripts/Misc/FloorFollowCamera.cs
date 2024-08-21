using UnityEngine;

namespace Live17Game
{
    public class FloorFollowCamera : MonoBehaviour
    {
        [SerializeField]
        private Transform _cameraTs = null;

        void LateUpdate()
        {
            Vector3 point = _cameraTs.position;
            point.y = 0f;

            transform.position = point;
        }
    }
}