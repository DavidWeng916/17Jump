using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Live17Game
{
    public static class GameLogicUtility
    {
        public static JumpResult GetJumpResult(Vector3 localPosition, PlatformUnit currentPlatformUnit, PlatformUnit targetPlatformUnit)
        {
            float distance;

            distance = MathUtility.GetXZDistance(localPosition, currentPlatformUnit.PlatformLocalPoint);
            if (distance <= currentPlatformUnit.Radius)
            {
                return JumpResult.None;
            }

            distance = MathUtility.GetXZDistance(localPosition, targetPlatformUnit.PlatformLocalPoint);
            if (distance <= targetPlatformUnit.Radius)
            {
                return JumpResult.Success;
            }

            return JumpResult.Fail;
        }
    }
}