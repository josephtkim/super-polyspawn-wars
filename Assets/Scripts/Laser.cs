using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {
    public Rigidbody2D rb;

    private float _speedUpTime = 0.0f;

    // Boundaries for laser
    private float topBorder = 6.5f;
    private float bottomBorder = -6.5f;
    private float leftBorder = -12f;
    private float rightBorder = 12f;
    
    void Update () {                        
        // Destroy at collision with boundaries
        if (transform.position.x > rightBorder || transform.position.x < leftBorder
            || transform.position.y > topBorder || transform.position.y < bottomBorder)
        {
            Destroy(gameObject);
        }

        if (Time.time > _speedUpTime)
        {
            // Speeds up the laser as it travels
            rb = GetComponent<Rigidbody2D>();
            Vector2 vel = rb.velocity;
            rb.AddForce(new Vector2(vel.x * .2f, vel.y * .2f));

            _speedUpTime = Time.time + 0.1f;
        }
    }    
}
