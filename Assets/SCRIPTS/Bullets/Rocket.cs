using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : BulletShared
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    #endregion
    #region NormalVariables
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Assign component
        rb = GetComponent<Rigidbody2D>();
        // Calculate Velocity
        CalculateVelocity();
        // Accelerate Bullet
        StartCoroutine(Accelerate());
    }

    // Update is called once per frame
    void Update()
    {
        CalculateDamage();
    }
    #endregion
}
