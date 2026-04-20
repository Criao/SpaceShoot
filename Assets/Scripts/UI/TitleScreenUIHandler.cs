using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 标题界面UI处理器 - 管理主菜单的按钮功能
/// </summary>
public class TitleScreenUIHandler : MonoBehaviour
{
    [SerializeField] private HowToPlayDialog howToPlayDialog; // 游戏说明对话框

    /// <summary>
    /// 开始新游戏
    /// </summary>
    public void StartNewScene()
    {
        Debug.Log("按钮被点击了");
        SceneManager.LoadScene(1);
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

}
