using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏说明对话框 - 管理信息页和操作页的切换
/// </summary>
public class HowToPlayDialog : DialogBase
{
    [SerializeField] private GameObject infoPage; // 信息页面
    [SerializeField] private GameObject controlPage; // 操作页面
    [SerializeField] private Button infoButton; // 信息按钮
    [SerializeField] private Button controlButton; // 操作按钮

    /// <summary>
    /// 初始化时显示信息页
    /// </summary>
    void Start()
    {
        infoPage.SetActive(true);
        controlPage.SetActive(false);

        infoButton.interactable = false;
        controlButton.interactable = true;
    }

    /// <summary>
    /// 切换到操作页面
    /// </summary>
    public void ControlButtonClicked()
    {
        infoPage.SetActive(false);
        controlPage.SetActive(true);

        infoButton.interactable = true;
        controlButton.interactable = false;
    }

    /// <summary>
    /// 切换到信息页面
    /// </summary>
    public void InfoButtonClicked()
    {
        infoPage.SetActive(true);
        controlPage.SetActive(false);

        infoButton.interactable = false;
        controlButton.interactable = true;
    }




}
