using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {
    GameObject player;
    private UIManager _uiManager;
    //[SerializeField] private AudioClip _clip;
    [SerializeField] GameObject _enemyExplosionPrefab;

    [SerializeField] int hitpoints;
    [SerializeField] float speed;
    [SerializeField] private int pointValue;
    [SerializeField] private int damage;    

    // Enemy movement
    Coroutine EnemyMovement;
    public bool isMoving = true;

    [SerializeField] public float movementRate; // How long it moves
    [SerializeField] public float movementPause; // How long it slows down

    private float _speedUpTime = 0.0f;

    // Checking for player contact
    private float playerRadius;
    private float currentPlayerX;
    private float currentPlayerY;

    // Prevent multiple explosions
    private bool isExploded = false;

    private void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();        
        player = GameObject.FindWithTag("Player");        
        playerRadius = 0.25f;

        StartCoroutines();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > _speedUpTime)
        {
            // Move towards player
            if (isMoving)
            {
                speed *= 1.06f;
            }
            else
            {
                speed *= 0.9f;
            }
            _speedUpTime = Time.time + 0.015f;
        }

        // Calculate current player position,
        // and check whether enemy is already touching it
        currentPlayerX = player.transform.position.x;
        currentPlayerY = player.transform.position.y;

        if (
            (transform.position.x >= (currentPlayerX - playerRadius) && transform.position.x <= (currentPlayerX + playerRadius)) &&
            (transform.position.y >= (currentPlayerY - playerRadius) && transform.position.y <= (currentPlayerY + playerRadius))
            )
        {            
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);            
        }    
        else
        {
            FollowPlayer();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            if (other.transform.parent != null)
            {
                Destroy(other.transform.parent.gameObject);
            }

            Destroy(other.gameObject);
            Damaged();
        }
        else if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Damaged(damage);
            }
            
            if (player.isInvincible == false) {
                Instantiate(_enemyExplosionPrefab, transform.position, Quaternion.identity);

                //AudioSource.PlayClipAtPoint(_clip, Camera.main.transform.position, 1f);
                player.startInvincibilityRoutine();                
                Destroy(this.gameObject);
            }
        }
    }

    private void Damaged()
    {
        hitpoints -= 1;

        if (hitpoints <= 0)
        {
            if (isExploded == false) {
                Instantiate(_enemyExplosionPrefab, transform.position, Quaternion.identity);
                isExploded = true;
            }

            _uiManager.UpdateScore(pointValue);
            //AudioSource.PlayClipAtPoint(_clip, Camera.main.transform.position, 1f);
            Destroy(this.gameObject);
        }
    }

    private void FollowPlayer()
    {        
        player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            Vector2 direction = playerDirection(player);
            float angleToPlayerVal = angleToPlayer(direction.x, direction.y);

            gameObject.GetComponent<Rigidbody2D>().velocity = direction;
            gameObject.transform.eulerAngles = new Vector3(0, 0, angleToPlayerVal);
        }
        else
        {
            gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);            
        }
    }

    private Vector2 playerDirection(GameObject player)
    {
        // Update direction toward player
        float newX;
        float newY;

        var curEnemyPosition = transform.position;
        float enemyX = curEnemyPosition.x;
        float enemyY = curEnemyPosition.y;
        float playerX = player.transform.position.x;
        float playerY = player.transform.position.y;

        var deltaX = playerX - enemyX;
        var deltaY = playerY - enemyY;

        if (deltaX == 0)
        {
            newX = 0;
            newY = Mathf.Sqrt(speed);
        }
        else if (deltaY == 0)
        {
            newX = Mathf.Sqrt(speed);
            newY = 0;
        }
        else
        {
            // Maths!
            // Calculate and set the values of x and y such that this holds always
            // x^2 + y^2 = laserSpeed
            float ratio = deltaX / deltaY;            

            float tempx = Mathf.Sqrt(speed / ((1 + (1 / (ratio * ratio)))));
            float tempy = tempx / ratio;

            newX = tempx;
            newY = tempy;
        }

        // quadrant determines signs of x and y values
        // this is relative to the position of the player not the world
        int quadrant;

        if (deltaX >= 0 && deltaY >= 0)
        {
            quadrant = 1;
        }
        else if (deltaX >= 0 && deltaY < 0)
        {
            quadrant = 2;
        }
        else if (deltaX < 0 && deltaY < 0)
        {
            quadrant = 3;
        }
        else
        {
            quadrant = 4;
        }
        // Set the quadrant sign
        if (quadrant == 1 || quadrant == 2)
        {
            newX = Mathf.Abs(newX);
        }
        else
        {
            newX = -(Mathf.Abs(newX));
        }
        if (quadrant == 1 || quadrant == 4)
        {
            newY = Mathf.Abs(newY);
        }
        else
        {
            newY = -(Mathf.Abs(newY));
        }

        return new Vector2(newX, newY);
    }

    private float angleToPlayer(float x, float y)
    {
        // Given the delta X and delta Y from enemy to player,
        // this function returns the angle based on 0 - 360 degrees starting from top
        // and going counter-clockwise
        float angle;

        if (x == 0) // Vertical shot
        {
            if (y >= 0) // quadrants 1 and 4
            {        
                angle = 0f;
            }
            else // quadrants 2 and 3
            {
                // straight down
                angle = 180f;
            }
        }
        else if (y == 0) // Horizontal shot
        {
            if (x >= 0) // quadrants 1 and 2
            {
                angle = 270f;
            }
            else
            {
                angle = 90f;
            }
        }
        else
        {
            float radians = Mathf.Atan(y / x);
            angle = radians * (180 / Mathf.PI);

            // quadrant 1
            if (x >= 0 && y >= 0) {
                angle += 270f;   
            }

            // quadrant 2
            if (x >= 0 && y < 0)
            {
                angle += 270f;
            }

            // quadrant 3
            if (x < 0 && y < 0)
            {
                angle += 90;
            }

            // quadrant 4
            if (x < 0 && y >= 0)
            {
                angle += 90;
            }
        }

        return angle;                 
    }
    
    public void StartCoroutines()
    {
        EnemyMovement = StartCoroutine(EnemyMovementRoutine());
    }

    public void StopCoroutines()
    {
        StopCoroutine(EnemyMovement);
    }

    IEnumerator EnemyMovementRoutine()
    {
        while (true)
        {
            if (isMoving)
            {                
                // Movement time
                yield return new WaitForSeconds(movementRate);
                isMoving = false;
                speed = 6f;
            }
            else
            {                
                // Slowing time
                yield return new WaitForSeconds(movementPause);
                isMoving = true;
                speed = 1f;
            }            
        }
    }
}