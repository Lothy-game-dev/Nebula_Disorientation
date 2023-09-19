using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public GameObject FighterDemo;
    public GameObject FuelCell;

    public GameObject Factory;
    #endregion
    #region NormalVariables
    public string LeftWeapon;
    public string RightWeapon;
    public string FirstPower;
    public string SecondPower;
    public Dictionary<string, int> Consumables;
    public string Model;
    public int CurrentFuelCells;
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
        transform.GetChild(0).GetComponent<Rigidbody2D>().simulated = true;
        Weapon1Bar.SetActive(true);
        Weapon2Bar.SetActive(true);
        ModelBoard.SetActive(true);
        Power1Bar.SetActive(true);
        Power2Bar.SetActive(true);
        ConsumableBar.SetActive(true);
        FighterDemo.SetActive(true);
        GetData();
    }
    public override void OnExitAnimation()
    {
        GetComponent<BackgroundBrieflyMoving>().enabled = false;
        transform.GetChild(0).GetComponent<Rigidbody2D>().simulated = false;
        Weapon1Bar.SetActive(false);
        Weapon2Bar.SetActive(false);
        ModelBoard.SetActive(false);
        Power1Bar.SetActive(false);
        Power2Bar.SetActive(false);
        ConsumableBar.SetActive(false);
        FighterDemo.SetActive(false);
    }
    #endregion
    #region On Enable Get Data
    private void GetData()
    {
        // Set Data to board/bar
        List<string> ListOwnedModel = FindObjectOfType<AccessDatabase>().GetAllOwnedModel(FindObjectOfType<UECMainMenuController>().PlayerId);
        if (ListOwnedModel.Count>0)
        {
            ModelBoard.GetComponent<LoadOutModelBoard>().SetItems(
            ListReplaceSpace(ListOwnedModel),
            ListReplaceSpace(ListOwnedModel)[0]);
        } else
        {
            // Do you want to move to factory
        }
        ConsumableBar.GetComponent<LoadOutConsumables>().SetInitData(
            FindObjectOfType<AccessDatabase>().GetOwnedConsumables(FindObjectOfType<UECMainMenuController>().PlayerId));
        // Set data to fuel cell bar
        Dictionary<string, object> ListData = FindObjectOfType<AccessDatabase>()
            .GetPlayerInformationById(FindObjectOfType<UECMainMenuController>().PlayerId);
        CurrentFuelCells = (int)ListData["FuelCell"];
        FuelCell.transform.GetChild(0).GetChild(0).GetComponent<Slider>().maxValue = 10;
        FuelCell.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value = CurrentFuelCells;
    }

    // Replace " " in string
    private List<string> ListReplaceSpace(List<string> inList)
    {
        for (int i=0;i<inList.Count;i++) {
            inList[i] = inList[i].Replace(" ", "");
        }
        return inList;
    }

    // Replace "-" in string
    private List<string> ListReplaceDash(List<string> inList)
    {
        for (int i = 0; i < inList.Count; i++)
        {
            inList[i] = inList[i].Replace("-", "");
        }
        return inList;
    }

    public void BuyFighter()
    {
        FindObjectOfType<UECMainMenuController>().TeleportToScene(gameObject, Factory);
    }

    public string SetDataToDb()
    {
        // WIP
        string check = "Model:" + Model
            + ",Left Weapon:" + LeftWeapon
            + ",Right Weapon:" + RightWeapon
            + ",First Power:" + FirstPower
            + ",Second Power:" + SecondPower;
        foreach (var item in Consumables)
        {
            check += ",Consumable:" + item.Key + " - " + item.Value;
        }
        Debug.Log(check);
        return "Success";
    }
    #endregion
}
