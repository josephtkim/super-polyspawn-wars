using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSpawner : MonoBehaviour {
    [SerializeField] private GameObject[] powerups;

    Coroutine PowerupSpawn;
		
	public void StartCoroutines()
    {
        PowerupSpawn = StartCoroutine(PowerupSpawnRoutine());        
    }

    public void StopCoroutines()
    {
        StopCoroutine(PowerupSpawn);        
    }

    IEnumerator PowerupSpawnRoutine()
    {
        while (true)
        {
            LoadPowerUp();
            yield return new WaitForSeconds(Random.Range(24f, 27f));
        }
    }

    private void LoadPowerUp()
    {
        // 0 = health, 1 = shield, 2 = speed, 3 = spreadshot
        int randomIndex = Random.Range(0, 4);

        float randomX = Random.Range(-2f, 2f);
        float randomY = Random.Range(-2f, 2f);

        Instantiate(powerups[randomIndex], new Vector3(randomX, randomY, 0), Quaternion.identity);
    }
}
