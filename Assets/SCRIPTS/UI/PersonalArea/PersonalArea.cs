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
    public List<GameObject> Achievements;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public List<List<string>> RankList;
    public Dictionary<string, object> PlayerInformation;
    public int PlayerId;
    private DateTime ResetDateTime;
    private DateTime CollectedTime;
    private DateTime CurrentTime;
    private string formattedTime;
    public bool IsCollected;
    private TimeSpan timeRemaining;
    private int WeapOwned;
    private int ModelOwned;
    public bool isUnranked;
    public Dictionary<string, object> PlayerAchievement;
    public Dictionary<string, string> Achievement;
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
        PlayerAchievement = FindAnyObjectByType<AccessDatabase>().GetPlayerAchievement(PlayerId);
        Achievement = FindAnyObjectByType<GlobalFunctionController>().ConvertEnemyDefeated(PlayerAchievement["EnemyDefeated"].ToString());
        SetData(PlayerInformation);
        if ((string)PlayerInformation["Rank"] != "Unranked")
        {
            PlayerRank.GetComponent<TextMeshPro>().text = "<color="+(string)PlayerInformation["RankColor"]+">" + (string)PlayerInformation["Rank"] + "</color>";
        } else
        {
            isUnranked = true;
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
        // check daily
        if ((string)PlayerInformation["DailyIncomeReceived"] == "N")
        {
            IsCollected = false;
            TimeRemaining.GetComponent<TextMeshPro>().text = "Your salary is here!";
            if (CollectButton.GetComponent<CursorUnallowed>() != null)
            {
                Destroy(CollectButton.GetComponent<CursorUnallowed>());
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
        // check if time system has changed to collect salary
        if (FindAnyObjectByType<AccessDatabase>().CheckIfCollected(PlayerId, System.DateTime.Now.ToString("dd/MM/yyyy")) == 0)
        {
            IsCollected = false;
        } else
        {
            CollectButton.transform.GetChild(1).gameObject.SetActive(true);
            if (CollectButton.GetComponent<CursorUnallowed>() == null)
            {
                CollectButton.AddComponent<CursorUnallowed>();
            }
            IsCollected = true;
        }

        for (int i = 0; i < Achievements.Count; i++)
        {
            if (i == 0)
            {
                Achievements[i].GetComponent<TextMeshPro>().text += PlayerAchievement[Achievements[i].name].ToString();
            } else
            {
                Achievements[i].GetComponent<TextMeshPro>().text += Achievement[Achievements[i].name];
            }
        }
        
    }
    public void ResetData()
    {
       if (Content.transform.childCount > 1)
        {
            for (int i = 0; i < Content.transform.childCount; i++)
            {
                if (i == 0)
                {
                    Content.transform.GetChild(i).GetComponent<Image>().color = Color.white;
                } else
                {
                    Destroy(Content.transform.GetChild(i).gameObject);
                }
            }
        }

        for (int i = 0; i < Achievements.Count; i++)
        {
            if (i == 0)
            {
                Achievements[i].GetComponent<TextMeshPro>().text = "Max SZ Reached: ";
            }
            else
            {
                Achievements[i].GetComponent<TextMeshPro>().text = Achievements[i].GetComponent<TextMeshPro>().text.Remove(Achievements[i].GetComponent<TextMeshPro>().text.IndexOf(Achievement[Achievements[i].name]));
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
        CurrentSalary.GetComponent<TMP_Text>().text = Data["DailyIncome"] + " <sprite index='3'> " + (int.Parse(Data["DailyIncomeShard"].ToString()) == 0 ? "" : Data["DailyIncomeShard"] + " <sprite index='0'> ");
        if ((int)Data["FuelCell"] == 10)
        {
            FuelCell.transform.GetChild(2).gameObject.SetActive(false);
        }
        if ((string)Data["Rank"] != "Unranked")
        {
            isUnranked = false;
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
        RankSalary.GetComponent<TMP_Text>().text = RankList[int.Parse(Id) - 1][5] + " <sprite index='3'>  " + (int.Parse(RankList[int.Parse(Id) - 1][8]) == 0 ? "" : RankList[int.Parse(Id) - 1][8] + " <sprite index='0'> ");
        if (!isUnranked)
        {
            CurrentSalary.GetComponent<TMP_Text>().text = PlayerInformation["DailyIncome"] + " <sprite index='3'>  " + (int.Parse(PlayerInformation["DailyIncomeShard"].ToString()) == 0 ? "" : PlayerInformation["DailyIncomeShard"] + " <sprite index='0'> ");
        }
    }
    #endregion
    #region Check current item
    public void CurrentItem(string Id)
    {
        for (int i = 0; i < Content.transform.childCount; i++)
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
        int check = FindAnyObjectByType<AccessDatabase>().CheckIfCollected(PlayerId, System.DateTime.Now.ToString("dd/MM/yyyy"));
        CollectedTime = DateTime.Now;
        ResetDateTime = CollectedTime.AddDays(1).Date + new TimeSpan(6, 0, 0);
        timeRemaining = ResetDateTime - CollectedTime;
        formattedTime = string.Format("{0:D2}:{1:D2}:{2:D2}", timeRemaining.Hours, timeRemaining.Minutes, timeRemaining.Seconds);
        TimeRemaining.GetComponent<TextMeshPro>().text = formattedTime;
        if (CollectedTime.Hour == 6 && CollectedTime.Minute == 0 && CollectedTime.Second == 0 && check == 0)
        {
            ResetDaily();
            IsCollected = false;
        }

    }
    #endregion
    #region Reset daily income
    public void ResetDaily()
    {
        FindAnyObjectByType<AccessDatabase>().ResetDailyIncome();
        TimeRemaining.GetComponent<TextMeshPro>().text = "Your salary is here!";
        if (CollectButton.GetComponent<CursorUnallowed>() != null)
        {
            Destroy(CollectButton.GetComponent<CursorUnallowed>());
            CollectButton.transform.GetChild(1).gameObject.SetActive(false);
        }
    }
    #endregion
}
