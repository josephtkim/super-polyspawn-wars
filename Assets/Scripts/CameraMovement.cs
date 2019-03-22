using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    // Music
    [SerializeField] private AudioSource _audioSource1;
    [SerializeField] private AudioSource _audioSource2;
    [SerializeField] private AudioSource _audioSource3;
    [SerializeField] private AudioSource _audioSource4;
    private bool musicPlaying = false;

    public GameObject player;
    private GameManager _gameManager;
    
    // Background layers
    GameObject bg1;
    GameObject bg2;
    GameObject bg3;

    [SerializeField] public float smoothTime = 2f;
    private Vector3 velocity = new Vector3(0, 0, 0);

    // Set boundaries for camera movement    
    private float leftBound = -3f;
    private float rightBound = 3f;
    private float topBound = 1.2f;
    private float bottomBound = -1.2f;

    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.FindWithTag("Player");
        
        playRandomSong();
        musicPlaying = true;

        // Background layers
        bg1 = GameObject.FindGameObjectWithTag("background1");
        bg2 = GameObject.FindGameObjectWithTag("background2");
        bg3 = GameObject.FindGameObjectWithTag("background3");
    }

    private void playRandomSong()
    {
        // Randomize song selection
        int songNumber = Random.Range(0,4);
        if (songNumber == 0)
        {
            _audioSource1.Play();
        }
        else if (songNumber == 1)
        {
            _audioSource2.Play();
        }
        else if (songNumber == 2)
        {
            _audioSource3.Play();
        }
        else
        {
            _audioSource4.Play();
        }
    }

    private void stopPlayingSongs()
    {
        _audioSource1.Stop();
        _audioSource2.Stop();
        _audioSource3.Stop();
        _audioSource4.Stop();
    }

    void Update()
    {
        if (player != null)
        {
            // Get current player's position
            Vector3 playerPosition = player.transform.position;
            playerPosition.z = -10; // prevent from changing the z-axis value
            Vector3 newPosition = playerPosition;

            // Makes sure camera doesn't go too beyond the borders of world
            if (playerPosition.x > rightBound)
            {
                newPosition.x = rightBound;
            }
            else if (playerPosition.x < leftBound)
            {
                newPosition.x = leftBound;
            }

            if (playerPosition.y > topBound)
            {
                newPosition.y = topBound;
            }
            else if (playerPosition.y < bottomBound)
            {
                newPosition.y = bottomBound;
            }

            transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);

            // Background parallax movement
            float x = playerPosition.x;
            float y = playerPosition.y;
            bg1.transform.position = new Vector3(-0.08f * x, -0.08f * y, 0);
            bg2.transform.position = new Vector3(-0.06f * x, -0.06f * y, 0);
            bg3.transform.position = new Vector3(-0.03f * x, -0.03f * y, 0);

            if (musicPlaying == false)
            {
                playRandomSong();
                musicPlaying = true;
            }
        }
        else
        {
            if (_gameManager.gameOver == false)
            {
                player = GameObject.FindWithTag("Player");                
            }

            if (musicPlaying == true) {
                stopPlayingSongs();
                musicPlaying = false;
            }
        }
    }
}
