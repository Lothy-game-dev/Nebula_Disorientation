using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterAttachedWeapon : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Fighter;
    public GameObject FighterModel;
    public GameObject LeftWeapon;
    public GameObject RightWeapon;
    #endregion
    #region NormalVariables
    private Vector3 LeftWeaponPos;
    private Vector3 RightWeaponPos;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion
    #region Attach Weapon
    public void AttachWeapon()
    {
        // Call function and timer only if possible
        LeftWeaponPos = new Vector3(Fighter.transform.position.x + FighterModel.GetComponent<FighterModelShared>().LeftWeaponPos.x * Fighter.transform.localScale.x,
            Fighter.transform.position.y + FighterModel.GetComponent<FighterModelShared>().LeftWeaponPos.y * Fighter.transform.localScale.y,
            Fighter.transform.position.z);
        RightWeaponPos = new Vector3(Fighter.transform.position.x + FighterModel.GetComponent<FighterModelShared>().RightWeaponPos.x * Fighter.transform.localScale.x,
            Fighter.transform.position.y + FighterModel.GetComponent<FighterModelShared>().RightWeaponPos.y * Fighter.transform.localScale.y,
            Fighter.transform.position.z);
        LeftWeapon.transform.position = LeftWeaponPos;
        LeftWeapon.transform.SetParent(Fighter.transform);
        RightWeapon.transform.position = RightWeaponPos;
        RightWeapon.transform.SetParent(Fighter.transform);
    }
    #endregion
}
