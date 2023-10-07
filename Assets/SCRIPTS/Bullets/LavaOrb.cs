using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaOrb : BulletShared
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public float ObjectRotationScale;
    public GameObject Tracer;
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
        if (ObjectRotationScale==0f)
        {
            ObjectRotationScale = 1f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        UpdateBullet();
        DistanceTravel += Time.deltaTime * rb.velocity.magnitude;
        CheckDistanceTravelLavaOrb();
        CalculateLavaOrbDamage();
        RotateOrb();
        Tracer.transform.position = transform.position;
    }
    #endregion
    #region Rotate Lava Orb
    private void RotateOrb()
    {
        transform.Rotate(new Vector3(0, 0, 1));
    }
    #endregion
}
