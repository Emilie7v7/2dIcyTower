using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    public float movementSpeed = 5f;
    public Rigidbody2D rb;

    [Header("Potion Settings")]
    public GameObject potionPrefab;
    public Transform throwPoint;
    public float throwForce = 10f;

    private Vector2 movementInput;

    void Update()
    {
        // Handle movement input
        movementInput.x = Input.GetAxis("Horizontal");
        movementInput.y = Input.GetAxis("Vertical");

        // Throw potion when pressing the space bar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ThrowPotion();
        }
    }

    private void FixedUpdate()
    {
        // Move the player
        rb.velocity = movementInput * movementSpeed;
    }

    public void ApplyExplosionForce(Vector2 explosionDirection, float explosionForce)
    {
        // Apply the explosion force to the player's Rigidbody2D
        rb.AddForce(explosionDirection * explosionForce, ForceMode2D.Impulse);
    
        // Debugging applied force and velocity
        Debug.Log($"Applying Explosion Force: {explosionDirection * explosionForce}");
        Debug.Log($"Player Velocity After Explosion: {rb.velocity}");
    }



    private void ThrowPotion()
    {
        if (potionPrefab != null && throwPoint != null)
        {
            // Instantiate the potion at the throwPoint's position
            GameObject potion = Instantiate(potionPrefab, throwPoint.position, Quaternion.identity);

            // Get the Rigidbody2D of the potion to apply force
            Rigidbody2D potionRb = potion.GetComponent<Rigidbody2D>();

            // Calculate the throw direction
            Vector2 throwDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - throwPoint.position).normalized;

            // Apply force to the potion
            potionRb.AddForce(throwDirection * throwForce, ForceMode2D.Impulse);
        }
    }
}