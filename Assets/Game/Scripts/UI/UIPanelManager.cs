using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Live17Game
{
    [Flags]
    public enum UIPanelFlag
    {
        None = 0,
        // Tutorial = 1 << 0,
        EndGame = 1 << 1,
    }

    public class UIPanelManager : MonoBehaviour
    {
        public static UIPanelManager Instance { get; private set; } = null;

        [SerializeField]
        private EndGameUI _endGameUI = null;
        public EndGameUI EndGameUI => _endGameUI;

        private UIPanelFlag _uiPanelFlag = UIPanelFlag.None;

        public bool IsDisable => _uiPanelFlag.Equals(UIPanelFlag.None);
        public bool IsEnable => !IsDisable;

        public void Init()
        {
            Instance = this;

            _endGameUI.Init();
        }

        void OnDestroy()
        {
            Instance = null;
        }

        // https://code-corner.dev/2023/11/04/Understanding-Flag-Enums-in-C/
        public void AddUIPanel(UIPanelFlag flag)
        {
            _uiPanelFlag |= flag;
        }

        public void RemoveUIPanel(UIPanelFlag flag)
        {
            _uiPanelFlag &= ~flag;
        }

        public bool HasFlag(UIPanelFlag flag)
        {
            return _uiPanelFlag.HasFlag(flag);
            // return (_uiPanelFlag & flag) != UIPanel.None;
            // return (_uiPanelFlag & flag) == flag;
        }
    }
}