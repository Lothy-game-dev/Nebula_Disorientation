using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsualThermalOrb : BulletShared
{
    #region ComponentVariables
    private Animator anim;
    #endregion
    #region InitializeVariables
    public float StartAnimTimer;
    public float EndAnimTimer;
    public bool isHeat;
    #endregion
    #region NormalVariables
    private float AnimTimer;
    private bool isStart;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        // Calculate Velocity
        CalculateVelocity();
        // Accelerate Bullet
        StartCoroutine(Accelerate(0.1f));
        AnimTimer = StartAnimTimer;
        isStart = true;
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
        if (AnimTimer <= 0f)
        {
            if (isStart)
            {
                anim.SetTrigger("Start");
                AnimTimer = EndAnimTimer;
            }
            else
            {
                anim.SetTrigger("End");
            }
        }
        else
        {
            AnimTimer -= Time.deltaTime;
        }
    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    #endregion
}
