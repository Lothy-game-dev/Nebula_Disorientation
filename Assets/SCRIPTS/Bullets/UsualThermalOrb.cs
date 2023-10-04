using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsualThermalOrb : BulletShared
{
    #region ComponentVariables
    #endregion
    #region InitializeVariables
    public float StartAnimTimer;
    public float EndAnimTimer;
    public bool isHeat;
    #endregion
    #region NormalVariables
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        rb = GetComponent<Rigidbody2D>();
        // Calculate Velocity
        CalculateVelocity();
        // Accelerate Bullet
        StartCoroutine(Accelerate(0.05f));
        InitializeBullet();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        UpdateBullet();
        CalculateThermalDamage(isHeat);
        DistanceTravel += Time.deltaTime * rb.velocity.magnitude;
        CheckDistanceTravel();
    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    #endregion
}
