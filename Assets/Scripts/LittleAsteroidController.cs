using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleAsteroidController : AsterodController
{
    private bool dropLifePowerUp;

    // Start is called before the first frame update
    protected override void Start()
    {
        movementSpeed = 2f;
        rotateSpeed = 5f;

        base.Start();
    }

    public void SetLifePowerUpDrop(bool value)
    {
        dropLifePowerUp = value;
    }

    protected override void SpawnDroppedPowerUp()
    {
        if (!dropLifePowerUp) return;
        if (SpawnManager.Instance == null) return;
        SpawnManager.Instance.SpawnLifePowerUp(transform.position);
    }

    // Update is called once per frame
    protected override void CreateLittleAsteroids()
    {
        // 小陨石不再生成更小陨石
    }
}
