using UnityEngine;
using TMPro;

namespace Live17Game
{
    public class DisplayLevelUI : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _levelLabel = null;

        private DataModel DataModel => JumpApp.Instance.DataModel;

        public void Init()
        {
            RegisterEvents();

            RefreshLevel(DataModel.CurrentLevel);
        }

        void OnDestroy()
        {
            UnregisterEvents();
        }

        private void RegisterEvents()
        {
            DataModel.onLevelUpdated += OnLevelUpdated;
        }

        private void UnregisterEvents()
        {
            if (!JumpApp.IsExist)
                return;

            DataModel.onLevelUpdated -= OnLevelUpdated;
        }

        private void OnLevelUpdated(uint createdPlatformCount, uint level)
        {
            RefreshLevel(level);
        }

        private void RefreshLevel(uint level)
        {
            _levelLabel.text = level.ToString();
        }
    }
}