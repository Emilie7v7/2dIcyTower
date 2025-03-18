using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts.InputHandler
{
    public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform joystickBase;
        [SerializeField] private RectTransform joystickKnob;

        private Vector2 _joystickStartPos;
        private Vector2 _inputVector;
        private float _radius;

        public Vector2 JoystickDirection => _inputVector.normalized;

        private void Start()
        {
            _joystickStartPos = joystickKnob.anchoredPosition;
            _radius = joystickBase.sizeDelta.x * 0.5f;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // Convert touch position to local UI space
            RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBase, eventData.position, eventData.pressEventCamera, out var localPoint);

            joystickKnob.anchoredPosition = localPoint;
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Convert touch position to local UI space
            RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBase, eventData.position, eventData.pressEventCamera, out var localPoint);

            _inputVector = localPoint;
        
            // Clamp movement inside joystick base radius
            _inputVector = Vector2.ClampMagnitude(_inputVector, _radius);

            joystickKnob.anchoredPosition = _inputVector;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _inputVector = Vector2.zero;
            joystickKnob.anchoredPosition = _joystickStartPos;
        }
    }
}