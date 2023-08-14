using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFlight : MonoBehaviour
{
    public Transform player;  // Reference to the player transform
    public float radius = 2.0f; // Radius of the circle
    public float speed = 1.0f; // Speed of rotation

    private float angle = 0.0f; // Current angle in radians

    void Update()
    {
        // Calculate new position
        float x = player.position.x + radius * Mathf.Cos(angle);
        float y = player.position.y + radius * Mathf.Sin(angle);
        transform.position = new Vector2(x, y);

        // Update angle based on speed
        angle += speed * Time.deltaTime;
        if (angle > 2 * Mathf.PI) angle -= 2 * Mathf.PI;
        transform.right = player.position - transform.position;
    }

}

