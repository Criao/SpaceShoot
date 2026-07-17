using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 生成管理器 - 管理陨石和道具的生成，以及波次系统
/// </summary>
public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject shieldPowerup; // 护盾道具预制体
    [SerializeField] private GameObject LifePowerUP; // 生命道具预制体
    [SerializeField] private GameObject fireRateBoostPowerup; // 射速增强道具预制体
    [SerializeField] private GameObject tripleShotPowerup; // 三连发道具预制体
    [SerializeField] private GameObject asteroidPrefab; // 普通陨石预制体
    [SerializeField] private GameObject fastAsteroidPrefab; // 快速陨石预制体
    [SerializeField] private GameObject heavyAsteroidPrefab; // 重型陨石预制体
    private float spawnRangex = 9; // X轴生成范围
    private float spawnRangey = 4; // Y轴生成范围

    private int wavenumber = 1; // 当前波次
    public static SpawnManager Instance { get; private set; } // 单例实例

    private int bigAsteroidAlive; // 追踪当前波次中大陨石的存活数
    private int smallAsteroidAlive; // 追踪当前波次中小陨石的存活数

    private GameManager gameManager; // 游戏管理器引用

    /// <summary>
    /// 单例模式初始化
    /// </summary>
    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 游戏开始时初始化生成逻辑
    /// </summary>
    private void Start()
    {
        var gmGo = GameObject.FindGameObjectWithTag("GameController");
        gameManager = gmGo != null ? gmGo.GetComponent<GameManager>() : null;

        // 定时生成各种道具
        InvokeRepeating("SpawnShieldPowerUp", 3f, 15f);
        InvokeRepeating("SpawnFireRateBoost", 10f, 20f);
        InvokeRepeating("SpawnTripleShot", 15f, 25f);
        SpawnAsteroid(wavenumber);
    }

    /// <summary>
    /// 生成护盾道具
    /// </summary>
    private void SpawnShieldPowerUp()
    {
        if (gameManager != null && gameManager.IsGameOver) return;
        if (shieldPowerup == null) return;
        Instantiate(shieldPowerup, GeneratePosition(), shieldPowerup.transform.rotation);
    }

    /// <summary>
    /// 生成射速增强道具
    /// </summary>
    private void SpawnFireRateBoost()
    {
        if (gameManager != null && gameManager.IsGameOver) return;
        if (fireRateBoostPowerup == null) return;
        Instantiate(fireRateBoostPowerup, GeneratePosition(), fireRateBoostPowerup.transform.rotation);
    }

    /// <summary>
    /// 生成三连发道具
    /// </summary>
    private void SpawnTripleShot()
    {
        if (gameManager != null && gameManager.IsGameOver) return;
        if (tripleShotPowerup == null) return;
        Instantiate(tripleShotPowerup, GeneratePosition(), tripleShotPowerup.transform.rotation);
    }

    /// <summary>
    /// 在指定位置生成生命道具
    /// </summary>
    public void SpawnLifePowerUp(Vector3 position)
    {
        if (gameManager != null && gameManager.IsGameOver) return;
        if (LifePowerUP == null) return;

        var lifePowerUpInstance = Instantiate(LifePowerUP, position, LifePowerUP.transform.rotation);
        // 移除旧的PowerUpController组件
        foreach (var oldController in lifePowerUpInstance.GetComponentsInChildren<PowerUpController>())
        {
            Destroy(oldController);
        }

        // 添加LifePowerUpController组件
        if (lifePowerUpInstance.GetComponentInChildren<LifePowerUpController>() == null)
        {
            lifePowerUpInstance.AddComponent<LifePowerUpController>();
        }
    }

    /// <summary>
    /// 生成指定数量的陨石
    /// </summary>
    private void SpawnAsteroid(int asteroidToSpawn)
    {
        if (gameManager != null && gameManager.IsGameOver) return;
        bigAsteroidAlive = asteroidToSpawn;
        smallAsteroidAlive = 0;
        for (int i = 0; i < asteroidToSpawn; i++)
        {
            // 随机选择陨石类型
            GameObject asteroidToInstantiate = GetRandomAsteroidType();
            if (asteroidToInstantiate == null) continue;
            Instantiate(asteroidToInstantiate, GeneratePosition(), asteroidToInstantiate.transform.rotation);
        }
    }

    /// <summary>
    /// 随机选择陨石类型：60%普通，25%快速，15%重型
    /// </summary>
    private GameObject GetRandomAsteroidType()
    {
        float random = Random.value;

        if (random < 0.6f)
        {
            // 60% 概率生成普通陨石
            return asteroidPrefab;
        }
        else if (random < 0.85f)
        {
            // 25% 概率生成快速陨石
            return fastAsteroidPrefab != null ? fastAsteroidPrefab : asteroidPrefab;
        }
        else
        {
            // 15% 概率生成重型陨石
            return heavyAsteroidPrefab != null ? heavyAsteroidPrefab : asteroidPrefab;
        }
    }

    /// <summary>
    /// 注册新生成的小陨石数量
    /// </summary>
    public void RegisterSmallAsteroids(int count)
    {
        if (count <= 0) return;
        smallAsteroidAlive += count;
    }

    /// <summary>
    /// 陨石被销毁时调用
    /// isLittle=true：小陨石被销毁 -> 只减少小陨石存活数
    /// isLittle=false：大陨石被销毁 -> 只减少大陨石存活数
    /// 当所有陨石都被清空后，进入下一波
    /// </summary>
    public void AsteroidDestroyed(bool isLittle)
    {
        if (gameManager != null && gameManager.IsGameOver) return;

        if (isLittle)
        {
            smallAsteroidAlive = Mathf.Max(0, smallAsteroidAlive - 1);
            TrySpawnNextWave();
            return;
        }

        bigAsteroidAlive = Mathf.Max(0, bigAsteroidAlive - 1);
        TrySpawnNextWave();
    }

    /// <summary>
    /// 尝试生成下一波陨石
    /// </summary>
    private void TrySpawnNextWave()
    {
        if (gameManager != null && gameManager.IsGameOver) return;
        if (bigAsteroidAlive > 0 || smallAsteroidAlive > 0) return;

        wavenumber++;
        SpawnAsteroid(wavenumber);
    }

    /// <summary>
    /// 生成随机位置（避免与玩家重叠）
    /// </summary>
    private Vector3 GeneratePosition()
    {
        float spawnPosx = Random.Range(-spawnRangex, spawnRangex);
        float spawnPosy = Random.Range(-spawnRangey, spawnRangey);
        Vector3 randomPos = new Vector3(spawnPosx, spawnPosy, 0);
        Collider[] hitColliders = Physics.OverlapSphere(randomPos, 3.0f);
        if (hitColliders != null)
        {
            foreach (Collider hit in hitColliders)
            {
                if (hit.CompareTag("Player"))
                {
                    // 递归判断是否重叠，从而找到合适位置
                    return GeneratePosition();
                }
            }
        }

        return randomPos;
    }
}
