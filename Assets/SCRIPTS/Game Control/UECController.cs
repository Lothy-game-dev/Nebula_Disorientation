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
        Camera.main.transparencySortAxis = new Vector3(0, 0, 1);
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
        
    }
    #endregion
    #region Data
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
        if (n<4)
        {
            string check = ad.GenerateDailyMission(currentId, 4 - n);
            if ("Fail".Equals(check))
            {
                checkDM = false;
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
                    "Failed to fetch daily missions' datas.\n Please contact to our email for more help.", 5f);
            }
        }
        if (checkDM)
        {
            List<List<string>> listDMUndone = ad.GetListDailyMissionUndone(currentId);
            List<List<string>> listDM = ad.GetListDailyMission(currentId);
            List<string> Missions = new List<string>();
            if (listDM != null)
            {
                for (int i = 0; i < listDM[0].Count; i++)
                {
                    string mission = "";
                    string missionColor = "red";
                    switch (listDM[0][i])
                    {
                        case "KE":
                            if (listDM[3][i] == "Y")
                            {
                                missionColor = "#008000";
                            }
                            mission = "<color="+ missionColor + "> Eliminate " + listDM[1][i] + " enemies.";
                            break;
                        case "SS":
                            if (listDM[3][i] == "N" )
                            {
                                if (int.Parse(listDM[2][i]) < int.Parse(listDM[1][i]))
                                {
                                    if (controller.ShardSpent > 0)
                                    {
                                        ad.UpdateDailyMissionProgess(currentId, listDM[0][i], controller.ShardSpent);
                                        controller.ShardSpent = 0;
                                    }
                                }
                                else
                                {
                                    ad.DailyMissionDone(currentId, listDM[0][i]);
                                    controller.GetData();
                                    FindAnyObjectByType<NotificationBoardController>().CreateMissionCompletedNotiBoard(mission, 2f);
                                }
                            } else
                            {
                                missionColor = "#00ff00";
                            }
                            mission = "<color=" + missionColor + "> Spend " + listDM[1][i] + " timeless shard.";
                            break;
                        case "C":
                            if (listDM[3][i] == "Y")
                            {
                                missionColor = "#00ff00";
                            }
                            mission = "<color=" + missionColor + "> Complete " + listDM[1][i] + " spacezones.";
                            break;
                        case "SC":
                            mission = "<color=" + missionColor + "> Spend " + listDM[1][i] + " cash.";
                            if (listDM[3][i] == "N")
                            {
                                if (int.Parse(listDM[2][i]) < int.Parse(listDM[1][i]))
                                {
                                    if (controller.CashSpent > 0)
                                    {
                                        ad.UpdateDailyMissionProgess(currentId, listDM[0][i], controller.CashSpent);
                                        controller.CashSpent = 0;
                                    }
                                }
                                else
                                {
                                    ad.DailyMissionDone(currentId, listDM[0][i]);
                                    controller.GetData();
                                    FindAnyObjectByType<NotificationBoardController>().CreateMissionCompletedNotiBoard(mission, 2f);
                                }
                            } else
                            {
                                missionColor = "#008000";
                            }
                            break;
                        case "P":
                            mission = "<color=" + missionColor + "> Play for at least " + listDM[1][i] + " minutes.";
                            if (listDM[3][i] == "N")
                            {
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
                                    controller.GetData();
                                    FindAnyObjectByType<RankController>().CheckToRankUp();
                                    FindAnyObjectByType<NotificationBoardController>().CreateMissionCompletedNotiBoard(mission, 2f);
                                }
                            } else
                            {
                                missionColor = "#008000";
                            }
                            break;
                        case "CD":
                            if (listDM[3][i] == "Y")
                            {
                                missionColor = "#008000";
                            }
                            mission = "<color=" + missionColor + "> Complete " + listDM[1][i] + " Defend/Escort Spacezones.";
                            break;
                        case "CA":
                            if (listDM[3][i] == "Y")
                            {
                                missionColor = "#008000";
                            }
                            mission = "<color=" + missionColor + "> Complete " + listDM[1][i] + " Assault Spacezones.";
                            break;
                        case "CAA":
                            if (listDM[3][i] == "Y")
                            {
                                missionColor = "#008000";
                            }
                            mission = "<color=" + missionColor + "> Complete " + listDM[1][i] + " Assault Advanced Spacezone.";
                            break;
                        case "B":
                            if (listDM[3][i] == "N")
                            {
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
                                    controller.GetData();
                                    FindAnyObjectByType<RankController>().CheckToRankUp();
                                    FindAnyObjectByType<NotificationBoardController>().CreateMissionCompletedNotiBoard(mission, 2f);
                                }
                            } else
                            {
                                missionColor = "#008000";
                            }
                            mission = "<color=" + missionColor + "> Purchase " + listDM[1][i] + " items.";
                            break;
                        default: break;
                    }
                    mission += "\n(" + listDM[2][i] + "/" + listDM[1][i] + ") </color>";
                    Missions.Add(mission);
                }
            }           
            DailyMissionBar.GetComponent<UECDailyMissions>().missions = Missions;
            DailyMissionBar.GetComponent<UECDailyMissions>().MissionUndone = (listDMUndone == null ? 0 : listDMUndone.Count);
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
