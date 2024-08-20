using UnityEngine;

namespace Live17Game
{
    public static class MathUtility
    {
        public static uint RandomRangeIncludeMax(uint min, uint max)
        {
            return (uint)Random.Range(min, max + 1);
        }

        public static Vector2 ToVectorXZ(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        public static float DistanceXZ(Vector3 a, Vector3 b)
        {
            return Vector2.Distance(a.ToVectorXZ(), b.ToVectorXZ());
        }

        public static Vector3 DirectionXZ(Vector3 origin, Vector3 target)
        {
            origin.y = 0f;
            target.y = 0f;
            return (target - origin).normalized;
        }

        public static Vector3 GetLandPoint(Vector3 origin, Vector3 target, float jumpLength)
        {
            return origin + DirectionXZ(origin, target) * jumpLength;
        }

        public static Vector3 GetClosestDirection(Transform transform, Vector3 targetDirection)
        {
            targetDirection.Normalize();

            float minAngle = Mathf.Infinity;

            Vector3 closestDirection = transform.forward;

            Vector3[] directions = new Vector3[]
            {
                transform.forward,
                transform.right,
                -transform.forward,
                -transform.right,
                // transform.up,
                // -transform.up
            };

            // 遍历每个方向，计算夹角
            foreach (Vector3 direction in directions)
            {
                float angle = Vector3.Angle(targetDirection, direction);
                if (angle < minAngle)
                {
                    minAngle = angle;
                    closestDirection = direction;
                }
            }

            return closestDirection;
        }
    }
}