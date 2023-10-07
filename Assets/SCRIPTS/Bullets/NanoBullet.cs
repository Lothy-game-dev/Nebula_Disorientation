using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NanoBullet : BulletShared
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public float PenetrateDistance;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
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
        StartCoroutine(AccelerateLaser(0.01f, transform.localScale.x));
        InitializeBullet();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        DistanceTravel += Time.deltaTime * rb.velocity.magnitude;
        CheckDistanceTravelPenetrate(PenetrateDistance);
        CalculatePenetrateDamage();
    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    #endregion
}
