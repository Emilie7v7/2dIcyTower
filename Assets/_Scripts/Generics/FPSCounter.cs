using _Scripts.Managers.Game_Manager_Logic;
using TMPro;
using UnityEngine;

namespace _Scripts.Generics
{
    public class FPSCounter : MonoBehaviour
    {
        private static FPSCounter _instance;
        private float _deltaTime = 0.0f;
        private TextMeshProUGUI _fpsText;
        private GameObject _fpsCanvas; // Store FPS Canvas for toggling visibility

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
            _fpsCanvas = new GameObject("FPSCanvas");
            var canvas = _fpsCanvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            DontDestroyOnLoad(_fpsCanvas);

            var fpsDisplay = new GameObject("FPS Counter");
            fpsDisplay.transform.SetParent(_fpsCanvas.transform);

            _fpsText = fpsDisplay.AddComponent<TextMeshProUGUI>();
            _fpsText.fontSize = 36;
            _fpsText.color = Color.green;
            _fpsText.alignment = TextAlignmentOptions.TopLeft;

            var rectTransform = fpsDisplay.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.anchoredPosition = new Vector2(10, -10);

            // Load player preference and apply visibility
            SetVisibility(SettingsManager.OptionsData.showFps);
        }
        
        private void Update()
        {
            _deltaTime += (Time.unscaledDeltaTime - _deltaTime) * 0.1f;
            var fps = 1.0f / _deltaTime;
            _fpsText.text = $"FPS: {Mathf.Ceil(fps)}";
        }

        public static void SetVisibility(bool visible)
        {
            if (_instance != null && _instance._fpsCanvas != null)
            {
                _instance._fpsCanvas.SetActive(visible);
            }
        }
    }
}