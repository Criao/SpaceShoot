using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 子弹控制器 - 控制子弹的移动、碰撞检测和生命周期
/// </summary>
public class BulletController : MonoBehaviour
{
    private float speed = 10f; // 子弹速度
    private float timeToDie = 2f; // 子弹存活时间
    private Vector3 moveDirection = Vector3.forward;

    /// <summary>
    /// 初始化时自动添加必要的碰撞组件
    /// </summary>
    private void Awake()
    {
        // 没有 Collider/Rigidbody 时，既不会触发 OnTriggerEnter，也不会触发 OnCollisionEnter
        if (GetComponent<Collider>() == null)
        {
            var sphere = gameObject.AddComponent<SphereCollider>();
            sphere.isTrigger = true;
            sphere.radius = 0.15f;
        }

        if (GetComponent<Rigidbody>() == null)
        {
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true; // 子弹用 transform 移动，这里只为触发回调
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }
    }

    /// <summary>
    /// 开始时启动销毁计时器
    /// </summary>
    void Start()
    {
        StartCoroutine(TimeToDie());
    }

    /// <summary>
    /// 每帧向前移动子弹
    /// </summary>
    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    public void SetMoveDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude <= Mathf.Epsilon) return;

        moveDirection = direction.normalized;
        transform.rotation = Quaternion.LookRotation(moveDirection, Vector3.back);
    }

    /// <summary>
    /// 触发器碰撞检测 - 检测是否击中陨石
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // 让子弹侧也能保证命中生效（避免 Tag/触发配置导致陨石侧没收到）
        var asteroid = other.GetComponent<AsterodController>() ?? other.GetComponentInParent<AsterodController>();
        if (asteroid != null)
        {
            asteroid.HandleBulletHit(gameObject);
        }
    }

    /// <summary>
    /// 协程：在指定时间后销毁子弹
    /// </summary>
    IEnumerator TimeToDie()
    {
        yield return new WaitForSeconds(timeToDie);
        Destroy(this.gameObject);
    }
}
