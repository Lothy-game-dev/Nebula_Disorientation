using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PersonalArea : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject Template;
    public GameObject Content;
    public GameObject RankDesc;
    public GameObject RankSalary;
    public GameObject CurrentSalary;
    public GameObject PlayerName;
    public GameObject PlayerRank;
    public GameObject TimeRemaining;
    public GameObject CollectButton;
    public GameObject Cash;
    public GameObject TimelessShard;
    public GameObject FuelCell;
    public GameObject FuelEnergy;
    public GameObject WeaponOwned;
    public GameObject FighterOwned;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public List<List<string>> RankList;
    public Dictionary<string, object> PlayerInformation;
    public int PlayerId;
    public DateTime ResetDateTime;
    public DateTime CollectedTime;
    private string formattedTime;
    public bool IsCollected;
    private TimeSpan timeRemaining;
    private int WeapOwned;
    private int ModelOwned;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        
    }

    // Update is called once per framenew 
    void Update()
    {
     
        if (IsCollected)
        {          
            SetTimer();
            if (timeRemaining <= TimeSpan.Zero)
            {              
                IsCollected = false;
            } 
        }
    }
    #endregion
    #region Get data / reset data
    // Group all function that serve the same algorithm
    public void GetData()
    {
        RankList = FindAnyObjectByType<AccessDatabase>().GetAllRank();
        PlayerId = FindAnyObjectByType<AccessDatabase>().GetCurrentSessionPlayerId();
        PlayerInformation = FindAnyObjectByType<AccessDatabase>().GetPlayerInformationById(PlayerId);
        WeapOwned = FindAnyObjectByType<AccessDatabase>().GetCurrentOwnershipWeaponPowerModel(PlayerId, "Weapon");
        ModelOwned = FindAnyObjectByType<AccessDatabase>().GetCurrentOwnershipWeaponPowerModel(PlayerId, "Model");
        PlayerName.GetComponent<TextMeshPro>().text = (string)PlayerInformation["Name"];
        WeaponOwned.GetComponent<TextMeshPro>().text = "Weapons Owned: "+ WeapOwned + "";
        FighterOwned.GetComponent<TextMeshPro>().text = "Fighters Owned: " + ModelOwned + "";
        SetData(PlayerInformation);
        if ((string)PlayerInformation["Rank"] != "Unranked")
        {
            PlayerRank.GetComponent<TextMeshPro>().text = "<color="+(string)PlayerInformation["RankColor"]+">" + (string)PlayerInformation["Rank"] + "</color>";
        }
        for (int i = 0; i < RankList.Count; i++)
        {
            GameObject game = Instantiate(Template, Template.transform.position, Quaternion.identity);
            game.name = RankList[i][1];
            game.transform.GetChild(0).GetComponent<TMP_Text>().text = "<color=" + RankList[i][7] + ">" + RankList[i][1] + "</color>";
            game.GetComponent<PersonalAreaItem>().ItemId = RankList[i][0];
            game.transform.SetParent(Content.transform);
            game.SetActive(true);
            if (int.Parse(RankList[i][0]) == (int)PlayerInformation["RankId"])
            {
                ShowRankInfo(RankList[i][0]);
            }
            
            LockItem(game, RankList[i][0]);
        }
        if ((string)PlayerInformation["DailyIncomeReceived"] == "N")
        {
            IsCollected = false;
            TimeRemaining.GetComponent<TextMeshPro>().text = "Your salary is here!";
            if (CollectButton.GetComponent<CursorUnallowed>() != null)
            {
                Destroy(CollectButton.AddComponent<CursorUnallowed>());
            }
        } else
        {
            IsCollected = true;
            CollectButton.transform.GetChild(1).gameObject.SetActive(true);
            if (CollectButton.GetComponent<CursorUnallowed>() == null)
            {
                CollectButton.AddComponent<CursorUnallowed>();
            }
        }
        
    }
    public void ResetData()
    {
       if (Content.transform.childCount > 1)
        {
            for (int i = 1; i < Content.transform.childCount; i++)
            {
                Destroy(Content.transform.GetChild(i).gameObject);
            }
        }
    }
    public void SetData(Dictionary<string, object> Data)
    {
        PlayerName.GetComponent<TextMeshPro>().text = (string)Data["Name"];
        Cash.transform.GetChild(1).GetComponent<TextMeshPro>().text = ((int)Data["Cash"]).ToString();
        TimelessShard.transform.GetChild(1).GetComponent<TextMeshPro>().text = ((int)Data["TimelessShard"]).ToString();
        FuelEnergy.transform.GetChild(1).GetComponent<TextMeshPro>().text = ((int)Data["FuelEnergy"]).ToString();
        FuelCell.transform.GetChild(0).GetChild(0).GetComponent<Slider>().maxValue = 10;
        FuelCell.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value = (int)Data["FuelCell"];
        if ((int)Data["FuelCell"] == 10)
        {
            FuelCell.transform.GetChild(2).gameObject.SetActive(false);
        }
    }
    #endregion
    #region Show rank's information (Daily income, condition to rank up,...)
    // Group all function that serve the same algorithm
    public void ShowRankInfo(string Id)
    {
        CurrentItem(Id);
        Content.transform.GetChild((int)PlayerInformation["RankId"]).GetComponent<Image>().color = Color.green + Color.red;
        string RankCondition = FindAnyObjectByType<GlobalFunctionController>().ConvertRankUpConditions(RankList[int.Parse(Id) - 1][2], RankList[int.Parse(Id) - 1][3], RankList[int.Parse(Id) - 1][4]);
        RankDesc.GetComponent<TMP_Text>().text =  RankCondition;
        RankSalary.GetComponent<TMP_Text>().text = RankList[int.Parse(Id) - 1][5] + " <sprite index='3'>";
        CurrentSalary.GetComponent<TMP_Text>().text = PlayerInformation["DailyIncome"] + " <sprite index='3'>";
    }
    #endregion
    #region Check current item
    public void CurrentItem(string Id)
    {
        for (int i = 1; i < Content.transform.childCount; i++)
        {
            Content.transform.GetChild(i).GetComponent<Image>().color = Color.white;
        }
        Content.transform.GetChild(int.Parse(Id)).GetComponent<Image>().color = Color.yellow;
    }
    #endregion
    #region Lock item
    public void LockItem(GameObject game, string Id)
    {
        if (int.Parse(PlayerInformation["RankId"].ToString()) < int.Parse(RankList[int.Parse(Id) - 1][0]))
        {
            game.transform.GetChild(1).gameObject.SetActive(true);
        }
    }
    #endregion
    #region Set a timer after collecting
    // Group all function that serve the same algorithm
    public void SetTimer()
    {
        CollectedTime = DateTime.Now;
        ResetDateTime = CollectedTime.AddDays(1).Date + new TimeSpan(6, 0, 0);
        timeRemaining = ResetDateTime - CollectedTime;
        formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}", timeRemaining.Hours, timeRemaining.Minutes, timeRemaining.Seconds);
        TimeRemaining.GetComponent<TextMeshPro>().text = formattedTime;
    }
    #endregion
}
