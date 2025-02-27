using System.Collections;
using _Scripts.PlayerComponent;
using UnityEngine;

namespace Scripts.Objects.PassTroughPlatform
{
    public class PassThrough : MonoBehaviour
    {
        private Collider2D _collider2D;
        private bool _isPlayerOnPlatform;

        private Player _player;

        private void Awake()
        {
            _collider2D = GetComponent<Collider2D>();
        }

        private void Update()
        {
            if(_player == null) return;
            if (_isPlayerOnPlatform && _player.InputHandler.NormInputY == -1)
            {
                _collider2D.enabled = false;
                StartCoroutine(EnableCollider());
            }
        }

        private IEnumerator EnableCollider()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            _collider2D.enabled = true;
        }


        private void SetPlayerOnPlatform(Collision2D other, bool value)
        {
            _player = other.gameObject.GetComponent<Player>();
            
            if (_player != null)
            {
                _isPlayerOnPlatform = value; 
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            SetPlayerOnPlatform(other, true);
        }

        private void OnCollisionExit2D(Collision2D other)
        {
            SetPlayerOnPlatform(other, true);
        }
    }
}
