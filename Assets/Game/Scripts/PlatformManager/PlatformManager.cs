using UnityEngine;

namespace Live17Game
{
    public class PlatformManager : ObjectPoolManagerBase<PlatformUnit>
    {
        public PlatformUnit CurrentPlatformUnit { get; private set; } = null;
        public PlatformUnit TargetPlatformUnit { get; private set; } = null;

        private DataModel DataModel => JumpApp.Instance.DataModel;

        public void Init()
        {
            base.Init(DEFAULT_CAPACITY, MAX_SIZE);

            SpawnDefaultFirstPlatformUnit();
            SpawnDefaultSecondPlatformUnit();
        }

        private void SpawnDefaultFirstPlatformUnit()
        {
            CurrentPlatformUnit = ObtainPlatformUnit(DataModel.GetDefaultFirstPlatformData());
        }

        private void SpawnDefaultSecondPlatformUnit()
        {
            SpawnTargetPlatformUnit(DataModel.GetDefaultSecondPlatformData(CurrentPlatformUnit), false);
        }

        public void SpawnNextPlatform()
        {
            if (TargetPlatformUnit != null)
            {
                CurrentPlatformUnit = TargetPlatformUnit;
            }

            SpawnTargetPlatformUnit(DataModel.GetNextPlatformData(CurrentPlatformUnit), true);
        }

        private void SpawnTargetPlatformUnit(PlatformData platformData, bool isAnimate)
        {
            PlatformUnit platformUnit = ObtainPlatformUnit(platformData);

            TargetPlatformUnit = platformUnit;
            TargetPlatformUnit.SetOppositePlatformUnit(CurrentPlatformUnit);
            CurrentPlatformUnit.SetOppositePlatformUnit(TargetPlatformUnit);

            if (isAnimate)
            {
                TargetPlatformUnit.PlaySpawnAnimation();
            }
        }

        private PlatformUnit ObtainPlatformUnit(PlatformData platformData)
        {
            // uint id = GetIdentity();

            DataModel.IncreasePlatformCount();

            PlatformUnit platformUnit = Obtain();
            platformUnit.SetData(platformData);

            // AddToActiveItemUnitMap(itemUnit);

            return platformUnit;
        }

        public Vector3 GetCenterPointOfBothPlatforms()
        {
            return (CurrentPlatformUnit.PlatformLocalPoint + TargetPlatformUnit.PlatformLocalPoint) * 0.5f;
        }
    }
}