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
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        Achievement = new Dictionary<string, object>();
        AchievementName = new List<string>() {"PlayerID" ,"EnemyDefeated", "SZMaxReach" };
        foreach (var name in AchievementName)
        {
            Achievement.Add(name, 0);
        }
        PlayerAchievement = FindAnyObjectByType<AccessDatabase>().GetPlayerAchievement(PlayerPrefs.GetInt("PlayerID"));
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
    }
    #endregion
    #region Update Achievement
    // Group all function that serve the same algorithm
    public void UpdateAchievement()
    {
        string EnemyDefeated = "EI-" + EnemyTierI + "|EII-" + EnemyTierII + "|EIII-" + EnemyTierIII + "|WS-" + Warship;
        MaxSZReach = FindAnyObjectByType<SpaceZoneGenerator>().SpaceZoneNo;
        SessionInformation = FindAnyObjectByType<AccessDatabase>().GetSessionInfoByPlayerId(PlayerPrefs.GetInt("PlayerID"));
        if (MaxSZReach < int.Parse(SessionInformation["CurrentStage"].ToString()))
        {
            MaxSZReach = int.Parse(SessionInformation["CurrentStage"].ToString());
        }
        Achievement["EnemyDefeated"] = EnemyDefeated;
        Achievement["SZMaxReach"] = MaxSZReach;
        Achievement["PlayerID"] = PlayerPrefs.GetInt("PlayerID");
        FindAnyObjectByType<AccessDatabase>().UpdateGameplayStatistic(Achievement);
    }
    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    #endregion
}
