using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpaceZoneMission : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public TextMeshPro MissionText;
    public SpaceZoneMissionText TextGO;
    public SpaceZoneTimer Timer;
    #endregion
    #region NormalVariables
    public int ObjectiveNumber;
    public int CurrentDoneNumber;
    public string MissionStageName;
    public string MissionTextString;
    public bool alreadyComplete;
    public int A2ReqTier;
    public int FailNumber;
    private AccessDatabase ad;
    private StatisticController stat;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        ad = FindAnyObjectByType<AccessDatabase>();
        stat = FindAnyObjectByType<StatisticController>();      
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (MissionStageName=="D3")
        {
            if (FailNumber <= (int)Mathf.Ceil(ObjectiveNumber / 2))
            {
                if (!alreadyComplete)
                {
                    alreadyComplete = true;
                    FailMission();
                }
            }
            if (CurrentDoneNumber >= FailNumber)
            {
                if (!alreadyComplete)
                {
                    alreadyComplete = true;
                    CompleteMission();
                }
            }
        } 
        else if (MissionStageName != "D1" && MissionStageName != "D2" && CurrentDoneNumber>=ObjectiveNumber)
        {
            if (!alreadyComplete)
            {
                alreadyComplete = true;
                CompleteMission();
            }
        }
    }
    #endregion
    #region Update Mission
    private void SetMissionText()
    {
        if (MissionStageName!="D1" && MissionStageName!="D2")
        {
            if (MissionStageName == "D3")
            {
                MissionText.text = MissionTextString + " (" + CurrentDoneNumber + "/" + Mathf.CeilToInt(ObjectiveNumber/2f) + ")";
            }
            else
            {
                MissionText.text = MissionTextString + " (" + CurrentDoneNumber + "/" + ObjectiveNumber + ")";
            }
        }
        else
        {
            MissionText.text = MissionTextString;
        }
    }
    #endregion
    #region Create Missions
    public void CreateMissionAssaultV1(int NumberOfEnemy)
    {
        MissionStageName = "A1";
        ObjectiveNumber = NumberOfEnemy;
        SetMissionText();
    }

    public void CreateMissionAssaultV2(int NumberOfTargetEnemy, int Tier)
    {
        MissionStageName = "A2";
        A2ReqTier = Tier;
        ObjectiveNumber = NumberOfTargetEnemy;
        SetMissionText();
    }

    public void CreateMissionDefendV1()
    {
        MissionStageName = "D1";
        SetMissionText();
    }
    public void CreateMissionDefendV2()
    {
        MissionStageName = "D2";
        SetMissionText();
    }
    public void CreateMissionDefendV3(int NumberOfEscort)
    {
        MissionStageName = "D3";
        ObjectiveNumber = NumberOfEscort;
        FailNumber = NumberOfEscort;
        SetMissionText();
    }

    public void CreateMissionOnslaughtV1(int NumberOfEnemy)
    {
        MissionStageName = "O1";
        ObjectiveNumber = NumberOfEnemy;
        SetMissionText();
    }
    public void CreateMissionOnslaughtV2(int NumberOfAllyWarship, int NumberOfWarship)
    {
        MissionStageName = "O2";
        ObjectiveNumber = NumberOfWarship;
        FailNumber = NumberOfAllyWarship;
        SetMissionText();
    }

    public void CreateMissionBossV1(int NumberOfWarship)
    {
        MissionStageName = "B1";
        ObjectiveNumber = NumberOfWarship;
        SetMissionText();
    }

    public void CreateMissionBossV2(int NumberOfEliteFighter)
    {
        MissionStageName = "B2";
        A2ReqTier = 1;
        ObjectiveNumber = NumberOfEliteFighter;
        SetMissionText();
    }
    #endregion
    #region Destroy check
    public void EnemyFighterDestroy(string FighterName, int Tier)
    {
        if (MissionStageName=="A1" || MissionStageName == "O1")
        {
            CurrentDoneNumber++;
        } else if (MissionStageName == "A2" || MissionStageName == "B2")
        {
            if (Tier==A2ReqTier)
            {
                CurrentDoneNumber++;
            }
        }
        SetMissionText();
    }

    public void AllyFighterDestroy(string FighterName)
    {
        if (FighterName.Contains("SSTP"))
        {
            FailNumber--;
            if (FailNumber <= 0 && !alreadyComplete)
            {
                alreadyComplete = true;
                FailMission();
            }
        }
    }

    public void AllyEscortDone()
    {
        if (MissionStageName=="D3")
        {
            CurrentDoneNumber++;
        }
        SetMissionText();
    }

    public void TimerEnd()
    {
        if (!alreadyComplete)
        {
            if (MissionStageName == "D1" || MissionStageName == "D2")
            {
                alreadyComplete = true;
                CompleteMission();
            }
        }
    }

    public void AllySpaceStationDestroy()
    {
        if (!alreadyComplete)
        {
            if (MissionStageName == "D1")
            {
                alreadyComplete = true;
                FailMission();
            }
        }
    }

    public void EnemySpaceStationDestroy()
    {

    }

    public void AllyWarshipDestroy()
    {
        if (MissionStageName == "O2")
        {
            FailNumber--;
            if (FailNumber <= 0f && !alreadyComplete)
            {
                alreadyComplete = true;
                FailMission();
            }
        }
    }

    public void EnemyWarshipDestroy()
    {
        if (MissionStageName == "O2" || MissionStageName == "B1")
        {
            CurrentDoneNumber++;
        }
        SetMissionText();
    }

    public void FailMission()
    {
        alreadyComplete = true;
        Timer.DoneSetupTimer = false;
        stat.UpdateStatistic();
        TextGO.CreateText("Mission Failed!", Color.red);
    }

    public void PlayerDestroyed()
    {
        Timer.DoneSetupTimer = false;
        alreadyComplete = true;
        stat.UpdateStatistic();
        TextGO.CreateText("Mission Failed!", Color.red);
    }

    public void CompleteMission()
    {
        alreadyComplete = true;
        Timer.DoneSetupTimer = false;
        //CheckDailyMission();
        stat.UpdateStatistic();
        TextGO.CreateText("Mission Accomplished!", Color.green);
        
    }
    #endregion
    #region Check space zone type daily mission
    private void CheckDailyMission()
    {
        List<List<string>> listDM = ad.GetListDailyMissionUndone(PlayerPrefs.GetInt("PlayerID"));
        if (listDM != null)
        {
            for (int i = 0; i < listDM[0].Count; i++)
            {
                switch (listDM[0][i])
                {
                    case "C":
                        if (listDM[3][i] == "N")
                        {
                            if (int.Parse(listDM[1][i]) > int.Parse(listDM[2][i]))
                            {
                                ad.UpdateDailyMissionProgess(PlayerPrefs.GetInt("PlayerID"), listDM[0][i], 1);
                            }
                            else
                            {
                                ad.DailyMissionDone(PlayerPrefs.GetInt("PlayerID"), listDM[0][i]);
                            }
                        }
                        break;
                    case "CD":
                        if (listDM[3][i] == "N")
                        {
                            if (int.Parse(listDM[1][i]) > int.Parse(listDM[2][i]))
                            {
                                if (MissionStageName.Contains("D"))
                                {
                                    ad.UpdateDailyMissionProgess(PlayerPrefs.GetInt("PlayerID"), listDM[0][i], 1);
                                }
                            }
                            else
                            {
                                ad.DailyMissionDone(PlayerPrefs.GetInt("PlayerID"), listDM[0][i]);
                            }
                        }
                        break;
                    case "CA":
                        if (listDM[3][i] == "N")
                        {
                            if (int.Parse(listDM[1][i]) > int.Parse(listDM[2][i]))
                            {
                                if (MissionStageName.Contains("A"))
                                {
                                    ad.UpdateDailyMissionProgess(PlayerPrefs.GetInt("PlayerID"), listDM[0][i], 1);
                                }
                            }
                            else
                            {
                                ad.DailyMissionDone(PlayerPrefs.GetInt("PlayerID"), listDM[0][i]);
                            }
                        }
                        break;
                    case "CAA":
                        if (listDM[3][i] == "N")
                        {
                            if (int.Parse(listDM[1][i]) > int.Parse(listDM[2][i]))
                            {
                                if (MissionStageName.Contains("O"))
                                {
                                    ad.UpdateDailyMissionProgess(PlayerPrefs.GetInt("PlayerID"), listDM[0][i], 1);
                                }
                            }
                            else
                            {
                                ad.DailyMissionDone(PlayerPrefs.GetInt("PlayerID"), listDM[0][i]);
                            }
                        }
                        break;
                    default: break;
                }
            }
        }       
    }
    #endregion
}
