using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 游戏管理器 - 管理游戏状态、生命值、分数和暂停功能
/// </summary>
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject puseScreen; // 暂停界面
    [SerializeField] private GameObject gameOverScreen; // 游戏结束界面
    [SerializeField] private GameObject player; // 玩家对象

    [SerializeField] private LivesUIController livesUI; // 生命值UI控制器
    [SerializeField] private int maxLives = 5; // 最大生命值
    [SerializeField] private TextMeshProUGUI scoreText; // 分数文本
    public bool isPause = false; // 是否暂停
    private int lives = 3; // 当前生命值
    private int score; // 当前分数
    public bool IsGameOver { get; private set; } // 游戏是否结束

    private void ResolveSceneReferences()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (livesUI == null)
        {
            livesUI = FindObjectOfType<LivesUIController>();
        }
    }

    /// <summary>
    /// 游戏开始时初始化
    /// </summary>
    private void Start()
    {
        ResolveSceneReferences();
        Time.timeScale = 1;
        IsGameOver = false;
        if (livesUI != null)
        {
            livesUI.UpdateLives(lives);
        }
        score = 0;
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    /// <summary>
    /// 显示暂停界面
    /// </summary>
    public void PuseScreen()
    {
        Time.timeScale = 0;
        isPause = true;
        if (puseScreen != null)
        {
            SetAnimatorsUnscaledTime(puseScreen);
            puseScreen.SetActive(true);
        }
    }

    /// <summary>
    /// 取消暂停，恢复游戏
    /// </summary>
    public void UnPuseScreen()
    {
        Time.timeScale = 1;
        isPause = false;
        if (puseScreen != null)
        {
            puseScreen.SetActive(false);
        }
    }

    /// <summary>
    /// 设置动画器使用非缩放时间（用于暂停时播放动画）
    /// </summary>
    private void SetAnimatorsUnscaledTime(GameObject root)
    {
        if (root == null) return;
        var animators = root.GetComponentsInChildren<Animator>(true);
        foreach (var animator in animators)
        {
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
    }

    /// <summary>
    /// 减少生命值
    /// </summary>
    public void RemoveLife(int livesToRemove)
    {
        if (IsGameOver) return;

        lives = Mathf.Max(0, lives - livesToRemove);
        if (livesUI != null)
        {
            livesUI.UpdateLives(lives);
        }
        if (lives <= 0)
        {
            GameOver();
            if (player != null)
            {
                player.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 增加生命值
    /// </summary>
    public void AddLife(int livesToAdd)
    {
        if (IsGameOver) return;

        lives = Mathf.Clamp(lives + livesToAdd, 0, maxLives);
        if (livesUI != null)
        {
            livesUI.UpdateLives(lives);
        }
    }

    /// <summary>
    /// 游戏结束处理
    /// </summary>
    private void GameOver()
    {
        IsGameOver = true;

        // 游戏结束时确保保存最高分
        if (DataManager.Instance != null)
        {
            DataManager.Instance.UpdateHighScore(score);
        }

        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }
    }

    /// <summary>
    /// 重新开始游戏
    /// </summary>
    public void RetartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void ExitGame()
    {
        Time.timeScale = 1;
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// 增加分数
    /// </summary>
    public void AddScore(int scoreToAdd)
    {
        if (IsGameOver) return;
        score += scoreToAdd;
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }

        // 实时更新最高分
        if (DataManager.Instance != null)
        {
            DataManager.Instance.UpdateHighScore(score);
        }
    }
}
