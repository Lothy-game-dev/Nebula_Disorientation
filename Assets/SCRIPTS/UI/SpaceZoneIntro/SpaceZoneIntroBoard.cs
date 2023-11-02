using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpaceZoneIntroBoard : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject AlliesModel;
    public GameObject EnemiesModel;
    public GameObject SpaceStationModel;
    public GameObject WarshipModel;
    public GameObject IncomingStage;
    public GameObject HazardInfo;
    public GameObject ModelSpinner;
    public GameObject Mission;
    public GameObject NotableAllies;
    public GameObject NotableEnemies;
    public GameObject Weapons;
    #endregion
    #region NormalVariables
    private Dictionary<string, object> SessionData;
    private int NextStage;
    public int ChosenVariant;
    public int ChosenHazard;
    public int SpaceZoneNo;
    private Dictionary<string, object> TemplateData;
    public List<string> NotableAlliesName;
    public List<string> NotableEnemiesName;
    private List<GameObject> ListNotableAlliesGO;
    private List<GameObject> ListNotableEnemiesGO;

    private GameObject ModelGO;
    public GameObject ModelLeftWeapon;
    public GameObject ModelRightWeapon;
    private GameObject LeftWeapon;
    private GameObject RightWeapon;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        GenerateData();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (ModelGO!=null && ModelGO.activeSelf)
        {
            ModelGO.transform.Rotate(new Vector3(0, 0, 2));
        }
    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    public void GenerateData()
    {
        ListNotableAlliesGO = new List<GameObject>();
        ListNotableEnemiesGO = new List<GameObject>();
        transform.position = new Vector3(Camera.main.transform.position.x,Camera.main.transform.position.y,transform.position.z);
        SessionData = FindObjectOfType<AccessDatabase>().GetSessionInfoByPlayerId(PlayerPrefs.GetInt("PlayerID"));
        // Get Next Stage Number, Variant and Hazard
        SpaceZoneNo = (int)SessionData["CurrentStage"];
        ChosenVariant = (int)SessionData["CurrentStageVariant"];
        ChosenHazard = (int)SessionData["CurrentStageHazard"];
        // Set Data To View Next Stage Number, Variant and Hazard
        IncomingStage.GetComponent<TextMeshPro>().text = "Upcoming: Space Zone no " + SpaceZoneNo;
        Dictionary<string, object> HazDatas = FindObjectOfType<AccessDatabase>().GetHazardAllDatas(ChosenHazard);
        HazardInfo.GetComponent<TextMeshPro>().text = "Environment<br><color=" + (string)HazDatas["HazardColor"] + ">" + (string)HazDatas["HazardName"] + "</color>";
        // Get Mission Data
        Dictionary<string, object> MissionData = FindObjectOfType<AccessDatabase>().GetMissionDataByValueAndVariant(SpaceZoneNo % 10, ChosenVariant);
        Mission.transform.GetChild(1).GetChild(0).GetComponent<TextMeshPro>().text = (string)MissionData["Mission"];
        string VCon = "";
        string DCon = "";
        string[] VictoryConditions = ((string)MissionData["VictoryCondition"]).Split("|");
        for (int i=0;i<VictoryConditions.Length;i++)
        {
            VCon += (i + 1).ToString() + ". " + VictoryConditions[i] + "\n";
        }
        string[] DefeatConditions = ((string)MissionData["DefeatCondition"]).Split("|");
        for (int i = 0; i < DefeatConditions.Length; i++)
        {
            DCon += (i + 1).ToString() + ". " + DefeatConditions[i] + "\n";
        }
        Mission.transform.GetChild(2).GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 76 / 255f);
        Mission.transform.GetChild(2).GetChild(1).GetComponent<TextMeshPro>().text = VCon;
        Mission.transform.GetChild(2).GetChild(3).GetComponent<TextMeshPro>().text = DCon;
        // Get Model
        string model = (string)SessionData["Model"];
        for (int i=0; i< AlliesModel.transform.childCount;i++)
        {
            if (AlliesModel.transform.GetChild(i).name.Replace(" ","").ToLower().Equals(model.Replace(" ","").ToLower())) {
                ModelGO = Instantiate(AlliesModel.transform.GetChild(i).gameObject, ModelSpinner.transform.position, Quaternion.identity);
                ModelGO.GetComponent<SpriteRenderer>().sortingOrder = 5005;
                ModelGO.transform.SetParent(ModelSpinner.transform);
                break;
            }
        }
        string weaponName1 = (string)SessionData["LeftWeapon"];
        string weaponName2 = (string)SessionData["RightWeapon"];
        Debug.Log(weaponName1);
        Debug.Log(weaponName2);
        bool alreadyLeft = false;
        bool alreadyRight = false;
        Debug.Log(Weapons.transform.childCount);
        for (int i = 0; i < Weapons.transform.childCount; i++)
        {
            if (alreadyLeft && alreadyRight)
            {
                break;
            }
            if (!alreadyLeft && Weapons.transform.GetChild(i).name.Replace(" ", "").ToLower().Equals(weaponName1.Replace(" ", "").ToLower()))
            {
                alreadyLeft = true;
                LeftWeapon = Weapons.transform.GetChild(i).gameObject;
            }
            if (!alreadyRight && Weapons.transform.GetChild(i).name.Replace(" ", "").ToLower().Equals(weaponName2.Replace(" ", "").ToLower()))
            {
                alreadyRight = true;
                RightWeapon = Weapons.transform.GetChild(i).gameObject;
            }
        }
        ModelLeftWeapon = Instantiate(LeftWeapon, transform.position, Quaternion.identity);
        ModelLeftWeapon.transform.localScale = new Vector3(1, 1, 1);
        ModelLeftWeapon.transform.SetParent(ModelGO.transform);
        ModelLeftWeapon.GetComponent<SpriteRenderer>().sortingOrder = 5004;
        ModelLeftWeapon.SetActive(true);
        Destroy(ModelLeftWeapon.GetComponent<Weapons>());

        ModelRightWeapon = Instantiate(RightWeapon, transform.position, Quaternion.identity);
        ModelRightWeapon.transform.localScale = new Vector3(1, 1, 1);
        ModelRightWeapon.transform.SetParent(ModelGO.transform);
        ModelRightWeapon.GetComponent<SpriteRenderer>().sortingOrder = 5004;
        ModelRightWeapon.SetActive(true);
        Destroy(ModelRightWeapon.GetComponent<Weapons>());

        Vector3 LeftWeaponPos = new Vector3(
            ModelGO.transform.position.x + ModelGO.GetComponent<FighterModelShared>().LeftWeaponPos.x * ModelGO.transform.localScale.x * 1f / ModelGO.transform.localScale.x,
            ModelGO.transform.position.y + ModelGO.GetComponent<FighterModelShared>().LeftWeaponPos.y * ModelGO.transform.localScale.y * 1f / ModelGO.transform.localScale.y,
            ModelGO.transform.position.z);
        Vector3 RightWeaponPos = new Vector3(
            ModelGO.transform.position.x + ModelGO.GetComponent<FighterModelShared>().RightWeaponPos.x * ModelGO.transform.localScale.x * 1f / ModelGO.transform.localScale.x,
            ModelGO.transform.position.y + ModelGO.GetComponent<FighterModelShared>().RightWeaponPos.y * ModelGO.transform.localScale.y * 1f / ModelGO.transform.localScale.y,
            ModelGO.transform.position.z);
        ModelLeftWeapon.transform.position = LeftWeaponPos;
        
        ModelRightWeapon.transform.position = RightWeaponPos;
        
        ModelGO.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        ModelGO.SetActive(true);
        ModelLeftWeapon.SetActive(true);
        ModelRightWeapon.SetActive(true);
        // Notable Allies and Enemies
        // Get Template Data
        TemplateData = FindObjectOfType<AccessDatabase>().GetStageZoneTemplateByStageValueAndVariant(SpaceZoneNo % 10, ChosenVariant);
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
        if (TemplateData["ArmyRating"] != null)
        {
            ArmyRating = (string)TemplateData["ArmyRating"];
            AllyWarship = (string)TemplateData["AllyWarship"];
            EnemyWarship = (string)TemplateData["EnemyWarship"];
        }
        float AllyArmyRating = 0;
        float EnemyArmyRating = 0;
        if (ArmyRating != "")
        {
            AllyArmyRating = float.Parse(ArmyRating.Split("|")[0]);
            EnemyArmyRating = float.Parse(ArmyRating.Split("|")[1]);
        }

        // Ally Army X Y Z
        float AllyArmyX = 0;
        float AllyArmyY = 0;
        float AllyArmyZ = 0;
        if (AllyWarship != "")
        {
            AllyArmyX = float.Parse(AllyWarship.Split("-")[0]);
            AllyArmyY = float.Parse(AllyWarship.Split("-")[1]);
            AllyArmyZ = float.Parse(AllyWarship.Split("-")[2]);
        }

        // Enemy Army X Y Z
        float EnemyArmyX = 0;
        float EnemyArmyY = 0;
        float EnemyArmyZ = 0;
        if (EnemyWarship != "")
        {
            EnemyArmyX = float.Parse(EnemyWarship.Split("-")[0]);
            EnemyArmyY = float.Parse(EnemyWarship.Split("-")[1]);
            EnemyArmyZ = float.Parse(EnemyWarship.Split("-")[2]);
        }

        int Scale20Even = SpaceZoneNo / 20;
        if (Scale20Even >= 1)
        {
            AllySquadRating *= (1 + Scale20Even / 10f);
            EnemySquadRating *= (1 + Scale20Even / 10f);
        }
        // (5n)*10
        // Enemy SS spawn
        int Scale50SSC = SpaceZoneNo / 50;
        if (Scale50SSC >= 1)
        {
            AllyArmyRating *= (1 + (float)Scale50SSC / 4);
            EnemyArmyRating *= (1 + (float)Scale50SSC / 4);
        }
        if (SpaceZoneNo < 350)
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
        }
        else
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
            if (SpaceZoneNo % 10 != 0 || ChosenVariant != 1)
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
        Dictionary<string, object> FighterGroupData = FindObjectOfType<AccessDatabase>().GetFighterGroupsDataByName((string)TemplateData["FighterGroup"] + (SpaceZoneNo > 350 ? "350" : ""));
        Dictionary<string, object> WarshipMilestone = FindObjectOfType<AccessDatabase>().GetWarshipMilestoneBySpaceZoneNo(SpaceZoneNo);
        NotableAlliesName.Clear();
        NotableEnemiesName.Clear();
        // Get Notable Allies
        if ((SpaceZoneNo%10==2 || SpaceZoneNo%10==4 || SpaceZoneNo%10==6))
        {
            if (ChosenVariant == 1)
                NotableAlliesName.Add("UEC_Station");
            if (ChosenVariant == 3)
                NotableAlliesName.Add("SSTP");
        }
        if (AllyArmyRating > 0)
        {
            if (AllyArmyX + AllyArmyY + AllyArmyZ != 0)
            {
                int AllyWarshipACount = (int)((float)AllyArmyRating / (AllyArmyX + AllyArmyY + AllyArmyZ) * AllyArmyX / 5);
                int AllyWarshipBCount = (int)((float)AllyArmyRating / (AllyArmyX + AllyArmyY + AllyArmyZ) * AllyArmyY / 5);
                int AllyWarshipCCount = (int)((float)AllyArmyRating / (AllyArmyX + AllyArmyY + AllyArmyZ) * AllyArmyZ / 5);
                if (AllyWarshipCCount > 0 && NotableAlliesName.Count < 3)
                {
                    int id = (int)WarshipMilestone["MilestoneAllyClassC"];
                    NotableAlliesName.Add((string)FindObjectOfType<AccessDatabase>().GetWSById(id)["WarshipName"]);
                }
                else if (AllyWarshipBCount > 0 && NotableAlliesName.Count < 3)
                {
                    int id = (int)WarshipMilestone["MilestoneAllyClassB"];
                    NotableAlliesName.Add((string)FindObjectOfType<AccessDatabase>().GetWSById(id)["WarshipName"]);
                }
                else if (AllyWarshipACount > 0 && NotableAlliesName.Count < 3)
                {
                    int id = (int)WarshipMilestone["MilestoneAllyClassA"];
                    NotableAlliesName.Add((string)FindObjectOfType<AccessDatabase>().GetWSById(id)["WarshipName"]);
                }
            }
        }
        int AllyFighterACount = (int)Mathf.Ceil((float)AllySquadRating / (AllySquadX + AllySquadY + AllySquadZ) * AllySquadX / 5);
        int AllyFighterBCount = (int)Mathf.Ceil((float)AllySquadRating / (AllySquadX + AllySquadY + AllySquadZ) * AllySquadY / 10);
        int AllyFighterCCount = (int)Mathf.Ceil((float)AllySquadRating / (AllySquadX + AllySquadY + AllySquadZ) * AllySquadZ / 30);
        if (AllyFighterCCount > 0 && NotableAlliesName.Count < 3)
        {
            string[] idList = ((string)FighterGroupData["AlliesFighterC"]).Split(",");
            for (int i = idList.Length - 1;i >= 0; i--)
            {
                if (NotableAlliesName.Count < 3)
                {
                    int id = int.Parse(idList[i]);
                    NotableAlliesName.Add((string)FindObjectOfType<AccessDatabase>().GetDataAlliesById(id)["Name"]);
                } else
                {
                    break;
                }
            }
        }
        if (AllyFighterBCount > 0 && NotableAlliesName.Count < 3)
        {
            string[] idList = ((string)FighterGroupData["AlliesFighterB"]).Split(",");
            for (int i = idList.Length - 1;i >= 0; i--)
            {
                if (NotableAlliesName.Count < 3)
                {
                    int id = int.Parse(idList[i]);
                    NotableAlliesName.Add((string)FindObjectOfType<AccessDatabase>().GetDataAlliesById(id)["Name"]);
                } else
                {
                    break;
                }
            }
        }
        if (AllyFighterACount > 0 && NotableAlliesName.Count < 3)
        {
            string[] idList = ((string)FighterGroupData["AlliesFighterA"]).Split(",");
            for (int i = idList.Length - 1;i >= 0; i--)
            {
                if (NotableAlliesName.Count < 3)
                {
                    int id = int.Parse(idList[i]);
                    NotableAlliesName.Add((string)FindObjectOfType<AccessDatabase>().GetDataAlliesById(id)["Name"]);
                } else
                {
                    break;
                }
            }
        }
        // Generate Notable Allies Model
        for (int i=0; i<NotableAlliesName.Count;i++)
        {
            GameObject Place = NotableAllies.transform.GetChild(i + 1).gameObject;
            Place.SetActive(true);
            bool done = false;
            if (!done)
                for (int j=0; j< AlliesModel.transform.childCount;j++)
                {
                    if (AlliesModel.transform.GetChild(j).name.Replace(" ","").ToLower() == NotableAlliesName[i].Replace(" ", "").ToLower())
                    {
                        GameObject go = Instantiate(AlliesModel.transform.GetChild(j).gameObject, Place.transform.position, Quaternion.identity);
                        go.transform.SetParent(Place.transform);
                        go.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                        go.GetComponent<SpriteRenderer>().sortingOrder = 5003;
                        go.SetActive(true);
                        ListNotableAlliesGO.Add(go);
                        Place.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = NotableAlliesName[i];
                        done = true;
                        break;
                    }
                }
            if (!done)
                for (int j = 0; j < SpaceStationModel.transform.childCount; j++)
                {
                    if (SpaceStationModel.transform.GetChild(j).name.Replace(" ", "").ToLower() == NotableAlliesName[i].Replace(" ", "").ToLower())
                    {
                        GameObject go = Instantiate(SpaceStationModel.transform.GetChild(j).gameObject, Place.transform.position, Quaternion.identity);
                        go.transform.SetParent(Place.transform);
                        go.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                        go.GetComponent<SpriteRenderer>().sortingOrder = 5003;
                        go.SetActive(true);
                        ListNotableAlliesGO.Add(go);
                        Place.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = NotableAlliesName[i];
                        done = true;
                        break;
                    }
                }
            if (!done)
                for (int j = 0; j < WarshipModel.transform.childCount; j++)
                {
                    if (WarshipModel.transform.GetChild(j).name.Replace(" ", "").Replace("_", "-").ToLower() == NotableAlliesName[i].Replace(" ", "").ToLower())
                    {
                        GameObject go = Instantiate(WarshipModel.transform.GetChild(j).gameObject, Place.transform.position, Quaternion.identity);
                        go.transform.SetParent(Place.transform);
                        go.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                        go.GetComponent<SpriteRenderer>().sortingOrder = 5003;
                        go.SetActive(true);
                        ListNotableAlliesGO.Add(go);
                        Place.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = NotableAlliesName[i];
                        done = true;
                        break;
                    }
                }
        }
        // Get Notable Enemies
        if (EnemyArmyRating > 0)
        {
            if (EnemyArmyX + EnemyArmyY + EnemyArmyZ != 0)
            {
                int EnemyWarshipACount = (int)((float)EnemyArmyRating / (EnemyArmyX + EnemyArmyY + EnemyArmyZ) * EnemyArmyX / 5);
                int EnemyWarshipBCount = (int)((float)EnemyArmyRating / (EnemyArmyX + EnemyArmyY + EnemyArmyZ) * EnemyArmyY / 5);
                int EnemyWarshipCCount = (int)((float)EnemyArmyRating / (EnemyArmyX + EnemyArmyY + EnemyArmyZ) * EnemyArmyZ / 5);
                if (EnemyWarshipCCount > 0 && NotableEnemiesName.Count < 3)
                {
                    int id = (int)WarshipMilestone["MilestoneEnemyClassC"];
                    NotableEnemiesName.Add((string)FindObjectOfType<AccessDatabase>().GetWSById(id)["WarshipName"]);
                }
                else if (EnemyWarshipBCount > 0 && NotableEnemiesName.Count < 3)
                {
                    int id = (int)WarshipMilestone["MilestoneEnemyClassB"];
                    NotableEnemiesName.Add((string)FindObjectOfType<AccessDatabase>().GetWSById(id)["WarshipName"]);
                }
                else if (EnemyWarshipACount > 0 && NotableEnemiesName.Count < 3)
                {
                    int id = (int)WarshipMilestone["MilestoneEnemyClassA"];
                    NotableEnemiesName.Add((string)FindObjectOfType<AccessDatabase>().GetWSById(id)["WarshipName"]);
                }
            }
        }
        int EnemyFighterACount = (int)Mathf.Ceil((float)EnemySquadRating / (EnemySquadX + EnemySquadY + EnemySquadZ) * EnemySquadX / 5);
        int EnemyFighterBCount = (int)Mathf.Ceil((float)EnemySquadRating / (EnemySquadX + EnemySquadY + EnemySquadZ) * EnemySquadY / 10);
        int EnemyFighterCCount = (int)Mathf.Ceil((float)EnemySquadRating / (EnemySquadX + EnemySquadY + EnemySquadZ) * EnemySquadZ / 30);
        if (EnemyFighterCCount > 0 && NotableEnemiesName.Count < 3)
        {
            string[] idList = ((string)FighterGroupData["EnemiesFighterC"]).Split(",");
            for (int i = idList.Length - 1; i >= 0; i--)
            {
                if (NotableEnemiesName.Count < 3)
                {
                    int id = int.Parse(idList[i]);
                    NotableEnemiesName.Add((string)FindObjectOfType<AccessDatabase>().GetDataEnemyById(id)["Name"]);
                }
                else
                {
                    break;
                }
            }
        }
        if (EnemyFighterBCount > 0 && NotableEnemiesName.Count < 3)
        {
            string[] idList = ((string)FighterGroupData["EnemiesFighterB"]).Split(",");
            for (int i = idList.Length - 1; i >= 0; i--)
            {
                if (NotableEnemiesName.Count < 3)
                {
                    int id = int.Parse(idList[i]);
                    NotableEnemiesName.Add((string)FindObjectOfType<AccessDatabase>().GetDataEnemyById(id)["Name"]);
                }
                else
                {
                    break;
                }
            }
        }
        if (EnemyFighterACount > 0 && NotableEnemiesName.Count < 3)
        {
            string[] idList = ((string)FighterGroupData["EnemiesFighterA"]).Split(",");
            for (int i = idList.Length - 1; i >= 0; i--)
            {
                if (NotableEnemiesName.Count < 3)
                {
                    int id = int.Parse(idList[i]);
                    NotableEnemiesName.Add((string)FindObjectOfType<AccessDatabase>().GetDataEnemyById(id)["Name"]);
                }
                else
                {
                    break;
                }
            }
        }
        // Generate Notable Enemies Model
        for (int i = 0; i < NotableEnemiesName.Count; i++)
        {
            GameObject Place = NotableEnemies.transform.GetChild(i + 1).gameObject;
            Place.SetActive(true);
            bool done = false;
                if (!done)
                for (int j = 0; j < EnemiesModel.transform.childCount; j++)
                {

                    if (EnemiesModel.transform.GetChild(j).name.Replace(" ", "").ToLower() == NotableEnemiesName[i].Replace(" ", "").ToLower())
                    {
                        GameObject go = Instantiate(EnemiesModel.transform.GetChild(j).gameObject, Place.transform.position, Quaternion.identity);
                        go.transform.SetParent(Place.transform);
                        go.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                        go.GetComponent<SpriteRenderer>().sortingOrder = 5003;
                        go.SetActive(true);
                        ListNotableEnemiesGO.Add(go);
                        Place.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = NotableEnemiesName[i];
                        done = true;
                        break;
                    }
                }
            if (!done)
                for (int j = 0; j < SpaceStationModel.transform.childCount; j++)
                {
                    if (SpaceStationModel.transform.GetChild(j).name.Replace(" ", "").ToLower() == NotableEnemiesName[i].Replace(" ", "").ToLower())
                    {
                        GameObject go = Instantiate(SpaceStationModel.transform.GetChild(j).gameObject, Place.transform.position, Quaternion.identity);
                        go.transform.SetParent(Place.transform);
                        go.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                        go.GetComponent<SpriteRenderer>().sortingOrder = 5003;
                        go.SetActive(true);
                        ListNotableEnemiesGO.Add(go);
                        Place.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = NotableEnemiesName[i];
                        done = true;
                        break;
                    }
                }
            if (!done)
                for (int j = 0; j < WarshipModel.transform.childCount; j++)
                {
                    if (WarshipModel.transform.GetChild(j).name.Replace(" ", "").Replace("_", "-").ToLower() == NotableEnemiesName[i].Replace(" ", "").ToLower())
                    {
                        GameObject go = Instantiate(WarshipModel.transform.GetChild(j).gameObject, Place.transform.position, Quaternion.identity);
                        go.transform.SetParent(Place.transform);
                        go.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                        go.GetComponent<SpriteRenderer>().sortingOrder = 5003;
                        go.SetActive(true);
                        ListNotableEnemiesGO.Add(go);
                        Place.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = NotableEnemiesName[i];
                        done = true;
                        break;
                    }
                }
        }
    }
    #endregion
    #region Data
    private int RandomHazardChoose(List<Dictionary<string, object>> dataDict)
    {
        float sum = 0;
        for (int i = 0; i < dataDict.Count; i++)
        {
            sum += (int)dataDict[i]["HazardChance"];
        }
        float n = Random.Range(0, sum);
        for (int i = 0; i < dataDict.Count; i++)
        {
            if (n < (int)dataDict[i]["HazardChance"])
            {
                return (int)dataDict[i]["HazardID"];
            }
            else
            {
                n -= (int)dataDict[i]["HazardChance"];
            }
        }
        return 1;
    }

    private void OnDisable()
    {
        Destroy(ModelGO);
        int n = 0;
        while (n < ListNotableAlliesGO.Count)
        {
            if (ListNotableAlliesGO[n]!=null)
            {
                GameObject temp = ListNotableAlliesGO[n];
                ListNotableAlliesGO.Remove(temp);
                Destroy(temp);
            } else
            {
                n++;
            }
        }
        n = 0;
        while (n < ListNotableEnemiesGO.Count)
        {
            if (ListNotableEnemiesGO[n] != null)
            {
                GameObject temp = ListNotableEnemiesGO[n];
                ListNotableEnemiesGO.Remove(temp);
                Destroy(temp);
            }
            else
            {
                n++;
            }
        }
    }
    #endregion
}
