using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    private Rigidbody playerRb;
    [Header("Movement")]
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float maxSpeed = 6f;
    [SerializeField] private float boostMultiplier = 2f;
    [SerializeField] private float boostedMaxSpeed = 10f;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject bulletAnchor;
    [SerializeField] private float fireCooldown = 0.2f;
    [SerializeField] private GameObject shield;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private ParticleSystem engineTrail;
    [SerializeField] private float shieldDuration = 5f;

    private float fireTimer;
    private float shieldTimer;

    private void Awake()
    {
        // 防止 Inspector 忘记拖引用导致 FixedUpdate 报错、整段移动逻辑不执行
        if (mainCamera == null) mainCamera = Camera.main;
        if (playerRb == null) playerRb = GetComponent<Rigidbody>();

        if (engineTrail != null)
        {
            var main = engineTrail.main;
            main.loop = true;
        }
    }

    private void Start()
    {
        // 兜底（Awake 已赋值时这里不重复）
        if (playerRb == null) playerRb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!gameManager.isPuse)
        {
            if (mainCamera == null) return;

            Vector3 mousePos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f));
            transform.LookAt(mousePos, Vector3.back);

            fireTimer -= Time.deltaTime;
            if (fireTimer <= 0f && Input.GetMouseButtonDown(0))
            {
                FireBullet();
                fireTimer = fireCooldown;
            }

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

            if (shield != null && shield.activeSelf)
            {
                shieldTimer -= Time.deltaTime;
                if (shieldTimer <= 0f)
                {
                    shield.SetActive(false);
                    shieldTimer = 0f;
                }
            }
        }

    }

    private void FixedUpdate()
    {
        if (!gameManager.isPuse)
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

            float sideInput = Input.GetAxis("Horizontal");
            float forwardInput = Input.GetAxis("Vertical");

            // 严格只在世界坐标 X/Y 平面移动（不参与 Z 轴）
            bool boosting = Input.GetKey(KeyCode.LeftShift);
            float currentAcceleration = acceleration * (boosting ? boostMultiplier : 1f);
            float currentMaxSpeed = boosting ? boostedMaxSpeed : maxSpeed;

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

    private void FireBullet()
    {
        if (bulletPrefab == null || bulletAnchor == null) return;

        Instantiate(bulletPrefab, bulletAnchor.transform.position, bulletAnchor.transform.rotation);
    }
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

    public bool TryConsumeShield()
    {
        if (shield == null || !shield.activeSelf) return false;
        shield.SetActive(false);
        shieldTimer = 0f;
        return true;
    }
}


