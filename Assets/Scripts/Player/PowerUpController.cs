using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 护盾能量拾取逻辑：优先用碰撞；若玩家与能量球都是 Trigger（Unity 常不触发），用距离兜底。
/// 注意：此脚本仅用于护盾能量，生命能量使用 LifePowerUpController
/// </summary>
public class PowerUpController : MonoBehaviour
{
    private float timeForDie = 5f;
    [SerializeField] [Tooltip("与玩家中心距离小于此值即视为吃到（双 Trigger 时的备用）")]
    private float pickupRadius = 1.25f;

    private bool consumed;
    private Transform playerTransform;

    private void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) playerTransform = p.transform;
        StartCoroutine(WaitToDie());
    }

    private void FixedUpdate()
    {
        if (consumed || playerTransform == null) return;

        if (Vector3.Distance(transform.position, playerTransform.position) <= pickupRadius)
        {
            var pc = playerTransform.GetComponent<PlayerController>();
            TryConsume(pc);
        }
    }

    /// <summary>由 PlayerController.OnTriggerEnter 调用（推荐路径）</summary>
    public void TryConsume(PlayerController player)
    {
        if (consumed || player == null) return;
        consumed = true;
        player.ActiveShield();
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var pc = other.GetComponent<PlayerController>()
                   ?? other.GetComponentInParent<PlayerController>();
        TryConsume(pc);
    }

    private IEnumerator WaitToDie()
    {
        yield return new WaitForSeconds(timeForDie);
        if (!consumed) Destroy(gameObject);
    }
}
