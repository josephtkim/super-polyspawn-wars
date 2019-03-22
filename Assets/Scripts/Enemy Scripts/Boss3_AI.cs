using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3_AI : MonoBehaviour
{
    GameObject player;
    private GameManager _gameManager;
    private UIManager _uiManager;
    //[SerializeField] private AudioClip _clip;
    [SerializeField] public GameObject _explosionPrefab;
    [SerializeField] private GameObject _furyModeGlow;
    private bool isFury = false;

    [SerializeField] private int hitpoints;
    [SerializeField] private float speed;
    [SerializeField] private int pointValue;
    [SerializeField] private int damage;
    [SerializeField] public float movementRate; // How long it moves (Higher is faster)
    [SerializeField] public float movementPause; // How long it pauses (Lower is faster)

    Coroutine BossMovement;
    private bool isMoving = true;

    private float _speedUpTime = 0.0f;

    // Checking for player contact
    private float playerRadius;
    private float currentPlayerX;
    private float currentPlayerY;

    // Unique boss movement
    private bool startedChargeMovementPattern = false;
    private bool isCharging = false;
    private Vector2 prevDirection;
    private float prevAngle;
    Coroutine ChargingMovement;

    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        player = GameObject.FindWithTag("Player");
        playerRadius = player.GetComponent<Player>().hitBoxRadius;

        _uiManager.updateBossHP(hitpoints);
        StartCoroutines();
    }
    private void Update()
    {
        currentPlayerX = player.transform.position.x;
        currentPlayerY = player.transform.position.y;

        if (
            (transform.position.x >= (currentPlayerX - playerRadius) && transform.position.x <= (currentPlayerX + playerRadius)) &&
            (transform.position.y >= (currentPlayerY - playerRadius) && transform.position.y <= (currentPlayerY + playerRadius)) &&
            (isCharging == false)
            )
        {
            // Hits player, then move to different position
            float RandomXoffset = Random.Range(-4f, 4f);
            float RandomYoffset = Random.Range(-4f, 4f);
            transform.position = new Vector3(currentPlayerX + RandomXoffset, currentPlayerY + RandomYoffset, 0);
        }
        else
        {
            if (hitpoints < 600) // At hp threshold enter charing movement pattern
            {
                if (isFury == false)
                {
                    _furyModeGlow.SetActive(true);
                    isFury = true;
                }

                if (isMoving)
                {
                    FollowPlayer();
                }
                else
                {
                    ChargeForward(prevDirection, prevAngle);
                }

                if (startedChargeMovementPattern == false)
                {
                    startedChargeMovementPattern = true;
                    StopCoroutines(); // Stop normal movement
                    isMoving = true;
                    MovementChargeForward();
                }
            }
            else
            {
                if (Time.time > _speedUpTime)
                {
                    // Move towards player            
                    if (isMoving)
                    {
                        speed *= 1.025f;
                    }
                    else
                    {
                        speed *= 0.9f;
                    }
                    _speedUpTime = Time.time + 0.015f;
                }
                FollowPlayer();
            }
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

            if (player.isInvincible == false)
            {
                //AudioSource.PlayClipAtPoint(_clip, Camera.main.transform.position, 1f);
                player.startInvincibilityRoutine();
            }
        }
    }
    private void Damaged()
    {
        hitpoints -= 5;
        if (hitpoints >= 0)
        {
            _uiManager.updateBossHP(hitpoints);
        }

        if (hitpoints <= 0)
        {            
            _gameManager.bossDestroyed();
            _uiManager.UpdateScore(pointValue);

            if (_gameManager.spawnsDestroyed <= 18)
            {
                _gameManager.updateSpawnsDestroyed(0);
            }
            else // After 18 spawns destroyed, it enters a cycle. Generates boss every phase (4 spawns destroyed).
            {
                _gameManager.bossSpawnExtra();
            }

            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            //AudioSource.PlayClipAtPoint(_clip, Camera.main.transform.position, 1f);
            Destroy(this.gameObject);
        }
    }
    private void FollowPlayer()
    {
        // Find the player
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

    // BOSS MOVEMENT CODE
    public void StartCoroutines()
    {
        BossMovement = StartCoroutine(BossMovementRoutine());
    }
    public void StopCoroutines()
    {
        StopCoroutine(BossMovement);
    }
    IEnumerator BossMovementRoutine()
    {
        while (true)
        {
            if (isMoving)
            {
                // Movement time
                yield return new WaitForSeconds(movementRate);
                isMoving = false;
                speed = 4f;
            }
            else
            {
                // Pause time
                yield return new WaitForSeconds(movementPause);
                isMoving = true;
                speed = 1.5f;
            }
        }
    }

    // BOSS CHARGING MOVEMENT
    public void StopChargingRoutine()
    {
        StopCoroutine(ChargingMovement);
    }
    public void MovementChargeForward()
    {        
        ChargingMovement = StartCoroutine(ChargingMovementRoutine());       
    }
    IEnumerator ChargingMovementRoutine()
    {
        yield return new WaitForSeconds(2f); // delay before fury mode attacks

        while (true)
        {
            if (isMoving)
            {
                speed = 10f;

                // Get snapshot of previous position     
                prevDirection = playerDirection(player);
                prevAngle = angleToPlayer(prevDirection.x, prevDirection.y);                

                isMoving = false;
                isCharging = true;
                
                yield return new WaitForSeconds(1f);
            }
            else
            {
                isCharging = false;
                isMoving = true;
                yield return new WaitForSeconds(4f);
            }
        }
    }
    
    public void ChargeForward(Vector2 prevDir, float prevAngle)
    {        
        gameObject.GetComponent<Rigidbody2D>().velocity = prevDir * 3.2f;
        gameObject.transform.eulerAngles = new Vector3(0, 0, prevAngle);
    }
}