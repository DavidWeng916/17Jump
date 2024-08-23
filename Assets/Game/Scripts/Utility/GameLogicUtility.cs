using UnityEngine;

namespace Live17Game
{
    public static class GameLogicUtility
    {
        public static JumpResult GetJumpResult(Vector3 localPosition, PlatformUnit currentPlatformUnit, PlatformUnit targetPlatformUnit)
        {
            float distance;

            distance = MathUtility.DistanceXZ(localPosition, currentPlatformUnit.PlatformLocalPoint);
            if (distance <= currentPlatformUnit.Radius)
            {
                return JumpResult.None;
            }

            distance = MathUtility.DistanceXZ(localPosition, targetPlatformUnit.PlatformLocalPoint);
            if (distance <= targetPlatformUnit.Radius)
            {
                return JumpResult.Success;
            }

            return JumpResult.Fail;
        }

        public static uint GetPlatformRandomSize(PlatformSizeRange platformSizeRange)
        {
            return (uint)Random.Range(platformSizeRange.UnitMin, platformSizeRange.UnitMax + 1);
        }

        public static float GetPlatformRandomDistance(uint safeDistance, uint distanceUnit)
        {
            uint baseDistance = 1;
            uint randomDistance = MathUtility.RandomRangeIncludeMax(0, distanceUnit);
            uint increaseUnit = baseDistance + randomDistance;
            uint finalDistance = safeDistance + increaseUnit;

            // Debug.Log($"===== safeDistance:{safeDistance} randomDistance:{randomDistance} increaseUnit:{increaseUnit} dinalDistance:{finalDistance}");

            return finalDistance;
        }

        public static Vector3 Interpolate(Vector3 currentPoint, Vector3 nearestPoint, Vector3 farestPoint, float accumulateProgress)
        {
            float jumpLength = accumulateProgress * DataModel.JUMP_LENGTH;
            float distanceP0ToP1 = MathUtility.DistanceXZ(currentPoint, nearestPoint);

            if (jumpLength < distanceP0ToP1)
            {
                return MathUtility.GetLandPoint(currentPoint, nearestPoint, jumpLength);
            }

            return MathUtility.GetLandPoint(nearestPoint, farestPoint, jumpLength - distanceP0ToP1);
        }
    }
}