using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundssCheck : MonoBehaviour
{
    private float screenLeft;
    private float screenRight;
    private float screenTop;
    private float screenBottom;

    private Camera mainCamera;
    private Rigidbody rb;
    private float lastDepth;

    private enum BoundsMode { Clamp, Wrap }

    [SerializeField] private bool autoDetectByTag = true;
    [SerializeField] private float wrapPadding = 0.5f;
    [SerializeField] private BoundsMode mode = BoundsMode.Clamp;

    private void Awake()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();

        if (autoDetectByTag)
        {
            // Player：夹紧边界；Asteroid（包含小陨石）：穿越边界从另一侧出现
            if (CompareTag("Player")) mode = BoundsMode.Clamp;
            else if (CompareTag("Asteroid")) mode = BoundsMode.Wrap;
            else mode = BoundsMode.Clamp;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        RecalculateBounds();
    }

    private void RecalculateBounds()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null) return;

        // 关键：用“玩家所在深度”的 z 来做 ScreenToWorldPoint
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

    private void FixedUpdate()
    {
        // 若摄像机/分辨率变化或玩家深度变化，动态重算边界，避免被 clamp 成一个点
        var depthNow = mainCamera != null ? mainCamera.WorldToScreenPoint(transform.position).z : lastDepth;
        if (mainCamera == null || Mathf.Abs(depthNow - lastDepth) > 0.01f)
        {
            RecalculateBounds();
        }

        var pos = rb != null ? rb.position : transform.position;

        if (mode == BoundsMode.Clamp)
        {
            pos.x = Mathf.Clamp(pos.x, screenLeft, screenRight);
            pos.y = Mathf.Clamp(pos.y, screenBottom, screenTop);
        }
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
