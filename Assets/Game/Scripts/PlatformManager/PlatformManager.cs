using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Live17Game
{
    public class PlatformManager : ObjectPoolManagerBase<PlatformUnit>
    {
        public PlatformUnit CurrentPlatformUnit { get; private set; } = null;
        public PlatformUnit TargetPlatformUnit { get; private set; } = null;

        public void Init()
        {
            base.Init(DEFAULT_CAPACITY, MAX_SIZE);

            SpawnFirstPlatformUnit();
            SpawnNextPlatform();
        }

        public void SpawnNextPlatform()
        {
            if (TargetPlatformUnit != null)
            {
                CurrentPlatformUnit = TargetPlatformUnit;
            }

            SpawnTargetPlatformUnit();
        }

        private void SpawnFirstPlatformUnit()
        {
            PlatformUnit platformUnit = ObtainPlatformUnit(3);
            platformUnit.SetLocalPositionAndRotation(Vector3.zero, Quaternion.AngleAxis(180f, Vector3.up));

            CurrentPlatformUnit = platformUnit;
        }

        private void SpawnTargetPlatformUnit()
        {
            float distance = 6f;

            SpawnDirection spawnDirection = (SpawnDirection)Random.Range(0, 2);
            // Debug.Log($"SpawnTargetPlatformUnit:{spawnDirection}");
            // SpawnDirection spawnDirection = SpawnDirection.Left;
            Vector3 direction = spawnDirection == SpawnDirection.Left ? Vector3.left : Vector3.forward;
            float angle = spawnDirection == SpawnDirection.Left ? 90f : 180f;

            PlatformUnit platformUnit = ObtainPlatformUnit();

            Vector3 targetLocalPosition = CurrentPlatformUnit.LocalPosition + direction * distance;
            platformUnit.SetLocalPositionAndRotation(targetLocalPosition, Quaternion.AngleAxis(angle, Vector3.up));

            TargetPlatformUnit = platformUnit;
        }

        private PlatformUnit ObtainPlatformUnit(uint size = 0)
        {
            // uint id = GetIdentity();
            if (size == 0)
            {
                size = (uint)Random.Range(2, 4);
            }

            PlatformUnit platformUnit = Obtain();
            platformUnit.SetData(size);

            // AddToActiveItemUnitMap(itemUnit);

            return platformUnit;
        }

        public Vector3 GetCenterPointOfBothPlatforms()
        {
            return (CurrentPlatformUnit.PlatformLocalPoint + TargetPlatformUnit.PlatformLocalPoint) * 0.5f;
        }
    }
}