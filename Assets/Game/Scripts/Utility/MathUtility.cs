using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Live17Game
{
    public static class MathUtility
    {
        public static Vector2 ToVectorXZ(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        public static float GetXZDistance(Vector3 a, Vector3 b)
        {
            return Vector2.Distance(a.ToVectorXZ(), b.ToVectorXZ());
        }

        public static Vector3 GetXZDirection(Vector3 origin, Vector3 target)
        {
            origin.y = 0f;
            target.y = 0f;
            return target - origin;
        }

        public static Vector3 GetLandPoint(Vector3 origin, Vector3 target, float jumpLength)
        {
            Vector3 direction = MathUtility.GetXZDirection(origin, target).normalized;
            return origin + direction * jumpLength;
        }
    }
}