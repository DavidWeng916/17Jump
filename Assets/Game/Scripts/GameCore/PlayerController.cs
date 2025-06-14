using System;
using UnityEngine;

namespace Live17Game
{
    public class PlayerController : MonoBehaviour
    {
#if UNITY_EDITOR
        public static bool IS_CHEAT_ENABLE { get; private set; } = false;
#endif
        private bool _isCanJump = false;
        private bool _isPassing = false;
        private float _pressTime = 0f;

        private float AccumulateProgress => _pressTime / DataModel.PRESS_TME_MAX;

        private IntervalSetting _accumulateEnergySFXSetting = new IntervalSetting(0.1f, 0.2f, 0.025f, () => AudioManager.Instance.PlaySFX(SFXEnum.AccumulateEnergy));

        public Action<float> onAccumulateEnergyReady = null;
        public Action<float> onAccumulateEnergy = null;
        public Action<float> onAccumulateEnergyComplete = null;

        public void Init()
        {
            SetControl(false);
        }

        void Update()
        {
            if (!_isCanJump)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                SetPassing(true);
                ResetEnergy();
                PropagateAccumulateEnergyReady();
            }

            if (Input.GetMouseButton(0) && _isPassing)
            {
                AccumulateEnergy();
            }

            if (Input.GetMouseButtonUp(0) && _isPassing)
            {
                PropagateAccumulateEnergyComplete();
                ResetEnergy();
                SetControl(false);
                SetPassing(false);
            }
        }

        public void SetControl(bool isCanJump)
        {
            _isCanJump = isCanJump;
        }

        private void SetPassing(bool isPassing)
        {
            _isPassing = isPassing;
        }

        private void ResetEnergy()
        {
            _pressTime = 0;
        }

        private void AccumulateEnergy()
        {
            _pressTime = Mathf.Min(_pressTime + Time.deltaTime, DataModel.PRESS_TME_MAX);
            PropagateAccumulateEnergy();
        }

        private void PropagateAccumulateEnergyReady()
        {
            _accumulateEnergySFXSetting.Play();
            onAccumulateEnergyReady(AccumulateProgress);
        }

        private void PropagateAccumulateEnergy()
        {
            _accumulateEnergySFXSetting.Update();
            onAccumulateEnergy(AccumulateProgress);
        }

        private void PropagateAccumulateEnergyComplete()
        {
            _accumulateEnergySFXSetting.Stop();
            onAccumulateEnergyComplete(AccumulateProgress);
        }

        public void ToggleCheat()
        {
#if UNITY_EDITOR
            IS_CHEAT_ENABLE = !IS_CHEAT_ENABLE;
            Debug.Log($"_isCheat:{IS_CHEAT_ENABLE}");
#endif
        }
    }
}