using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject LeftOverheatBar;
    public GameObject RightOverheatBar;
    public GameObject LeftOverheatImage;
    public GameObject RightOverheatImage;
    public Slider LeftReloadBar;
    public Slider RightReloadBar;
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
        LeftWeapon.GetComponent<Weapons>().OverHeatImage = LeftOverheatImage;
        RightWeapon.GetComponent<Weapons>().OverHeatImage = RightOverheatImage;
        // Set left right weapon to overheat bar
        LeftOverheatBar.GetComponent<OverheatBar>().Weapon = LeftWeapon.GetComponent<Weapons>();
        RightOverheatBar.GetComponent<OverheatBar>().Weapon = RightWeapon.GetComponent<Weapons>();
        GameObject LeftWeaponIcon = Instantiate(CurrentLeftWeapon, LeftOverheatImage.transform.position, Quaternion.identity);
        GameObject RightWeaponIcon = Instantiate(CurrentRightWeapon, RightOverheatImage.transform.position, Quaternion.identity);
        LeftWeaponIcon.SetActive(true);
        LeftWeaponIcon.GetComponent<Weapons>().enabled = false;
        LeftWeaponIcon.transform.localScale =
            new Vector3(LeftWeaponIcon.transform.localScale.x * 12,
            LeftWeaponIcon.transform.localScale.y * 12,
            LeftWeaponIcon.transform.localScale.z);
        LeftWeaponIcon.GetComponent<SpriteRenderer>().sortingOrder = 11;
        Color lc = LeftWeaponIcon.GetComponent<SpriteRenderer>().color;
        lc.a = 120 / 255f;
        LeftWeaponIcon.GetComponent<SpriteRenderer>().color = lc;
        LeftWeaponIcon.transform.SetParent(LeftOverheatImage.transform);
        RightWeaponIcon.SetActive(true);
        RightWeaponIcon.GetComponent<Weapons>().enabled = false;
        RightWeaponIcon.transform.localScale =
            new Vector3(RightWeaponIcon.transform.localScale.x * 12,
            RightWeaponIcon.transform.localScale.y * 12,
            RightWeaponIcon.transform.localScale.z);
        RightWeaponIcon.GetComponent<SpriteRenderer>().sortingOrder = 11;
        RightWeaponIcon.transform.SetParent(RightOverheatImage.transform);
        Color rc = RightWeaponIcon.GetComponent<SpriteRenderer>().color;
        rc.a = 120 / 255f;
        RightWeaponIcon.GetComponent<SpriteRenderer>().color = rc;
        // Set Reload Bar
        LeftWeapon.GetComponent<Weapons>().ReloadBar = LeftReloadBar;
        RightWeapon.GetComponent<Weapons>().ReloadBar = RightReloadBar;
    }
    #endregion
}
