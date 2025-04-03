using System;
using UnityEngine;

namespace _Scripts.Parallax
{
    public class BackgroundController : MonoBehaviour
    {
        private float _startPos, _length;
        [SerializeField] private GameObject cam;
        [SerializeField] private float parallaxFactor;

        private void Start()
        {
            _startPos = transform.position.y;
            _length = GetComponent<SpriteRenderer>().bounds.size.y;
        }

        private void FixedUpdate()
        {
            var distance = cam.transform.position.y * parallaxFactor;
            var movement = cam.transform.position.y * (1 - parallaxFactor);
            
            transform.position = new Vector3(transform.position.x, _startPos + distance, transform.position.z);

            if (movement > _startPos + _length)
            {
                _startPos += _length;
            }
            else if (movement < _startPos - _length)
            {
                _startPos -= _length;
            }
        }
    }
}
