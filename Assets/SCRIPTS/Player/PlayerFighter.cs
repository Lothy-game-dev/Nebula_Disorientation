using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFighter : FighterShared
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    #endregion
    #region NormalVariables
    private float testTimer;
    private int testCount=0;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        InitializeFighter();
    }

    // Update is called once per frame
    void Update()
    {
        CheckThermal();
        if (testTimer <= 0f)
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                ReceiveThermalDamage(false);
                RegenTimer = 2f;
                testTimer = 1f;
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                ReceiveThermalDamage(true);
                RegenTimer = 2f;
                testTimer = 1f;
            }
        } else
        {
            testTimer -= Time.deltaTime;
        }
        
    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    #endregion
}
