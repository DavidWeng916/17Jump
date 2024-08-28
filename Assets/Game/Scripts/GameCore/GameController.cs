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
        private CharacterController _characterController = null;

        [SerializeField]
        private EffectManager _effectManager = null;

        private DataModel DataModel => JumpApp.Instance.DataModel;

        public Action onStartGame = null;
        public Action onEndGame = null;
        public Action<Vector3, uint> onScore = null;

        public void Init()
        {
            _effectManager.Init();

            _platformManager.Init();

            _cameraManager.Init(_platformManager.GetCenterPointOfBothPlatforms());

            _characterController.Init();
            _characterController.onRequestCurrentPlatformUnit = OnRequestCurrentPlatformUnit;
            _characterController.onRequestTargetPlatformUnit = OnRequestNextPlatformUnit;
            _characterController.onSpawnAnimationComplete = OnSpawnAnimationComplete;

            _characterController.onJumpStart = OnJumpStart;
            _characterController.onJumpComplete = OnJumpComplete;
            _characterController.RefreshOriginPositionAndForward();

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
            _characterController.PrepareJump();
        }

        private void OnAccumulateEnergy(float accumulateProgress)
        {
            PlatformUnit currentPlatformUnit = GetCurrentPlatformUnit();
            float offsetY = currentPlatformUnit.UpdateScaleHeight(accumulateProgress);

            _characterController.UpdateScaleHeight(accumulateProgress, offsetY);
        }

        private void OnAccumulateEnergyComplete(float accumulateProgress)
        {
            PlatformUnit targetPlatformUnit = GetTargetPlatformUnit();
            targetPlatformUnit.SetDisplayCenterTip(false);

            _characterController.Jump(accumulateProgress);
        }

        private void OnJumpStart()
        {
            PlatformUnit currentPlatformUnit = GetCurrentPlatformUnit();
            currentPlatformUnit.RestoreScaleHeight();

            PlaySFX(SFXEnum.Jump);
        }

        private void OnJumpComplete(Vector3 localPosition)
        {
            PlatformUnit currentPlatformUnit = GetCurrentPlatformUnit();
            PlatformUnit targetPlatformUnit = GetTargetPlatformUnit();

            JumpResult jumpResult = GameLogicUtility.GetJumpResult(localPosition, currentPlatformUnit, targetPlatformUnit);
            switch (jumpResult)
            {
                case JumpResult.None:
                    HandleNoneState();
                    break;
                case JumpResult.Success:
                    HandleSuccessState();
                    break;
                case JumpResult.Fail:
                    HandleFailState();
                    break;
            }
        }

        private void HandleNoneState()
        {
            PlaySFX(SFXEnum.FallGround);
            _playerController.SetControl(true);
        }

        private void HandleSuccessState()
        {
            PlaySFX(SFXEnum.FallGround);
            _characterController.PlayOnGroundEffect();

            CheckScore();
            StartNextRound();
        }

        private void HandleFailState()
        {
            _characterController.PlayFallDownAnimation(() =>
            {
                PlaySFX(SFXEnum.FallGround);
                EndGame();
            });
        }

        #endregion

        public void StartGame()
        {
            _characterController.PlaySpawnAnimation();

            onStartGame();
        }

        private void StartNextRound()
        {
            _platformManager.SpawnNextPlatform();
            _playerController.SetControl(true);
            // _characterController.RefreshForward();

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

        private void CheckScore()
        {
            PlatformUnit targetPlatformUnit = GetTargetPlatformUnit();

            bool isPerfect = DataModel.CheckIsPerfect(_characterController.LocalPosition, targetPlatformUnit.PlatformLocalPoint);
            uint score = DataModel.GetScore(isPerfect);
            Debug.Log($"isPerfect:{isPerfect} score:{score}");

            DataModel.AddScore(score);
            onScore(_characterController.Position + new Vector3(0, 1f, 0), score);

            if (isPerfect)
            {
                PlaySFX(SFXEnum.PerfectScore);
                PlayVFX(EffectID.LightCircle, _characterController.LightCircleEffectPosition, _characterController.Rotation);
            }
        }

        private void PlaySFX(SFXEnum sfxID)
        {
            AudioManager.Instance.PlaySFX(sfxID);
        }

        private void PlayVFX(EffectID vfxID, Vector3 position, Quaternion rotation)
        {
            EffectManager.Instance.Play(vfxID, position, rotation);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) _playerController.ToggleCheat();
            // if (Input.GetKeyDown(KeyCode.Alpha2)) EffectManager.Instance.Play(EffectID.LightCircle, _characterController.Position, _characterController.Rotation);
        }
    }
}