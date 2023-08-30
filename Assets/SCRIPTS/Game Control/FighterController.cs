using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controller used to generate Fighter as start of each stage.
public class FighterController : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject[] Weapons;
    public GameObject[] Bullets;
    public GameObject RightWeaponPosition;
    public GameObject LeftWeaponPosition;
    public GameObject PlayerFighter;
    #endregion
    #region NormalVariables
    public GameObject CurrentLeftWeapon;
    public GameObject LeftWeaponBullet;
    public GameObject CurrentRightWeapon;
    public GameObject RightWeaponBullet;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        // Retrieve Datas from DB to get inits data
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    #endregion
}
