using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    private GameManager _gameManager;
    private UIManager _uiManager;

    [SerializeField] public GameObject _spawnerExplosion;
    //[SerializeField] private AudioClip _clip;
    [SerializeField] public GameObject _enemyPrefab;

    [SerializeField] private int hitpoints;
    [SerializeField] public int pointValue;

    private bool firstLoad = true;
    private int laserHits = 0; // Used to prevent multiple laser hits at once
    
    Coroutine EnemySpawn;

    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        
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
            if (firstLoad) { // Delay the enemy spawning when first loaded
                yield return new WaitForSeconds(1.5f);
                firstLoad = false;
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(0.5f, 0.65f));
            }

            // Slightly offset enemy spawning from center of spawner
            float offSetX = transform.position.x + Random.Range(-0.4f, 0.4f);
            float offSetY = transform.position.y + Random.Range(-0.4f, 0.4f);

            Instantiate(_enemyPrefab, new Vector3(offSetX, offSetY, 0), Quaternion.identity);
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
                _gameManager.updateSpawnsDestroyed(1);
                _uiManager.UpdateScore(pointValue);
                StopCoroutines();

                Instantiate(_spawnerExplosion, transform.position, Quaternion.identity);
                //AudioSource.PlayClipAtPoint(_clip, Camera.main.transform.position, 1f);
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
}
