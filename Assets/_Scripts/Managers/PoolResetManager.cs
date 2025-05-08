using System.Collections;
using _Scripts.ObjectPool.ObjectsToPool;
using _Scripts.Projectiles;
using UnityEngine;

namespace _Scripts.Managers
{
    public class PoolResetManager : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log("PoolResetManager Awake");
            ResetPool();
        }

        private void Start()
        {
            Debug.Log("PoolResetManager Start");
            StartCoroutine(DelayedReset());
        }

        private IEnumerator DelayedReset()
        {
            yield return null;
            ResetPool();
            
            // Add additional check after a few frames
            //StartCoroutine(CheckArrowsAfterDelay());
        }

        private static void ResetPool()
        {
            var activeArrows = FindObjectsOfType<Arrow>();
            if (activeArrows.Length > 0)
            {
                // Debug.Log($"Found {activeArrows.Length} active arrows before reset at time: {Time.time}");
                foreach (var arrow in activeArrows)
                {
                    // Debug.Log($"Active arrow at position: {arrow.transform.position}, parent: {arrow.transform.parent?.name}");
                }
            }
            
            foreach (var arrow in activeArrows)
            {
                arrow.gameObject.SetActive(false);
                ArrowPool.Instance.ReturnObject(arrow);
            }
        }

        // private IEnumerator CheckArrowsAfterDelay()
        // {
        //     // Check after 0.1 seconds
        //     yield return new WaitForSeconds(0.1f);
        //     var activeArrows = FindObjectsOfType<Arrow>();
        //     if (activeArrows.Length > 0)
        //     {
        //         Debug.Log($"Found {activeArrows.Length} active arrows after delay at time: {Time.time}");
        //         foreach (var arrow in activeArrows)
        //         {
        //             Debug.Log($"Arrow still active at position: {arrow.transform.position}, parent: {arrow.transform.parent?.name}");
        //         }
        //     }
        // }
    }
}