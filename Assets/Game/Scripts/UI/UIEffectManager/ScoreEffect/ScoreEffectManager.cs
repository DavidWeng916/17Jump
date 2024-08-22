using UnityEngine;

namespace Live17Game
{
    public class ScoreEffectManager : ObjectPoolManagerBase<ScoreEffectUnit, RectTransform>
    {
        [SerializeField]
        private Camera _camera = null;

        [SerializeField]
        private Canvas _canvas = null;

        public void Init()
        {
            base.Init();
        }

        public void SpawnScore(Vector3 worldPoint, uint score)
        {
            ScoreEffectUnit scoreEffectUnit = Obtain();
            scoreEffectUnit.onAnimationComplete = OnRecycle;

            CoordConvertData coordConvertData = new CoordConvertData(_camera, _canvas, Container, worldPoint);
            scoreEffectUnit.Show(coordConvertData, score);
        }

        private void OnRecycle(ScoreEffectUnit scoreEffectUnit)
        {
            Release(scoreEffectUnit);
        }
    }
}