using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    GameObject shipCenter;
    private UIManager _uiManager;
    private GameManager _gameManager;    
    private AudioSource _audioSource;

    [SerializeField] public GameObject _playerExplosion;
    [SerializeField] public GameObject _invincibilityAnim;
    [SerializeField] public GameObject _boosterAnim;

    [SerializeField] public GameObject _laserPrefab;
    [SerializeField] GameObject _healthPrefab;
    [SerializeField] GameObject _speedPrefab;
    [SerializeField] GameObject _spreadShotPrefab;
    [SerializeField] public GameObject _shieldGameObject;
    [SerializeField] public GameObject _centerGameObject; // The center of the ship from which shots fire out

    [SerializeField] private float playerSpeed = 8f;    
    [SerializeField] private float rotationSpeed = 9f;
    private float diagonalPlayerSpeed;

    [SerializeField] private float _fireRate = 0.08f;
    [SerializeField] private float laserSpeed = 50f;
    [SerializeField] private int lives = 5; // Hitpoints
    private float _fireTime = 0.0f;

    // World borders
    private float topBorder = 4.6f;
    private float bottomBorder = -4.6f;
    private float leftBorder = -10.1f;
    private float rightBorder = 10.1f;

    private float shootingRadius = 0.5f; // Determines the radius around player at which lasers shoot out from    
    public float hitBoxRadius = .5f;
    public bool hasShield = false;
    public bool hasSpeedBoost = false;
    public bool canSpreadShot = false;
    public bool isInvincible = false;
    
    void Start () {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _audioSource = GetComponent<AudioSource>();

        _uiManager.UpdateLives(lives);
        _uiManager.score = 0;
        _uiManager.UpdateScore(0);
        _uiManager.difficulty = 1;
    }
	
	void Update () {
        if (_gameManager.isPaused == false)
        {
            diagonalPlayerSpeed = playerSpeed / (Mathf.Sqrt(2));

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ||
                Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) ||
                Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) ||
                Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                _boosterAnim.gameObject.SetActive(true);
                Movement();
            }
            else
            {
                _boosterAnim.gameObject.SetActive(false);
            }

            if (Input.GetMouseButton(0))
            {
                Shoot();
            }
        }
	}

    private void Movement()
    {
        // Rotation
        // Up left
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)))
        {
            if (transform.position.y < topBorder)
            {
                transform.position += (Vector3.up * Time.deltaTime * diagonalPlayerSpeed);
            }
            if (transform.position.x > leftBorder)
            {
                transform.position += (Vector3.left * Time.deltaTime * diagonalPlayerSpeed);
            }
            
            Quaternion target = Quaternion.Euler(0, 0, 45);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * rotationSpeed);
        }
        // Up right
        else if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)))
        {
            if (transform.position.y < topBorder)
            {
                transform.position += (Vector3.up * Time.deltaTime * diagonalPlayerSpeed);
            }
            if (transform.position.x < rightBorder)
            {
                transform.position += (Vector3.right * Time.deltaTime * diagonalPlayerSpeed);
            }
            
            Quaternion target = Quaternion.Euler(0, 0, 315);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * rotationSpeed);
        }
        // Down left
        else if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)))
        {
            if (transform.position.y > bottomBorder)
            {
                transform.position += (Vector3.down * Time.deltaTime * diagonalPlayerSpeed);
            }
            if (transform.position.x > leftBorder)
            {
                transform.position += (Vector3.left * Time.deltaTime * diagonalPlayerSpeed);
            }

            Quaternion target = Quaternion.Euler(0, 0, 135);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * rotationSpeed);
        }
        // Down right
        else if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)))
        {

            if (transform.position.y > bottomBorder)
            {
                transform.position += (Vector3.down * Time.deltaTime * diagonalPlayerSpeed);
            }
            if (transform.position.x < rightBorder)
            {
                transform.position += (Vector3.right * Time.deltaTime * diagonalPlayerSpeed);
            }

            Quaternion target = Quaternion.Euler(0, 0, 225);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * rotationSpeed);
        }        

        // Single directions only
        else
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                if (transform.position.x > leftBorder)
                {
                    transform.position += (Vector3.left * Time.deltaTime * playerSpeed);
                }
                Quaternion target = Quaternion.Euler(0, 0, 90);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * rotationSpeed);
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                if (transform.position.x < rightBorder)
                {
                    transform.position += (Vector3.right * Time.deltaTime * playerSpeed);
                }
                Quaternion target = Quaternion.Euler(0, 0, 270);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * rotationSpeed);
            }
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                if (transform.position.y < topBorder)
                {
                    transform.position += (Vector3.up * Time.deltaTime * playerSpeed);
                }
                Quaternion target = Quaternion.Euler(0, 0, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * rotationSpeed);
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                if (transform.position.y > bottomBorder)
                {
                    transform.position += (Vector3.down * Time.deltaTime * playerSpeed);
                }
                Quaternion target = Quaternion.Euler(0, 0, 180);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * rotationSpeed);
            }
        }
    }

    public void Damaged(int damage)
    {
        if (isInvincible == false)
        {                    
            if (hasShield == true)
            {
                hasShield = false;
                _shieldGameObject.SetActive(false);
                return;
            }

            else
            {
                lives -= damage;
                if (lives <= 0)
                {
                    lives = 0;

                    Instantiate(_playerExplosion, transform.position, Quaternion.identity);
                    _gameManager.StopCoroutines();
                    _gameManager.GameOver();
                    Destroy(gameObject);
                }

                if (_uiManager != null && gameObject != null)
                {
                    _uiManager.UpdateLives(lives);
                }
            }            
        }
    }

    private void Shoot()
    {
        if (Time.time > _fireTime)
        {
            _audioSource.Play();

            GameObject clone;
            Vector2 laserDir = laserDirection();
            float laserAng = laserAngle(laserDir.x, laserDir.y);
            Vector2 laserStartPos = laserStartPosition(laserDir.x, laserDir.y);

            shipCenter = _centerGameObject;

            clone = (GameObject)Instantiate(_laserPrefab, new Vector3(shipCenter.transform.position.x + laserStartPos.x, shipCenter.transform.position.y + laserStartPos.y, 0), Quaternion.Euler(0, 0, laserAng));
            clone.GetComponent<Rigidbody2D>().velocity = laserDir;            

            if (canSpreadShot == true) {
                spreadShot(laserAng);
            }   

            // Limit fire rate
            _fireTime = Time.time + _fireRate;
        }
    }

    // Spreadshot code
    private float offsetAngleToPositive(float offsetAngle)
    {
        return 360 - (360 - ((360 + offsetAngle) % 360));
    }
    private int angle360ToQuadrant(float angle)
    {
        // Given an angle from 0 to 360, starting north and counterclockwise
        // return the quadrant
        angle = angle % 360;

        if (angle > 0 && angle < 90)
        {
            return 4;
        }
        else if (angle > 90 && angle < 180)
        {
            return 3;
        }
        else if (angle > 180 && angle < 270)
        {
            return 2;
        }
        else if (angle > 270 && angle < 360)
        {
            return 1;
        }
        return -1; // Must be one of the 4 cardinal directions 0, 90, 180 or 270
    }
    private float degreesToRadians(float degrees)
    {
        return degrees * (Mathf.PI / 180f);
    }
    private Vector2 angleAndQuadrantToDirection(float angle, int quadrant)
    {
        float newX = 0;
        float newY = 0;
        float r = Mathf.Sqrt(laserSpeed);
        float radians = 0;

        if (quadrant == -1) // Cardinal directions
        {
            if (angle == 0)
            {
                newX = 0;
                newY = r;
            }
            else if (angle == 90)
            {
                newX = -r;
                newY = 0;
            }
            else if (angle == 180)
            {
                newX = 0;
                newY = -r;
            }
            else if (angle == 270)
            {
                newX = r;
                newY = 0;
            }
        }

        // Approach:
        // Translate each angle to 0-90 degree angles (exclusive).
        // This way sine and cosine always give back positive values
        else
        {
            if (quadrant == 1)
            {
                // +X, +Y
                angle = angle - 270;
                radians = degreesToRadians(angle);

                newX = r * Mathf.Cos(radians);
                newY = r * Mathf.Sin(radians);
            }
            else if (quadrant == 2)
            {
                // +X, -Y
                angle = angle - 180;
                radians = degreesToRadians(angle);

                newX = r * Mathf.Sin(radians);
                newY = -(r * Mathf.Cos(radians));
            }
            else if (quadrant == 3)
            {
                // -X, -Y
                angle = angle - 90;
                radians = degreesToRadians(angle);

                newX = -(r * Mathf.Cos(radians));
                newY = -(r * Mathf.Sin(radians));
            }
            else if (quadrant == 4)
            {
                // -X, +Y
                radians = degreesToRadians(angle);

                newX = -(r * Mathf.Sin(radians));
                newY = r * Mathf.Cos(radians);
            }
        }

        return new Vector2(newX, newY);
    }
    private void spreadShot(float centerAngle)
    {
        // Given the center laser's position, create the other lasers in the spread shot
        // Offset angle to degrees equation
        // new angle (0-360)
        shipCenter = _centerGameObject; // Ship's center
        
        // Spread laser 1
        spreadLaser(centerAngle - 7f);

        // Spread laser 2
        spreadLaser(centerAngle + 7f);

        // 5 laser spread shot
        // spreadLaser(centerAngle - 13f);
        // spreadLaser(centerAngle + 13f);
    }

    private void spreadLaser(float angle)
    {
        GameObject laserclone;
        float laserAng = offsetAngleToPositive(angle);
        int laserQuadrant = angle360ToQuadrant(laserAng);
        Vector2 laserDirection = angleAndQuadrantToDirection(laserAng, laserQuadrant);
        Vector2 laserStartPos = laserStartPosition(laserDirection.x, laserDirection.y);
        laserclone = (GameObject)Instantiate(_laserPrefab, new Vector3(_centerGameObject.transform.position.x + laserStartPos.x, _centerGameObject.transform.position.y + laserStartPos.y, 0), Quaternion.Euler(0, 0, laserAng));
        laserclone.GetComponent<Rigidbody2D>().velocity = laserDirection;
    }

    // Main laser code
    // calculates direction of laser path based on player position and mouse click position        
    private Vector2 laserDirection()
    {                        
        float newX;
        float newY;        

        var curMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float mouseX = curMousePosition.x;
        float mouseY = curMousePosition.y;        
        float playerX = transform.position.x;
        float playerY = transform.position.y;
        
        var deltaX = mouseX - playerX;
        var deltaY = mouseY - playerY;
        
        if (deltaX == 0)
        {
            newX = 0;
            newY = Mathf.Sqrt(laserSpeed);
        }
        else if (deltaY == 0)
        {
            newX = Mathf.Sqrt(laserSpeed);
            newY = 0;
        }
        else
        {
            // Maths!
            // Calculate and set the values of x and y such that this holds always
            // x^2 + y^2 = laserSpeed
            float ratio = deltaX / deltaY;            

            float tempx = Mathf.Sqrt(laserSpeed / ((1 + (1 / (ratio * ratio)))));
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
    private float laserAngle(float x, float y)
    {
        float angle;
        
        // Check for zeros
        if (x == 0) // Vertical shot
        {
            if (y >= 0) // quadrants 1 and 4
            {
                // Straight up
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
    private Vector2 laserStartPosition(float x, float y)
    {
        // Make x^2 + y^2 = shootingRadius^2
        // x and y are the velocity vectors.
        // This function will translate the triangle x, y, R to a similar triangle.
        float velocityRadius = Mathf.Sqrt((x * x) + (y * y));
        float newRatio = shootingRadius / velocityRadius;

        float shootingX = newRatio * x;
        float shootingY = newRatio * y;

        return new Vector2(shootingX, shootingY);
    }

    // Powerups implement
    public void powerupHealth()
    {
        lives = 5;
        _uiManager.UpdateLives(lives);
    }
    public void powerupShield()
    {
        hasShield = true;
        _shieldGameObject.SetActive(true);
    }
    public void powerupSpeed()
    {
        hasSpeedBoost = true;
        playerSpeed = 10;
        rotationSpeed = 11.8f;
        StartCoroutine(SpeedPowerDownRoutine());
    }
    public void powerupRapidShot()
    {
        canSpreadShot = true;
        _fireRate = 0.075f;
        laserSpeed = 70f;
        StartCoroutine(RapidShotPowerDownRoutine());
    }

    // Coroutines
    public IEnumerator SpeedPowerDownRoutine()
    {
        yield return new WaitForSeconds(6.0f);
        hasSpeedBoost = false;
        playerSpeed = 8f;
        rotationSpeed = 9f;
    }
    public IEnumerator RapidShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(10.0f);
        canSpreadShot = false;
        _fireRate = 0.08f;
        laserSpeed = 50f;
    }

    // Temporary invincibility on hit
    public void startInvincibilityRoutine()
    {
        isInvincible = true;
        _invincibilityAnim.gameObject.SetActive(true);
        StartCoroutine(InvincibilityPowerDownRoutine());
    }
    public IEnumerator InvincibilityPowerDownRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        isInvincible = false;
        _invincibilityAnim.gameObject.SetActive(false);
    }
}
