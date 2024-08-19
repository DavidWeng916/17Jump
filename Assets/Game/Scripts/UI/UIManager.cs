using System;
using UnityEngine;

namespace Live17Game
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private UIPanelManager _uiPanelManager = null;

        public Action onResetGame = null;
        public Action onPlayAgain = null;

        public void Init()
        {
            _uiPanelManager.Init();
            _uiPanelManager.EndGameUI.onPlayAgain = OnPlayAgain;
        }

        private void OnShowSettingUI()
        {
            Debug.Log("OnShowSettingUI");
        }

        private void OnResetGame()
        {
            onResetGame();
        }

        private void OnPlayAgain()
        {
            onPlayAgain();
        }

        public void GameStart()
        {

        }

        public void GameEnd()
        {
            _uiPanelManager.EndGameUI.Show();
        }

        public void Reset()
        {

        }
    }
}