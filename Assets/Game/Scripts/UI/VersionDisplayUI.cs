using UnityEngine;
using TMPro;

namespace Live17Game
{
    public class VersionDisplayUI : MonoBehaviour
    {
        private const string VERSION = "0.3.0";

        [SerializeField]
        private TextMeshProUGUI _versionText = null;

        void Awake()
        {
            _versionText.text = $"v{VERSION}";
        }
    }
}