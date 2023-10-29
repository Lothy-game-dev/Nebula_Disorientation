using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatisticController : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject MissionCompeletedBoard;
    public GameObject Fighter;
    public TextMeshPro Shard;
    public TextMeshPro Cash;
    public TextMeshPro Timer;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public int EnemyTierI;
    public int EnemyTierII;
    public int EnemyTierIII;
    public int Warship;
    public int MaxSZReach;
    public float DamageDealt;
    public int TotalEnemyDefeated;
    public string CurrentShard;
    public string CurrentCash;
    public float CurrentHP;
    public float MaxHP;
    public string PlayedTime;
    public int CurrentSZNo;
    public int CurrentFuelCell;
    public int MissionCompleted;
    private Dictionary<string, object> PlayerAchievement;
    private Dictionary<string, object> Achievement;
    private Dictionary<string, string> CurrentAchievement;
    private List<string> AchievementName;
    private Dictionary<string, object> SessionInformation;
    public bool KillEnemy;
    public bool KillBossEnemy;
    private AccessDatabase ad;
    private int PlayerID;
    public string StageName;
    private float InitScale;
    private int Playedtime;
    private bool isCount;
    public bool isStart;
    public DateTime StartTime;
    private string Cons;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        ad = FindAnyObjectByType<AccessDatabase>();
        Achievement = new Dictionary<string, object>();
        AchievementName = new List<string>() {"PlayerID" ,"EnemyDefeated", "SZMaxReach" };
        foreach (var name in AchievementName)
        {
            Achievement.Add(name, 0);
        }
        PlayerID = PlayerPrefs.GetInt("PlayerID");
        PlayerAchievement = ad.GetPlayerAchievement(PlayerID);
        CurrentAchievement = FindAnyObjectByType<GlobalFunctionController>().ConvertEnemyDefeated(PlayerAchievement["EnemyDefeated"].ToString());
        EnemyTierI = int.Parse(CurrentAchievement["EnemyTierI"]);
        EnemyTierII = int.Parse(CurrentAchievement["EnemyTierII"]);
        EnemyTierIII = int.Parse(CurrentAchievement["EnemyTierIII"]);
        Warship = int.Parse(CurrentAchievement["Warship"]);
        MaxSZReach = int.Parse(PlayerAchievement["MaxSZReach"].ToString());
        MissionCompleted = int.Parse(PlayerAchievement["TotalMission"].ToString());
        TotalEnemyDefeated = int.Parse(PlayerAchievement["TotalEnemyDefeated"].ToString());
        InitScale = MissionCompeletedBoard.transform.GetChild(0).localScale.x;
        // Set data to fuel cell bar
        Dictionary<string, object> ListData = ad.GetPlayerInformationById(PlayerPrefs.GetInt("PlayerID"));
        CurrentFuelCell = (int)ListData["FuelCell"];
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (isStart)
        {
            SetTimer(StartTime);
        }
        CheckDailyMission();
    }
    #endregion
    #region Update Achievement
    // Group all function that serve the same algorithm
    public void UpdateStatistic()
    {
        //Update Statistic
        CurrentHP = Fighter.GetComponent<PlayerFighter>().CurrentHP;
        MaxHP = Fighter.GetComponent<PlayerFighter>().MaxHP;
        CurrentShard = Shard.text;
        CurrentCash = Cash.text;
        if (!Timer.transform.parent.gameObject.GetComponent<SpaceZoneTimer>().Countdown)
        {
            PlayedTime = Timer.text;
        } else
        {
            PlayedTime = "1:00";
        }
        CurrentSZNo = FindAnyObjectByType<SpaceZoneGenerator>().SpaceZoneNo;
        string EnemyDefeated = "EI-" + EnemyTierI + "|EII-" + EnemyTierII + "|EIII-" + EnemyTierIII + "|WS-" + Warship;
        SessionInformation = ad.GetSessionInfoByPlayerId(PlayerID);
        if (MaxSZReach < CurrentSZNo)
        {
            MaxSZReach = CurrentSZNo;
        }
        Achievement["EnemyDefeated"] = EnemyDefeated;
        Achievement["SZMaxReach"] = MaxSZReach;
        Achievement["PlayerID"] = PlayerID;

        int index = 0;
        foreach (var x in Fighter.GetComponent<PlayerFighter>().Consumables)
        {
            if (index == 0)
            {
                Cons += x.Key + "-" + x.Value;
            } else
            {
                Cons += "|" + x.Key + "-" + x.Value;
            }
            index++;
        }

        ad.UpdateGameplayStatistic(Achievement);

        //Update session current HP
        ad.UpdateSessionStat(Mathf.RoundToInt(CurrentHP), TotalEnemyDefeated, Mathf.RoundToInt(DamageDealt), (int)SessionInformation["SessionID"], Cons);
    }
    #endregion
    #region Check daily mission
    // Group all function that serve the same algorithm
    private void CheckDailyMission()
    {       
        List<List<string>> listDM = ad.GetListDailyMissionUndone(PlayerID);
        if (listDM != null)
        {
            for (int i = 0; i < listDM[0].Count; i++)
            {
                string mission = "";
                switch (listDM[0][i])
                {
                    case "KE":
                        mission = "Kill " + listDM[1][i] + " enemy(s).";
                        if (int.Parse(listDM[1][i]) > int.Parse(listDM[2][i]))
                        {
                            if (KillEnemy)
                            {
                                ad.UpdateDailyMissionProgess(PlayerID, listDM[0][i], 1);
                            }
                        }
                        else
                        {
                            ad.DailyMissionDone(PlayerID, listDM[0][i]);                          
                            FindAnyObjectByType<NotificationBoardController>().CreateMissionCompletedNotiBoard(mission, 2f);
                        }
                        break;
                    case "KB":
                        mission = "Kill " + listDM[1][i] + " boss enemy.";
                        if (int.Parse(listDM[1][i]) > int.Parse(listDM[2][i]))
                        {
                            if (KillBossEnemy)
                            {
                                ad.UpdateDailyMissionProgess(PlayerID, listDM[0][i], 1);
                            }
                        }
                        else
                        {
                            ad.DailyMissionDone(PlayerID, listDM[0][i]);
                            FindAnyObjectByType<NotificationBoardController>().CreateMissionCompletedNotiBoard(mission, 2f);
                        }
                        break;
                    case "P":
                        mission = "Play for at least " + listDM[1][i] + " minute(s).";
                        if (int.Parse(listDM[2][i]) < int.Parse(listDM[1][i]))
                        {                           
                            if (isCount)
                            {
                                ad.UpdateDailyMissionProgess(PlayerID, listDM[0][i], 1);
                                
                            }
                        }
                        else
                        {
                            ad.DailyMissionDone(PlayerID, listDM[0][i]);
                            FindAnyObjectByType<NotificationBoardController>().CreateMissionCompletedNotiBoard(mission, 2f);
                        }
                        break;
                    default: break;
                }
            }
        }       
        KillEnemy = false;
        KillBossEnemy = false;
        isCount = false;
    }

    public void SetTimer(DateTime startTime)
    {
        int oldTime = Playedtime;
        DateTime myDateTime = DateTime.Now;
        TimeSpan currentTimePlayed = myDateTime - startTime;
        Playedtime = (int)currentTimePlayed.TotalMinutes;
        Debug.Log(Playedtime);
        if (oldTime < Playedtime)
        {
            isCount = true;
        }
    }
    #endregion
}
