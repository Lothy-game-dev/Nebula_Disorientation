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
    public TextMeshPro MissionText;
    public SpaceZoneMission Mission;
    public SpaceZoneIntro SpaceZoneIntro;
    #endregion
    #region NormalVariables
    public int SpaceZoneNo;
    public int ChosenVariant;
    public string ChosenBG;
    private Dictionary<string, object> TemplateData;
    private EnemyFighterSpawn EnemyFighterSpawn;
    private SpawnAlliesFighter AllyFighterSpawn;
    public int AllyFighterACount;
    public int AllyFighterBCount;
    public int AllyFighterCCount;
    public int EnemyFighterACount;
    public int EnemyFighterBCount;
    public int EnemyFighterCCount;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        EnemyFighterSpawn = GetComponent<EnemyFighterSpawn>();
        AllyFighterSpawn = GetComponent<SpawnAlliesFighter>();
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
        Dictionary<string, object> variantData = FindObjectOfType<AccessDatabase>().GetVariantCountsAndBackgroundByStageValue(SpaceZoneNo % 10);
        int VariantCount = (int)variantData["VariantCounts"];
        if (ChosenVariant==0) ChosenVariant = Random.Range(1, 1 + VariantCount);
        string[] BGs = ((string)variantData["AvailableBackground"]).Split(",");
        ChosenBG = BGs[Random.Range(0, BGs.Length)];
        SpaceZoneBackground.ChangeBackground(ChosenBG);
        CalculateAndGenerateSpaceZone((string)variantData["TierColor"]);
    }

    private void CalculateAndGenerateSpaceZone(string color)
    {
        // Get Template Data
        TemplateData = FindObjectOfType<AccessDatabase>().GetStageZoneTemplateByStageValueAndVariant(SpaceZoneNo % 10, ChosenVariant);
        // Set UI Data
        SpaceZoneNoText.text = "Space Zone No." + SpaceZoneNo;
        MissionText.text = (string)TemplateData["Missions"];
        // Set Mission To Mission Object WIP
        Timer.SetTimer((int)TemplateData["Time"]);
        SpaceZoneIntro.SetData(SpaceZoneNo, (string)TemplateData["Type"], (string)TemplateData["Missions"], color);
        // Squad Rating
        string SquadRating = (string)TemplateData["SquadRating"];
        int AllySquadRating = int.Parse(SquadRating.Split("|")[0]);
        int EnemySquadRating = int.Parse(SquadRating.Split("|")[1]);
        // Ally Squad X Y Z
        string AllySquad = (string)TemplateData["AllySquad"];
        int AllySquadX = int.Parse(AllySquad.Split("-")[0]);
        int AllySquadY = int.Parse(AllySquad.Split("-")[1]);
        int AllySquadZ = int.Parse(AllySquad.Split("-")[2]);

        // Enemy Squad X Y Z
        string EnemySquad = (string)TemplateData["EnemySquad"];
        int EnemySquadX = int.Parse(EnemySquad.Split("-")[0]);
        int EnemySquadY = int.Parse(EnemySquad.Split("-")[1]);
        int EnemySquadZ = int.Parse(EnemySquad.Split("-")[2]);

        // Get Data Fighter Group
        Dictionary<string, object> FighterGroupData = FindObjectOfType<AccessDatabase>().GetFighterGroupsDataByName((string)TemplateData["FighterGroup"]);
        
        // Count Number Of Ally By Type
        AllyFighterACount = (int)Mathf.Ceil((float)AllySquadRating / (AllySquadX + AllySquadY + AllySquadZ) * AllySquadX / 5);
        AllyFighterBCount = (int)Mathf.Ceil((float)AllySquadRating / (AllySquadX + AllySquadY + AllySquadZ) * AllySquadY / 10);
        AllyFighterCCount = (int)Mathf.Ceil((float)AllySquadRating / (AllySquadX + AllySquadY + AllySquadZ) * AllySquadZ / 30);
        // Spawn Ally
        List<int> AllyID = new List<int>();
        List<Vector2> AllySpawnPos = new List<Vector2>();
        // Spawn Ally A
        for (int i=0;i<AllyFighterACount;i++)
        {
            string[] idList = ((string)FighterGroupData["AlliesFighterA"]).Split(",");
            AllyID.Add(int.Parse(idList[Random.Range(0, idList.Length)]));
            Dictionary<string, object> SpawnPosData = FindObjectOfType<AccessDatabase>().GetSpawnPositionDataByType("AA");
            string[] VectorRangeTopLeft = ((string)SpawnPosData["PositionLimitTopLeft"]).Split("|");
            string[] VectorRangeBottomRight = ((string)SpawnPosData["PositionLimitBottomRight"]).Split("|");
            int n = Random.Range(0, VectorRangeTopLeft.Length);
            int LeftLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int TopLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            int RightLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int BottomLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            AllySpawnPos.Add(new Vector2(Random.Range(LeftLimit, RightLimit), Random.Range(BottomLimit, TopLimit)));
        }
        // Spawn Ally B
        for (int i = 0; i < AllyFighterBCount; i++)
        {
            string[] idList = ((string)FighterGroupData["AlliesFighterB"]).Split(",");
            AllyID.Add(int.Parse(idList[Random.Range(0, idList.Length)]));
            Dictionary<string, object> SpawnPosData = FindObjectOfType<AccessDatabase>().GetSpawnPositionDataByType("AB");
            string[] VectorRangeTopLeft = ((string)SpawnPosData["PositionLimitTopLeft"]).Split("|");
            string[] VectorRangeBottomRight = ((string)SpawnPosData["PositionLimitBottomRight"]).Split("|");
            int n = Random.Range(0, VectorRangeTopLeft.Length);
            int LeftLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int TopLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            int RightLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int BottomLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            AllySpawnPos.Add(new Vector2(Random.Range(LeftLimit, RightLimit), Random.Range(BottomLimit, TopLimit)));
        }
        // Spawn Ally C
        for (int i = 0; i < AllyFighterCCount; i++)
        {
            string[] idList = ((string)FighterGroupData["AlliesFighterC"]).Split(",");
            AllyID.Add(int.Parse(idList[Random.Range(0, idList.Length)]));
            Dictionary<string, object> SpawnPosData = FindObjectOfType<AccessDatabase>().GetSpawnPositionDataByType("AC");
            string[] VectorRangeTopLeft = ((string)SpawnPosData["PositionLimitTopLeft"]).Split("|");
            string[] VectorRangeBottomRight = ((string)SpawnPosData["PositionLimitBottomRight"]).Split("|");
            int n = Random.Range(0, VectorRangeTopLeft.Length);
            int LeftLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int TopLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            int RightLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int BottomLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            AllySpawnPos.Add(new Vector2(Random.Range(LeftLimit, RightLimit), Random.Range(BottomLimit, TopLimit)));
        }
        // Set Spawn to Spawner
        AllyFighterSpawn.AllySpawnID = AllyID.ToArray();
        AllyFighterSpawn.AllySpawnPosition = AllySpawnPos.ToArray();
        AllyFighterSpawn.SpawnAlly();

        // Count Number Of Enemy By Type
        EnemyFighterACount = (int)Mathf.Ceil((float)EnemySquadRating / (EnemySquadX + EnemySquadY + EnemySquadZ) * EnemySquadX / 5);
        EnemyFighterBCount = (int)Mathf.Ceil((float)EnemySquadRating / (EnemySquadX + EnemySquadY + EnemySquadZ) * EnemySquadY / 10);
        EnemyFighterCCount = (int)Mathf.Ceil((float)EnemySquadRating / (EnemySquadX + EnemySquadY + EnemySquadZ) * EnemySquadZ / 30);
        // Spawn Enemy
        List<int> EnemyID = new List<int>();
        List<Vector2> EnemySpawnPos = new List<Vector2>();
        // Spawn Enemy A
        for (int i = 0; i < EnemyFighterACount; i++)
        {
            string[] idList = ((string)FighterGroupData["EnemiesFighterA"]).Split(",");
            EnemyID.Add(int.Parse(idList[Random.Range(0, idList.Length)]));
            Dictionary<string, object> SpawnPosData = FindObjectOfType<AccessDatabase>().GetSpawnPositionDataByType("EA");
            string[] VectorRangeTopLeft = ((string)SpawnPosData["PositionLimitTopLeft"]).Split("|");
            string[] VectorRangeBottomRight = ((string)SpawnPosData["PositionLimitBottomRight"]).Split("|");
            int n = Random.Range(0, VectorRangeTopLeft.Length);
            int LeftLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int TopLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            int RightLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int BottomLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            EnemySpawnPos.Add(new Vector2(Random.Range(LeftLimit, RightLimit), Random.Range(BottomLimit, TopLimit)));
        }
        // Spawn Enemy B
        for (int i = 0; i < EnemyFighterBCount; i++)
        {
            string[] idList = ((string)FighterGroupData["EnemiesFighterB"]).Split(",");
            EnemyID.Add(int.Parse(idList[Random.Range(0, idList.Length)]));
            Dictionary<string, object> SpawnPosData = FindObjectOfType<AccessDatabase>().GetSpawnPositionDataByType("EB");
            string[] VectorRangeTopLeft = ((string)SpawnPosData["PositionLimitTopLeft"]).Split("|");
            string[] VectorRangeBottomRight = ((string)SpawnPosData["PositionLimitBottomRight"]).Split("|");
            int n = Random.Range(0, VectorRangeTopLeft.Length);
            int LeftLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int TopLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            int RightLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int BottomLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            EnemySpawnPos.Add(new Vector2(Random.Range(LeftLimit, RightLimit), Random.Range(BottomLimit, TopLimit)));
        }
        // Spawn Enemy C
        for (int i = 0; i < EnemyFighterCCount; i++)
        {
            string[] idList = ((string)FighterGroupData["EnemiesFighterC"]).Split(",");
            EnemyID.Add(int.Parse(idList[Random.Range(0, idList.Length)]));
            Dictionary<string, object> SpawnPosData = FindObjectOfType<AccessDatabase>().GetSpawnPositionDataByType("EC");
            string[] VectorRangeTopLeft = ((string)SpawnPosData["PositionLimitTopLeft"]).Split("|");
            string[] VectorRangeBottomRight = ((string)SpawnPosData["PositionLimitBottomRight"]).Split("|");
            int n = Random.Range(0, VectorRangeTopLeft.Length);
            int LeftLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int TopLimit = int.Parse(VectorRangeTopLeft[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            int RightLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[0]);
            int BottomLimit = int.Parse(VectorRangeBottomRight[n].Replace("(", "").Replace(")", "").Split(",")[1]);
            EnemySpawnPos.Add(new Vector2(Random.Range(LeftLimit, RightLimit), Random.Range(BottomLimit, TopLimit)));
        }
        // Set Spawn to Spawner
        EnemyFighterSpawn.EnemySpawnID = EnemyID.ToArray();
        EnemyFighterSpawn.EnemySpawnPosition = EnemySpawnPos.ToArray();
        EnemyFighterSpawn.SpawnEnemy();
    }
    #endregion
}
