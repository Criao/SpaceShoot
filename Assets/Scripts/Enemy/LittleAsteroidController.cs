using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 小陨石控制器 - 由大陨石分裂产生，不再继续分裂
/// </summary>
public class LittleAsteroidController : AsterodController
{
    private bool dropLifePowerUp; // 是否掉落生命道具

    /// <summary>
    /// 初始化小陨石的速度参数
    /// </summary>
    protected override void Start()
    {
        movementSpeed = 2f; // 中等移动速度
        rotateSpeed = 5f; // 中等旋转速度

        base.Start();
    }

    /// <summary>
    /// 设置是否掉落生命道具
    /// </summary>
    public void SetLifePowerUpDrop(bool value)
    {
        dropLifePowerUp = value;
    }

    /// <summary>
    /// 生成掉落的生命道具
    /// </summary>
    protected override void SpawnDroppedPowerUp()
    {
        if (!dropLifePowerUp) return;
        if (SpawnManager.Instance == null) return;
        SpawnManager.Instance.SpawnLifePowerUp(transform.position);
    }

    /// <summary>
    /// 小陨石不再生成更小的陨石
    /// </summary>
    protected override void CreateLittleAsteroids()
    {
        // 小陨石不再生成更小陨石
    }
}
