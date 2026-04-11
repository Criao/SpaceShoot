using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private float speed = 10f;
    private float timeToDie = 2f;

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
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TimeToDie());
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 让子弹侧也能保证命中生效（避免 Tag/触发配置导致陨石侧没收到）
        var asteroid = other.GetComponent<AsterodController>() ?? other.GetComponentInParent<AsterodController>();
        if (asteroid != null)
        {
            asteroid.HandleBulletHit(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        var other = collision.collider;
        var asteroid = other.GetComponent<AsterodController>() ?? other.GetComponentInParent<AsterodController>();
        if (asteroid != null)
        {
            asteroid.HandleBulletHit(gameObject);
        }
    }

    IEnumerator TimeToDie()
    {
        yield return new WaitForSeconds(timeToDie);
        Destroy(this.gameObject);
    }
}
