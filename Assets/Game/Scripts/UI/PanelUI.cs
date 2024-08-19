using UnityEngine;

namespace Live17Game
{
    public abstract class PanelUI : MonoBehaviour
    {
        public UIPanelFlag UIPanel { get; private set; } = UIPanelFlag.None;

        private UIPanelManager uiPanelManager => UIPanelManager.Instance;

        public virtual void Init(UIPanelFlag uiPanel)
        {
            UIPanel = uiPanel;
            Hide();
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
            uiPanelManager.AddUIPanel(UIPanel);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
            uiPanelManager.RemoveUIPanel(UIPanel);
        }
    }
}