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
    public GameObject PlayerFighter;
    public GameObject Aim;
    #endregion
    #region NormalVariables
    public GameObject CurrentLeftWeapon;
    public GameObject CurrentRightWeapon;
    public GameObject RightWeaponPosition;
    public GameObject LeftWeaponPosition;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        // Retrieve Datas from DB to get inits data
        InitData();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Initialize Datas
    // Group all function that serve the same algorithm
    private void InitData()
    {
        // set current weapons from DB
        // clone and set Left Right weapon to aim
        LeftWeaponPosition = PlayerFighter.transform.GetChild(0).gameObject;
        RightWeaponPosition = PlayerFighter.transform.GetChild(1).gameObject;
        GameObject LeftWeapon = Instantiate(CurrentLeftWeapon, LeftWeaponPosition.transform.position, Quaternion.identity);
        GameObject RightWeapon = Instantiate(CurrentRightWeapon, RightWeaponPosition.transform.position, Quaternion.identity);
        LeftWeapon.SetActive(true);
        Weapons LW = LeftWeapon.GetComponent<Weapons>();
        LW.isLeftWeapon = true;
        LW.Fighter = PlayerFighter;
        LW.Aim = Aim;
        LW.WeaponPosition = LeftWeaponPosition;
        RightWeapon.SetActive(true);
        Weapons RW = RightWeapon.GetComponent<Weapons>();
        RW.Fighter = PlayerFighter;
        RW.Aim = Aim;
        RW.WeaponPosition = RightWeaponPosition;
        Aim.GetComponent<TargetCursor>().LeftWeapon = LeftWeapon;
        Aim.GetComponent<TargetCursor>().RightWeapon = RightWeapon;
    }
    #endregion
}
