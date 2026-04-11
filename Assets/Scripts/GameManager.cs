using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject puseScreen;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject player;

    [SerializeField] private LivesUIController livesUI;
    [SerializeField] private int maxLives = 5;
    [SerializeField] private TextMeshProUGUI scoreText;
    public bool isPuse = false;
    private int lives = 3;
    private int score;
    public bool IsGameOver { get; private set; }
    private void Start()
    {
        Time.timeScale = 1;
        IsGameOver = false;
        livesUI.UpdateLives(lives);
        score = 0;
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }
    public void PuseScreen()
    {
        Time.timeScale = 0;
        isPuse = true;
        SetAnimatorsUnscaledTime(puseScreen);
        puseScreen.SetActive(true);
    }
    public void UnPuseScreen()
    {
        Time.timeScale = 1;
        isPuse = false;
        puseScreen.SetActive(false);
    }

    private void SetAnimatorsUnscaledTime(GameObject root)
    {
        if (root == null) return;
        var animators = root.GetComponentsInChildren<Animator>(true);
        foreach (var animator in animators)
        {
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
    }

    public void RemoveLife(int livesToRemove)
    {
        if (IsGameOver) return;

        lives = Mathf.Max(0, lives - livesToRemove);
        livesUI.UpdateLives(lives);
        if (lives <= 0)
        {
            GameOver();
            player.SetActive(false);
        }
    }

    public void AddLife(int livesToAdd)
    {
        if (IsGameOver) return;

        lives = Mathf.Clamp(lives + livesToAdd, 0, maxLives);
        livesUI.UpdateLives(lives);
    }

    private void GameOver()
    {
        IsGameOver = true;
        gameOverScreen.SetActive(true);
    }
    public void RetartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }
    public void ExitGame()
    {
        Time.timeScale = 1;
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
    public void AddScore(int scoreToAdd)
    {
        if (IsGameOver) return;
        score += scoreToAdd;
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }



}
