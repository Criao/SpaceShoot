using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 快速陨石控制器 - 速度快，分裂少，给更多分数
/// </summary>
public class FastAsteroidController : AsterodController
{
    /// <summary>
    /// 初始化快速陨石的速度参数
    /// </summary>
    protected override void Start()
    {
        movementSpeed = 3.0f; // 较快的移动速度
        rotateSpeed = 8f; // 较快的旋转速度
        base.Start();
    }

    /// <summary>
    /// 创建分裂的小陨石（数量较少）
    /// </summary>
    protected override void CreateLittleAsteroids()
    {
        int randomNumber = Random.Range(1, 3); // 分裂1-2个小陨石
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
    /// 获取击毁快速陨石的分数值
    /// </summary>
    protected override int GetScoreValue()
    {
        return 20; // 快速陨石给更多分数
    }
}
