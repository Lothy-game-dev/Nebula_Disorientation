using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
    public int ShardCollected;
    public int CashCollected;
    public string SessionPlayTime;
    private Dictionary<string, object> PlayerAchievement;
    private Dictionary<string, object> Achievement;
    private Dictionary<string, string> CurrentAchievement;
    private List<string> AchievementName;
    public Dictionary<string, object> SessionInformation;
    public bool KillEnemy;
    public bool KillBossEnemy;
    private AccessDatabase ad;
    public int PlayerID;
    public int SessionID;
    public string StageName;
    private float InitScale;
    private int Playedtime;
    private bool isCount;
    public bool isStart;
    public DateTime StartTime;
    private string Cons;
    private Dictionary<string, int> Consumable;
    public int CurrentShardReward;
    public int CurrentCashReward;
    private TimeSpan currentTimePlayed;
    private int SessionTotalMinutes;
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
        InitScale = MissionCompeletedBoard.transform.GetChild(0).localScale.x;
        TotalEnemyDefeated = 0;
        // Set data to fuel cell bar
        Consumable = new Dictionary<string, int>();
        SessionInformation = ad.GetSessionInfoByPlayerId(PlayerID);
        CurrentFuelCell = (int)SessionInformation["SessionFuelCore"];
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (isStart)
        {
            SetTimer(StartTime);
        }
        //CheckDailyMission();
    }
    #endregion
    #region Update Statistic
    // Group all function that serve the same algorithm
    public void UpdateStatistic()
    {
        //Update Statistic
        CurrentHP = Fighter.GetComponent<PlayerFighter>().CurrentHP;
        MaxHP = Fighter.GetComponent<PlayerFighter>().MaxHP;
        CurrentShard = Shard.text;
        CurrentCash = Cash.text;
        PlayedTime = currentTimePlayed.ToString(@"mm\:ss");     
        CurrentSZNo = FindAnyObjectByType<SpaceZoneGenerator>().SpaceZoneNo;
        string EnemyDefeated = "EI-" + EnemyTierI + "|EII-" + EnemyTierII + "|EIII-" + EnemyTierIII + "|WS-" + Warship;
        SessionID = (int)SessionInformation["SessionID"];
        if (MaxSZReach < CurrentSZNo)
        {
            MaxSZReach = CurrentSZNo;
        }
        Achievement["EnemyDefeated"] = EnemyDefeated;
        Achievement["SZMaxReach"] = MaxSZReach;
        Achievement["PlayerID"] = PlayerID;
     
        ad.UpdateGameplayStatistic(Achievement);

        string Consum = (string)SessionInformation["Consumables"];
        if (Consum.Length > 0)
        {
            string[] ListCons = Consum.Split("|");
            for (int i = 0; i < ListCons.Length; i++)
            {
                Consumable.Add(ListCons[i].Split("-")[0], int.Parse(ListCons[i].Split("-")[1]));
            }
        }

        if (Fighter.GetComponent<PlayerFighter>().Consumables != null)
        {
            int index = 0;
            foreach (var x in Fighter.GetComponent<PlayerFighter>().Consumables)
            {
                if (x.Value != 0)
                {
                    if (index == 0)
                    {
                        Cons += x.Key + "-" + x.Value;
                    }
                    else
                    {
                        Cons += "|" + x.Key + "-" + x.Value;
                    }
                    index++;
                }
                ad.DecreaseSessionOwnershipToItem(PlayerPrefs.GetInt("PlayerID"), x.Key, "Consumable", Consumable[x.Key] - x.Value);
            }
        }

        //Mission Reward
        // Stage 10,20,30,....
        if (CurrentSZNo % 10 == 0)
        {
            CurrentShardReward = (CurrentSZNo / 20) + 1;           
        } else
        {
            CurrentShardReward = 0;
        }
        CurrentCashReward = 500 * (((CurrentSZNo / 20 + (CurrentSZNo / 10) % 2) / 2) / 2 + 1);
       
        //Update session stat
        ad.UpdateSessionStat(Mathf.RoundToInt(CurrentHP), TotalEnemyDefeated, Mathf.RoundToInt(DamageDealt), (int)SessionInformation["SessionID"], Cons);
        ad.UpdateSessionFuelEnergy(SessionID, true, (int)SessionInformation["EnemyDestroyed"]);
    }
    #endregion
    #region Check daily mission
    // Group all function that serve the same algorithm
    public void CheckDailyMission()
    {       
        List<List<string>> listDM = ad.GetListDailyMissionUndone(PlayerID);
        int missionDone = 0;
        if (listDM != null)
        {
            for (int i = 0; i < listDM.Count; i++)
            {
                string mission = "";
                switch (listDM[i][1])
                {
                    case "KE":
                        mission = "Eliminate  " + listDM[i][2] + " enemy(s).";
                        ad.UpdateDailyMissionProgess(PlayerID, listDM[i][1], TotalEnemyDefeated);
                        if (int.Parse(listDM[i][2]) <= int.Parse(listDM[i][0]) + TotalEnemyDefeated)
                        {
                            ad.DailyMissionDone(PlayerID, listDM[i][1]);
                            missionDone++;
                            FindAnyObjectByType<NotificationBoardController>().CreateMissionDoneNoti(mission, 1f, missionDone);
                        }
                        break;
                    case "P":
                        mission = "Play for at least " + listDM[i][2] + " minute(s).";
                        if (SessionInformation != null && (string)SessionInformation["TotalPlayedTime"] != "")
                        {
                            string timeString = (string)SessionInformation["TotalPlayedTime"];
                            string[] timeParts = timeString.Split(':');
                            int hours = int.Parse(timeParts[0]);
                            int minutes = int.Parse(timeParts[1]);
                            int seconds = int.Parse(timeParts[2]);
                            SessionTotalMinutes = hours * 60 + minutes + seconds / 60;
                        }
                        ad.UpdateDailyMissionProgess(PlayerID, listDM[i][1], SessionTotalMinutes);                                                        
                        if (int.Parse(listDM[i][2]) <= int.Parse(listDM[i][0]) + SessionTotalMinutes)                       
                        {
                            ad.DailyMissionDone(PlayerID, listDM[i][1]);
                            missionDone++;
                            FindAnyObjectByType<NotificationBoardController>().CreateMissionDoneNoti(mission, 1f, missionDone);
                        }
                        break;
                    case "C":
                        mission = "Complete " + listDM[i][2] + " Space Zones.";
                        if (StageName != null && StageName != "")
                        {
                            if (StageName.Contains("D") || StageName.Contains("A") || StageName.Contains("O"))
                            {
                                ad.UpdateDailyMissionProgess(PlayerID, listDM[i][1], 1);                                                        
                            }
                            if (int.Parse(listDM[i][2]) <= int.Parse(listDM[i][0]) + 1)                       
                            {
                                ad.DailyMissionDone(PlayerID, listDM[i][1]);
                                missionDone++;
                                FindAnyObjectByType<NotificationBoardController>().CreateMissionDoneNoti(mission, 1f, missionDone);
                            }
                        }
                        break;
                    case "CD":
                        mission = "Complete " + listDM[i][2] + " Defend Space Zones.";
                        if (StageName != null && StageName != "")
                        {
                            if (StageName.Contains("D"))
                            {
                                ad.UpdateDailyMissionProgess(PlayerID, listDM[i][1], 1);
                                if (int.Parse(listDM[i][2]) <= int.Parse(listDM[i][0]) + 1)
                                {
                                    ad.DailyMissionDone(PlayerID, listDM[i][0]);
                                    missionDone++;
                                    FindAnyObjectByType<NotificationBoardController>().CreateMissionDoneNoti(mission, 1f, missionDone);
                                }
                            }
                        }
                        break;
                    case "CA":
                        mission = "Complete " + listDM[i][2] + " Assault Space Zones.";
                        if (StageName != null && StageName != "")
                        {
                            if (StageName.Contains("A"))
                            {
                                ad.UpdateDailyMissionProgess(PlayerID, listDM[i][1], 1);
                                if (int.Parse(listDM[i][2]) <= int.Parse(listDM[i][0]) + 1)
                                {
                                    ad.DailyMissionDone(PlayerID, listDM[i][1]);
                                    missionDone++;
                                    FindAnyObjectByType<NotificationBoardController>().CreateMissionDoneNoti(mission, 1f, missionDone);
                                }
                            }
                        }
                        break;
                    case "CAA":
                        mission = "Complete " + listDM[i][2] + " Onslaught Space Zone.";
                        if (StageName != null && StageName != "")
                        {
                            if (StageName.Contains("O"))
                            {
                                ad.UpdateDailyMissionProgess(PlayerID, listDM[i][1], 1);
                                if (int.Parse(listDM[i][2]) <= int.Parse(listDM[i][0]) + 1)
                                {
                                    ad.DailyMissionDone(PlayerID, listDM[i][1]);
                                    missionDone++;
                                    FindAnyObjectByType<NotificationBoardController>().CreateMissionDoneNoti(mission, 1f, missionDone);
                                }
                            }
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
        currentTimePlayed = myDateTime - startTime;      
        Playedtime = (int)currentTimePlayed.TotalMinutes;
        if (oldTime < Playedtime)
        {
            isCount = true;
        }
    }
    #endregion
    #region Price Collected
    public void PriceCollected(int cash, int shard)
    {
        CashCollected += cash;
        ShardCollected += shard;
    }
    public void SessionPriceUpdated(int cash, int shard)
    {
        CurrentCash = (int.Parse(CurrentCash.Replace("<sprite index='3'>", "")) + cash).ToString();
        CurrentShard = (int.Parse(CurrentShard.Replace("<sprite index='0'>", "")) + shard).ToString();
    }
    #endregion
    #region Session 
    public void UpdateSessionPlaytime()
    {
        isStart = false;
        if (SessionInformation != null)
        {
            if ((string)SessionInformation["TotalPlayedTime"] != "")
            {
                TimeSpan oldTime = TimeSpan.Parse((string)SessionInformation["TotalPlayedTime"]);
                TimeSpan newTime = oldTime.Add(currentTimePlayed);
                SessionPlayTime = newTime.ToString(@"hh\:mm\:ss");

            } else 
            {
                SessionPlayTime = currentTimePlayed.ToString(@"hh\:mm\:ss"); 
            }
        }
        ad.UpdateSessionPlayTime(PlayerID, SessionPlayTime);
    }
    #endregion
}
