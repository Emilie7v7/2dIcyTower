using UnityEngine;

namespace _Scripts.Pickups
{
    public class PowerUp : MonoBehaviour
    {
        protected virtual void Activate()
        {
            Debug.Log("PowerUp Activated!");
        }
    }
}
