using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject shieldPowerup;
    [SerializeField] private GameObject LifePowerUP;
    [SerializeField] private GameObject fireRateBoostPowerup;
    [SerializeField] private GameObject tripleShotPowerup;
    [SerializeField] private GameObject asteroidPrefab;
    [SerializeField] private GameObject fastAsteroidPrefab;
    [SerializeField] private GameObject heavyAsteroidPrefab;
    private float spawnRangex = 9;
    private float spawnRangey = 4;

    private int wavenumber = 1;
    public static SpawnManager Instance { get; private set; }

    // 追踪当前波次中大陨石和小陨石的存活数。
    private int bigAsteroidAlive;
    private int smallAsteroidAlive;

    private GameManager gameManager;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        var gmGo = GameObject.FindGameObjectWithTag("GameController");
        gameManager = gmGo != null ? gmGo.GetComponent<GameManager>() : null;

        InvokeRepeating("SpawnShieldPowerUp", 3f, 15f);
        InvokeRepeating("SpawnFireRateBoost", 10f, 20f);
        InvokeRepeating("SpawnTripleShot", 15f, 25f);
        SpawnAsteroid(wavenumber);
    }

    private void SpawnShieldPowerUp()
    {
        if (gameManager != null && gameManager.IsGameOver) return;
        Instantiate(shieldPowerup, GeneratePosition(), shieldPowerup.transform.rotation);
    }

    private void SpawnFireRateBoost()
    {
        if (gameManager != null && gameManager.IsGameOver) return;
        if (fireRateBoostPowerup == null) return;
        Instantiate(fireRateBoostPowerup, GeneratePosition(), fireRateBoostPowerup.transform.rotation);
    }

    private void SpawnTripleShot()
    {
        if (gameManager != null && gameManager.IsGameOver) return;
        if (tripleShotPowerup == null) return;
        Instantiate(tripleShotPowerup, GeneratePosition(), tripleShotPowerup.transform.rotation);
    }

    public void SpawnLifePowerUp(Vector3 position)
    {
        if (gameManager != null && gameManager.IsGameOver) return;
        if (LifePowerUP == null) return;

        var lifePowerUpInstance = Instantiate(LifePowerUP, position, LifePowerUP.transform.rotation);
        foreach (var oldController in lifePowerUpInstance.GetComponentsInChildren<PowerUpController>())
        {
            Destroy(oldController);
        }

        if (lifePowerUpInstance.GetComponentInChildren<LifePowerUpController>() == null)
        {
            lifePowerUpInstance.AddComponent<LifePowerUpController>();
        }
    }

    private void SpawnAsteroid(int asteroidToSpawn)
    {
        if (gameManager != null && gameManager.IsGameOver) return;
        bigAsteroidAlive = asteroidToSpawn;
        smallAsteroidAlive = 0;
        for (int i = 0; i < asteroidToSpawn; i++)
        {
            // 随机选择陨石类型
            GameObject asteroidToInstantiate = GetRandomAsteroidType();
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

    public void RegisterSmallAsteroids(int count)
    {
        if (count <= 0) return;
        smallAsteroidAlive += count;
    }

    /// <summary>
    /// isLittle=true：小陨石被销毁 -> 只减少小陨石存活数；当当前波次的大陨石和小陨石都清空后，进入下一波
    /// isLittle=false：父类/大陨石被销毁 -> 只减少大陨石存活数，并生成小陨石；当大陨石清空且没有剩余小陨石时，进入下一波
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

    private void TrySpawnNextWave()
    {
        if (gameManager != null && gameManager.IsGameOver) return;
        if (bigAsteroidAlive > 0 || smallAsteroidAlive > 0) return;

        wavenumber++;
        SpawnAsteroid(wavenumber);
    }
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
                    return GeneratePosition();
                }
            }
        }

        //递归判断是否重叠，从而找到合适位置
        return randomPos;
    }
}
