using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutScene : UECMenuShared
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
    public string LeftWeapon;
    public string RightWeapon;
    public string FirstPower;
    public string SecondPower;
    public string FirstConsumable;
    public string SecondConsumable;
    public string ThirdConsumable;
    public string FourthConsumable;
    public string Model;
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
    }
    #endregion
    #region Animation
    public override void OnEnterAnimation()
    {
        GetComponent<BackgroundBrieflyMoving>().enabled = true;
    }
    public override void OnExitAnimation()
    {
        GetComponent<BackgroundBrieflyMoving>().enabled = false;
    }
    #endregion
}
