using System.Collections;
using System.Collections.Generic;
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
    public int CurrentShard;
    public int CurrentCash;
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
        
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        CheckDailyMission();
    }
    #endregion
    #region Update Achievement
    // Group all function that serve the same algorithm
    public void UpdateAchievement()
    {
        string EnemyDefeated = "EI-" + EnemyTierI + "|EII-" + EnemyTierII + "|EIII-" + EnemyTierIII + "|WS-" + Warship;
        MaxSZReach = FindAnyObjectByType<SpaceZoneGenerator>().SpaceZoneNo;
        SessionInformation = ad.GetSessionInfoByPlayerId(PlayerID);
        if (MaxSZReach < int.Parse(SessionInformation["CurrentStage"].ToString()))
        {
            MaxSZReach = int.Parse(SessionInformation["CurrentStage"].ToString());
        }
        Achievement["EnemyDefeated"] = EnemyDefeated;
        Achievement["SZMaxReach"] = MaxSZReach;
        Achievement["PlayerID"] = PlayerID;
        ad.UpdateGameplayStatistic(Achievement);
    }
    #endregion
    #region Check destroy enemy type daily mission
    // Group all function that serve the same algorithm
    private void CheckDailyMission()
    {       
        List<List<string>> listDM = ad.GetListDailyMissionUndone(PlayerID);
        for (int i = 0; i < listDM[0].Count; i++)
        {
            string mission = "";
            switch (listDM[0][i])
            {
                case "KE":
                    if (int.Parse(listDM[1][i]) > int.Parse(listDM[2][i]))
                    {
                        if (KillEnemy)
                        {
                            ad.UpdateDailyMissionProgess(PlayerID, listDM[0][i]);
                        } 
                    } else
                    {
                        ad.DailyMissionDone(PlayerID, listDM[0][i]);
                    }
                    break;
                case "KB":
                    if (int.Parse(listDM[1][i]) > int.Parse(listDM[2][i]))
                    {
                        if (KillBossEnemy)
                        {
                            ad.UpdateDailyMissionProgess(PlayerID, listDM[0][i]);
                        }
                    }
                    else
                    {
                        ad.DailyMissionDone(PlayerID, listDM[0][i]);
                    }
                    break;
                case "C":
                    mission = "Complete " + listDM[1][i] + " spacezone(s).";
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
                default: break;
            }
        }
        KillEnemy = false;
        KillBossEnemy = false;
    }
    #endregion
}
