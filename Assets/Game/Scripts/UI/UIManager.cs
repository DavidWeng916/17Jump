using System;
using System.Collections;
using UnityEngine;

namespace Live17Game
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private DisplayLevelUI _displayLevelUI = null;

        [SerializeField]
        private UIPanelManager _uiPanelManager = null;

        [SerializeField]
        private ScoreManager _scoreManager = null;

        [SerializeField]
        private UIEffectManager _uiEffectManager = null;

        public Action onResetGame = null;
        public Action onPlayAgain = null;

        public void Init()
        {
            _displayLevelUI.Init();

            _scoreManager.Init();
            _uiEffectManager.Init();

            _uiPanelManager.Init();
            _uiPanelManager.EndGameUI.onPlayAgain = OnPlayAgain;
        }

        public void Reset()
        {

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
            StartCoroutine(DelayShowEndGameUI());
        }

        private IEnumerator DelayShowEndGameUI()
        {
            yield return new WaitForSeconds(1f);
            _uiPanelManager.EndGameUI.Show();
        }

        public void SpawnScore(Vector3 worldPoint, uint score)
        {
            _uiEffectManager.SpawnScore(worldPoint, score);
        }
    }
}