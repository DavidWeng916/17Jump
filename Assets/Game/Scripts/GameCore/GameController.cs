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

        private DataModel DataModel => JumpApp.Instance.DataModel;

        public Action onStartGame = null;
        public Action onEndGame = null;

        public void Init()
        {
            _platformManager.Init();

            _cameraManager.Init(_platformManager.GetCenterPointOfBothPlatforms());

            _playerController.Init();
            _playerController.onRequestCurrentPlatformUnit = OnRequestCurrentPlatformUnit;
            _playerController.onRequestTargetPlatformUnit = OnRequestNextPlatformUnit;
            _playerController.onAccumulateEnergy = OnAccumulateEnergy;
            _playerController.onJumpStart = OnJumpStart;
            _playerController.onJumpComplete = OnJumpComplete;
            _playerController.RefreshOriginPositionAndForward();
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

        private void OnAccumulateEnergy(float accumulateProgress)
        {
            PlatformUnit currentPlatformUnit = GetCurrentPlatformUnit();
            float offsetY = currentPlatformUnit.UpdateScaleHeight(accumulateProgress);

            _playerController.UpdateScaleHeight(accumulateProgress, offsetY);
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
                    _playerController.PlayFallDownAnimation(EndGame);
                    break;
            }
        }

        #endregion

        public void StartGame()
        {
            _playerController.SetControl(true);

            onStartGame();
        }

        private void StartNextRound()
        {
            _platformManager.SpawnNextPlatform();

            _playerController.RefreshForward();
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
            uint score = GameLogicUtility.CalculateScore(_playerController.LocalPosition, targetPlatformUnit.PlatformLocalPoint);

            DataModel.AddScore(score);
        }

        /* void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) RefreshCamerPosition();
        } */
    }
}