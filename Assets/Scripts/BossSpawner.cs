using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour {    
    private GameManager _gameManager;
    private UIManager _uiManager;
    //[SerializeField] private AudioClip _clip;

    [SerializeField] public GameObject _bossSpawnExplosion;
    [SerializeField] public GameObject _boss1Prefab;
    [SerializeField] public GameObject _boss2Prefab;
    [SerializeField] public GameObject _boss3Prefab;
    [SerializeField] public GameObject _boss4Prefab;
    [SerializeField] public GameObject[] extraBosses;
    [SerializeField] public GameObject _enemyPrefab;
    
    [SerializeField] private int hitpoints = 300;
    [SerializeField] public int pointValue = 1000;
    public bool bossSpawned = false; // First, spawn the boss, then set to true. Continue to spawn enemies (hard)
    
    Coroutine EnemySpawn;
    
    private bool firstLoad = true;
    private bool bossLoaded = false;
    public int currentBossPhase = 1;

    private int laserHits = 0; // Prevent multiple collision events   
    
    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _gameManager.bossFightStart();
        currentBossPhase = _gameManager.phase;

        StartCoroutines();
    }

    public void StartCoroutines()
    {
        EnemySpawn = StartCoroutine(SpawnEnemyRoutine());
    }

    public void StopCoroutines()
    {
        StopCoroutine(EnemySpawn);
    }

    IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            if (firstLoad)
            {
                _uiManager.ShowBossWarning();
                yield return new WaitForSeconds(2.0f);
                firstLoad = false;      
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(0.9f, 1.3f));
            }

            if (bossLoaded == false)
            {
                _uiManager.HideBossWarning();
                if (currentBossPhase == 2) {
                    Instantiate(_boss1Prefab, transform.position, Quaternion.identity);
                }
                else if (currentBossPhase == 3)
                {
                    Instantiate(_boss2Prefab, transform.position, Quaternion.identity);
                }
                else if (currentBossPhase == 4)
                {
                    Instantiate(_boss3Prefab, transform.position, Quaternion.identity);
                }
                else if (currentBossPhase == 5)
                {
                    Instantiate(_boss4Prefab, transform.position, Quaternion.identity);
                }
                else if (currentBossPhase > 5) // Beyond phase 4 starts to generate one of the extra bosses
                {
                    int randBoss = Random.Range(0, 3);
                    Instantiate(extraBosses[randBoss], transform.position, Quaternion.identity);
                }
                
                bossLoaded = true;
                _uiManager.ShowBossInfo();
            }
            else
            {
                // Slightly offset from center
                generateEnemy();

                if (currentBossPhase > 3) // Beyond phase 2 (offset by + 1), generate 2 enemies at once
                {
                    generateEnemy();

                    if (currentBossPhase > 5) // Beyond phase 4 (offset by + 1), generate 3 enemies
                    {
                        generateEnemy();
                    }
                }

            }
        }
    }

    public void Damaged()
    {
        hitpoints -= 5;

        if (hitpoints <= 0)
        {
            laserHits += 1;
            if (laserHits == 1)
            {
                _uiManager.UpdateScore(pointValue);
                StopCoroutines();

                _gameManager.bossSpawnDestroyed(); // TODO create method
                if (bossLoaded == false)
                {
                    _gameManager.bossDestroyed(); // Spawn was destroyed before boss spawned -_-
                }

                if (_gameManager.spawnsDestroyed <= 18) 
                {
                    _gameManager.updateSpawnsDestroyed(0);                    
                }
                else // After 18 spawns destroyed, it enters a cycle. Generates boss every phase (4 spawns destroyed).
                { 
                    _gameManager.bossSpawnExtra();
                }

                Instantiate(_bossSpawnExplosion, transform.position, Quaternion.identity);
                //AudioSource.PlayClipAtPoint(_clip, Camera.main.transform.position, 1f);
                _uiManager.HideBossWarning();
                Destroy(this.gameObject);
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
    }

    public void generateEnemy()
    {
        float offSetX = transform.position.x + Random.Range(-0.8f, 0.8f);
        float offSetY = transform.position.y + Random.Range(-0.8f, 0.8f);

        Instantiate(_enemyPrefab, new Vector3(offSetX, offSetY, 0), Quaternion.identity);
    }
}
