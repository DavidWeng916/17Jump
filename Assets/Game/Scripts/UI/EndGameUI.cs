using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Live17Game
{
    public class EndGameUI : PanelUI
    {
        [SerializeField]
        private TextMeshProUGUI _titleText = null;

        [SerializeField]
        private Button _playAgain = null;

        public Action onPlayAgain = null;

        void OnEnable()
        {
            _playAgain.onClick.AddListener(OnPlayAgain);
        }

        void OnDisable()
        {
            _playAgain.onClick.RemoveListener(OnPlayAgain);
        }

        public void Init()
        {
            base.Init(UIPanelFlag.EndGame);
        }

        public void Show(bool isWin)
        {
            _titleText.text = isWin ? "YOU WIN!!" : "YOU LOSE...";
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        private void OnPlayAgain()
        {
            Hide();
            onPlayAgain();
        }
    }
}