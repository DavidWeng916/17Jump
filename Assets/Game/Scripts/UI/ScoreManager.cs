using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Live17Game
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _bestScoreText = null;

        [SerializeField]
        private TextMeshProUGUI _currentScoreText = null;

        private DataModel DataModel => JumpApp.Instance.DataModel;

        public void Init()
        {
            RegisterEvents();

            RefreshBestScore(DataModel.BestScore);
            RefreshCurrentScore(DataModel.CurrentScore);
        }

        void OnDestroy()
        {
            UnregisterEvents();
        }

        private void RegisterEvents()
        {
            DataModel.onBestScoreUpdated += OnBestScoreUpdated;
            DataModel.onCurrentScoreUpdated += OnCurrentScoreUpdated;
        }

        private void UnregisterEvents()
        {
            if (!JumpApp.IsExist)
                return;

            DataModel.onBestScoreUpdated -= OnBestScoreUpdated;
            DataModel.onCurrentScoreUpdated -= OnCurrentScoreUpdated;
        }

        private void OnBestScoreUpdated(uint bestScore)
        {
            RefreshBestScore(bestScore);
        }

        private void OnCurrentScoreUpdated(uint currentScore)
        {
            RefreshCurrentScore(currentScore);
        }

        private void RefreshBestScore(uint bestScore)
        {
            _bestScoreText.text = bestScore.ToString();
        }

        private void RefreshCurrentScore(uint score)
        {
            _currentScoreText.text = score.ToString();
        }
    }
}