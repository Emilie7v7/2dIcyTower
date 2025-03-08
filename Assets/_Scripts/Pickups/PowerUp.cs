using UnityEngine;

namespace _Scripts.Pickups
{
    public class PowerUp : MonoBehaviour
    {
        protected Transform Player;
        [SerializeField] protected float maxPullSpeed = 20f;
        protected virtual void Activate() { }
    }
}
