using System;
using _Scripts.CoreSystem;
using _Scripts.ScriptableObjects.ProjectileData;
using UnityEngine;

namespace _Scripts.Projectile
{
    public class Projectile : MonoBehaviour
    {
        private Vector2 _startPosition;
        private bool _hasHitGround;
        
        
        [field: SerializeField] public ProjectileDataSo ProjectileData { get; private set; }
        public Core Core { get; private set; }
        public CollisionSenses CollisionSenses { get; private set; }
        public Rigidbody2D MyRigidbody2D { get; private set; }

        private void Awake()
        {
            Core = GetComponentInChildren<Core>();
            CollisionSenses = Core.GetCoreComponent<CollisionSenses>();
        }

        private void Start()
        {
            MyRigidbody2D = GetComponent<Rigidbody2D>();
            _hasHitGround = false;

            _startPosition = transform.position;
        }

        private void Update()
        {
            if(_hasHitGround) return;
        }

        private void FixedUpdate()
        {
            if (!_hasHitGround)
            {
                
            }
        }
    }
}
