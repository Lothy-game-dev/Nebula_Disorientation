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
        InitScale = MissionCompeletedBoard.transform.GetChild(0).localScale.x;
        // Set data to fuel cell bar
        Dictionary<string, object> ListData = ad.GetPlayerInformationById(PlayerPrefs.GetInt("PlayerID"));
        CurrentFuelCell = (int)ListData["FuelCell"];     
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
        //Update Statistic
        CurrentHP = Fighter.GetComponent<PlayerFighter>().CurrentHP;
        MaxHP = Fighter.GetComponent<PlayerFighter>().MaxHP;
        CurrentShard = Shard.text;
        CurrentCash = Cash.text;
        PlayedTime = Timer.text;
        CurrentSZNo = FindAnyObjectByType<SpaceZoneGenerator>().SpaceZoneNo;
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
                            CreateMissionCompletedNotiBoard(mission, 2f);
                        }
                        break;
                    case "KB":
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
        }       
        KillEnemy = false;
        KillBossEnemy = false;
    }
    #endregion
    #region Notification
    public void CreateMissionCompletedNotiBoard(string text, float autoCloseTimer)
    {
        Vector2 Position = Camera.main.transform.position;
        GameObject notiBoard = Instantiate(MissionCompeletedBoard, new Vector3(Position.x, Position.y - 303, MissionCompeletedBoard.transform.position.z), Quaternion.identity);
        notiBoard.transform.GetChild(0).localScale = new Vector2(notiBoard.transform.GetChild(0).localScale.x / 1.5f, notiBoard.transform.GetChild(0).localScale.y);
        notiBoard.transform.GetChild(0).GetComponent<TextMeshPro>().text = text + "<br> Mission Completed!";
        notiBoard.SetActive(true);
        notiBoard.transform.SetParent(MissionCompeletedBoard.transform.parent);
        StartCoroutine(NotiBoardAnim(autoCloseTimer, notiBoard.transform.GetChild(0).gameObject));
    }

    private IEnumerator NotiBoardAnim(float autoCloseTimer, GameObject go)
    {
        for (int i = 0; i < 20; i++)
        {
            go.transform.localScale = new Vector2(go.transform.localScale.x + InitScale * 4.5f / 100, go.transform.localScale.y + InitScale * 4.5f / 100);
            yield return new WaitForSeconds(0.01f);
        }
        if (autoCloseTimer > 0)
        {
            if (go.transform.parent.gameObject != null)
            {
                Destroy(go.transform.parent.gameObject, autoCloseTimer);
            }
        }
    }
    #endregion
}
