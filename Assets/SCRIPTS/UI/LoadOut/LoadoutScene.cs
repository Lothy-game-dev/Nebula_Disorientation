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
    public GameObject Weapon1Bar;
    public GameObject Weapon2Bar;
    public GameObject ModelBoard;
    public GameObject Power1Bar;
    public GameObject Power2Bar;
    public GameObject ConsumableBar;
    #endregion
    #region NormalVariables
    public string LeftWeapon;
    public string RightWeapon;
    public string FirstPower;
    public string SecondPower;
    public Dictionary<string, int> Consumables;
    public string Model;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        Consumables = new Dictionary<string, int>();
        GetData();
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
    #region On Enable Get Data
    private void GetData()
    {
        Weapon1Bar.GetComponent<LoadOutBar>().PresetItems(
            ListReplaceSpace(FindObjectOfType<AccessDatabase>().GetAllWeaponName()),
            ListReplaceSpace(FindObjectOfType<AccessDatabase>().GetAllWeaponName())[0]);
        Weapon1Bar.GetComponent<LoadOutBar>().PresetItems(
            ListReplaceSpace(FindObjectOfType<AccessDatabase>().GetAllWeaponName()),
            ListReplaceSpace(FindObjectOfType<AccessDatabase>().GetAllWeaponName())[0]);
        ModelBoard.GetComponent<LoadOutModelBoard>().SetItems(
            ListReplaceSpace(FindObjectOfType<AccessDatabase>().GetAllModelName()),
            ListReplaceSpace(FindObjectOfType<AccessDatabase>().GetAllModelName())[0]);
        Power1Bar.GetComponent<LoadOutPowerBar>().SetItems(
            ListReplaceSpace(FindObjectOfType<AccessDatabase>().GetAllPowerName()),"");
        Power2Bar.GetComponent<LoadOutPowerBar>().SetItems(
            ListReplaceSpace(FindObjectOfType<AccessDatabase>().GetAllPowerName()),"");
        ConsumableBar.GetComponent<LoadOutConsumables>().SetInitData(
            FindObjectOfType<AccessDatabase>().GetAllDictionarySpaceShopCons());
    }

    private List<string> ListReplaceSpace(List<string> inList)
    {
        for (int i=0;i<inList.Count;i++) {
            inList[i] = inList[i].Replace(" ", "");
        }
        return inList;
    }

    private List<string> ListReplaceDash(List<string> inList)
    {
        for (int i = 0; i < inList.Count; i++)
        {
            inList[i] = inList[i].Replace("-", "");
        }
        return inList;
    }
    #endregion
}
