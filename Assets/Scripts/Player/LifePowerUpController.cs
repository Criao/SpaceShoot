using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 生命道具控制器 - 玩家拾取后增加一条生命
/// </summary>
public class LifePowerUpController : MonoBehaviour
{
    private GameManager gameManager; // 游戏管理器引用
    private float lifeTime = 10f; // 道具存活时间

    /// <summary>
    /// 初始化时移除旧的PowerUpController组件并设置刚体
    /// </summary>
    private void Awake()
    {
        // 移除旧的护盾道具控制器（避免冲突）
        foreach (var oldController in GetComponentsInChildren<PowerUpController>())
        {
            Destroy(oldController);
        }

        // 设置刚体为静止状态
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    /// <summary>
    /// 开始时查找游戏管理器并设置销毁计时
    /// </summary>
    private void Start()
    {
        // 通过标签查找游戏管理器
        var gmGo = GameObject.FindGameObjectWithTag("GameController");
        if (gmGo != null)
        {
            gameManager = gmGo.GetComponent<GameManager>();
        }

        // 如果通过标签没找到，使用FindObjectOfType查找
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        // 在指定时间后自动销毁
        Destroy(gameObject, lifeTime);
    }

    /// <summary>
    /// 触发器碰撞检测
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        TryPickup(other.gameObject);
    }

    /// <summary>
    /// 物理碰撞检测
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        TryPickup(collision.gameObject);
    }

    /// <summary>
    /// 尝试拾取道具 - 检测是否为玩家并增加生命
    /// </summary>
    private void TryPickup(GameObject other)
    {
        if (other == null) return;

        // 检查是否为玩家
        var playerController = other.GetComponentInParent<PlayerController>();
        if (playerController == null) return;

        // 增加生命值
        if (gameManager != null)
        {
            gameManager.AddLife(1);
        }
        Destroy(gameObject);
    }
}
