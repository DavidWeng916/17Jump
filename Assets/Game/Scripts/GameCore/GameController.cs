using System;
using UnityEngine;

namespace Live17Game
{
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        private CameraManager _cameraManager = null;

        [SerializeField]
        private PlatformManager _platformManager = null;

        [SerializeField]
        private PlayerController _playerController = null;

        [SerializeField]
        private CharacterUnit _characterUnit = null;

        private DataModel DataModel => JumpApp.Instance.DataModel;

        public Action onStartGame = null;
        public Action onEndGame = null;
        public Action<Vector3, uint> onScore = null;

        public void Init()
        {
            _platformManager.Init();

            _cameraManager.Init(_platformManager.GetCenterPointOfBothPlatforms());

            _characterUnit.Init();
            _characterUnit.onRequestCurrentPlatformUnit = OnRequestCurrentPlatformUnit;
            _characterUnit.onRequestTargetPlatformUnit = OnRequestNextPlatformUnit;
            _characterUnit.onSpawnAnimationComplete = OnSpawnAnimationComplete;

            _characterUnit.onJumpStart = OnJumpStart;
            _characterUnit.onJumpComplete = OnJumpComplete;
            _characterUnit.RefreshOriginPositionAndForward();

            _playerController.Init();
            _playerController.onAccumulateEnergyReady = OnAccumulateEnergyReady;
            _playerController.onAccumulateEnergy = OnAccumulateEnergy;
            _playerController.onAccumulateEnergyComplete = OnAccumulateEnergyComplete;
        }

        public void ResetGame()
        {
            DataModel.Reset();

            StartGame();
        }

        #region PlayerController

        private PlatformUnit OnRequestCurrentPlatformUnit()
        {
            return GetCurrentPlatformUnit();
        }

        private PlatformUnit OnRequestNextPlatformUnit()
        {
            return GetTargetPlatformUnit();
        }

        private void OnSpawnAnimationComplete()
        {
            _playerController.SetControl(true);
        }

        private void OnAccumulateEnergyReady(float accumulateProgress)
        {
            _characterUnit.RefreshForward();
        }

        private void OnAccumulateEnergy(float accumulateProgress)
        {
            PlatformUnit currentPlatformUnit = GetCurrentPlatformUnit();
            float offsetY = currentPlatformUnit.UpdateScaleHeight(accumulateProgress);

            _characterUnit.UpdateScaleHeight(accumulateProgress, offsetY);
        }

        private void OnAccumulateEnergyComplete(float accumulateProgress)
        {
            PlatformUnit targetPlatformUnit = GetTargetPlatformUnit();
            targetPlatformUnit.SetDisplayCenterTip(false);

            _characterUnit.Jump(accumulateProgress);
        }

        private void OnJumpStart()
        {
            PlatformUnit currentPlatformUnit = GetCurrentPlatformUnit();
            currentPlatformUnit.RestoreScaleHeight();
        }

        private void OnJumpComplete(Vector3 localPosition)
        {
            PlatformUnit currentPlatformUnit = GetCurrentPlatformUnit();
            PlatformUnit targetPlatformUnit = GetTargetPlatformUnit();

            JumpResult jumpResult = GameLogicUtility.GetJumpResult(localPosition, currentPlatformUnit, targetPlatformUnit);
            // Debug.Log($"jumpResult:{jumpResult}");

            switch (jumpResult)
            {
                case JumpResult.None:
                    _playerController.SetControl(true);
                    break;
                case JumpResult.Success:
                    RefreshScore();
                    StartNextRound();
                    break;
                case JumpResult.Fail:
                    _characterUnit.PlayFallDownAnimation(EndGame);
                    break;
            }
        }

        #endregion

        public void StartGame()
        {
            _characterUnit.PlaySpawnAnimation();

            onStartGame();
        }

        private void StartNextRound()
        {
            _platformManager.SpawnNextPlatform();

            _characterUnit.RefreshForward();
            _playerController.SetControl(true);

            RefreshCamerPosition();
        }

        private void EndGame()
        {
            onEndGame();
        }

        private PlatformUnit GetCurrentPlatformUnit()
        {
            return _platformManager.CurrentPlatformUnit;
        }

        private PlatformUnit GetTargetPlatformUnit()
        {
            return _platformManager.TargetPlatformUnit;
        }

        private void RefreshCamerPosition()
        {
            _cameraManager.SetFollowTarget(_platformManager.GetCenterPointOfBothPlatforms(), true);
        }

        private void RefreshScore()
        {
            PlatformUnit targetPlatformUnit = GetTargetPlatformUnit();
            bool isPerfect = DataModel.CheckIsPerfect(_characterUnit.LocalPosition, targetPlatformUnit.PlatformLocalPoint);
            uint score = DataModel.GetScore(isPerfect);
            Debug.Log($"isPerfect:{isPerfect} score:{score}");
            DataModel.AddScore(score);
            onScore(_characterUnit.WorldPosition + new Vector3(0, 1f, 0), score);
        }

        /* void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha2)) onScore(_playerController.WorldPosition, 1);
        } */
    }
}