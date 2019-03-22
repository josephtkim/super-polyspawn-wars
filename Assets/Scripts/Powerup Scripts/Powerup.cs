using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour {
    // 0 = health, 1 = shield, 2 = speed, 3 = spreadshot 
    [SerializeField] private int powerupID;
    [SerializeField] private float timeUntilDissolve = 20f; // Disappears after time
    [SerializeField] private AudioClip _clip;

    float time = 0f;

    Coroutine timer;
    
    private void Start()
    {
        StartTimer();
    }   
    
    void Update () {
		if (time >= timeUntilDissolve)
        {
            StopCoroutine(TimerUntilGoneRoutine());
            Destroy(this.gameObject);
        }
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {                
                AudioSource.PlayClipAtPoint(_clip, Camera.main.transform.position, 1f);

                // 0 = health, 1 = shield, 2 = speed, 3 = spreadshot 
                if (powerupID == 0)
                {
                    player.powerupHealth();
                }
                else if (powerupID == 1)
                {
                    player.powerupShield();
                }
                else if (powerupID == 2)
                {
                    player.powerupSpeed();
                }
                else if (powerupID == 3)
                {
                    player.powerupRapidShot();
                }
            }

            Destroy(this.gameObject);
        }
    }

    public void StartTimer()
    {
        timer = StartCoroutine(TimerUntilGoneRoutine());
    }

    IEnumerator TimerUntilGoneRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);            
            time += 5;            
        }
    }
}
