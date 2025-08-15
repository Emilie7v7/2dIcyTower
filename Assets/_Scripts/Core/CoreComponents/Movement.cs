using UnityEngine;

namespace _Scripts.CoreSystem
{
    public class Movement : CoreComponent
    {
        public Rigidbody2D Rb2D  { get; private set; }
        
        public int FacingDirection { get; private set; }
        
        public bool CanSetVelocity { get; set; }
        
        public Vector2 CurrentVelocity { get; private set; }
        
        private Vector2 _workspace;

        protected override void Awake()
        {
            base.Awake();

            Rb2D = GetComponentInParent<Rigidbody2D>();

            FacingDirection = 1;
            
            CanSetVelocity = true;
        }

        public override void LogicUpdate()
        {
            CurrentVelocity = Rb2D.linearVelocity;
        }

        #region Set Velocity
        
        public void SetZeroVelocity()
        {
            _workspace = Vector2.zero;
            SetFinalVelocity();
        }

        public void SetVelocity(float velocity, Vector2 angle, int direction)
        {
            angle.Normalize();
            _workspace.Set(angle.x * velocity * direction, angle.y * velocity);
            SetFinalVelocity();
        }

        public void SetVelocity(float velocity, Vector2 direction)
        {
            _workspace = direction * velocity;
            SetFinalVelocity();
        }
        
        public void SetVelocityX(float velocity)
        {
            _workspace.Set(velocity, CurrentVelocity.y);
            SetFinalVelocity();
        }

        public void SetVelocityY(float velocity)
        {
            _workspace.Set(CurrentVelocity.x, velocity);
            SetFinalVelocity();
        }

        public void LaunchProjectile(Vector2 direction, float force)
        {
            if (Rb2D == null) return;

            // Ensure the direction is normalized
            direction = direction.normalized;

            // Apply a force in the given direction with impulse mode
            Rb2D.linearVelocity = Vector2.zero; // Reset velocity to avoid accumulation
            Rb2D.AddForce(direction * force, ForceMode2D.Impulse);
            //Debug.Log($"Projectile launched with force: {force}, direction: {direction}");
        }
        private void SetFinalVelocity()
        {
            if(!CanSetVelocity) return;
            Rb2D.linearVelocity = _workspace;
            CurrentVelocity = _workspace;
        }
        
        public void Flip()
        {
            //Flips a sprite of a Player or an Entity whether they hit a detection for a flip
            FacingDirection *= -1;
            Rb2D.transform.Rotate(0f, 180f, 0f);
        }

        public void CheckIfShouldFlip(int xInput)
        {
            if (xInput != 0 && xInput != FacingDirection)
            {
                Flip();
            }
        }
        #endregion
    }
}
