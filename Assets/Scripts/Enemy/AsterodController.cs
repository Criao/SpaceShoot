using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class AsterodController : MonoBehaviour
{
    [SerializeField] protected float movementSpeed = 1.5f;
    [SerializeField] protected float rotateSpeed  = 3f;
    [SerializeField] protected GameObject littleAsteroidPrefab;
     private GameManager gameManager;
    private Vector2 randomDirection;
    private Vector3 randomAngle;
    private bool destroyed;
    protected  virtual void Start()
    {
        randomDirection = SetRandomDirection();
        randomAngle = SetRandomAngle();
        // 注意：当生命归零时 GameManager 会把 Player SetActive(false)，此时 FindGameObjectWithTag 不一定能找到
        // 所以这里不再强依赖 Player 对象，避免 NullReferenceException
        var gmGo = GameObject.FindGameObjectWithTag("GameController");
        gameManager = gmGo != null ? gmGo.GetComponent<GameManager>() : null;
    }
    private void Update()
    {
        transform.Translate(randomDirection * (movementSpeed * Time.deltaTime),Space.World);
        transform.Rotate(randomAngle * (rotateSpeed *Time.deltaTime));
    }
    private Vector2 SetRandomDirection()
    {
        return Random.insideUnitCircle.normalized;    
    }
    private Vector3 SetRandomAngle()
    {
        float x = Random.Range(-1f,1f);
        float y = Random.Range(-1f,1f);
        float z = Random.Range(-1f,1f);
        return new Vector3(x, y, z);
    }
    private void OnTriggerEnter(Collider other)
    {
        bool isLittle = this is LittleAsteroidController;

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
        else if (other.CompareTag("Bullet"))
        {
            gameManager?.AddScore(GetScoreValue());
            HandleBulletHit(other.gameObject);
        }
       

    }

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

    protected virtual void SpawnDroppedPowerUp()
    {
    }

    protected virtual void CreateLittleAsteroids()
    {
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
