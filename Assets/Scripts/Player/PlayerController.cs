using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家控制器 - 负责玩家飞船的移动、射击和道具效果管理
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera; // 主摄像机引用
    private Rigidbody playerRb; // 玩家刚体组件

    [Header("Movement")]
    [SerializeField] private float acceleration = 25f; // 加速度
    [SerializeField] private float maxSpeed = 6f; // 最大速度
    [SerializeField] private float boostMultiplier = 2f; // 加速倍率
    [SerializeField] private float boostedMaxSpeed = 10f; // 加速时的最大速度

    [SerializeField] private GameObject bulletPrefab; // 子弹预制体
    [SerializeField] private GameObject bulletAnchor; // 子弹发射点
    private const float MinFireCooldown = 0.5f;
    private const float TripleShotSpreadAngle = 15f;

    [SerializeField, Min(MinFireCooldown)] private float fireCooldown = MinFireCooldown; // 射击冷却时间
    [SerializeField] private GameObject shield; // 护盾对象
    [SerializeField] private GameManager gameManager; // 游戏管理器引用
    [SerializeField] private ParticleSystem engineTrail; // 引擎尾焰粒子效果
    [SerializeField] private float shieldDuration = 5f; // 护盾持续时间
    [SerializeField] private float fireRateBoostDuration = 5f; // 射速增强持续时间
    [SerializeField] private float tripleShotDuration = 5f; // 三连发持续时间

    private float fireTimer; // 射击计时器
    private float shieldTimer; // 护盾计时器
    private float fireRateBoostTimer; // 射速增强计时器
    private float tripleShotTimer; // 三连发计时器
    private bool hasFireRateBoost; // 是否拥有射速增强
    private bool hasTripleShot; // 是否拥有三连发

    private bool IsPaused()
    {
        return gameManager != null && gameManager.isPause;
    }

    private void ResolveRuntimeReferences()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (playerRb == null) playerRb = GetComponent<Rigidbody>();

        if (gameManager == null)
        {
            var gameManagerObject = GameObject.FindGameObjectWithTag("GameController");
            if (gameManagerObject != null)
            {
                gameManager = gameManagerObject.GetComponent<GameManager>();
            }

            if (gameManager == null)
            {
                gameManager = FindObjectOfType<GameManager>();
            }
        }
    }

    /// <summary>
    /// 初始化组件引用
    /// </summary>
    private void Awake()
    {
        // 防止 Inspector 忘记拖引用导致 FixedUpdate 报错、整段移动逻辑不执行
        ResolveRuntimeReferences();

        // 设置引擎尾焰粒子循环播放
        if (engineTrail != null)
        {
            var main = engineTrail.main;
            main.loop = true;
        }
    }

    /// <summary>
    /// 游戏开始时的初始化
    /// </summary>
    private void Start()
    {
        // 兜底（Awake 已赋值时这里不重复）
        ResolveRuntimeReferences();
    }

    /// <summary>
    /// 每帧更新 - 处理输入、射击、粒子效果和道具计时
    /// </summary>
    private void Update()
    {
        ResolveRuntimeReferences();
        if (!IsPaused())
        {
            if (mainCamera == null) return;

            // 让飞船朝向鼠标位置
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f));
            transform.LookAt(mousePos, Vector3.back);

            // 计算当前射速（如果有射速增强则减半冷却时间）
            float currentFireCooldown = Mathf.Max(MinFireCooldown, hasFireRateBoost ? fireCooldown * 0.5f : fireCooldown);

            // 射击逻辑
            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0f && Input.GetMouseButton(0))
            {
                FireBullet();
                fireTimer = currentFireCooldown;
            }

            // 引擎尾焰粒子效果控制
            if (engineTrail != null)
            {
                if (Input.GetKey(KeyCode.W))
                {
                    if (!engineTrail.isPlaying)
                        engineTrail.Play();
                }
                else if (engineTrail.isPlaying)
                {
                    engineTrail.Stop();
                }
            }

            // 护盾计时器
            if (shield != null && shield.activeSelf)
            {
                shieldTimer -= Time.deltaTime;
                if (shieldTimer <= 0f)
                {
                    shield.SetActive(false);
                    shieldTimer = 0f;
                }
            }

            // 火力增强计时器
            if (hasFireRateBoost)
            {
                fireRateBoostTimer -= Time.deltaTime;
                if (fireRateBoostTimer <= 0f)
                {
                    hasFireRateBoost = false;
                    fireRateBoostTimer = 0f;
                }
            }

            // 三连发计时器
            if (hasTripleShot)
            {
                tripleShotTimer -= Time.deltaTime;
                if (tripleShotTimer <= 0f)
                {
                    hasTripleShot = false;
                    tripleShotTimer = 0f;
                }
            }
        }

    }

    /// <summary>
    /// 固定时间步更新 - 处理物理移动
    /// </summary>
    private void FixedUpdate()
    {
        ResolveRuntimeReferences();
        if (!IsPaused())
        {
            if (playerRb == null)
            {
                Debug.LogError("PlayerController: Rigidbody 未找到，无法移动。请确认 Player 物体上挂了 Rigidbody。");
                return;
            }
            if (mainCamera == null)
            {
                Debug.LogError("PlayerController: mainCamera 未赋值且找不到 Camera.main，无法计算朝向。请在 Inspector 里拖 Main Camera。");
                return;
            }

            // 获取输入
            float sideInput = Input.GetAxis("Horizontal");
            float forwardInput = Input.GetAxis("Vertical");

            // 严格只在世界坐标 X/Y 平面移动（不参与 Z 轴）
            bool boosting = Input.GetKey(KeyCode.LeftShift);
            float currentAcceleration = acceleration * (boosting ? boostMultiplier : 1f);
            float currentMaxSpeed = boosting ? boostedMaxSpeed : maxSpeed;

            // 施加加速度
            Vector3 accelWorld = new Vector3(sideInput, forwardInput, 0f) * currentAcceleration;
            playerRb.AddForce(accelWorld, ForceMode.Acceleration);

            // 速度上限（只限制 XY 平面）
            Vector3 v = playerRb.velocity;
            Vector3 vXY = new Vector3(v.x, v.y, 0f);
            float speed = vXY.magnitude;
            if (speed > currentMaxSpeed)
            {
                Vector3 limited = vXY.normalized * currentMaxSpeed;
                playerRb.velocity = new Vector3(limited.x, limited.y, 0f);
            }
            else
            {
                // 任何情况下都把 Z 轴速度归零，避免出现 Z 方向漂移
                playerRb.velocity = new Vector3(v.x, v.y, 0f);
            }
            // 粒子逻辑已改到 Update 中处理，避免使用 GetKeyDown/GetKeyUp 造成瞬间播放。
        }

    }

    /// <summary>
    /// 发射子弹 - 根据是否有三连发道具决定发射模式
    /// </summary>
    private Vector3 GetBulletDirection()
    {
        if (mainCamera == null || bulletAnchor == null)
        {
            return transform.forward;
        }

        float depth = mainCamera.WorldToScreenPoint(bulletAnchor.transform.position).z;
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, depth));
        Vector3 direction = mouseWorldPosition - bulletAnchor.transform.position;
        direction.z = 0f;

        if (direction.sqrMagnitude > Mathf.Epsilon)
        {
            return direction.normalized;
        }

        direction = bulletAnchor.transform.forward;
        direction.z = 0f;
        return direction.sqrMagnitude > Mathf.Epsilon ? direction.normalized : transform.forward;
    }

    private void SpawnBullet(Vector3 direction)
    {
        direction = direction.normalized;
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.back);
        GameObject bullet = Instantiate(bulletPrefab, bulletAnchor.transform.position, rotation);
        var bulletController = bullet.GetComponent<BulletController>() ?? bullet.GetComponentInChildren<BulletController>();
        if (bulletController != null)
        {
            bulletController.SetMoveDirection(direction);
        }
    }

    private void FireBullet()
    {
        if (bulletPrefab == null || bulletAnchor == null) return;

        Vector3 bulletDirection = GetBulletDirection();
        if (hasTripleShot)
        {
            // 三连发：中间、左侧、右侧
            SpawnBullet(bulletDirection);

            // 左侧子弹（向左偏移15度）
            Vector3 leftDirection = Quaternion.AngleAxis(-TripleShotSpreadAngle, Vector3.forward) * bulletDirection;
            SpawnBullet(leftDirection);

            // 右侧子弹（向右偏移15度）
            Vector3 rightDirection = Quaternion.AngleAxis(TripleShotSpreadAngle, Vector3.forward) * bulletDirection;
            SpawnBullet(rightDirection);
        }
        else
        {
            // 普通单发
            SpawnBullet(bulletDirection);
        }
    }

    /// <summary>
    /// 激活射速增强（双倍射速）
    /// </summary>
    public void ActivateFireRateBoost()
    {
        hasFireRateBoost = true;
        fireRateBoostTimer = fireRateBoostDuration;
        Debug.Log("Fire Rate Boost Activated!");
    }

    /// <summary>
    /// 激活三连发
    /// </summary>
    public void ActivateTripleShot()
    {
        hasTripleShot = true;
        tripleShotTimer = tripleShotDuration;
        Debug.Log("Triple Shot Activated!");
    }
    /// <summary>
    /// 激活护盾效果
    /// </summary>
    public void ActiveShield()
    {
        if (shield == null)
        {
            Debug.LogWarning("PlayerController: shield 未在 Inspector 中赋值。");
            return;
        }
        shield.SetActive(true);
        shieldTimer = shieldDuration;
    }

    /// <summary>
    /// 尝试消耗护盾（当受到伤害时调用）
    /// </summary>
    /// <returns>如果护盾存在并被消耗返回true，否则返回false</returns>
    public bool TryConsumeShield()
    {
        if (shield == null || !shield.activeSelf) return false;
        shield.SetActive(false);
        shieldTimer = 0f;
        return true;
    }
}


