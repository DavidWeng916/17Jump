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

            CoordConvertData coordConvertData = new CoordConvertData
            {
                Camera = _camera,
                Canvas = _canvas,
                Container = Container,
                WorldPoint = worldPoint,
            };

            // Vector2 point = CoordinateUtility.ConvertWorldPointToCanvas(_camera, _canvas, ts.position, Container);
            scoreEffectUnit.Show(coordConvertData, score);
        }

        private void OnRecycle(ScoreEffectUnit scoreEffectUnit)
        {
            Release(scoreEffectUnit);
        }
    }
}