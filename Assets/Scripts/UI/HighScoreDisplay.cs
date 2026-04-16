using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// 按Tab键显示/隐藏最高分面板
/// </summary>
public class HighScoreDisplay : MonoBehaviour
{
    [SerializeField] private GameObject highScorePanel;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private KeyCode toggleKey = KeyCode.Tab;

    private void Start()
    {
        // 初始时隐藏面板
        if (highScorePanel != null)
        {
            highScorePanel.SetActive(false);
        }
    }

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

    private void ToggleHighScorePanel()
    {
        if (highScorePanel == null) return;

        highScorePanel.SetActive(true);
        UpdateHighScoreText();
    }

    private void HideHighScorePanel()
    {
        if (highScorePanel != null)
        {
            highScorePanel.SetActive(false);
        }
    }

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
