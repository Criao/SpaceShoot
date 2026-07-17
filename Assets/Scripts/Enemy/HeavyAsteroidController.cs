using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 重型陨石控制器 - 速度慢，分裂多，血量更高（可选）
/// </summary>
public class HeavyAsteroidController : AsterodController
{
    /// <summary>
    /// 初始化重型陨石的速度参数
    /// </summary>
    protected override void Start()
    {
        movementSpeed = 0.8f; // 较慢的移动速度
        rotateSpeed = 2f; // 较慢的旋转速度
        base.Start();
    }

    /// <summary>
    /// 创建分裂的小陨石（数量较多）
    /// </summary>
    protected override void CreateLittleAsteroids()
    {
        if (littleAsteroidPrefab == null) return;

        int randomNumber = Random.Range(4, 7); // 分裂4-6个小陨石
        int lifeDropIndex = Random.Range(0, randomNumber);

        for (int i = 0; i < randomNumber; i++)
        {
            GameObject littleAsteroid = Instantiate(littleAsteroidPrefab, transform.position, littleAsteroidPrefab.transform.rotation);
            if (i == lifeDropIndex)
            {
                var littleController = littleAsteroid.GetComponent<LittleAsteroidController>();
                if (littleController != null)
                {
                    littleController.SetLifePowerUpDrop(true);
                }
            }
        }

        if (SpawnManager.Instance != null)
        {
            SpawnManager.Instance.RegisterSmallAsteroids(randomNumber);
        }
    }

    /// <summary>
    /// 获取击毁重型陨石的分数值
    /// </summary>
    protected override int GetScoreValue()
    {
        return 15; // 重型陨石给中等分数
    }
}
