using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 边界检查控制器 - 限制游戏对象在屏幕边界内或实现边界穿越效果
/// </summary>
public class BoundssCheck : MonoBehaviour
{
    private float screenLeft; // 屏幕左边界
    private float screenRight; // 屏幕右边界
    private float screenTop; // 屏幕上边界
    private float screenBottom; // 屏幕下边界

    private Camera mainCamera; // 主摄像机引用
    private Rigidbody rb; // 刚体组件
    private float lastDepth; // 上次计算的深度值

    /// <summary>
    /// 边界模式枚举
    /// </summary>
    private enum BoundsMode { Clamp, Wrap }

    [SerializeField] private bool autoDetectByTag = true; // 是否根据标签自动检测模式
    [SerializeField] private float wrapPadding = 0.5f; // 穿越模式的边界缓冲距离
    [SerializeField] private BoundsMode mode = BoundsMode.Clamp; // 边界模式

    /// <summary>
    /// 初始化组件并根据标签设置边界模式
    /// </summary>
    private void Awake()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();

        if (autoDetectByTag)
        {
            // Player：夹紧边界；Asteroid（包含小陨石）：穿越边界从另一侧出现
            if (CompareTag(“Player”)) mode = BoundsMode.Clamp;
            else if (CompareTag(“Asteroid”)) mode = BoundsMode.Wrap;
            else mode = BoundsMode.Clamp;
        }
    }

    /// <summary>
    /// 游戏开始时计算屏幕边界
    /// </summary>
    void Start()
    {
        RecalculateBounds();
    }

    /// <summary>
    /// 重新计算屏幕边界（根据当前摄像机和对象深度）
    /// </summary>
    private void RecalculateBounds()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null) return;

        // 关键：用”玩家所在深度”的 z 来做 ScreenToWorldPoint
        lastDepth = mainCamera.WorldToScreenPoint(transform.position).z;
        if (lastDepth <= 0.001f) lastDepth = 10f; // 兜底：避免极端情况下得到 0

        var left = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, lastDepth));
        var right = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, lastDepth));
        var top = mainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height, lastDepth));
        var bottom = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, lastDepth));

        screenLeft = left.x;
        screenRight = right.x;
        screenTop = top.y;
        screenBottom = bottom.y;
    }

    /// <summary>
    /// 固定更新 - 应用边界限制或穿越效果
    /// </summary>
    private void FixedUpdate()
    {
        // 若摄像机/分辨率变化或玩家深度变化，动态重算边界，避免被 clamp 成一个点
        var depthNow = mainCamera != null ? mainCamera.WorldToScreenPoint(transform.position).z : lastDepth;
        if (mainCamera == null || Mathf.Abs(depthNow - lastDepth) > 0.01f)
        {
            RecalculateBounds();
        }

        var pos = rb != null ? rb.position : transform.position;

        // 夹紧模式：限制在边界内
        if (mode == BoundsMode.Clamp)
        {
            pos.x = Mathf.Clamp(pos.x, screenLeft, screenRight);
            pos.y = Mathf.Clamp(pos.y, screenBottom, screenTop);
        }
        // 穿越模式：从一侧出去从另一侧进来
        else
        {
            float left = screenLeft - wrapPadding;
            float right = screenRight + wrapPadding;
            float bottom = screenBottom - wrapPadding;
            float top = screenTop + wrapPadding;

            if (pos.x < left) pos.x = right;
            else if (pos.x > right) pos.x = left;

            if (pos.y < bottom) pos.y = top;
            else if (pos.y > top) pos.y = bottom;
        }

        // 应用位置变化
        if (rb != null && !rb.isKinematic)
        {
            rb.MovePosition(pos);
        }
        else
        {
            transform.position = pos;
        }
    }
}
