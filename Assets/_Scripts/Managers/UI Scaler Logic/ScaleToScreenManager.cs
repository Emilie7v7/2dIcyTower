using UnityEngine;

namespace _Scripts.Managers.UI_Scaler_Logic
{
    public class ScaleToScreenManager : MonoBehaviour
    {
        public SpriteRenderer[] objectsToScale;

        private void Start()
        {
            FitAllObjectsToScreen();
        }

        private void FitAllObjectsToScreen()
        {
            foreach (var obj in objectsToScale)
            {
                if (obj == null) continue;

                if (UnityEngine.Camera.main == null) continue;
                var worldScreenHeight = UnityEngine.Camera.main.orthographicSize * 2;
                var worldScreenWidth = worldScreenHeight * Screen.width / Screen.height;
                Vector2 spriteSize = obj.sprite.bounds.size;

                obj.transform.localScale = new Vector3(worldScreenWidth / spriteSize.x, worldScreenHeight / spriteSize.y, 1);
            }
        }
    }
}
