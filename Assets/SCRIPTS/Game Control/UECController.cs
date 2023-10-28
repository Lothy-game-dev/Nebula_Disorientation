using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;

public class UECController : UECMenuShared
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Cash;
    public GameObject TimelessShard;
    public GameObject FuelCell;
    public GameObject FuelEnergy;
    public GameObject NameRankBox;
    public GameObject DailyMissionBar;
    public GameObject EnterBattleButton;
    #endregion
    #region NormalVariables
    public bool isPlanetMoving;
    public GameObject FuelCellInfo;
    private AccessDatabase ad;
    private int currentId;
    private UECMainMenuController controller;
    private string LastTimeFuel;
    public string RegenFuelTime;
    private Dictionary<string, object> Data;
    private float timer;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    private void OnEnable()
    {
        FindObjectOfType<MainMenuCameraController>().GenerateLoadingScene(1f);
    }
    void Start()
    {
        // Initialize variables
        isPlanetMoving = true;
        ad = FindObjectOfType<AccessDatabase>();
        controller = FindAnyObjectByType<UECMainMenuController>();
        CheckDailyMission();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        CheckDailyMission();
        timer -= Time.deltaTime;
        if (timer<=0f)
        {
            timer = 1f;
            UpdateCheckFuel();
        }
    }
    #endregion
    #region Data

    private void UpdateCheckFuel()
    {
        if (FindObjectOfType<UECMainMenuController>().PlayerId!=0)
        {
            Data = ad.GetPlayerInformationById(FindObjectOfType<UECMainMenuController>().PlayerId);
            LastTimeFuel = (string)Data["LastFuelCellUsed"];
            if (LastTimeFuel==null || LastTimeFuel.Length==0)
            {
                RegenFuelTime = "";
            } else
            {
                CultureInfo culture = CultureInfo.InvariantCulture;
                System.DateTime LastTime = System.DateTime.ParseExact(LastTimeFuel, "dd/MM/yyyy HH:mm:ss", culture);
                System.DateTime date = System.DateTime.Now;
                double Result = (date - LastTime).TotalSeconds;
                if (Result <= 0)
                {
                    RegenFuelTime = "";
                } else
                {
                    while (Result>43200)
                    {
                        Result = Result - 43200;
                        // Add Fuel cell
                        FindObjectOfType<AccessDatabase>().AddFuelCell(FindObjectOfType<UECMainMenuController>().PlayerId);
                        Data = ad.GetPlayerInformationById(FindObjectOfType<UECMainMenuController>().PlayerId);
                        if ((int)Data["FuelCell"]==10)
                        {
                            break;
                        }
                    }
                    Result = (int)(43200 - Result);
                    if (Result == 0)
                    {
                        // Add Fuel cell
                        FindObjectOfType<AccessDatabase>().AddFuelCell(FindObjectOfType<UECMainMenuController>().PlayerId);
                    }
                    Data = ad.GetPlayerInformationById(FindObjectOfType<UECMainMenuController>().PlayerId);
                    FuelCell.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value = (int)Data["FuelCell"];
                    if ((int)Data["FuelCell"] == 10)
                    {
                        FuelCell.transform.GetChild(2).gameObject.SetActive(false);
                    }
                    System.TimeSpan t = System.TimeSpan.FromSeconds(Result);
                    System.DateTime final = new System.DateTime().Add(t);
                    RegenFuelTime = "Restore 1 in<br>" + final.ToString("HH:mm:ss");
                }
            }
            if (FuelCellInfo != null)
            {
                FuelCellInfo.transform.GetChild(1).GetComponent<TextMeshPro>().text =
                    (RegenFuelTime != "" ?
                     "Fuel Core. Energy for teleporting back to UEC during Mission.<br>" + RegenFuelTime
                     : "Fuel Core. Energy for teleporting back to UEC during Mission.");
            }
        }
    }
    public void SetDataToView(Dictionary<string, object> datas)
    {
        currentId = (int)datas["ID"];
        Cash.transform.GetChild(1).GetComponent<TextMeshPro>().text = ((int)datas["Cash"]).ToString();
        TimelessShard.transform.GetChild(1).GetComponent<TextMeshPro>().text = ((int)datas["TimelessShard"]).ToString();
        FuelEnergy.transform.GetChild(1).GetComponent<TextMeshPro>().text = ((int)datas["FuelEnergy"]).ToString();
        FuelCell.transform.GetChild(0).GetChild(0).GetComponent<Slider>().maxValue = 10;
        FuelCell.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value = (int)datas["FuelCell"];
        if ((int)datas["FuelCell"]==10)
        {
            FuelCell.transform.GetChild(2).gameObject.SetActive(false);
        }
        NameRankBox.transform.GetChild(0).GetComponent<TextMeshPro>().text = GenerateNameRankText((string)datas["Name"], (string)datas["Rank"], (datas.ContainsKey("RankColor")? (string)datas["RankColor"]:"") );
    }

    private string GenerateNameRankText(string name, string rank, string rankColor)
    {
        System.DateTime currentTime = System.DateTime.Now;
        string text = "";
        string ranking = "";
        if ("unranked".Equals(rank.ToLower()))
        {
            ranking = "Pilot";
        }
        else
        {
            ranking = "<color=" + rankColor + ">" + rank + "</color>";
        }
        string time = currentTime.ToString("HH:mm");
        int hour;
        try
        {
            hour = int.Parse(time.Split(":")[0]);
        } catch
        {
            hour = -1;
        }
        string[] morningQuotes = { "Good morning", "Have a nice day,", "Getting up too early, huh" };
        string[] morningQuotesAfter = { "!", "!", "?" };
        string[] afternoonQuotes = { "Good afternoon", "What an afternoon to be at the UEC, right", "Get some nap," };
        string[] afternoonQuotesAfter = { "!", "?", "!" };
        string[] eveningQuotes = { "Good evening", "Have you got your dinner yet, ", "Time to study some new mechanics," };
        string[] eveningQuotesAfter = { "!", "?", "!" };
        string[] nightQuotes = { "Nighty night,", "Have you done your homework yet,", "Sleep when," };
        string[] nightQuotesAfter = { "!", "?", "?" };
        if (hour>=6 && hour < 11)
        {
            int n = Random.Range(0, 3);
            text += morningQuotes[n] + " " + ranking + " " + name + morningQuotesAfter[n];
        } else if (hour >= 11 && hour < 17)
        {
            int n = Random.Range(0, 3);
            text += afternoonQuotes[n] + " " + ranking + " " + name + afternoonQuotesAfter[n];
        } else if (hour >= 17 && hour < 21)
        {
            int n = Random.Range(0, 3);
            text += eveningQuotes[n] + " " + ranking + " " + name + eveningQuotesAfter[n];
        } else if (hour >= 21 || hour<6)
        {
            int n = Random.Range(0, 3);
            text += nightQuotes[n] + " " + ranking + " " + name + nightQuotesAfter[n];
        }
        return text;
    }

    private void CheckDailyMission()
    {
        int n = ad.NumberOfDailyMissionById(currentId);
        bool checkDM = true;
        if (n<2)
        {
            string check = ad.GenerateDailyMission(currentId, 2 - n);
            if ("Fail".Equals(check))
            {
                checkDM = false;
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
                    "Failed to fetch daily missions' datas.\n Please contact to our email for more help.", 5f);
            }
        }
        if (checkDM)
        {
            List<List<string>> listDM = ad.GetListDailyMissionUndone(currentId);
            List<string> Missions = new List<string>();
            if (listDM != null)
            {
                for (int i = 0; i < listDM[0].Count; i++)
                {
                    string mission = "";
                    switch (listDM[0][i])
                    {
                        case "KE":
                            mission = "Kill " + listDM[1][i] + " enemy(s).";
                            break;
                        case "KB":
                            mission = "Kill " + listDM[1][i] + " boss enemy(s).";
                            break;
                        case "C":
                            mission = "Complete " + listDM[1][i] + " spacezone(s).";
                            break;
                        case "S":
                            mission = "Spend " + listDM[1][i] + " cash(s) during a session.";
                            break;
                        case "P":
                            mission = "Play for at least " + listDM[1][i] + " minute(s).";
                            if (int.Parse(listDM[2][i]) < int.Parse(listDM[1][i]))
                            {
                                if (controller.isCount)
                                {
                                    ad.UpdateDailyMissionProgess(currentId, listDM[0][i], 1);
                                }
                            }
                            else
                            {
                                ad.DailyMissionDone(currentId, listDM[0][i]);
                                FindAnyObjectByType<RankController>().CheckToRankUp();
                                FindAnyObjectByType<NotificationBoardController>().CreateMissionCompletedNotiBoard(mission, 2f);
                            }
                            break;
                        case "CD":
                            mission = "Complete " + listDM[1][i] + " Defend/Escort Spacezone(s).";
                            break;
                        case "CA":
                            mission = "Complete " + listDM[1][i] + " Assault Spacezone(s).";
                            break;
                        case "CAA":
                            mission = "Complete " + listDM[1][i] + " Assault Advanced Spacezone(s).";
                            break;
                        case "B":
                            mission = "Purchase " + listDM[1][i] + " item(s).";
                            if (int.Parse(listDM[2][i]) < int.Parse(listDM[1][i]))
                            {
                                if (controller.BuyAmount > 0)
                                {
                                    ad.UpdateDailyMissionProgess(currentId, listDM[0][i], controller.BuyAmount);
                                }
                            }
                            else
                            {
                                ad.DailyMissionDone(currentId, listDM[0][i]);
                                FindAnyObjectByType<RankController>().CheckToRankUp();
                                FindAnyObjectByType<NotificationBoardController>().CreateMissionCompletedNotiBoard(mission, 2f);
                            }
                            break;
                        default: break;
                    }
                    mission += "\n(" + listDM[2][i] + "/" + listDM[1][i] + ")";
                    Missions.Add(mission);
                }
            }           
            DailyMissionBar.GetComponent<UECDailyMissions>().missions = Missions;
            controller.BuyAmount = 0;
            controller.isCount = false;
        }
    }
    #endregion
    #region Animation
    public override void OnEnterAnimation()
    {
        GetComponent<BackgroundBrieflyMoving>().enabled = true;
        transform.GetChild(0).GetComponent<Rigidbody2D>().simulated = true;
        isPlanetMoving = true;
        FindObjectOfType<MainMenuCameraController>().GenerateLoadingSceneAtPos(transform.position, 1f);
    }
    public override void OnExitAnimation()
    {
        GetComponent<BackgroundBrieflyMoving>().enabled = false;
        transform.GetChild(0).GetComponent<Rigidbody2D>().simulated = false;
        isPlanetMoving = false;
    }
    #endregion
}
