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

        public static uint GetPlatformSize(PlatformSizeRange platformSizeRange)
        {
            return (uint)Random.Range(platformSizeRange.UnitMin, platformSizeRange.UnitMax + 1);
        }

        public static float GetPlatformDistance(uint safeDistance, uint distanceUnit)
        {
            uint baseDistance = 1;
            uint randomDistance = MathUtility.RandomRangeIncludeMax(0, distanceUnit);
            uint increaseUnit = baseDistance + randomDistance;
            uint finalDistance = safeDistance + increaseUnit;

            // Debug.Log($"===== safeDistance:{safeDistance} randomDistance:{randomDistance} increaseUnit:{increaseUnit} dinalDistance:{finalDistance}");

            return finalDistance;
        }

        public static uint CalculateScore(Vector3 playerPoint, Vector3 platformPoint)
        {
            Vector2 p1 = playerPoint.ToVectorXZ();
            Vector2 p2 = platformPoint.ToVectorXZ();

            Debug.Log($"playerPoint:{playerPoint} platformPoint:{platformPoint}");

            float distance = Vector2.Distance(p1, p2);
            uint score = distance < 0.1f ? 2u : 1u;


            Debug.Log($"distance:{distance} score:{score}");


            return score;
        }
    }
}