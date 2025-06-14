using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Live17Game
{
    public class DataModel
    {
        public const float PRESS_TME_MAX = 3f;
        public const float JUMP_LENGTH = 20.0f;
        public const uint DEFAULT_PLATFORM_HEIGHT = 2;

        public const float CAMERA_MOVE_DURATION = 0.5f;
        public const float CAMERA_DISTANCE = 15f;

        private const uint SCORE_NORMAL = 1u;
        private const uint SCORE_PERFECT = 2u;
        public const float PERFECT_RADIUS = 0.25f;
        public const float PERFECT_DIAMETER = PERFECT_RADIUS * 2;

        private GameData _gameData = null;
        public bool IsPerfect { get; private set; } = false;
        private uint _perfectCount = 0u;

        private uint _createdPlatformCount = 0;
        public uint CreatedPlatformCount
        {
            get => _createdPlatformCount;
            set
            {
                _createdPlatformCount = value;
                CurrentLevel = (uint)Mathf.FloorToInt(CreatedPlatformCount / 10);
                onLevelUpdated?.Invoke(CreatedPlatformCount, CurrentLevel);
            }
        }
        public uint CurrentLevel { get; private set; } = 0;

        public uint BestScore
        {
            get => _gameData.BestScore;
            private set => _gameData.BestScore = value;
        }

        public uint _currentScore = 0;
        public uint CurrentScore
        {
            get => _currentScore;
            private set
            {
                _currentScore = value;
                onCurrentScoreUpdated?.Invoke(_currentScore);
            }
        }

        public bool MusicToggle
        {
            get => _gameData.MusicToggle;
            private set
            {
                _gameData.MusicToggle = value;
                onMusicToggleUpdated?.Invoke(_gameData.MusicToggle);
            }
        }

        public bool IsDisplayCenterTip => _perfectCount > 0;

        public GameState GameState { get; private set; } = GameState.None;
        public bool IsGameStart => GameState == GameState.Start;
        public bool IsGameEnd => GameState == GameState.End;

        public event Action<uint> onBestScoreUpdated = null;
        public event Action<uint> onCurrentScoreUpdated = null;
        public event Action<bool> onMusicToggleUpdated = null;
        public event Action<uint, uint> onLevelUpdated = null;

        public void Init()
        {
            Reset();
            LoadGameData();
        }

        public void Reset()
        {
            SetGameState(GameState.None);
            DispatchBestScoreUpdate();

            CurrentScore = 0;
            CreatedPlatformCount = 0;

            IsPerfect = false;
            _perfectCount = 0u;
        }

        private void LoadGameData()
        {
            _gameData = StorageManager.LoadGameData();
        }

        public bool CheckIsPerfect(Vector3 playerPoint, Vector3 platformPoint)
        {
            Vector2 p1 = playerPoint.ToVectorXZ();
            Vector2 p2 = platformPoint.ToVectorXZ();

            return Vector2.Distance(p1, p2) <= PERFECT_RADIUS;
        }

        public uint GetScore(bool isPerfect)
        {
            if (IsPerfect && isPerfect)
            {
                _perfectCount++;
            }
            else
            {
                _perfectCount = 0u;

            }
            IsPerfect = isPerfect;

            uint baseScore = IsPerfect ? SCORE_PERFECT : SCORE_NORMAL;
            uint score = baseScore + _perfectCount * SCORE_PERFECT;

            return score;
        }

        public void AddScore(uint score)
        {
            CurrentScore += score;

            if (CurrentScore > BestScore)
            {
                BestScore = CurrentScore;
                SaveGameData();
            }
        }

        public void SetMusicToggle(bool inOn)
        {
            if (MusicToggle != inOn)
            {
                MusicToggle = inOn;
                SaveGameData();
            }
        }

        private void SaveGameData()
        {
            StorageManager.SaveGameData(_gameData);
        }

        public void SetGameState(GameState gameState)
        {
            GameState = gameState;
        }

        public void IncreasePlatformCount()
        {
            CreatedPlatformCount++;
        }

        public PlatformData GetDefaultFirstPlatformData()
        {
            return new PlatformData(3, 180f, Vector3.zero);
        }

        public PlatformData GetDefaultSecondPlatformData(PlatformUnit currentPlatformUnit)
        {
            SpawnDirection spawnDirection = SpawnDirection.Right;
            Vector3 direction = spawnDirection == SpawnDirection.Left ? Vector3.left : Vector3.forward;
            int distance = 6;

            float angleY = spawnDirection == SpawnDirection.Left ? 90f : 180f;
            Vector3 localPosition = currentPlatformUnit.LocalPosition + direction * distance;
            localPosition.y = 0f;

            return new PlatformData(3, angleY, localPosition);
        }

        public PlatformData GetNextPlatformData(PlatformUnit currentPlatformUnit)
        {
            LevelData levelData = LevelModel.GetLevelData(CurrentLevel);
            uint size = GameLogicUtility.GetPlatformRandomSize(levelData.SizeRange);
            float radius = size * 0.5f;
            uint distanceMin = (uint)Mathf.CeilToInt(currentPlatformUnit.Radius + radius);
            float distance = GameLogicUtility.GetPlatformRandomDistance(distanceMin, levelData.DistanceUnit);

            // test
            // distance = GameLogicUtility.GetPlatformRandomDistance(distanceMin, 6);

            SpawnDirection spawnDirection = UnityEngine.Random.Range(0, 2) == 0 ? SpawnDirection.Left : SpawnDirection.Right;
            Vector3 direction = spawnDirection == SpawnDirection.Left ? Vector3.left : Vector3.forward;

            float angleY = spawnDirection == SpawnDirection.Left ? 90f : 180f;
            Vector3 localPosition = currentPlatformUnit.LocalPosition + direction * distance;
            localPosition.y = 0f;

            return new PlatformData(size, angleY, localPosition);
        }

        private void DispatchBestScoreUpdate()
        {
            onBestScoreUpdated?.Invoke(BestScore);
        }

        public void RemoveAllEvents()
        {
            onBestScoreUpdated = null;
            onCurrentScoreUpdated = null;
        }

        /// <summary> For Test </summary>
        public void DeleteAllData()
        {
            StorageManager.DeleteAllData();
            LoadGameData();
            DispatchBestScoreUpdate();
            CurrentScore = 0;
            CreatedPlatformCount = 0;
        }
    }
}