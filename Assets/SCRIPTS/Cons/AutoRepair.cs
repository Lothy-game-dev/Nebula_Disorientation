using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRepair : Consumable
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
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    private float PlayerCurrentHP;
    private float PlayerMaxHP;
    private bool isStart;
    private float Timer;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (isStart)
        {
            Timer += Time.deltaTime;
            if (Timer >= Duration)
            {
                isStart = false;
            } else
            {
                if (PlayerCurrentHP < PlayerMaxHP)
                {
                    Fighter.GetComponent<PlayerFighter>().CurrentHP += PlayerMaxHP * int.Parse(Effect.Split("-")[1]) / 100; 
                }
            }
            
        }
    }
    #endregion
    #region Activate cons
    // Group all function that serve the same algorithm
    public void ActivateAutoRepair()
    {
        isStart = true;
        PlayerCurrentHP = Fighter.GetComponent<PlayerFighter>().CurrentHP;
        PlayerMaxHP = Fighter.GetComponent<PlayerFighter>().MaxHP;
    }
    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    #endregion
}
