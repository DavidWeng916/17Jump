using UnityEngine;

namespace Live17Game
{
    public class UIEffectManager : MonoBehaviour
    {
        // public static UIEffectManager Instance { get; private set; }

        [SerializeField]
        private ScoreEffectManager _scoreEffectManager = null;

        public void Init()
        {
            // Instance = this;
            _scoreEffectManager.Init();
        }

        void OnDestroy()
        {
            // Instance = null;
        }

        public void SpawnScore(Vector3 worldPoint, uint score)
        {
            _scoreEffectManager.SpawnScore(worldPoint, score);
        }
    }
}