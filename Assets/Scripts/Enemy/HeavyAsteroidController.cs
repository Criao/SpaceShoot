using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 重型陨石：速度慢，分裂多，血量更高（可选）
/// </summary>
public class HeavyAsteroidController : AsterodController
{
    protected override void Start()
    {
        movementSpeed = 0.8f;
        rotateSpeed = 2f;
        base.Start();
    }

    protected override void CreateLittleAsteroids()
    {
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

    protected override int GetScoreValue()
    {
        return 15; // 重型陨石给中等分数
    }
}
