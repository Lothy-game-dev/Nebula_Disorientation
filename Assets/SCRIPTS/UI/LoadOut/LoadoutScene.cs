using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public GameObject BlackFade;
    public GameObject Tutorial;
    #endregion
    #region NormalVariables
    public string LeftWeapon;
    public string RightWeapon;
    public string FirstPower;
    public string SecondPower;
    public Dictionary<string, int> Consumables;
    public string Model;
    public int CurrentFuelCells;
    public int CurrentHP;
    public string ConsSaveString;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        Consumables = new Dictionary<string, int>();
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
        Dictionary<string, object> Data = FindObjectOfType<AccessDatabase>().GetLoadoutSaveData(FindObjectOfType<UECMainMenuController>().PlayerId);
        if (Data.ContainsKey("Model"))
            Model = (string)Data["Model"];
        if (Data.ContainsKey("LeftWeapon"))
            LeftWeapon = (string)Data["LeftWeapon"];
        if (Data.ContainsKey("RightWeapon"))
            RightWeapon = (string)Data["RightWeapon"];
        if (Data.ContainsKey("FirstPower"))
            FirstPower = (string)Data["FirstPower"];
        if (Data.ContainsKey("SecondPower"))
            SecondPower = (string)Data["SecondPower"];
        if (Data.ContainsKey("Consumables"))
            ConsSaveString = (string)Data["Consumables"];
        Weapon1Bar.SetActive(true);
        Weapon2Bar.SetActive(true);
        ModelBoard.SetActive(true);
        Power1Bar.SetActive(true);
        Power2Bar.SetActive(true);
        ConsumableBar.SetActive(true);
        FighterDemo.SetActive(true);
        Tutorial.SetActive(true);
        GetData();
    }
    public override void OnExitAnimation()
    {
        GetComponent<BackgroundBrieflyMoving>().enabled = false;
        transform.GetChild(0).GetComponent<Rigidbody2D>().simulated = false;
        Weapon1Bar.SetActive(false);
        Weapon2Bar.SetActive(false);
        Power1Bar.SetActive(false);
        Power2Bar.SetActive(false);
        ConsumableBar.SetActive(false);
        FighterDemo.SetActive(false);
        Tutorial.SetActive(false);
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
            Model == "" ?
            ListReplaceSpace(ListOwnedModel)[0] : Model);
        } else
        {
            // Do you want to move to factory
        }
        bool newLeftWeapon = false;
        bool newRightWeapon = false;
        List<string> ListWeapon = FindObjectOfType<AccessDatabase>().GetAllOwnedWeapon(FindObjectOfType<UECMainMenuController>().PlayerId);
        if (ListWeapon.Count > 0)
        {
            if (LeftWeapon == "")
            {
                newLeftWeapon = true;
            }
            else
            {
                int n = FindObjectOfType<AccessDatabase>().GetCurrentOwnershipWeaponPowerModelByName(FindObjectOfType<UECMainMenuController>().PlayerId, LeftWeapon, "Weapon");
                Debug.Log("Left:" + n);
                if (n==0)
                {
                    newLeftWeapon = true;
                } else if (n==1)
                {
                    if (RightWeapon!=null && RightWeapon == LeftWeapon)
                    {
                        newLeftWeapon = false;
                        newRightWeapon = true;
                    }
                } else
                {
                    newLeftWeapon = false;
                }
            }
            if (newLeftWeapon)
            {
                LeftWeapon = ListWeapon[0];
                Weapon1Bar.GetComponent<LoadOutBar>().SetItem(ListWeapon, LeftWeapon, true);
            } else
            {
                Weapon1Bar.GetComponent<LoadOutBar>().SetItem(ListWeapon, LeftWeapon, true);
            }
            if (RightWeapon=="")
            {
                newRightWeapon = true;
            } else
            {
                int n = FindObjectOfType<AccessDatabase>().GetCurrentOwnershipWeaponPowerModelByName(FindObjectOfType<UECMainMenuController>().PlayerId, RightWeapon, "Weapon");
                Debug.Log("Right:" + n);
                if (n == 0)
                {
                    newRightWeapon = true;
                }
                else if (n == 1)
                {
                    if (LeftWeapon != null && RightWeapon == LeftWeapon)
                    {
                        newLeftWeapon = false;
                        newRightWeapon = true;
                    }
                }
                else
                {
                    newRightWeapon = false;
                }
            }
            List<string> WeaponTemp = FindObjectOfType<AccessDatabase>().GetOwnedWeaponExceptForName(FindObjectOfType<UECMainMenuController>().PlayerId, LeftWeapon);

            if (newRightWeapon)
            {
                RightWeapon = WeaponTemp[0];
                Weapon2Bar.GetComponent<LoadOutBar>().SetItem(WeaponTemp, RightWeapon, true);
            } else
            {
                Weapon2Bar.GetComponent<LoadOutBar>().SetItem(WeaponTemp, RightWeapon, true);
            }
        }
        
        
        int SP = 0;
        if (Model == "")
        {
            string stats = FindObjectOfType<AccessDatabase>().GetFighterStatsByName(ListReplaceSpace(ListOwnedModel)[0]);
            SP = int.Parse((string)FindObjectOfType<GlobalFunctionController>().ConvertModelStatsToDictionary(stats)["SP"]);
        } else
        {
            string stats = FindObjectOfType<AccessDatabase>().GetFighterStatsByName(Model);
            SP = int.Parse((string)FindObjectOfType<GlobalFunctionController>().ConvertModelStatsToDictionary(stats)["SP"]);
        }
        Power1Bar.GetComponent<LoadOutPowerBar>().SetItem(FirstPower);
        if (SP==2)
        Power2Bar.GetComponent<LoadOutPowerBar>().SetItem(SecondPower);
        ConsumableBar.GetComponent<LoadOutConsumables>().SetInitData(
            FindObjectOfType<AccessDatabase>().GetOwnedConsumables(FindObjectOfType<UECMainMenuController>().PlayerId));
        if (ConsSaveString!=null && ConsSaveString.Length>0)
        {
            string[] ConsData = ConsSaveString.Split("|");
            Dictionary<string, int> ConsDict = new();
            foreach (var con in ConsData)
            {
                ConsDict.Add(con.Split("-")[0], int.Parse(con.Split("-")[1]));
            }
            foreach (var item in ConsDict)
            {
                Dictionary<string, int> OwnedData = FindObjectOfType<AccessDatabase>().GetOwnedConsumables(FindObjectOfType<UECMainMenuController>().PlayerId);
                int k = 0;
                if (OwnedData.ContainsKey(item.Key))
                {
                    if (item.Value < OwnedData[item.Key]) k = item.Value;
                    else k = OwnedData[item.Key];
                    for (int i = 0; i < k; i++)
                    {
                        int n = ConsumableBar.GetComponent<LoadOutConsumables>().IncreaseItem(item.Key);
                    }
                }
            }
            ConsumableBar.GetComponent<LoadOutConsumables>().SaveLoadData();
        }
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

    public void SaveLoadoutData()
    {
        string ConsText = "";
        string ConsCDText = "";
        if (Consumables != null)
        {
            foreach (var item in Consumables)
            {
                ConsText += item.Key + "-" + item.Value + "|";
                ConsCDText += item.Key + "-" + FindAnyObjectByType<AccessDatabase>().GetConsumableDataByName(item.Key)["Cooldown"] + "|";
            }
            if (ConsText.Length > 0)
            {
                ConsText = ConsText.Substring(0, ConsText.Length - 1);
            }
        }
        FindObjectOfType<AccessDatabase>().InputLoadoutSaveData(FindObjectOfType<UECMainMenuController>().PlayerId,
            Model, LeftWeapon, RightWeapon, FirstPower, SecondPower, ConsText, ConsCDText);
    }

    public void BuyFighter()
    {
        FindObjectOfType<UECMainMenuController>().TeleportToScene(gameObject, Factory);
    }

    public void SetDataToDb()
    {
        string ConsText = "";
        string ConsCDText = "";
        foreach (var item in Consumables)
        {
            ConsText += item.Key + "-" + item.Value + "|";
            ConsCDText += item.Key + "-" + FindAnyObjectByType<AccessDatabase>().GetConsumableDataByName(item.Key)["Cooldown"] + "|";
        }
        if (ConsText.Length>0)
        {
            ConsText = ConsText.Substring(0, ConsText.Length - 1);
        }

        int FuelCore = (int)FindObjectOfType<AccessDatabase>().GetPlayerInformationById(FindObjectOfType<UECMainMenuController>().PlayerId)["FuelCell"];
        string check = FindObjectOfType<AccessDatabase>().AddNewSession(FindObjectOfType<UECMainMenuController>().PlayerId,
            Model, LeftWeapon, RightWeapon, FirstPower, SecondPower, ConsText, ConsCDText, FuelCore - 1);
        FindObjectOfType<AccessDatabase>().InputLoadoutSaveData(FindObjectOfType<UECMainMenuController>().PlayerId,
            Model, LeftWeapon, RightWeapon, FirstPower, SecondPower, ConsText, ConsCDText);
        if ("Success".Equals(check))
        {
            FindObjectOfType<AccessDatabase>().AddSessionCurrentSaveData(FindObjectOfType<UECMainMenuController>().PlayerId, "LOTW");
            StartCoroutine(MoveToLOTW());
        }
        else if ("Fail".Equals(check))
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
                "Cannot create session!\nPlease contact to our email!", 5f);
        }
        else if ("No Exist".Equals(check))
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
                "Cannot fetch data for this pilot!\nPlease try again!", 5f);
        }
        FindObjectOfType<AccessDatabase>().AddSessionOwnershipToItem(FindObjectOfType<UECMainMenuController>().PlayerId
            , Model, "Model", 1);
        FindObjectOfType<AccessDatabase>().AddSessionOwnershipToItem(FindObjectOfType<UECMainMenuController>().PlayerId
            , LeftWeapon, "Weapon", 1);
        FindObjectOfType<AccessDatabase>().AddSessionOwnershipToItem(FindObjectOfType<UECMainMenuController>().PlayerId
            , RightWeapon, "Weapon", 1);
        if (FirstPower!="")
        {
           FindObjectOfType<AccessDatabase>().AddSessionOwnershipToItem(FindObjectOfType<UECMainMenuController>().PlayerId
                , FirstPower, "Power", 1);
        }

        if (SecondPower!="")
        {
            FindObjectOfType<AccessDatabase>().AddSessionOwnershipToItem(FindObjectOfType<UECMainMenuController>().PlayerId
                , SecondPower, "Power", 1);
        }
        foreach (var item in Consumables)
        {
            FindObjectOfType<AccessDatabase>().AddSessionOwnershipToItem(FindObjectOfType<UECMainMenuController>().PlayerId
                , item.Key, "Consumable", item.Value);
            FindObjectOfType<AccessDatabase>().DecreaseOwnershipToItem(FindObjectOfType<UECMainMenuController>().PlayerId
                , item.Key, "Consumable", item.Value);
        }
    }

    private IEnumerator MoveToLOTW()
    {
        for (int i=0;i<50;i++)
        {
            Color c = BlackFade.GetComponent<SpriteRenderer>().color;
            c.a += 1 / 50f;
            BlackFade.GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(2 / 50f);
        }
        FindObjectOfType<AccessDatabase>().ClearFuelCell(FindObjectOfType<UECMainMenuController>().PlayerId);
        PlayerPrefs.SetInt("PlayerID", FindObjectOfType<UECMainMenuController>().PlayerId);
        PlayerPrefs.SetString("InitTeleport", "LOTW");
        SceneManager.UnloadSceneAsync("UECMainMenu");
        SceneManager.LoadSceneAsync("GameplayExterior");
    }
    #endregion
}
