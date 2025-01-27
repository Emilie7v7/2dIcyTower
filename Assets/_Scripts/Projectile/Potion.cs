using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class NewScript : MonoBehaviour
{
    public GameObject projectile;
    [SerializeField] private float spawnOffset;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask enemyLayer;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(projectile, transform.position + new Vector3(spawnOffset, spawnOffset, 0f), transform.rotation);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check for Ground layer
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            Debug.Log("Hit Ground.");
            Destroy(gameObject);
        }

        // Check for Enemy layer
        if (((1 << collision.gameObject.layer) & enemyLayer) != 0)
        {
            Debug.Log("Hit Enemy.");
            Destroy(gameObject);
        }
    }
}