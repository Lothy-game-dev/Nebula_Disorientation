using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingShield : Consumable
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
                Fighter.GetComponent<PlayerFighter>().isWingShield = false;
                Fighter.GetComponent<PlayerFighter>().ShieldReducedScale -= float.Parse(Effect.Split("-")[1].ToString());
                isStart = false;
                Timer = 0f;
            }
        }
    }
    #endregion
    #region Activate
    // Group all function that serve the same algorithm
    public void WingmanShieldEffect()
    {
        isStart = true;
        Fighter.GetComponent<PlayerFighter>().isWingShield = true;
        Fighter.GetComponent<PlayerFighter>().ShieldReducedScale += float.Parse(Effect.Split("-")[1].ToString());
    }
    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    #endregion
}
