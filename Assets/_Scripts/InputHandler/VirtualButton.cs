using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.InputHandler
{
    public class VirtualButton : MonoBehaviour
    {
        [SerializeField] private PlayerInputHandler playerInput;
        private Button _button;

        private void Start()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnThrowButtonPressed);
        }

        public void OnThrowButtonPressed()
        {
            playerInput.SetThrowButton();
        }
        
    }
}
