using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对话框基类 - 提供对话框的打开、关闭和动画功能
/// </summary>
public class DialogBase : MonoBehaviour
{
    private Animator animator; // 动画控制器

    /// <summary>
    /// 初始化动画器，设置为非缩放时间模式
    /// </summary>
    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
    }

    /// <summary>
    /// 打开对话框（虚方法，子类可以重写）
    /// </summary>
    public virtual void Open()
    {
        this.gameObject.SetActive(true);
    }

    /// <summary>
    /// 关闭对话框（播放关闭动画后隐藏）
    /// </summary>
    public void Close()
    {
        if (animator != null)
        {
            animator.SetTrigger("Close");
        }
        StartCoroutine(HideDialog());

    }

    /// <summary>
    /// 协程：等待动画播放完毕后隐藏对话框
    /// </summary>
    private IEnumerator HideDialog()
    {
        yield return new WaitForSecondsRealtime(0.4f);
        this.gameObject.SetActive(false);
    }
}
