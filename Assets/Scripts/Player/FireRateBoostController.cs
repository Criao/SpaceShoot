using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 射速增强道具控制器 - 玩家拾取后获得双倍射速
/// </summary>
public class FireRateBoostController : MonoBehaviour
{
    private float timeForDie = 5f; // 道具存活时间
    [SerializeField] [Tooltip("与玩家中心距离小于此值即视为吃到")]
    private float pickupRadius = 1.25f; // 拾取半径

    private bool consumed; // 是否已被拾取
    private Transform playerTransform; // 玩家Transform引用

    /// <summary>
    /// 开始时查找玩家并启动销毁计时
    /// </summary>
    private void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) playerTransform = p.transform;
        StartCoroutine(WaitToDie());
    }

    /// <summary>
    /// 固定更新 - 检测与玩家的距离（双Trigger时的备用方案）
    /// </summary>
    private void FixedUpdate()
    {
        if (consumed || playerTransform == null) return;

        // 距离检测作为碰撞检测的补充
        if (Vector3.Distance(transform.position, playerTransform.position) <= pickupRadius)
        {
            var pc = playerTransform.GetComponent<PlayerController>();
            TryConsume(pc);
        }
    }

    /// <summary>
    /// 尝试消耗道具 - 激活玩家的射速增强能力
    /// </summary>
    public void TryConsume(PlayerController player)
    {
        if (consumed || player == null) return;
        consumed = true;
        player.ActivateFireRateBoost();
        Destroy(gameObject);
    }

    /// <summary>
    /// 触发器碰撞检测 - 检测是否为玩家
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var pc = other.GetComponent<PlayerController>()
                   ?? other.GetComponentInParent<PlayerController>();
        TryConsume(pc);
    }

    /// <summary>
    /// 协程：在指定时间后销毁道具
    /// </summary>
    private IEnumerator WaitToDie()
    {
        yield return new WaitForSeconds(timeForDie);
        if (!consumed) Destroy(gameObject);
    }
}
