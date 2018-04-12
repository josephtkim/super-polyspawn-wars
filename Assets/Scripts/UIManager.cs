using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    private GameManager _gameManager;

    [SerializeField] private Sprite[] lives;

    [SerializeField] private Image startUpScreenImage;
    [SerializeField] private Text LifeLabelText;
    [SerializeField] private Image LivesImage;

    [SerializeField] private Text ScoreLabelText;
    [SerializeField] private Text ScoreValueText;

    [SerializeField] private Text CurrentPhaseText;
    [SerializeField] private Image bossWarningImage;
    [SerializeField] private Text BossHealthLabelText;
    [SerializeField] private Text BossHealthValueText;

    [SerializeField] private Text finalScoreLabel;
    [SerializeField] private Text finalScoreText;

    [SerializeField] private Text finalGradeLabel;
    [SerializeField] private Text finalGradeText;
    [SerializeField] private Text newGameText;
    [SerializeField] private Text evalScreenQuitText;

    [SerializeField] private Image pausedImage;

    public int score;
    public int difficulty;    
    public bool shouldShowPhase = false;

    Coroutine ShowPhase;
    
    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        score = 0;
        difficulty = 1;
    }
    private void Update()
    {
        if (shouldShowPhase == true)
        {            
            CurrentPhaseText.text = "PHASE " + _gameManager.phase;
        }
        else
        {
            CurrentPhaseText.gameObject.SetActive(false);           
        }
    }

    // Title Screen
    public void ShowTitleScreen()
    {
        startUpScreenImage.gameObject.SetActive(true);
        HideLivesAndScore();
    }
    public void HideTitleScreen()
    {
        startUpScreenImage.gameObject.SetActive(false);        
        ShowLivesAndScore();
        ShowCurrentPhase();
    }

    // Game Over screen
    public void ShowGameOverScreen()
    {
        // Evaluation based on overall score
        string[] finalGrades = new string[] 
           {"CADET",
            "SPACE COWBOY",
            "ADMIRAL",
            "GALAXY GUARDIAN",
            "SPACE-TIME HERO",
            "LEGEND"};

        if (score < 10000)
        {
            finalGradeText.text = finalGrades[0];
        }
        else if(score >= 10000 && score < 50000)
        {
            finalGradeText.text = finalGrades[1];
        }
        else if (score >= 50000 && score < 100000)
        {
            finalGradeText.text = finalGrades[2];
        }
        else if (score >= 100000 && score < 200000)
        {
            finalGradeText.text = finalGrades[3];
        }
        else if (score >= 200000 && score < 300000)
        {
            finalGradeText.text = finalGrades[4];
        }
        else if (score > 300000)
        {
            finalGradeText.text = finalGrades[5];
        }

        HideLivesAndScore();
        HidePauseScreen();
        HideCurrentPhase();
        HideBossWarning();
        HideBossInfo();

        finalScoreText.text = score.ToString();
        showEvaluationScreen();
    }
    public void HideGameOverScreen()
    {
        hideEvaluationScreen();
        ShowLivesAndScore();
        ShowCurrentPhase();
    }

    private void showEvaluationScreen()
    {
        finalScoreLabel.gameObject.SetActive(true);
        finalScoreText.gameObject.SetActive(true);
        finalGradeLabel.gameObject.SetActive(true);
        finalGradeText.gameObject.SetActive(true);

        newGameText.gameObject.SetActive(true);
        evalScreenQuitText.gameObject.SetActive(true);
    }

    private void hideEvaluationScreen()
    {
        finalScoreLabel.gameObject.SetActive(false);
        finalScoreText.gameObject.SetActive(false);
        finalGradeLabel.gameObject.SetActive(false);
        finalGradeText.gameObject.SetActive(false);

        newGameText.gameObject.SetActive(false);
        evalScreenQuitText.gameObject.SetActive(false);
    }

    // Update game values
    public void UpdateLives(int currentLives)
    {                
        LivesImage.sprite = lives[currentLives];
    }
    public void UpdateScore(int points)
    {
        score += points;
        ScoreValueText.text = score.ToString();
    }

    // Lives and Score displays
    public void ShowLivesAndScore()
    {
        ScoreLabelText.gameObject.SetActive(true);
        ScoreValueText.gameObject.SetActive(true);

        LifeLabelText.gameObject.SetActive(true);
        LivesImage.gameObject.SetActive(true);
    }
    public void HideLivesAndScore()
    {
        ScoreLabelText.gameObject.SetActive(false);
        ScoreValueText.gameObject.SetActive(false);

        LifeLabelText.gameObject.SetActive(false);
        LivesImage.gameObject.SetActive(false);

        HideBossInfo(); // If boss info was showing, hide that too
    }

    // Boss Warning Image
    public void ShowBossWarning()
    {
        bossWarningImage.gameObject.SetActive(true);
    }
    public void HideBossWarning()
    {
        bossWarningImage.gameObject.SetActive(false);
    }

    // Current Phase
    // Display for 3 seconds then disappear
    public void ShowCurrentPhase()
    {
        shouldShowPhase = true;
        CurrentPhaseText.gameObject.SetActive(true);
        StartCoroutines();
    }
    public void HideCurrentPhase()
    {
        CurrentPhaseText.gameObject.SetActive(false);
    }
  
    public void StartCoroutines()
    {
        ShowPhase = StartCoroutine(ShowPhaseRoutine());
    }
    public void StopCoroutines()
    {
        StopCoroutine(ShowPhase);
    }
    
    IEnumerator ShowPhaseRoutine()
    {
        if (shouldShowPhase == true)
        {                
            yield return new WaitForSeconds(3f);

            shouldShowPhase = false;
            StopCoroutines();
        }                    
    }

    // Display boss info
    public void ShowBossInfo()
    {
        BossHealthLabelText.gameObject.SetActive(true);
        BossHealthValueText.gameObject.SetActive(true);
    }
    public void HideBossInfo()
    {
        BossHealthLabelText.gameObject.SetActive(false);
        BossHealthValueText.gameObject.SetActive(false);
    }

    public void updateBossHP(int hitpoints)
    {
        BossHealthValueText.text = hitpoints.ToString();
    }

    // Pause screen
    public void ShowPauseScreen()
    {
        pausedImage.gameObject.SetActive(true);
    }

    public void HidePauseScreen()
    {
        pausedImage.gameObject.SetActive(false);
    }
}
