using UnityEngine;

namespace _Scripts.CoreSystem
{
    public class Movement : CoreComponent
    {
        public Rigidbody2D R2BD  { get; private set; }
        
        public int FacingDirection { get; private set; }
        
        public bool CanSetVelocity { get; set; }
        
        public Vector2 CurrentVelocity { get; private set; }
        
        private Vector2 _workspace;

        protected override void Awake()
        {
            base.Awake();

            R2BD = GetComponentInParent<Rigidbody2D>();

            FacingDirection = 1;
            
            CanSetVelocity = true;
        }

        public override void LogicUpdate()
        {
            CurrentVelocity = R2BD.velocity;
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

        private void SetFinalVelocity()
        {
            if(!CanSetVelocity) return;
            R2BD.velocity = _workspace;
            CurrentVelocity = _workspace;
        }
        
        public void Flip()
        {
            //Flips a sprite of a Player or an Entity whether they hit a detection for a flip
            FacingDirection *= -1;
            R2BD.transform.Rotate(0f, 180f, 0f);
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
