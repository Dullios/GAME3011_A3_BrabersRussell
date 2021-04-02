using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject startPanel;
    public GameObject gamePanel;
    public GameObject gameOverPanel;

    [Header("UI Components")]
    public TMP_Text difText;
    public TMP_Text timerText;
    public TMP_Text reqScoreText;
    public TMP_Text scoreText;
    public TMP_Text gameOverText;

    public static UIManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(false);

        SetTimer(GameManager.instance.maxTime);
        SetRequiredScore(GameManager.instance.reqScore);
    }

    public void OnDifficultySelect(TMP_Dropdown drop)
    {
        switch (drop.value)
        {
            case 0:
                GameManager.instance.difficulty = Difficulty.EASY;
                GameManager.instance.maxTime = 60.0f;
                GameManager.instance.reqScore = 1000;
                break;
            case 1:
                GameManager.instance.difficulty = Difficulty.MEDIUM;
                GameManager.instance.maxTime = 90.0f;
                GameManager.instance.reqScore = 2500;
                break;
            case 2:
                GameManager.instance.difficulty = Difficulty.HARD;
                GameManager.instance.maxTime = 120.0f;
                GameManager.instance.reqScore = 4000;
                break;
        }

        SetTimer(GameManager.instance.maxTime);
        SetRequiredScore(GameManager.instance.reqScore);
    }

    public void OnStartButton()
    {
        GameManager.instance.InstantiateBoard();
        startPanel.SetActive(false);
        gamePanel.SetActive(true);

        difText.text = "Difficulty: " + GameManager.instance.difficulty.ToString();
    }

    public void SetTimer(float value)
    {
        if (value <= 0)
            timerText.text = "Timer : 00.0";
        else
            timerText.text = "Timer : " + value.ToString("00.0");
    }

    public void SetRequiredScore(int value)
    {
        reqScoreText.text = "Required : " + value.ToString("0000");
    }

    public void SetScore(int value)
    {
        scoreText.text = "Score : " + value.ToString("0000");
    }

    public void SetGameOverText(bool hasWon)
    {
        gameOverPanel.SetActive(true);

        if (hasWon)
            gameOverText.text = "You Win!";
        else
            gameOverText.text = "You Lose!";
    }

    public void OnRestartClick()
    {
        SceneManager.LoadScene(0);
    }
}