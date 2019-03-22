using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PowerupSpawner _powerupSpawnManager;
    private UIManager _uiManager;

    [SerializeField] public GameObject player;    
    [SerializeField] public GameObject spawner_easy;
    [SerializeField] public GameObject spawner_med;
    [SerializeField] public GameObject spawner_hard;
    [SerializeField] public GameObject boss_spawner;

    public GameObject currentPlayer; // TESTING, used to get current player and destroy on new game

    private GameObject[] enemies;
    private GameObject[] powerups;
    private GameObject[] spawners;
    private GameObject[] lasers;

    // Game Borders
    public float topBorder = 4f;
    public float bottomBorder = -4f;
    public float rightBorder = 9.5f;
    public float leftBorder = -9.5f;

    // Powerup zone
    public float topPZone = 2.4f;
    public float bottomPZone = -2.4f;
    public float rightPZone = 2.4f;
    public float leftPZone = -2.4f;

    public int currentScore;
    public int previousScore;
    public bool gameOver = true;
    public bool firstTime = true; // First startup of game used for title screen

    [SerializeField] public int spawnsDestroyed = 0; // Counts the number of spawns destroyed.
    public int phase = 1; // phase in game, 1-4
    public int currentSpawners = 0;
    public int currentMaxSpawners = 1;        

    public bool bossAlive = false;
    public bool bossSpawnerAlive = false;
    public int phaseCounter = 0;

    public bool isPaused = false;
    
    void Start() {                
        _powerupSpawnManager = GameObject.Find("PowerupSpawner").GetComponent<PowerupSpawner>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        // Display the startup title Screen
        _uiManager.ShowTitleScreen();
        previousScore = 0;
    }
		
	void Update() {
        if (gameOver == false && firstTime == false)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (Time.timeScale == 1)
                {
                    Time.timeScale = 0;
                    isPaused = true;
                    _uiManager.ShowPauseScreen();
                }
                else if (Time.timeScale == 0)
                {
                    Time.timeScale = 1;
                    isPaused = false;
                    _uiManager.HidePauseScreen();
                }
            }

            if (isPaused == true) {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    GameOver();
                    Application.Quit();
                }
            }
        }
        
        if (gameOver == true)
        {                       
            if (Input.GetKeyDown(KeyCode.Space))
            {
                newGame();
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Application.Quit();
            }
        }
	}

    public void StopCoroutines()
    {        
        _powerupSpawnManager.StopCoroutines();
    }

    public void GameOver()
    {
        StopCoroutines();

        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
        }

        gameOver = true;
        _uiManager.ShowGameOverScreen();

        // Check if player still exists
        currentPlayer = GameObject.FindWithTag("Player");
        if (currentPlayer != null) {
            Destroy(currentPlayer);
        }

        // Destroy all gameobjects on screen
        spawners = GameObject.FindGameObjectsWithTag("Spawner");
        foreach (GameObject spawn in spawners)
        {
            Destroy(spawn);
        }

        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        lasers = GameObject.FindGameObjectsWithTag("Laser");
        foreach (GameObject laser in lasers)
        {
            Destroy(laser);
        }

        powerups = GameObject.FindGameObjectsWithTag("Powerup");
        foreach(GameObject powerup in powerups)
        {
            Destroy(powerup);
        }
    }
    public void newGame()
    {
        currentSpawners = 0;
        currentMaxSpawners = 1;        
        phase = 1;
        gameOver = false;
        spawnsDestroyed = 0;        

        bossAlive = false;
        bossSpawnerAlive = false;

        // Make sure no longer paused
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            isPaused = false;
        }

        if (firstTime == true)
        {
            _uiManager.HideTitleScreen();            
            firstTime = false;
        }
        else
        {
            _uiManager.HideGameOverScreen();
        }

        Instantiate(spawner_easy, new Vector3(0, 4.5f, 0), Quaternion.identity);        
        Vector3 playerPosition = new Vector3(0, -4.5f, 0);
        Instantiate(player, playerPosition, Quaternion.identity);        
        _powerupSpawnManager.StartCoroutines();
        currentSpawners += 1;
    }

    public void updateSpawnsDestroyed(int number)
    {        
        spawnsDestroyed += number;
        currentSpawners -= number;

        // Depending on spawners destroyed, set phase
        if (spawnsDestroyed == 3 && phase == 1)
        {
            phase = 2;            
            generateBossSpawner();            
            
            currentMaxSpawners = 2;
        }
        else if (spawnsDestroyed == 9 && phase == 2)
        {
            phase = 3;            
            generateBossSpawner();
            
            currentMaxSpawners = 3;
        }
        else if (spawnsDestroyed == 18 && phase == 3)
        {
            phase = 4;            
            generateBossSpawner();            

            currentMaxSpawners = 4;
        }
        else if (spawnsDestroyed > 18)
        {
            if ((spawnsDestroyed - 2) % 12 == 0) // Every 12 spawners (4, 4, 4) destroyed, start new phase
            {
                phase += 1;
                generateBossSpawner();
            }
        }

        // Depending on phase and currentSpawners left, instantiate spawners
        if (currentSpawners == 0 && bossAlive == false && bossSpawnerAlive == false) {
            generateSpawners(phase);
        }
    }
    public void bossSpawnExtra() // Once enters cycle after phase 4, runs this after bossSpawner dies
    {
        if (currentSpawners == 0 && bossAlive == false && bossSpawnerAlive == false)
        {
            generateSpawners(phase);
        }
    }
    public void bossDestroyed()
    {        
        bossAlive = false;
        _uiManager.HideBossInfo();

        if (bossSpawnerAlive == false)
        {
            _uiManager.ShowCurrentPhase();
        }
    }
    public void bossSpawnDestroyed()
    {        
        bossSpawnerAlive = false;

        if (bossAlive == false)
        {
            _uiManager.ShowCurrentPhase();
        }
    }
    public void bossFightStart()
    {        
        bossAlive = true;
        bossSpawnerAlive = true;
    }

    public bool isInsidePowerupZone(float x, float y)
    {
        if ((x >= leftPZone && x <= rightPZone) && (y >= bottomPZone && y <= topPZone)) {
            return true;           
        }
        else
        {
            return false;
        }
    }
    public void generateSpawners(int currentPhase)
    {
        if (currentPhase == 1 && currentSpawners == 0)
        {
            float newX = Random.Range(leftBorder, rightBorder);
            float newY = Random.Range(bottomBorder, topBorder);

            // Prevent from spawning inside of powerup spawner
            while (isInsidePowerupZone(newX, newY))
            {
                newX = Random.Range(leftBorder, rightBorder);
                newY = Random.Range(bottomBorder, topBorder);
            }

            Vector3 enemyPos = new Vector3(newX, newY, 0);
                        
            Instantiate(spawner_easy, enemyPos, Quaternion.identity);

            currentSpawners += 1;
        }

        else if (currentPhase == 2 && currentSpawners == 0)
        {            
            // Left half space
            float newX1 = Random.Range(leftBorder, -1f);
            float newY1 = Random.Range(bottomBorder, topBorder);

            while (isInsidePowerupZone(newX1, newY1))
            {
                newX1 = Random.Range(leftBorder, -1f);
                newY1 = Random.Range(bottomBorder, topBorder);
            }

            // Right half space
            float newX2 = Random.Range(1f, rightBorder);
            float newY2 = Random.Range(bottomBorder, topBorder);

            while (isInsidePowerupZone(newX2, newY2))
            {
                newX2 = Random.Range(1f, rightBorder);
                newY2 = Random.Range(bottomBorder, topBorder);
            }
            
            Vector3 enemy1Pos = new Vector3(newX1, newY1, 0);
            Instantiate(spawner_med, enemy1Pos, Quaternion.identity);

            Vector3 enemy2Pos = new Vector3(newX2, newY2, 0);
            Instantiate(spawner_med, enemy2Pos, Quaternion.identity);

            currentSpawners += 2;
        }

        else if (currentPhase == 3 && currentSpawners == 0)
        {
            // left            
            float newX1 = Random.Range(leftBorder, (leftBorder/2) - 0.6f);
            float newY1 = Random.Range(bottomBorder, topBorder);

            while (isInsidePowerupZone(newX1, newY1))
            {
                newX1 = Random.Range(leftBorder, (leftBorder/2) - 0.6f);
                newY1 = Random.Range(bottomBorder, topBorder);
            }

            // middle            
            float newX2 = Random.Range(leftBorder/2, rightBorder/2);
            float newY2 = Random.Range(bottomBorder, topBorder);

            while (isInsidePowerupZone(newX2, newY2))
            {
                newX2 = Random.Range(leftBorder/2, rightBorder/2);
                newY2 = Random.Range(bottomBorder, topBorder);
            }

            // right            
            float newX3 = Random.Range((rightBorder/2) + 0.6f, rightBorder);
            float newY3 = Random.Range(bottomBorder, topBorder);

            while (isInsidePowerupZone(newX3, newY3))
            {
                newX3 = Random.Range((rightBorder/2) + 0.6f, rightBorder);
                newY3 = Random.Range(bottomBorder, topBorder);
            }

            Vector3 enemy1Pos = new Vector3(newX1, newY1, 0);
            Instantiate(spawner_hard, enemy1Pos, Quaternion.identity);

            Vector3 enemy2Pos = new Vector3(newX2, newY2, 0);
            Instantiate(spawner_hard, enemy2Pos, Quaternion.identity);

            Vector3 enemy3Pos = new Vector3(newX3, newY3, 0);
            Instantiate(spawner_hard, enemy3Pos, Quaternion.identity);

            currentSpawners += 3;
        }

        else if (currentPhase == 4 && currentSpawners == 0)
        {
            // upper left            
            float newX1 = Random.Range(leftBorder, -0.75f);
            float newY1 = Random.Range(0.75f, topBorder);

            while (isInsidePowerupZone(newX1, newY1))
            {
                newX1 = Random.Range(leftBorder, -0.75f);
                newY1 = Random.Range(0.75f, topBorder);
            }

            // upper right
            float newX2 = Random.Range(0.75f, rightBorder);
            float newY2 = Random.Range(0.75f, topBorder);

            while (isInsidePowerupZone(newX2, newY2))
            {
                newX2 = Random.Range(0.75f, rightBorder);
                newY2 = Random.Range(0.75f, topBorder);
            }

            // lower left            
            float newX3 = Random.Range(leftBorder, -0.75f);
            float newY3 = Random.Range(bottomBorder, -0.75f);

            while (isInsidePowerupZone(newX3, newY3))
            {
                newX3 = Random.Range(leftBorder, -0.75f);
                newY3 = Random.Range(bottomBorder, -0.75f);
            }

            // lower right
            float newX4 = Random.Range(0.75f, rightBorder);
            float newY4 = Random.Range(bottomBorder, -0.75f);

            while (isInsidePowerupZone(newX4, newY4))
            {
                newX4 = Random.Range(0.75f, rightBorder);
                newY4 = Random.Range(bottomBorder, -0.75f);
            }

            Vector3 enemy1Pos = new Vector3(newX1, newY1, 0);
            Instantiate(spawner_hard, enemy1Pos, Quaternion.identity);

            Vector3 enemy2Pos = new Vector3(newX2, newY2, 0);
            Instantiate(spawner_hard, enemy2Pos, Quaternion.identity);

            Vector3 enemy3Pos = new Vector3(newX3, newY3, 0);
            Instantiate(spawner_hard, enemy3Pos, Quaternion.identity);

            Vector3 enemy4Pos = new Vector3(newX4, newY4, 0);
            Instantiate(spawner_hard, enemy4Pos, Quaternion.identity);

            currentSpawners += 4;
        }

        else if (currentPhase > 4 && currentSpawners == 0)
        {
            // upper left            
            float newX1 = Random.Range(leftBorder, -0.75f);
            float newY1 = Random.Range(0.75f, topBorder);

            while (isInsidePowerupZone(newX1, newY1))
            {
                newX1 = Random.Range(leftBorder, -0.75f);
                newY1 = Random.Range(0.75f, topBorder);
            }

            // upper right
            float newX2 = Random.Range(0.75f, rightBorder);
            float newY2 = Random.Range(0.75f, topBorder);

            while (isInsidePowerupZone(newX2, newY2))
            {
                newX2 = Random.Range(0.75f, rightBorder);
                newY2 = Random.Range(0.75f, topBorder);
            }

            // lower left            
            float newX3 = Random.Range(leftBorder, -0.75f);
            float newY3 = Random.Range(bottomBorder, -0.75f);

            while (isInsidePowerupZone(newX3, newY3))
            {
                newX3 = Random.Range(leftBorder, -0.75f);
                newY3 = Random.Range(bottomBorder, -0.75f);
            }

            // lower right
            float newX4 = Random.Range(0.75f, rightBorder);
            float newY4 = Random.Range(bottomBorder, -0.75f);

            while (isInsidePowerupZone(newX4, newY4))
            {
                newX4 = Random.Range(0.75f, rightBorder);
                newY4 = Random.Range(bottomBorder, -0.75f);
            }

            Vector3 enemy1Pos = new Vector3(newX1, newY1, 0);
            Instantiate(spawner_hard, enemy1Pos, Quaternion.identity);

            Vector3 enemy2Pos = new Vector3(newX2, newY2, 0);
            Instantiate(spawner_hard, enemy2Pos, Quaternion.identity);

            Vector3 enemy3Pos = new Vector3(newX3, newY3, 0);
            Instantiate(spawner_hard, enemy3Pos, Quaternion.identity);

            Vector3 enemy4Pos = new Vector3(newX4, newY4, 0);
            Instantiate(spawner_hard, enemy4Pos, Quaternion.identity);

            currentSpawners += 4;
        }
    }

    public void generateBossSpawner()
    {
        // Generate at one of 4 corners
        int randPos = Random.Range(0, 4);
        float newX = 7f;
        float newY = 2.5f;
        
        // For randPos=0, it would just be 7, 2.5
        if (randPos == 1)
        {
            newY = -2.5f;
        }        
        else if (randPos == 2)
        {
            newX = -7f;
            newY = -2.5f;
        }
        else
        {
            newX = -7f;
        }
                
        Vector3 bossSpawnerPos = new Vector3(newX, newY, 0);
        Instantiate(boss_spawner, bossSpawnerPos, Quaternion.identity);

        bossAlive = true;
        bossSpawnerAlive = true;
    }    
}
