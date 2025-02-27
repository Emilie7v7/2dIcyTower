using UnityEngine;

namespace _Scripts.Intermediaries
{
    public class CoinAnimationEventHandler : MonoBehaviour
    {
        public void TurnOffGameObject()
        {
            var coinAnimator = gameObject.GetComponent<Animator>();
            coinAnimator.SetBool("notEnoughCoins", false);
        }
    }
}
