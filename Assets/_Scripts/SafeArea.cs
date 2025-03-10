using UnityEngine;
using UnityEngine.UI;

public class SafeArea : MonoBehaviour
{
    [SerializeField] private RectTransform _canvasRect;
    private RectTransform rectTransform;

    void Start()
    {
        // Get the RectTransform of the UI element this script is attached to
        rectTransform = GetComponent<RectTransform>();

        // Calculate width and height ratios based on the canvas and screen dimensions
        float widthRatio = _canvasRect.rect.width / Screen.width;
        float heightRatio = _canvasRect.rect.height / Screen.height;

        // Calculate offsets to adjust the RectTransform to the safe area
        float offsetTop = (Screen.safeArea.yMax - Screen.height) * heightRatio;
        float offsetBottom = Screen.safeArea.yMin * heightRatio;
        float offsetLeft = Screen.safeArea.xMin * widthRatio;
        float offsetRight = (Screen.safeArea.xMax - Screen.width) * widthRatio;

        // Apply the offsets to the RectTransform
        rectTransform.offsetMax = new Vector2(offsetRight, offsetTop);
        rectTransform.offsetMin = new Vector2(offsetLeft, offsetBottom);

        // Adjust the CanvasScaler's reference resolution to account for vertical insets
        CanvasScaler canvasScaler = _canvasRect.GetComponent<CanvasScaler>();
        if (canvasScaler != null)
        {
            canvasScaler.referenceResolution = new Vector2(
                canvasScaler.referenceResolution.x,
                canvasScaler.referenceResolution.y + Mathf.Abs(offsetTop) + Mathf.Abs(offsetBottom)
            );
        }
    }
}