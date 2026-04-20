using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 最高分显示控制器 - 按Tab键显示/隐藏最高分面板
/// </summary>
public class HighScoreDisplay : MonoBehaviour
{
    [SerializeField] private GameObject highScorePanel; // 最高分面板
    [SerializeField] private TextMeshProUGUI highScoreText; // 最高分文本
    [SerializeField] private KeyCode toggleKey = KeyCode.Tab; // 切换显示的按键

    /// <summary>
    /// 初始化时隐藏面板
    /// </summary>
    private void Start()
    {
        // 初始时隐藏面板
        if (highScorePanel != null)
        {
            highScorePanel.SetActive(false);
        }
    }

    /// <summary>
    /// 每帧检测Tab键输入
    /// </summary>
    private void Update()
    {
        // 按下Tab键时切换显示并暂停游戏
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleHighScorePanel();
            Time.timeScale = 0; // 暂停游戏
        }
        // 松开Tab键时隐藏并恢复游戏
        else if (Input.GetKeyUp(toggleKey))
        {
            HideHighScorePanel();
            Time.timeScale = 1; // 恢复游戏
        }
    }

    /// <summary>
    /// 显示最高分面板并更新文本
    /// </summary>
    private void ToggleHighScorePanel()
    {
        if (highScorePanel == null) return;

        highScorePanel.SetActive(true);
        UpdateHighScoreText();
    }

    /// <summary>
    /// 隐藏最高分面板
    /// </summary>
    private void HideHighScorePanel()
    {
        if (highScorePanel != null)
        {
            highScorePanel.SetActive(false);
        }
    }

    /// <summary>
    /// 更新最高分文本显示
    /// </summary>
    private void UpdateHighScoreText()
    {
        if (highScoreText == null) return;

        if (DataManager.Instance != null)
        {
            highScoreText.text = $"High Score: {DataManager.Instance.highScore}";
        }
        else
        {
            highScoreText.text = "High Score: 0";
        }
    }
}
