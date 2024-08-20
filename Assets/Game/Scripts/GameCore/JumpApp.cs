using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Live17Game
{
    public class JumpApp : MonoBehaviour
    {
        public static JumpApp Instance { get; private set; } = null;
        public static bool IsExist { get; private set; } = Instance != null;

        [SerializeField]
        private UIManager _uiManager = null;
        public UIManager UIManager => _uiManager;

        [SerializeField]
        private GameController _gameController = null;

        private DataModel _dataModel = null;
        public DataModel DataModel => _dataModel;

        void Awake()
        {
            Instance = this;
            InitGameConfig();

            _dataModel = new DataModel();
            _dataModel.Init();

            _uiManager.Init();
            _uiManager.onResetGame = OnResetGame;
            _uiManager.onPlayAgain = OnPlayAgain;

            _gameController.Init();
            _gameController.onStartGame = OnStartGame;
            _gameController.onEndGame = OnEndGame;
            _gameController.StartGame();
        }

        void OnDestroy()
        {
            Instance = null;
        }

        private void InitGameConfig()
        {
            Application.targetFrameRate = 60;
        }

        private void OnStartGame()
        {
            _uiManager.GameStart();
        }

        private void OnEndGame()
        {
            _uiManager.GameEnd();
        }

        private void OnResetGame()
        {
            ResetGame();
        }

        private void OnPlayAgain()
        {
            ResetGame();
        }

        private void ResetGame()
        {
            SceneManager.LoadScene("DemoScene");
            // _uiManager.Reset();
            // _gameController.ResetGame();
        }
    }
}