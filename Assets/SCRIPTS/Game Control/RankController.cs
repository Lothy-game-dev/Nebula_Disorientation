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
    private Dictionary<string, object> ListData;
    private Dictionary<string, object> RankStat;
    private bool FirstCondition;
    private bool SecondCondition;
    private int PlayerID;
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
       
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        CheckToRankUp();
    }
    #endregion
    #region Check condition to rank up
    // Group all function that serve the same algorithm
    public void CheckToRankUp()
    {
        RankStat = ad.GetRankById(int.Parse(ListData["RankId"].ToString()) + 1, int.Parse(ListData["SupremeWarriorNo"].ToString()) + 1);
        Dictionary<string, object> PlayerAchievement = ad.GetPlayerAchievement(PlayerID);
        Dictionary<string, string> CurrentAchievement = FindAnyObjectByType<GlobalFunctionController>().ConvertEnemyDefeated(PlayerAchievement["EnemyDefeated"].ToString());
       
        int EnemyTierI = int.Parse(CurrentAchievement["EnemyTierI"]);
        int EnemyTierII = int.Parse(CurrentAchievement["EnemyTierII"]);
        int EnemyTierIII = int.Parse(CurrentAchievement["EnemyTierIII"]);
        int Warship = int.Parse(CurrentAchievement["Warship"]);
        int MaxSZReach = int.Parse(PlayerAchievement["MaxSZReach"].ToString());
        int MissionCompleted = int.Parse(PlayerAchievement["TotalMission"].ToString());

        int ArsenalItem = ad.GetCurrentOwnershipWeaponPowerModel(PlayerID, "Weapon") + ad.GetCurrentOwnershipWeaponPowerModel(PlayerID, "Power");
        int FactoryItem = ad.GetCurrentOwnershipWeaponPowerModel(PlayerID, "Model");
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
                FindAnyObjectByType<NotificationBoardController>().CreateRankUpNotiBoard(RankStat["RankName"].ToString(), 2f);
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
    
}
