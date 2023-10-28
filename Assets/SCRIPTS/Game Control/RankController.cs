using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankController : MonoBehaviour
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
    private AccessDatabase ad;
    private GlobalFunctionController glc;
    private NotificationBoardController nc;
    private Dictionary<string, object> ListData;
    private Dictionary<string, object> RankStat;
    private bool FirstCondition;
    private bool SecondCondition;
    private int PlayerID;
    private int EnemyTierI;
    private int EnemyTierII;
    private int EnemyTierIII;
    private int Warship;
    private int MaxSZReach;
    private int MissionCompleted;
    private int ArsenalItem;
    private int FactoryItem;
    private Dictionary<string, object> PlayerAchievement;
    private Dictionary<string, string> CurrentAchievement;
    private float Timer;
    public float ShowTime;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        ad = FindAnyObjectByType<AccessDatabase>();
        if (FindAnyObjectByType<UECMainMenuController>() != null)
        {
            PlayerID = FindAnyObjectByType<UECMainMenuController>().PlayerId;
            
        } else
        {
            PlayerID = PlayerPrefs.GetInt("PlayerID");
        }
        ListData = ad.GetPlayerInformationById(PlayerID);
        FirstCondition = false;
        SecondCondition = false;
        glc = FindAnyObjectByType<GlobalFunctionController>();
        nc = FindAnyObjectByType<NotificationBoardController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible       
    }
    #endregion
    #region Check condition to rank up
    // Group all function that serve the same algorithm
    public void CheckToRankUp()
    {
        RankStat = ad.GetRankById(int.Parse(ListData["RankId"].ToString()) + 1, int.Parse(ListData["SupremeWarriorNo"].ToString()) + 1);
        PlayerAchievement = ad.GetPlayerAchievement(PlayerID);
        CurrentAchievement = glc.ConvertEnemyDefeated(PlayerAchievement["EnemyDefeated"].ToString());
       
        EnemyTierI = int.Parse(CurrentAchievement["EnemyTierI"]);
        EnemyTierII = int.Parse(CurrentAchievement["EnemyTierII"]);
        EnemyTierIII = int.Parse(CurrentAchievement["EnemyTierIII"]);
        Warship = int.Parse(CurrentAchievement["Warship"]);
        MaxSZReach = int.Parse(PlayerAchievement["MaxSZReach"].ToString());
        MissionCompleted = int.Parse(PlayerAchievement["TotalMission"].ToString());

        ArsenalItem = ad.GetCurrentOwnershipWeaponPowerModel(PlayerID, "Weapon") + ad.GetCurrentOwnershipWeaponPowerModel(PlayerID, "Power");
        FactoryItem = ad.GetCurrentOwnershipWeaponPowerModel(PlayerID, "Model");
        if (RankStat != null)
        {
            // Check space zone condition
            if (int.Parse(RankStat["RankConditionSZ"].ToString()) <= MaxSZReach)
            {
                FirstCondition = true;
            }
            // Check the second condition
            switch(RankStat["RankCondition2Verb"].ToString())
            {
                case "C":
                    if (int.Parse(RankStat["RankCondition2Num"].ToString()) <= MissionCompleted)
                    {
                        SecondCondition = true;
                    }
                    break;
                case "D-I":
                    if (int.Parse(RankStat["RankCondition2Num"].ToString()) <= EnemyTierI)
                    {
                        SecondCondition = true;
                    }
                    break;
                case "D-II":
                    if (int.Parse(RankStat["RankCondition2Num"].ToString()) <= EnemyTierII)
                    {
                        SecondCondition = true;
                    }
                    break;
                case "D-III":
                    if (int.Parse(RankStat["RankCondition2Num"].ToString()) <= EnemyTierIII)
                    {
                        SecondCondition = true;
                    }
                    break;
                case "D-WS":
                    if (int.Parse(RankStat["RankCondition2Num"].ToString()) <= Warship)
                    {
                        SecondCondition = true;
                    }
                    break;
                case "PA":
                    if (int.Parse(RankStat["RankCondition2Num"].ToString()) < ArsenalItem)
                    {
                        SecondCondition = true;
                    }
                    break;
                case "O":
                    if (int.Parse(RankStat["RankCondition2Num"].ToString()) <= FactoryItem)
                    {
                        SecondCondition = true;
                    }
                    break;
                default: SecondCondition = true; break;
            }

            if (FirstCondition && SecondCondition)
            {
                Debug.Log("Rank Up!");
                ad.UpdateRank(PlayerID, RankStat);
                nc.CreateRankUpNotiBoard(RankStat["RankName"].ToString(), 1f);
                ListData = ad.GetPlayerInformationById(PlayerID);
                if (FindAnyObjectByType<UECMainMenuController>() != null)
                {
                    FindAnyObjectByType<UECMainMenuController>().GetData();
                }
                FirstCondition = false;
                SecondCondition = false;
            }
        }
    }
    #endregion
    #region Destroy
    public void ToDestroy()
    {
        Destroy(gameObject);
    }
    #endregion

}
