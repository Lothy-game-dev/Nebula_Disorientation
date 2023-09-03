using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackholeOrb : BulletShared
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public float ObjectRotationScale;
    public GameObject BlackHole;
    public float BlackHoleRadius;
    public float BlackHolePullingForce;
    public float BlackHoleExistTime;
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
        StartCoroutine(Accelerate(0.1f));
        InitializeBullet();
        if (ObjectRotationScale == 0f)
        {
            ObjectRotationScale = 1f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        UpdateBullet();
        CheckCreateBlackhole(BlackHole, BlackHoleRadius, BlackHoleExistTime, BlackHolePullingForce);
        DistanceTravel += Time.deltaTime * rb.velocity.magnitude;
        CheckDistanceTravelBlackhole(BlackHole, BlackHoleRadius, BlackHoleExistTime, BlackHolePullingForce);
        RotateBullet();
    }
    #endregion
    #region Bullet Rotation
    public void RotateBullet()
    {
        transform.Rotate(new Vector3(0, 0, 1));
    }
    #endregion
}
