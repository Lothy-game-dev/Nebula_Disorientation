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
    #endregion
    #region NormalVariables
    public int ObjectiveNumber;
    public int CurrentDoneNumber;
    public string MissionStageName;
    public string MissionTextString;
    public bool alreadyComplete;
    public int A2ReqTier;
    private int EscortNumber;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (MissionStageName=="D3")
        {
            if (EscortNumber <= (int)Mathf.Ceil(ObjectiveNumber / 2))
            {
                if (!alreadyComplete)
                {
                    alreadyComplete = true;
                    FailMission();
                }
            }
            if (CurrentDoneNumber >= EscortNumber)
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
        MissionText.text = MissionTextString + " (" + CurrentDoneNumber + "/" + ObjectiveNumber + ")";
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
        EscortNumber = NumberOfEscort;
        SetMissionText();
    }

    public void CreateMissionOnslaughtV1(int NumberOfEnemy)
    {
        MissionStageName = "O1";
        ObjectiveNumber = NumberOfEnemy;
        SetMissionText();
    }
    public void CreateMissionOnslaughtV2(int NumberOfWarship)
    {
        MissionStageName = "O2";
        ObjectiveNumber = NumberOfWarship;
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
            EscortNumber--;
            if (EscortNumber<=0)
            {
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
        if (MissionStageName=="D1" || MissionStageName=="D2")
        {
            CompleteMission();
        }
    }
    public void FailMission()
    {
        TextGO.CreateText("Mission Failed!", Color.red);
    }

    public void PlayerDestroyed()
    {
        TextGO.CreateText("Mission Fail!", Color.red);
    }

    public void CompleteMission()
    {
        TextGO.CreateText("Mission Accomplished!", Color.green);
    }
    #endregion
}
