using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 三连发道具：一次发射三颗子弹
/// </summary>
public class TripleShotController : MonoBehaviour
{
    private float timeForDie = 5f;
    [SerializeField] [Tooltip("与玩家中心距离小于此值即视为吃到")]
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

    public void TryConsume(PlayerController player)
    {
        if (consumed || player == null) return;
        consumed = true;
        player.ActivateTripleShot();
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
