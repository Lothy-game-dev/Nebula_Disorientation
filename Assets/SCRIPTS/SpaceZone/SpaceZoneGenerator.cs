using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SpaceZoneGenerator : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public SpaceZoneBackground SpaceZoneBackground;
    public SpaceZoneTimer Timer;
    public TextMeshPro SpaceZoneNoText;
    public SpaceZoneMission Mission;
    public SpaceZoneIntro SpaceZoneIntro;
    public AudioClip AssaultMusic;
    public AudioClip DefendMusic;
    public AudioClip OnslaughtMusic;
    public AudioClip BossMusic;
    public GameObject StartTeleport;
    public SpaceZoneHazardEnvironment Hazard;
    public TextMeshPro HazardNameText;
    public TextMeshPro HazardDesciptionText;
    #endregion
    #region NormalVariables
    public int SpaceZoneNo;
    public int ChosenVariant;
    public string ChosenBG;
    private Dictionary<string, object> TemplateData;
    private EnemyFighterSpawn EnemyFighterSpawn;
    private SpawnAlliesFighter AllyFighterSpawn;
    private SpawnAllyWarship AllyWarshipSpawner;
    private SpawnEnemyWarship EnemyWarshipSpawner;
    private AllySpaceStationSpawn AllySSSpawner;
    private EnemySpaceStationSpawn EnemySSSpawner;
    public int AllyFighterACount;
    public int AllyFighterBCount;
    public int AllyFighterCCount;
    public int EnemyFighterACount;
    public int EnemyFighterBCount;
    public int EnemyFighterCCount;
    public int AllyWarshipACount;
    public int AllyWarshipBCount;
    public int AllyWarshipCCount;
    public int EnemyWarshipACount;
    public int EnemyWarshipBCount;
    public int EnemyWarshipCCount;
    public float AllyMaxHP;
    public float EnemyMaxHP;
    public List<int> AllyFighterIDs;
    public int[] EnemyFighterIDs;
    public float EnemyBountyScale;
    public float AllyBountyScale;
    public int[] EnemiesTier;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        EnemyFighterSpawn = GetComponent<EnemyFighterSpawn>();
        AllyFighterSpawn = GetComponent<SpawnAlliesFighter>();
        
        EnemyWarshipSpawner = GetComponent<SpawnEnemyWarship>();
        AllyWarshipSpawner = GetComponent<SpawnAllyWarship>();
       
        EnemySSSpawner = GetComponent<EnemySpaceStationSpawn>();
        AllySSSpawner = GetComponent<AllySpaceStationSpawn>();
        GenerateSpaceZone();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Generate Space Zone By Template
    public void GenerateSpaceZone()
    {
        Dictionary<string, object> SessionData = FindObjectOfType<AccessDatabase>().GetSessionInfoByPlayerId(PlayerPrefs.GetInt("PlayerID"));
        SpaceZoneNo = (int)SessionData["CurrentStage"];
        PlayerPrefs.SetInt("Variant", 0);
        PlayerPrefs.SetInt("Hazard", 0);
        Dictionary<string, object> variantData = FindObjectOfType<AccessDatabase>().GetVariantCountsAndBackgroundByStageValue(SpaceZoneNo % 10);
        ChosenVariant = (int)SessionData["CurrentStageVariant"];
        int HazardId = (int)SessionData["CurrentStageHazard"];
        Dictionary<string, object> HazardData = FindObjectOfType<AccessDatabase>().GetHazardAllDatas(HazardId);
        HazardNameText.text = "<color=" + (string)HazardData["HazardColor"] + ">" + (string)HazardData["HazardName"] + "</color>";
        HazardDesciptionText.text = "<color=" + (string)HazardData["HazardColor"] + ">" + (string)HazardData["HazardDescription"] + "</color>";
        if (HazardId > 1)
        {
            if ((string)HazardData["HazardBackground"] != "")
            {
                ChosenBG = (string)HazardData["HazardBackground"];
            }
            Hazard.HazardID = HazardId;
            Hazard.InitializeHazard();
        }
        else
        ChosenBG = (string)variantData["AvailableBackground"];
        SpaceZoneBackground.ChangeBackground(ChosenBG);
        Debug.Log((string)variantData["AvailableBackground"]);
        // Music
        if (SpaceZoneNo%10 == 1 || SpaceZoneNo%10 == 3 || SpaceZoneNo%10 == 5 || SpaceZoneNo%10 == 7)
        {
            Camera.main.GetComponent<AudioSource>().clip = AssaultMusic;
        }
        else if (SpaceZoneNo % 10 == 2 || SpaceZoneNo % 10 == 4 || SpaceZoneNo % 10 == 6)
        {
            Camera.main.GetComponent<AudioSource>().clip = DefendMusic;
        }
        else if (SpaceZoneNo % 10 == 8 || SpaceZoneNo % 10 == 9)
        {
            Camera.main.GetComponent<AudioSource>().clip = OnslaughtMusic;
        } 
        else
        {
            Camera.main.GetComponent<AudioSource>().clip = BossMusic;
        }
        StartCoroutine(StartSound());
        // Teleport Fighter
        if ((SpaceZoneNo%10 == 2 || SpaceZoneNo%10 == 4 || SpaceZoneNo%10 == 6) && ChosenVariant==3)
        {
            Dictionary<string, object> SpawnPosData = FindObjectOfType<AccessDatabase>().GetSpawnPositionDataByType("PE");
            string[] VectorRangeTopLeft = ((string)SpawnPosData["PositionLimitTopLeft"]).Split("|");
            string[] VectorRangeBottomRight = ((string)SpawnPosData["PositionLimitBottomRight"]).Split("|");
            int n = Random.Range(0, VectorRangeTopLeft.Length);
            int LeftLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int TopLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            int RightLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int BottomLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            FindObjectOfType<GameController>().Player.transform.position = new Vector2(Random.Range(LeftLimit, RightLimit), Random.Range(BottomLimit, TopLimit));
        } else
        {
            Dictionary<string, object> SpawnPosData = FindObjectOfType<AccessDatabase>().GetSpawnPositionDataByType("PN");
            string[] VectorRangeTopLeft = ((string)SpawnPosData["PositionLimitTopLeft"]).Split("|");
            string[] VectorRangeBottomRight = ((string)SpawnPosData["PositionLimitBottomRight"]).Split("|");
            int n = Random.Range(0, VectorRangeTopLeft.Length);
            int LeftLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int TopLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            int RightLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int BottomLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            FindObjectOfType<GameController>().Player.transform.position = new Vector2(Random.Range(LeftLimit, RightLimit), Random.Range(BottomLimit, TopLimit));
        }
        
        GameObject Player = FindObjectOfType<GameController>().Player;
        GameObject spawnEff = Instantiate(StartTeleport, Player.transform.position, Quaternion.identity);
        spawnEff.SetActive(true);
        Destroy(spawnEff, 1.5f);
        Player.transform.Rotate(new Vector3(0, 0, -90));
        Player.GetComponent<PlayerFighter>().OnFireGO.transform.Rotate(new Vector3(0, 0, 90));
        Player.GetComponent<PlayerFighter>().OnFreezeGO.transform.Rotate(new Vector3(0, 0, 90));
        Player.GetComponent<PlayerMovement>().CurrentRotateAngle = 90;
        // Calculate and Generate SpaceZone
        CalculateAndGenerateSpaceZone((string)variantData["TierColor"]);
    }

    private void CalculateAndGenerateSpaceZone(string color)
    {
        // Get Template Data
        TemplateData = FindObjectOfType<AccessDatabase>().GetStageZoneTemplateByStageValueAndVariant(SpaceZoneNo % 10, ChosenVariant);
        // Set UI Data
        SpaceZoneNoText.text = "Space Zone No." + SpaceZoneNo;
        int Time = Mathf.RoundToInt(((int)TemplateData["Time"]) * (1 + (SpaceZoneNo / 20)/10f));
        Mission.MissionTextString = ((string)TemplateData["Missions"]).Replace("an amount of time", Time + " seconds");
        // Set Mission To Mission Object WIP
        Timer.SetTimer(Time);
        SpaceZoneIntro.SetData(SpaceZoneNo, (string)TemplateData["Type"], ((string)TemplateData["Missions"]).Replace("an amount of time", Time + " seconds"), color);
        
        // Squad Rating
        string SquadRating = (string)TemplateData["SquadRating"];
        float AllySquadRating = float.Parse(SquadRating.Split("|")[0]);
        float EnemySquadRating = float.Parse(SquadRating.Split("|")[1]);

        // Ally Squad X Y Z
        string AllySquad = (string)TemplateData["AllySquad"];
        float AllySquadX = float.Parse(AllySquad.Split("-")[0]);
        float AllySquadY = float.Parse(AllySquad.Split("-")[1]);
        float AllySquadZ = float.Parse(AllySquad.Split("-")[2]);

        // Enemy Squad X Y Z
        string EnemySquad = (string)TemplateData["EnemySquad"];
        float EnemySquadX = float.Parse(EnemySquad.Split("-")[0]);
        float EnemySquadY = float.Parse(EnemySquad.Split("-")[1]);
        float EnemySquadZ = float.Parse(EnemySquad.Split("-")[2]);

        // Army Rating
        string ArmyRating = "";
        string AllyWarship = "";
        string EnemyWarship = "";
        if (TemplateData["ArmyRating"]!=null)
        {
            ArmyRating = (string)TemplateData["ArmyRating"];
            AllyWarship = (string)TemplateData["AllyWarship"];
            EnemyWarship = (string)TemplateData["EnemyWarship"];
        }
        float AllyArmyRating = 0;
        float EnemyArmyRating = 0;
        if (ArmyRating!="")
        {
            AllyArmyRating = float.Parse(ArmyRating.Split("|")[0]);
            EnemyArmyRating = float.Parse(ArmyRating.Split("|")[1]);
        }

        // Ally Army X Y Z
        float AllyArmyX = 0;
        float AllyArmyY = 0;
        float AllyArmyZ = 0;
        if (AllyWarship!="")
        {
            AllyArmyX = float.Parse(AllyWarship.Split("-")[0]);
            AllyArmyY = float.Parse(AllyWarship.Split("-")[1]);
            AllyArmyZ = float.Parse(AllyWarship.Split("-")[2]);
        }

        Debug.Log(AllyWarship);
        // Enemy Army X Y Z
        float EnemyArmyX = 0;
        float EnemyArmyY = 0;
        float EnemyArmyZ = 0;
        if (EnemyWarship!="")
        {
            EnemyArmyX = float.Parse(EnemyWarship.Split("-")[0]);
            EnemyArmyY = float.Parse(EnemyWarship.Split("-")[1]);
            EnemyArmyZ = float.Parse(EnemyWarship.Split("-")[2]);
        }

        // Enemy Space station Spawn Chance
        float EnemySpaceStationSpawn = 0;

        AllyFighterSpawn.AllyMaxHPScale = 1;
        AllyMaxHP = AllyFighterSpawn.AllyMaxHPScale;
        AllyFighterSpawn.AllyBountyScale = 1;


        EnemyFighterSpawn.EnemyMaxHPScale = 1;
        EnemyMaxHP = EnemyFighterSpawn.EnemyMaxHPScale;
        EnemyFighterSpawn.EnemyBountyScale = 1;

        // Change Squad Rating Based On Milestones
        // Increase Squad rating for (2n-1)*10 and 2n * 10
        int Scale20Odd = SpaceZoneNo / 20 + SpaceZoneNo / 10 % 2;
        if (Scale20Odd >= 1)
        {
            AllyFighterSpawn.AllyMaxHPScale = 1 + Scale20Odd / 20f;
            AllyFighterSpawn.AllyBountyScale = 1 + Scale20Odd / 5f;
            AllyMaxHP = AllyFighterSpawn.AllyMaxHPScale;
            AllyBountyScale = AllyFighterSpawn.AllyBountyScale;

            EnemyFighterSpawn.EnemyMaxHPScale = 1 + Scale20Odd / 20f;
            EnemyFighterSpawn.EnemyBountyScale = 1 + Scale20Odd / 5f;
            EnemyMaxHP = EnemyFighterSpawn.EnemyMaxHPScale;
            EnemyBountyScale = EnemyFighterSpawn.EnemyBountyScale;

            AllySquadRating *= (1 + Scale20Odd / 10f);
            EnemySquadRating *= (1 + Scale20Odd / 10f);
        }
        int Scale20Even = SpaceZoneNo / 20;
        if (Scale20Even >= 1)
        {
            
        }
        // (5n)*10
        // Enemy SS spawn
        int Scale50SSC = SpaceZoneNo / 50;
        if (Scale50SSC >= 1 && SpaceZoneNo % 50==0)
        {
            if (Scale50SSC * 10 / 100f > 50f)
            {
                EnemySpaceStationSpawn = 50f;
            } else
            EnemySpaceStationSpawn = Scale50SSC * 10 / 100f;
        }
        if (Scale50SSC >= 1)
        {
            AllyArmyRating *= (1 + (float)Scale50SSC / 4);
            EnemyArmyRating *= (1 + (float)Scale50SSC / 4);
        }
        if (SpaceZoneNo<350)
        {
            int Scale50 = SpaceZoneNo / 50;
            if (Scale50 >= 1)
            {
                float realScaleAlly = 0;
                if (AllySquadX - Scale50 < 5)
                {
                    realScaleAlly = AllySquadX - 5;
                    AllySquadX = 5;
                }
                else
                {
                    realScaleAlly = Scale50;
                    AllySquadX -= Scale50;
                }
                AllySquadY += realScaleAlly / 2f;
                AllySquadZ += realScaleAlly / 2f;
                float realScaleEnemy = 0;
                if (EnemySquadX - Scale50 < 5)
                {
                    realScaleEnemy = EnemySquadX - 5;
                    EnemySquadX = 5;
                }
                else
                {
                    realScaleEnemy = Scale50;
                    EnemySquadX -= Scale50;
                }
                EnemySquadY += Scale50 / 2f;
                EnemySquadZ += Scale50 / 2f;
                // Chance spawn space station WAIT
            }
        } else
        {
            AllySquadX = 0;
            AllySquadY = 7;
            AllySquadZ = 3;
            EnemySquadX = 0;
            EnemySquadY = 7;
            EnemySquadZ = 3;
        }
        
        // Squad for (10n)*10
        int SquadScale = SpaceZoneNo / 100;
        if (SquadScale >= 1)
        {
            if (SpaceZoneNo%10 != 0 || ChosenVariant != 1)
            {
                float realScaleAllyArmy = 0;
                if (AllyArmyX > 0)
                {
                    if (AllyArmyX - (float)SquadScale / 2 < 5)
                    {
                        realScaleAllyArmy = AllyArmyX - 5;
                        AllyArmyX = 5;
                    }
                    else
                    {
                        realScaleAllyArmy = (float)SquadScale / 2;
                        AllyArmyX -= (float)SquadScale / 2;
                    }
                    AllyArmyZ += realScaleAllyArmy;
                }
                if (EnemyArmyX > 0)
                {
                    float realScaleEnemyArmy = 0;
                    if (EnemyArmyX - (float)SquadScale / 2 < 5)
                    {
                        realScaleEnemyArmy = EnemyArmyX - 5;
                        EnemyArmyX = 5;
                    }
                    else
                    {
                        realScaleEnemyArmy = (float)SquadScale / 2;
                        EnemyArmyX -= (float)SquadScale / 2;
                    }
                    EnemyArmyZ += realScaleEnemyArmy;
                }
            }
        }

        // Get Data Fighter Group
        Dictionary<string, object> FighterGroupData = FindObjectOfType<AccessDatabase>().GetFighterGroupsDataByName((string)TemplateData["FighterGroup"] + (SpaceZoneNo>350? "350":""));
        
        // Count Number Of Ally By Type
        AllyFighterACount = (int)Mathf.Ceil((float)AllySquadRating / (AllySquadX + AllySquadY + AllySquadZ) * AllySquadX / 5);
        AllyFighterBCount = (int)Mathf.Ceil((float)AllySquadRating / (AllySquadX + AllySquadY + AllySquadZ) * AllySquadY / 10);
        AllyFighterCCount = (int)Mathf.Ceil((float)AllySquadRating / (AllySquadX + AllySquadY + AllySquadZ) * AllySquadZ / 30);
        // Spawn Ally
        List<int> AllyID = new List<int>();
        List<Vector2> AllySpawnPos = new List<Vector2>();
        List<int> EnemyTier = new List<int>();
        List<string> AllyClass = new List<string>();
        // Spawn Ally A
        for (int i=0;i<AllyFighterACount;i++)
        {
            string[] idList = ((string)FighterGroupData["AlliesFighterA"]).Split(",");
            AllyID.Add(int.Parse(idList[Random.Range(0, idList.Length)]));
            string Spawnstr = "AA";
            if ((SpaceZoneNo % 10 == 2 || SpaceZoneNo % 10 == 4 || SpaceZoneNo % 10 == 6) && ChosenVariant == 3)
            {
                Spawnstr = "EAS";
            }
            Dictionary<string, object> SpawnPosData = FindObjectOfType<AccessDatabase>().GetSpawnPositionDataByType(Spawnstr);
            string[] VectorRangeTopLeft = ((string)SpawnPosData["PositionLimitTopLeft"]).Split("|");
            string[] VectorRangeBottomRight = ((string)SpawnPosData["PositionLimitBottomRight"]).Split("|");
            int n = Random.Range(0, VectorRangeTopLeft.Length);
            int LeftLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int TopLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            int RightLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int BottomLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            AllySpawnPos.Add(new Vector2(Random.Range(LeftLimit, RightLimit), Random.Range(BottomLimit, TopLimit)));
            AllyClass.Add("A");
        }
        // Spawn Ally B
        for (int i = 0; i < AllyFighterBCount; i++)
        {
            string[] idList = ((string)FighterGroupData["AlliesFighterB"]).Split(",");
            AllyID.Add(int.Parse(idList[Random.Range(0, idList.Length)]));
            AllyFighterIDs.Add(int.Parse(idList[Random.Range(0, idList.Length)]));
            string Spawnstr = "AB";
            if ((SpaceZoneNo % 10 == 2 || SpaceZoneNo % 10 == 4 || SpaceZoneNo % 10 == 6) && ChosenVariant == 3)
            {
                Spawnstr = "EAS";
            }
            Dictionary<string, object> SpawnPosData = FindObjectOfType<AccessDatabase>().GetSpawnPositionDataByType(Spawnstr);
            string[] VectorRangeTopLeft = ((string)SpawnPosData["PositionLimitTopLeft"]).Split("|");
            string[] VectorRangeBottomRight = ((string)SpawnPosData["PositionLimitBottomRight"]).Split("|");
            int n = Random.Range(0, VectorRangeTopLeft.Length);
            int LeftLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int TopLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            int RightLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int BottomLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            AllySpawnPos.Add(new Vector2(Random.Range(LeftLimit, RightLimit), Random.Range(BottomLimit, TopLimit)));
            AllyClass.Add("B");
        }
        // Spawn Ally C
        for (int i = 0; i < AllyFighterCCount; i++)
        {
            string[] idList = ((string)FighterGroupData["AlliesFighterC"]).Split(",");
            AllyID.Add(int.Parse(idList[Random.Range(0, idList.Length)]));
            string Spawnstr = "AC";
            if ((SpaceZoneNo % 10 == 2 || SpaceZoneNo % 10 == 4 || SpaceZoneNo % 10 == 6) && ChosenVariant == 3)
            {
                Spawnstr = "EAS";
            }
            Dictionary<string, object> SpawnPosData = FindObjectOfType<AccessDatabase>().GetSpawnPositionDataByType(Spawnstr);
            string[] VectorRangeTopLeft = ((string)SpawnPosData["PositionLimitTopLeft"]).Split("|");
            string[] VectorRangeBottomRight = ((string)SpawnPosData["PositionLimitBottomRight"]).Split("|");
            int n = Random.Range(0, VectorRangeTopLeft.Length);
            int LeftLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int TopLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            int RightLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int BottomLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            AllySpawnPos.Add(new Vector2(Random.Range(LeftLimit, RightLimit), Random.Range(BottomLimit, TopLimit)));
            AllyClass.Add("C");
        }
        // Set Spawn to Spawner
        AllyFighterSpawn.AllySpawnID = AllyID.ToArray();
        AllyFighterSpawn.AllySpawnPosition = AllySpawnPos.ToArray();
        AllyFighterSpawn.AllyClass = AllyClass.ToArray();
        if ((SpaceZoneNo%10==2 || SpaceZoneNo%10==4 || SpaceZoneNo%10==6))
        {
            if (ChosenVariant==3)
            {
                AllyFighterSpawn.EscortSpawnNumber = (int)Mathf.Ceil(AllySquadRating / 10);
                AllyFighterSpawn.Escort = true;
            } else if (ChosenVariant==1)
            {
                AllyFighterSpawn.Defend = true;
            }
        } else if ((SpaceZoneNo % 10 == 0 && ChosenVariant == 1))
        {
           AllyFighterSpawn.Priority = "WS";
        }
        else if ((SpaceZoneNo % 10 == 8 || SpaceZoneNo % 10 == 9) && ChosenVariant == 2)
        {
            AllyFighterSpawn.Priority = "WS";
        }
        AllyFighterSpawn.SpawnAlly();

        // Count Number Of Enemy By Type
        EnemyFighterACount = (int)Mathf.Ceil((float)EnemySquadRating / (EnemySquadX + EnemySquadY + EnemySquadZ) * EnemySquadX / 5);
        EnemyFighterBCount = (int)Mathf.Ceil((float)EnemySquadRating / (EnemySquadX + EnemySquadY + EnemySquadZ) * EnemySquadY / 10);
        EnemyFighterCCount = (int)Mathf.Ceil((float)EnemySquadRating / (EnemySquadX + EnemySquadY + EnemySquadZ) * EnemySquadZ / 30);
        // Spawn Enemy
        bool SpawningByTime = false;
        if ((int)TemplateData["SpawnIRate"]>0)
        {
            EnemyFighterSpawn.DelayBetweenSpawn = 1 / (EnemySquadRating / (int) TemplateData["SpawnIRate"]);
            SpawningByTime = true;
        } else
        {
            EnemyFighterSpawn.DelayBetweenSpawn = 0;
        }
        if ((int)TemplateData["SpawnIIRate"]>0)
        {
            EnemyFighterSpawn.DelaySpawnSBB = 1 / (EnemySquadRating / ((int)TemplateData["SpawnIIRate"]));
        } else
        {
            EnemyFighterSpawn.DelaySpawnSBB = 0;
        }
        List<int> EnemyIDA = new List<int>();
        List<int> EnemyIDB = new List<int>();
        List<int> EnemyIDC = new List<int>();
        List<Vector2> EnemySpawnPos = new List<Vector2>();
        // Spawn Enemy A
        for (int i = 0; i < EnemyFighterACount; i++)
        {
            string[] idList = ((string)FighterGroupData["EnemiesFighterA"]).Split(",");
            EnemyIDA.Add(int.Parse(idList[Random.Range(0, idList.Length)]));
            Dictionary<string, object> SpawnPosData = FindObjectOfType<AccessDatabase>().GetSpawnPositionDataByType("EA");
            string[] VectorRangeTopLeft = ((string)SpawnPosData["PositionLimitTopLeft"]).Split("|");
            string[] VectorRangeBottomRight = ((string)SpawnPosData["PositionLimitBottomRight"]).Split("|");
            int n = Random.Range(0, VectorRangeTopLeft.Length);
            int LeftLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int TopLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            int RightLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int BottomLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            EnemySpawnPos.Add(new Vector2(Random.Range(LeftLimit, RightLimit), Random.Range(BottomLimit, TopLimit)));
            EnemyTier.Add(3);
        }
        // Spawn Enemy B
        for (int i = 0; i < EnemyFighterBCount; i++)
        {
            string[] idList = ((string)FighterGroupData["EnemiesFighterB"]).Split(",");
            EnemyIDB.Add(int.Parse(idList[Random.Range(0, idList.Length)]));
            Dictionary<string, object> SpawnPosData = FindObjectOfType<AccessDatabase>().GetSpawnPositionDataByType("EB");
            string[] VectorRangeTopLeft = ((string)SpawnPosData["PositionLimitTopLeft"]).Split("|");
            string[] VectorRangeBottomRight = ((string)SpawnPosData["PositionLimitBottomRight"]).Split("|");
            int n = Random.Range(0, VectorRangeTopLeft.Length);
            int LeftLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int TopLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            int RightLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int BottomLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            EnemySpawnPos.Add(new Vector2(Random.Range(LeftLimit, RightLimit), Random.Range(BottomLimit, TopLimit)));
            EnemyTier.Add(2);
        }
        // Spawn Enemy C
        for (int i = 0; i < EnemyFighterCCount; i++)
        {
            string[] idList = ((string)FighterGroupData["EnemiesFighterC"]).Split(",");
            EnemyIDC.Add(int.Parse(idList[Random.Range(0, idList.Length)]));
            Dictionary<string, object> SpawnPosData = FindObjectOfType<AccessDatabase>().GetSpawnPositionDataByType("EC");
            string[] VectorRangeTopLeft = ((string)SpawnPosData["PositionLimitTopLeft"]).Split("|");
            string[] VectorRangeBottomRight = ((string)SpawnPosData["PositionLimitBottomRight"]).Split("|");
            int n = Random.Range(0, VectorRangeTopLeft.Length);
            int LeftLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int TopLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            int RightLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int BottomLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            EnemySpawnPos.Add(new Vector2(Random.Range(LeftLimit, RightLimit), Random.Range(BottomLimit, TopLimit)));
            EnemyTier.Add(1);
        }
        // Set Spawn to Spawner
        List<int> IDTotal = new List<int>();
        List<Vector2> SpawnTotal = new List<Vector2>();
        List<int> TierTotal = new List<int>();
        if (SpawningByTime)
        {
            int a = 0, b = 0, c = 0;
            for (int i=0;i<EnemyFighterACount + EnemyFighterBCount + EnemyFighterCCount; i++)
            {
                bool NoA = false;
                bool NoB = false;
                bool NoC = false;
                float end = 10;
                if (a >= EnemyIDA.Count)
                {
                    end -= EnemySquadX;
                    NoA = true;
                }
                
                if (b >= EnemyIDB.Count)
                {
                    end -= EnemySquadY;
                    NoB = true;
                }
                
                if (c >= EnemyIDC.Count)
                {
                    end -= EnemySquadZ;
                    NoC = true;
                }

                if (NoA && NoB && NoC)
                {
                    break;
                }
                float n = Random.Range(0f, end);
                if (NoA && NoB && !NoC)
                {
                    IDTotal.Add(EnemyIDC[c]);
                    c++;
                    TierTotal.Add(1);
                } 
                else if (NoA && !NoB && NoC)
                {
                    IDTotal.Add(EnemyIDB[b]);
                    b++;
                    TierTotal.Add(2);
                } 
                else if (NoA && !NoB && !NoC)
                {
                    if (n <= EnemySquadY)
                    {
                        IDTotal.Add(EnemyIDB[b]);
                        b++;
                        TierTotal.Add(2);
                    } else
                    {
                        IDTotal.Add(EnemyIDC[c]);
                        c++;
                        TierTotal.Add(1);
                    }
                }
                else if (!NoA && NoB && !NoC)
                {
                    if (n <= EnemySquadX)
                    {
                        IDTotal.Add(EnemyIDA[a]);
                        a++;
                        TierTotal.Add(3);
                    }
                    else
                    {
                        IDTotal.Add(EnemyIDC[c]);
                        c++;
                        TierTotal.Add(1);
                    }
                }
                else if (!NoA && !NoB && NoC)
                {
                    if (n <= EnemySquadX)
                    {
                        IDTotal.Add(EnemyIDA[a]);
                        a++;
                        TierTotal.Add(3);
                    }
                    else
                    {
                        IDTotal.Add(EnemyIDB[b]);
                        b++;
                        TierTotal.Add(2);
                    }
                }
                else if (!NoA && !NoB && !NoC)
                {
                    if (n <= EnemySquadX)
                    {
                        IDTotal.Add(EnemyIDA[a]);
                        a++;
                        TierTotal.Add(3);
                    }
                    else if (n <= EnemySquadX + EnemySquadY)
                    {
                        IDTotal.Add(EnemyIDB[b]);
                        b++;
                        TierTotal.Add(2);
                    } else
                    {
                        IDTotal.Add(EnemyIDC[c]);
                        c++;
                        TierTotal.Add(1);
                    }
                } else if (!NoA && NoB && NoC)
                {
                    IDTotal.Add(EnemyIDA[a]);
                    a++;
                    TierTotal.Add(3);
                }
                string Spawnstr = "ES";
                if ((SpaceZoneNo % 10 == 2 || SpaceZoneNo % 10 == 4 || SpaceZoneNo % 10 == 6) && ChosenVariant == 3)
                {
                    Spawnstr = "EES";
                }
                Dictionary<string, object> SpawnPosData = FindObjectOfType<AccessDatabase>().GetSpawnPositionDataByType(Spawnstr);
                string[] VectorRangeTopLeft = ((string)SpawnPosData["PositionLimitTopLeft"]).Split("|");
                string[] VectorRangeBottomRight = ((string)SpawnPosData["PositionLimitBottomRight"]).Split("|");
                int k = Random.Range(0, VectorRangeTopLeft.Length);
                int LeftLimit = int.Parse(VectorRangeTopLeft[k].Replace("(", "").Replace(")", "").Split(",")[0]);
                int TopLimit = int.Parse(VectorRangeTopLeft[k].Replace("(", "").Replace(")", "").Split(",")[1]);
                int RightLimit = int.Parse(VectorRangeBottomRight[k].Replace("(", "").Replace(")", "").Split(",")[0]);
                int BottomLimit = int.Parse(VectorRangeBottomRight[k].Replace("(", "").Replace(")", "").Split(",")[1]);
                SpawnTotal.Add(new Vector2(Random.Range(LeftLimit, RightLimit), Random.Range(BottomLimit, TopLimit)));
            }
        } else
        {
            IDTotal.AddRange(EnemyIDA);
            IDTotal.AddRange(EnemyIDB);
            IDTotal.AddRange(EnemyIDC);
            SpawnTotal.AddRange(EnemySpawnPos);
            TierTotal.AddRange(EnemyTier);
        }
        EnemyFighterSpawn.EnemySpawnID = IDTotal.ToArray();
        EnemyFighterIDs = EnemyIDB.ToArray();
        EnemyFighterSpawn.EnemySpawnPosition = SpawnTotal.ToArray();
        EnemyFighterSpawn.EnemyTier = TierTotal.ToArray();
        EnemiesTier = TierTotal.ToArray();
        if ((SpaceZoneNo % 10 == 2 || SpaceZoneNo % 10 == 4 || SpaceZoneNo % 10 == 6) && ChosenVariant == 1)
        {
            EnemyFighterSpawn.Priority = "SS";
        } 
        else if ((SpaceZoneNo % 10 == 2 || SpaceZoneNo % 10 == 4 || SpaceZoneNo % 10 == 6) && ChosenVariant == 2)
        {
            EnemyFighterSpawn.Priority = "Player";
        }
        else if ((SpaceZoneNo % 10 == 2 || SpaceZoneNo % 10 == 4 || SpaceZoneNo % 10 == 6) && ChosenVariant == 3)
        {
            EnemyFighterSpawn.Priority = "SSTP";
            EnemyFighterSpawn.Escort = true;
        }
        else if ((SpaceZoneNo % 10 == 8 || SpaceZoneNo % 10 == 9) && ChosenVariant == 2)
        {
            EnemyFighterSpawn.Priority = "WS";
        }
        EnemyFighterSpawn.SpawnEnemy();
        // Warship && SpaceStation
        // Get Warship Milestone
        Dictionary<string, object> WarshipMilestone = FindObjectOfType<AccessDatabase>().GetWarshipMilestoneBySpaceZoneNo(SpaceZoneNo);

        // Count Number Of Warship Ally By Type
        if (AllyArmyX + AllyArmyY + AllyArmyZ != 0)
        {
            AllyWarshipACount = (int)((float)AllyArmyRating / (AllyArmyX + AllyArmyY + AllyArmyZ) * AllyArmyX / 5);
            AllyWarshipBCount = (int)((float)AllyArmyRating / (AllyArmyX + AllyArmyY + AllyArmyZ) * AllyArmyY / 5);
            AllyWarshipCCount = (int)((float)AllyArmyRating / (AllyArmyX + AllyArmyY + AllyArmyZ) * AllyArmyZ / 5);
            // Spawn Ally
            List<int> AllyWarshipID = new List<int>();
            List<Vector2> AllyWarshipSpawnPos = new List<Vector2>();
            List<string> WSClass = new List<string>();
            // Spawn Ally A
            for (int i = 0; i < AllyWarshipACount; i++)
            {
                AllyWarshipID.Add((int)WarshipMilestone["MilestoneAllyClassA"]);
                string Spawnstr = "AX";
                Dictionary<string, object> SpawnPosData = FindObjectOfType<AccessDatabase>().GetSpawnPositionDataByType(Spawnstr);
                string[] VectorRangeTopLeft = ((string)SpawnPosData["PositionLimitTopLeft"]).Split("|");
                string[] VectorRangeBottomRight = ((string)SpawnPosData["PositionLimitBottomRight"]).Split("|");
                int n = Random.Range(0, VectorRangeTopLeft.Length);
                int LeftLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[0]);
                int TopLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[1]);
                int RightLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[0]);
                int BottomLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[1]);
                AllyWarshipSpawnPos.Add(new Vector2(Random.Range(LeftLimit, RightLimit), Random.Range(BottomLimit, TopLimit)));
                WSClass.Add("A");
            }
            // Spawn Ally B
            for (int i = 0; i < AllyWarshipBCount; i++)
            {
                AllyWarshipID.Add((int)WarshipMilestone["MilestoneAllyClassB"]);
                string Spawnstr = "AY";
                Dictionary<string, object> SpawnPosData = FindObjectOfType<AccessDatabase>().GetSpawnPositionDataByType(Spawnstr);
                string[] VectorRangeTopLeft = ((string)SpawnPosData["PositionLimitTopLeft"]).Split("|");
                string[] VectorRangeBottomRight = ((string)SpawnPosData["PositionLimitBottomRight"]).Split("|");
                int n = Random.Range(0, VectorRangeTopLeft.Length);
                int LeftLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[0]);
                int TopLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[1]);
                int RightLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[0]);
                int BottomLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[1]);
                AllyWarshipSpawnPos.Add(new Vector2(Random.Range(LeftLimit, RightLimit), Random.Range(BottomLimit, TopLimit)));
                WSClass.Add("B");
            }
            // Spawn Ally C
            for (int i = 0; i < AllyWarshipCCount; i++)
            {
                AllyWarshipID.Add((int)WarshipMilestone["MilestoneAllyClassC"]);
                string Spawnstr = "AC";
                Dictionary<string, object> SpawnPosData = FindObjectOfType<AccessDatabase>().GetSpawnPositionDataByType(Spawnstr);
                string[] VectorRangeTopLeft = ((string)SpawnPosData["PositionLimitTopLeft"]).Split("|");
                string[] VectorRangeBottomRight = ((string)SpawnPosData["PositionLimitBottomRight"]).Split("|");
                int n = Random.Range(0, VectorRangeTopLeft.Length);
                int LeftLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[0]);
                int TopLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[1]);
                int RightLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[0]);
                int BottomLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[1]);
                AllyWarshipSpawnPos.Add(new Vector2(Random.Range(LeftLimit, RightLimit), Random.Range(BottomLimit, TopLimit)));
                WSClass.Add("C");
            }
            // Set Spawn to Spawner
            AllyWarshipSpawner.WarshipID = AllyWarshipID.ToArray();
            AllyWarshipSpawner.WarshipPosition = AllyWarshipSpawnPos.ToArray();
            AllyWarshipSpawner.WarshipClass = WSClass.ToArray();
            AllyWarshipSpawner.SpawnAllyWarships();
        }

        // Count Number Of Warship Enemy By Type
        EnemyWarshipACount = (int)((float)EnemyArmyRating / (EnemyArmyX + EnemyArmyY + EnemyArmyZ) * EnemyArmyX / 5);
        EnemyWarshipBCount = (int)((float)EnemyArmyRating / (EnemyArmyX + EnemyArmyY + EnemyArmyZ) * EnemyArmyY / 5);
        EnemyWarshipCCount = (int)((float)EnemyArmyRating / (EnemyArmyX + EnemyArmyY + EnemyArmyZ) * EnemyArmyZ / 5);
        // Spawn Enemy
        List<int> EnemyWarshipID = new List<int>();
        List<Vector2> EnemyWarshipSpawnPos = new List<Vector2>();
        List<string> EnemyWarshipClass = new List<string>();
        // Spawn Enemy A
        for (int i = 0; i < EnemyWarshipACount; i++)
        {
            EnemyWarshipID.Add((int)WarshipMilestone["MilestoneEnemyClassA"]);
            string Spawnstr = "EX";
            Dictionary<string, object> SpawnPosData = FindObjectOfType<AccessDatabase>().GetSpawnPositionDataByType(Spawnstr);
            string[] VectorRangeTopLeft = ((string)SpawnPosData["PositionLimitTopLeft"]).Split("|");
            string[] VectorRangeBottomRight = ((string)SpawnPosData["PositionLimitBottomRight"]).Split("|");
            int n = Random.Range(0, VectorRangeTopLeft.Length);
            int LeftLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int TopLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            int RightLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int BottomLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            EnemyWarshipSpawnPos.Add(new Vector2(Random.Range(LeftLimit, RightLimit), Random.Range(BottomLimit, TopLimit)));
            EnemyWarshipClass.Add("A");
        }
        // Spawn Enemy B
        for (int i = 0; i < EnemyWarshipBCount; i++)
        {
            EnemyWarshipID.Add((int)WarshipMilestone["MilestoneEnemyClassB"]);
            string Spawnstr = "EY";
            Dictionary<string, object> SpawnPosData = FindObjectOfType<AccessDatabase>().GetSpawnPositionDataByType(Spawnstr);
            string[] VectorRangeTopLeft = ((string)SpawnPosData["PositionLimitTopLeft"]).Split("|");
            string[] VectorRangeBottomRight = ((string)SpawnPosData["PositionLimitBottomRight"]).Split("|");
            int n = Random.Range(0, VectorRangeTopLeft.Length);
            int LeftLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int TopLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            int RightLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int BottomLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            EnemyWarshipSpawnPos.Add(new Vector2(Random.Range(LeftLimit, RightLimit), Random.Range(BottomLimit, TopLimit)));
            EnemyWarshipClass.Add("B");
        }
        // Spawn Enemy C
        for (int i = 0; i < EnemyWarshipCCount; i++)
        {
            EnemyWarshipID.Add((int)WarshipMilestone["MilestoneEnemyClassC"]);
            string Spawnstr = "EC";
            Dictionary<string, object> SpawnPosData = FindObjectOfType<AccessDatabase>().GetSpawnPositionDataByType(Spawnstr);
            string[] VectorRangeTopLeft = ((string)SpawnPosData["PositionLimitTopLeft"]).Split("|");
            string[] VectorRangeBottomRight = ((string)SpawnPosData["PositionLimitBottomRight"]).Split("|");
            int n = Random.Range(0, VectorRangeTopLeft.Length);
            int LeftLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int TopLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            int RightLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int BottomLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            EnemyWarshipSpawnPos.Add(new Vector2(Random.Range(LeftLimit, RightLimit), Random.Range(BottomLimit, TopLimit)));
            EnemyWarshipClass.Add("C");
        }
        // Set Spawn to Spawner
        EnemyWarshipSpawner.WarshipID = EnemyWarshipID.ToArray();
        EnemyWarshipSpawner.WarshipPosition = EnemyWarshipSpawnPos.ToArray();
        EnemyWarshipSpawner.WarshipClass = EnemyWarshipClass.ToArray();
        EnemyWarshipSpawner.SpawnEnemyWarships();
        // Ally Space Station
        if ((SpaceZoneNo % 10 == 2 || SpaceZoneNo % 10 == 4 || SpaceZoneNo % 10 == 6) && ChosenVariant == 1)
        {
            Dictionary<string, object> SpawnPosData = FindObjectOfType<AccessDatabase>().GetSpawnPositionDataByType("AO");
            string[] VectorRangeTopLeft = ((string)SpawnPosData["PositionLimitTopLeft"]).Split("|");
            string[] VectorRangeBottomRight = ((string)SpawnPosData["PositionLimitBottomRight"]).Split("|");
            int n = Random.Range(0, VectorRangeTopLeft.Length);
            int LeftLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int TopLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            int RightLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int BottomLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            // Chance
            float k = Random.Range(0, 100f);
            if (k<=50)
            {
                AllyWarshipSpawner.SpawnImmobileWS(11, new Vector2(Random.Range(LeftLimit, RightLimit), Random.Range(BottomLimit, TopLimit)));
            } else if (k<=85)
            {
                AllyWarshipSpawner.SpawnImmobileWS(12, new Vector2(Random.Range(LeftLimit, RightLimit), Random.Range(BottomLimit, TopLimit)));
            } else
            {
                AllySSSpawner.SpaceStationID = new int[] { 1 };
                AllySSSpawner.SpaceStationPosition = new Vector2[] { new Vector2(Random.Range(LeftLimit, RightLimit), Random.Range(BottomLimit, TopLimit)) };
                AllySSSpawner.SpawnAllySpaceStation();
            }
        }
        // Enemy Space Station
        float random = Random.Range(0, 100f);
        if (random < EnemySpaceStationSpawn)
        {
            Dictionary<string, object> SpawnPosData = FindObjectOfType<AccessDatabase>().GetSpawnPositionDataByType("EO");
            string[] VectorRangeTopLeft = ((string)SpawnPosData["PositionLimitTopLeft"]).Split("|");
            string[] VectorRangeBottomRight = ((string)SpawnPosData["PositionLimitBottomRight"]).Split("|");
            int n = Random.Range(0, VectorRangeTopLeft.Length);
            int LeftLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int TopLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            int RightLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int BottomLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            EnemySSSpawner.SpaceStationID = new int[] { 1 };
            EnemySSSpawner.SpaceStationPos = new Vector2[] { new Vector2(Random.Range(LeftLimit, RightLimit), Random.Range(BottomLimit, TopLimit)) };
            EnemySSSpawner.SpawnEnemySpaceStation();
        }
        // Mission
        if (SpaceZoneNo % 10 == 1 || SpaceZoneNo % 10 == 3 || SpaceZoneNo % 10 == 5 || SpaceZoneNo % 10 == 7)
        {
            if (ChosenVariant == 1)
            {
                Mission.CreateMissionAssaultV1(EnemyFighterACount + EnemyFighterBCount + EnemyFighterCCount);
            } else
            {
                if (EnemyFighterCCount > 0)
                {
                    Mission.MissionTextString = "Eliminate Tier I Enemies";
                    Mission.CreateMissionAssaultV2(EnemyFighterCCount, 1);
                } else if (EnemyFighterBCount > 0)
                {
                    Mission.MissionTextString = "Eliminate Tier II Enemies";
                    Mission.CreateMissionAssaultV2(EnemyFighterBCount, 2);
                } else if (EnemyFighterCCount > 0)
                {
                    Mission.MissionTextString = "Eliminate Tier III Enemies";
                    Mission.CreateMissionAssaultV2(EnemyFighterACount, 3);
                }
                    
            }
        } else if (SpaceZoneNo % 10 == 2 || SpaceZoneNo % 10 == 4 || SpaceZoneNo % 10 == 6)
        {
            if (ChosenVariant == 1)
            {
                Mission.CreateMissionDefendV1();
            } else if (ChosenVariant == 2)
            {
                Mission.CreateMissionDefendV2();
            }
            else if (ChosenVariant==3)
            {
                Mission.CreateMissionDefendV3((int)Mathf.Ceil(AllySquadRating / 10));
            }
        } else if (SpaceZoneNo % 10 == 8 || SpaceZoneNo % 10 == 9)
        {
            if (ChosenVariant==1)
            {
                Mission.CreateMissionOnslaughtV1(EnemyFighterACount + EnemyFighterBCount + EnemyFighterCCount);
            } else
            {
                Mission.CreateMissionOnslaughtV2(AllyWarshipACount + AllyWarshipBCount + AllyWarshipCCount,
                    EnemyWarshipACount + EnemyWarshipBCount + EnemyWarshipCCount);
            }
        } else if (SpaceZoneNo % 10 == 0)
        {
            if (ChosenVariant == 1)
            {
                Mission.CreateMissionBossV1(EnemyWarshipACount + EnemyWarshipBCount + EnemyWarshipCCount);
            }
            else
            {
                Mission.CreateMissionBossV2(EnemyFighterCCount);
            }
        }
    }

    #endregion
    #region Sound
    private IEnumerator StartSound()
    {
        yield return new WaitForSeconds(3f);
        Camera.main.GetComponent<AudioSource>().enabled = true;
    }
    #endregion
}
