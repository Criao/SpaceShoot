using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// 陨石基类控制器 - 处理陨石的移动、旋转、碰撞和分裂逻辑
/// </summary>
public class AsterodController : MonoBehaviour
{
    [SerializeField] protected float movementSpeed = 1.5f; // 移动速度
    [SerializeField] protected float rotateSpeed  = 3f; // 旋转速度
    [SerializeField] protected GameObject littleAsteroidPrefab; // 小陨石预制体
    private GameManager gameManager; // 游戏管理器引用
    private Vector2 randomDirection; // 随机移动方向
    private Vector3 randomAngle; // 随机旋转角度
    private bool destroyed; // 是否已被销毁（防止重复触发）

    /// <summary>
    /// 初始化陨石的随机方向和旋转
    /// </summary>
    protected virtual void Start()
    {
        randomDirection = SetRandomDirection();
        randomAngle = SetRandomAngle();
        // 注意：当生命归零时 GameManager 会把 Player SetActive(false)，此时 FindGameObjectWithTag 不一定能找到
        // 所以这里不再强依赖 Player 对象，避免 NullReferenceException
        var gmGo = GameObject.FindGameObjectWithTag("GameController");
        gameManager = gmGo != null ? gmGo.GetComponent<GameManager>() : null;
    }

    /// <summary>
    /// 每帧更新陨石的位置和旋转
    /// </summary>
    private void Update()
    {
        transform.Translate(randomDirection * (movementSpeed * Time.deltaTime),Space.World);
        transform.Rotate(randomAngle * (rotateSpeed *Time.deltaTime));
    }

    /// <summary>
    /// 生成随机的移动方向（单位圆内的随机方向）
    /// </summary>
    private Vector2 SetRandomDirection()
    {
        return Random.insideUnitCircle.normalized;
    }

    /// <summary>
    /// 生成随机的旋转角度
    /// </summary>
    private Vector3 SetRandomAngle()
    {
        float x = Random.Range(-1f,1f);
        float y = Random.Range(-1f,1f);
        float z = Random.Range(-1f,1f);
        return new Vector3(x, y, z);
    }

    /// <summary>
    /// 触发器碰撞检测 - 处理与玩家和子弹的碰撞
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        bool isLittle = this is LittleAsteroidController;

        // 与玩家碰撞
        if (other.CompareTag("Player"))
        {
            if (gameManager != null && gameManager.IsGameOver) return;
            if (destroyed) return;
            destroyed = true;
            var player = other.GetComponent<PlayerController>() ?? other.GetComponentInParent<PlayerController>();
            bool shieldBlocked = player != null && player.TryConsumeShield();
            SpawnDroppedPowerUp();
            CreateLittleAsteroids();
            if (!shieldBlocked && gameManager != null) gameManager.RemoveLife(1);
            if (SpawnManager.Instance != null) SpawnManager.Instance.AsteroidDestroyed(isLittle);
            Destroy(gameObject);
        }
        // 与子弹碰撞
        else if (other.CompareTag("Bullet"))
        {
            gameManager?.AddScore(GetScoreValue());
            HandleBulletHit(other.gameObject);
        }


    }

    /// <summary>
    /// 处理子弹击中逻辑
    /// </summary>
    public void HandleBulletHit(GameObject bullet)
    {
        if (gameManager != null && gameManager.IsGameOver) return;
        if (destroyed) return;
        destroyed = true;

        SpawnDroppedPowerUp();
        bool isLittle = this is LittleAsteroidController;
        CreateLittleAsteroids();
        if (SpawnManager.Instance != null) SpawnManager.Instance.AsteroidDestroyed(isLittle);
        if (bullet != null) Destroy(bullet);
        Destroy(gameObject);
    }

    /// <summary>
    /// 生成掉落的道具（由子类重写）
    /// </summary>
    protected virtual void SpawnDroppedPowerUp()
    {
    }

    /// <summary>
    /// 创建分裂的小陨石
    /// </summary>
    protected virtual void CreateLittleAsteroids()
    {
        if (littleAsteroidPrefab == null) return;

        int randomNumber = Random.Range(2,5);
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
    /// 获取击毁该陨石的分数值，子类可以重写
    /// </summary>
    protected virtual int GetScoreValue()
    {
        return 10; // 普通陨石默认10分
    }
}
