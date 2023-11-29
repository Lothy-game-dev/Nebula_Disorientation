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
    private List<List<string>> RankList;
    private bool FirstCondition;
    private bool SecondCondition;
    private List<int> RankUpList;
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
    private int RankUpId;
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
        RankList = ad.GetAllRank();
        RankUpId = -1;
        RankUpList = new List<int>();
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
        for (int i = 0; i < RankList.Count; i++)
        {
            RankStat = ad.GetRankById(int.Parse(RankList[i][0].ToString()), int.Parse(ListData["SupremeWarriorNo"].ToString()) + 1);
            if (RankStat != null)
        {
            // Check space zone condition
            if (int.Parse(RankStat["RankConditionSZ"].ToString()) <= MaxSZReach)
            {
                FirstCondition = true;
            } else
            {
                FirstCondition = false;
            }
            // Check the second condition
            switch(RankStat["RankCondition2Verb"].ToString())
            {
                case "C":
                    if (int.Parse(RankStat["RankCondition2Num"].ToString()) <= MissionCompleted)
                    {
                        
                        SecondCondition = true;
                    } else
                        {
                            SecondCondition = false;
                        }
                    break;
                case "D-I":
                    if (int.Parse(RankStat["RankCondition2Num"].ToString()) <= EnemyTierI)
                    {
                        SecondCondition = true;
                    } else
                        {
                            SecondCondition = false;
                        }
                    break;
                case "D-II":
                    if (int.Parse(RankStat["RankCondition2Num"].ToString()) <= EnemyTierII)
                    {
                        SecondCondition = true;
                    } else
                        {
                            SecondCondition = false;
                        }
                    break;
                case "D-III":
                    if (int.Parse(RankStat["RankCondition2Num"].ToString()) <= EnemyTierIII)
                    {
                        SecondCondition = true;
                    } else
                        {
                            SecondCondition = false;
                        }
                    break;
                case "D-WS":
                    if (int.Parse(RankStat["RankCondition2Num"].ToString()) <= Warship)
                    {
                        SecondCondition = true;
                    } else
                        {
                            SecondCondition = false;
                        }
                    break;
                case "PA":
                    if (int.Parse(RankStat["RankCondition2Num"].ToString()) < ArsenalItem)
                    {
                        SecondCondition = true;
                    } else
                        {
                            SecondCondition = false;
                        }
                    break;
                case "O":
                    if (int.Parse(RankStat["RankCondition2Num"].ToString()) <= FactoryItem)
                    {
                        SecondCondition = true;
                    } else
                        {
                            SecondCondition = false;
                        }
                    break;
                default: SecondCondition = true; break;
            }

            if (FirstCondition && SecondCondition)
            {
                RankUpList.Add(int.Parse(RankList[i][0].ToString()));
            }
        }

        }
        // Update rank 
        if (RankUpList.Count > 0 && int.Parse(ListData["RankId"].ToString()) < RankUpList[RankUpList.Count - 1])
        {
            RankStat = ad.GetRankById(RankUpList[RankUpList.Count - 1], int.Parse(ListData["SupremeWarriorNo"].ToString()) + 1);
            ad.UpdateRank(PlayerID, RankStat);
            nc.CreateRankUpNotiBoard(RankStat["RankName"].ToString(), 3f);
            ListData = ad.GetPlayerInformationById(PlayerID);
            if (FindAnyObjectByType<UECMainMenuController>() != null)
            {
                FindAnyObjectByType<UECMainMenuController>().GetData();
            }
            RankUpList = new List<int>();
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
