using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 快速陨石：速度快，分裂少，给更多分数
/// </summary>
public class FastAsteroidController : AsterodController
{
    protected override void Start()
    {
        movementSpeed = 3.0f;
        rotateSpeed = 8f;
        base.Start();
    }

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

    protected override int GetScoreValue()
    {
        return 20; // 快速陨石给更多分数
    }
}
