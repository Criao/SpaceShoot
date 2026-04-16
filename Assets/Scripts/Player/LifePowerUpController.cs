using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifePowerUpController : MonoBehaviour
{
    private GameManager gameManager;
    private float lifeTime = 10f;

    private void Awake()
    {
        foreach (var oldController in GetComponentsInChildren<PowerUpController>())
        {
            Destroy(oldController);
        }

        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    private void Start()
    {
        var gmGo = GameObject.FindGameObjectWithTag("GameController");
        if (gmGo != null)
        {
            gameManager = gmGo.GetComponent<GameManager>();
        }

        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryPickup(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryPickup(collision.gameObject);
    }

    private void TryPickup(GameObject other)
    {
        if (other == null) return;

        var playerController = other.GetComponentInParent<PlayerController>();
        if (playerController == null) return;

        if (gameManager != null)
        {
            gameManager.AddLife(1);
        }
        Destroy(gameObject);
    }
}
