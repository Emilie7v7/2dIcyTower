using TMPro;
using UnityEngine;

namespace _Scripts.Generics
{
    public class FPSCounter : MonoBehaviour
    {
        private static FPSCounter _instance;
        private float _deltaTime = 0.0f;
        private TextMeshProUGUI _fpsText;

        private void Awake()
        {
            // Ensure only one instance exists
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            // Create FPS Display
            GameObject canvasObj = new GameObject("FPSCanvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            DontDestroyOnLoad(canvasObj);

            GameObject fpsDisplay = new GameObject("FPS Counter");
            fpsDisplay.transform.SetParent(canvas.transform);

            _fpsText = fpsDisplay.AddComponent<TextMeshProUGUI>();
            _fpsText.fontSize = 36;
            _fpsText.color = Color.green;
            _fpsText.alignment = TextAlignmentOptions.TopLeft;

            RectTransform rectTransform = fpsDisplay.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.anchoredPosition = new Vector2(10, -10);
        }

        private void Update()
        {
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
            float fps = 1.0f / _deltaTime;
            _fpsText.text = $"FPS: {Mathf.Ceil(fps)}";
        }
    }
}