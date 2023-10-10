using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsualKineticBullet : BulletShared
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public bool isGravitationalLine;
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
        StartCoroutine(Accelerate(0.005f));
        InitializeBullet();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        DistanceTravel += Time.deltaTime * rb.velocity.magnitude;
        CheckDistanceTravel();
        CalculateDamage();
        CheckHitAsteroidAndRock();
    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    #endregion
}
