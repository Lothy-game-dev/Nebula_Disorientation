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
    public List<GameObject> DisableColliders;
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
    private string ChosenSZD1;
    private string ChosenEnemySS;
    private List<string> ChosenAllyFC;
    private List<string> ChosenAllyFB;
    private List<string> ChosenAllyFA;
    private List<string> ChosenEnemyFC;
    private List<string> ChosenEnemyFB;
    private List<string> ChosenEnemyFA;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        ChosenSZD1 = "";
        ChosenEnemySS = "";
        ChosenAllyFC = new();
        ChosenAllyFB = new();
        ChosenAllyFA = new();
        ChosenEnemyFC = new();
        ChosenEnemyFB = new();
        ChosenEnemyFA = new();
        if (DisableColliders.Count > 0)
        {
            foreach (var col in DisableColliders)
            {
                if (col.GetComponent<Collider2D>()!=null)
                col.GetComponent<Collider2D>().enabled = false;
            }
        }
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
        // Get Predict data
        string Predict = PlayerPrefs.GetString("Predict" + PlayerPrefs.GetInt("PlayerID"));
        if (Predict.Length > 0)
        {
            ChosenSZD1 = Predict.Split("|")[0];
            ChosenEnemySS = Predict.Split("|")[1];
            ChosenAllyFC = new List<string>(Predict.Split("|")[2].Split("~"));
            ChosenAllyFB = new List<string>(Predict.Split("|")[3].Split("~"));
            ChosenAllyFA = new List<string>(Predict.Split("|")[4].Split("~"));
            ChosenEnemyFC = new List<string>(Predict.Split("|")[5].Split("~"));
            ChosenEnemyFB = new List<string>(Predict.Split("|")[6].Split("~"));
            ChosenEnemyFA = new List<string>(Predict.Split("|")[7].Split("~"));
        }

        ListNotableAlliesGO = new List<GameObject>();
        ListNotableEnemiesGO = new List<GameObject>();
        transform.position = new Vector3(Camera.main.transform.position.x,Camera.main.transform.position.y,transform.position.z);
        SessionData = FindObjectOfType<AccessDatabase>().GetSessionInfoByPlayerId(PlayerPrefs.GetInt("PlayerID"));
        // Get Next Stage Number, Variant and Hazard
        SpaceZoneNo = (int)SessionData["CurrentStage"];
        ChosenVariant = (int)SessionData["CurrentStageVariant"];
        ChosenHazard = (int)SessionData["CurrentStageHazard"];
        // Set Data To View Next Stage Number, Variant and Hazard
        IncomingStage.GetComponent<TextMeshPro>().text = "Upcoming: Space Zone no <color=#3bccec>" + SpaceZoneNo + "</color>";
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
        if (SpaceZoneNo < 200)
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
            AllySquadY = 8;
            AllySquadZ = 2;
            EnemySquadX = 0;
            EnemySquadY = 8;
            EnemySquadZ = 2;
        }

        // Squad for (10n)*10
        int SquadScale = SpaceZoneNo / 100;

        int XLimit = 6;
        if (SquadScale >= 1)
        {
            if (SpaceZoneNo % 10 != 0 || ChosenVariant != 1)
            {
                float realScaleAllyArmy = 0;
                if (AllyArmyX > 0)
                {
                    if (AllyArmyX - (float)SquadScale / 2 < XLimit)
                    {
                        realScaleAllyArmy = AllyArmyX - XLimit;
                        AllyArmyX = XLimit;
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
                    if (EnemyArmyX - (float)SquadScale / 2 < XLimit)
                    {
                        realScaleEnemyArmy = EnemyArmyX - XLimit;
                        EnemyArmyX = XLimit;
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
        Dictionary<string, object> FighterGroupData = FindObjectOfType<AccessDatabase>().GetFighterGroupsDataByName((string)TemplateData["FighterGroup"] + (SpaceZoneNo > 200 ? "200" : ""));
        Dictionary<string, object> WarshipMilestone = FindObjectOfType<AccessDatabase>().GetWarshipMilestoneBySpaceZoneNo(SpaceZoneNo);
        NotableAlliesName.Clear();
        NotableEnemiesName.Clear();
        // Get Notable Allies
        if ((SpaceZoneNo%10==2 || SpaceZoneNo%10==4 || SpaceZoneNo%10==6))
        {
            if (ChosenVariant == 1)
            {
                float n = Random.Range(0, 100f);
                string chosen = "";
                if (ChosenSZD1=="")
                {
                    if (n <= 50f)
                    {
                        int r = Random.Range(0, 2);
                        if (r==0)
                        {
                            chosen = ("UEC-Frigate");
                        } else if (r==1)
                        {
                            chosen = ("UEC-Carrier");
                        }
                    }
                    else if (n <= 80f)
                    {
                        int r = Random.Range(0, 2);
                        if (r == 0)
                        {
                            chosen = ("UEC-Cruiser");
                        }
                        else if (r == 1)
                        {
                            chosen = ("UEC-Battleship");
                        }
                    }
                    else if (n<=95f)
                    {
                        int r = Random.Range(0, 2);
                        if (r == 0)
                        {
                            chosen = ("UEC-Dreadnaught");
                        }
                        else if (r == 1)
                        {
                            chosen = ("UEC-Flagship");
                        }
                    } 
                    else
                    {
                        chosen = ("UEC-Station");
                    }
                    ChosenSZD1 = chosen;
                } else
                {
                    chosen = ChosenSZD1;
                }
                NotableAlliesName.Add(chosen);
            }
            if (ChosenVariant == 3)
                NotableAlliesName.Add("<color=#00ff00>SSTP</color>");
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
            if (ChosenAllyFC.Count > 0)
            {
                foreach (var str in ChosenAllyFC)
                {
                    NotableAlliesName.Add("<color=#ff0000>" + str + "</color>");
                    if (NotableAlliesName.Count >= 3)
                    {
                        break;
                    }
                }
            } else
            {
                string[] idList = ((string)FighterGroupData["AlliesFighterC"]).Split(",");
                int count = AllyFighterCCount;
                for (int i = idList.Length - 1; i >= 0; i--)
                {
                    if (NotableAlliesName.Count < 3)
                    {
                        int id = int.Parse(idList[i]);
                        NotableAlliesName.Add("<color=#ff0000>" + (string)FindObjectOfType<AccessDatabase>().GetDataAlliesById(id)["Name"] +"</color>");
                        ChosenAllyFC.Add((string)FindObjectOfType<AccessDatabase>().GetDataAlliesById(id)["Name"]);
                    }
                    else
                    {
                        break;
                    }
                    count--;
                    if (count<=0)
                    {
                        break;
                    }
                }
            }
        }
        if (AllyFighterBCount > 0 && NotableAlliesName.Count < 3)
        {
            if (ChosenAllyFB.Count > 0)
            {
                foreach (var str in ChosenAllyFB)
                {
                    NotableAlliesName.Add("<color=#0000ff>" + str + "</color>");
                    if (NotableAlliesName.Count >= 3)
                    {
                        break;
                    }
                }
            }
            else
            {
                int count = AllyFighterBCount;
                string[] idList = ((string)FighterGroupData["AlliesFighterB"]).Split(",");
                for (int i = idList.Length - 1; i >= 0; i--)
                {
                    if (NotableAlliesName.Count < 3)
                    {
                        int id = int.Parse(idList[i]);
                        NotableAlliesName.Add("<color=#0000ff>" + (string)FindObjectOfType<AccessDatabase>().GetDataAlliesById(id)["Name"] + "</color>");
                        ChosenAllyFB.Add((string)FindObjectOfType<AccessDatabase>().GetDataAlliesById(id)["Name"]);
                    }
                    else
                    {
                        break;
                    }
                    count--;
                    if (count <= 0)
                    {
                        break;
                    }
                }
            }
        }
        if (AllyFighterACount > 0 && NotableAlliesName.Count < 3)
        {
            if (ChosenAllyFA.Count > 0)
            {
                foreach (var str in ChosenAllyFA)
                {
                    NotableAlliesName.Add("<color=#00ff00>" + str + "</color>");
                    if (NotableAlliesName.Count >= 3)
                    {
                        break;
                    }
                }
            }
            else
            {
                int count = AllyFighterACount;
                string[] idList = ((string)FighterGroupData["AlliesFighterA"]).Split(",");
                for (int i = idList.Length - 1; i >= 0; i--)
                {
                    if (NotableAlliesName.Count < 3)
                    {
                        int id = int.Parse(idList[i]);
                        NotableAlliesName.Add("<color=#00ff00>" + (string)FindObjectOfType<AccessDatabase>().GetDataAlliesById(id)["Name"] + "</color>");
                        ChosenAllyFA.Add((string)FindObjectOfType<AccessDatabase>().GetDataAlliesById(id)["Name"]);
                    }
                    else
                    {
                        break;
                    }
                    count--;
                    if (count <= 0)
                    {
                        break;
                    }
                }
            } 
        }
        // Generate Notable Allies Model
        for (int i=0; i<NotableAlliesName.Count;i++)
        {
            GameObject Place = NotableAllies.transform.GetChild(i + 1).gameObject;
            bool done = false;
            if (!done)
                for (int j=0; j< AlliesModel.transform.childCount;j++)
                {
                    if (AlliesModel.transform.GetChild(j).name.Replace(" ", "").ToLower() == NotableAlliesName[i].Replace(" ", "").Replace("<color=#0000ff>","").Replace("<color=#00ff00>","").Replace("<color=#ff0000>","").Replace("</color>","").ToLower())
                    {
                        GameObject go = Instantiate(AlliesModel.transform.GetChild(j).gameObject, Place.transform.position, Quaternion.identity);
                        go.transform.SetParent(Place.transform);
                        go.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                        go.GetComponent<SpriteRenderer>().sortingOrder = 5003;
                        go.SetActive(true);
                        ListNotableAlliesGO.Add(go);
                        Place.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = NotableAlliesName[i];
                        Place.GetComponent<SpaceZoneIntroBox>().ObjectName = AlliesModel.transform.GetChild(j).name;
                        Place.GetComponent<SpaceZoneIntroBox>().Type = "Ally";
                        Place.GetComponent<SpaceZoneIntroBox>().GenerateValue();
                        done = true;
                        break;
                    }
                }
            if (!done)
                for (int j = 0; j < SpaceStationModel.transform.childCount; j++)
                {
                    if (SpaceStationModel.transform.GetChild(j).name.Replace(" ", "").Replace("_", "-").ToLower() == NotableAlliesName[i].Replace(" ", "").ToLower())
                    {
                        GameObject go = Instantiate(SpaceStationModel.transform.GetChild(j).gameObject, Place.transform.position, Quaternion.identity);
                        go.transform.SetParent(Place.transform);
                        go.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                        go.GetComponent<SpriteRenderer>().sortingOrder = 5003;
                        go.SetActive(true);
                        ListNotableAlliesGO.Add(go);
                        Place.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = NotableAlliesName[i];
                        Place.GetComponent<SpaceZoneIntroBox>().ObjectName = SpaceStationModel.transform.GetChild(j).name;
                        Place.GetComponent<SpaceZoneIntroBox>().Type = "SpaceStation";
                        Place.GetComponent<SpaceZoneIntroBox>().GenerateValue();
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
                        go.transform.localScale = new Vector3(0.225f, 0.225f, 0.225f);
                        go.GetComponent<SpriteRenderer>().sortingOrder = 5003;
                        go.SetActive(true);
                        ListNotableAlliesGO.Add(go);
                        Place.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = NotableAlliesName[i];
                        Place.GetComponent<SpaceZoneIntroBox>().ObjectName = WarshipModel.transform.GetChild(j).name;
                        Place.GetComponent<SpaceZoneIntroBox>().Type = "Warship";
                        Place.GetComponent<SpaceZoneIntroBox>().GenerateValue();
                        done = true;
                        break;
                    }
                }
            Place.SetActive(true);
        }
        if (SpaceZoneNo%50==0 && SpaceZoneNo>0)
        {
            int n = SpaceZoneNo / 50;
            if (ChosenEnemySS!="")
            {
                if (ChosenEnemySS != "NO")
                    NotableEnemiesName.Add(ChosenEnemySS);
            } else
            {
                float k = Random.Range(1, 100f);
                if (k <= (n * 10 > 50 ? 50 : n * 10))
                {
                    ChosenEnemySS = "Zat-Station";
                }
                else
                {
                    ChosenEnemySS = "NO";
                }
                if (ChosenEnemySS!="NO")
                NotableEnemiesName.Add(ChosenEnemySS);
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
        if ((SpaceZoneNo % 10 == 1 || SpaceZoneNo % 10 == 3 || SpaceZoneNo % 10 == 5 || SpaceZoneNo % 10 == 7) && ChosenVariant == 2)
        {
            if (EnemyFighterCCount > 0)
            {
                Mission.transform.GetChild(1).GetChild(0).GetComponent<TextMeshPro>().text = ((string)MissionData["Mission"]).Replace("Target", "<color=#ff0000>Class C</color>");
            } 
            else if (EnemyFighterBCount > 0)
            {
                Mission.transform.GetChild(1).GetChild(0).GetComponent<TextMeshPro>().text = ((string)MissionData["Mission"]).Replace("Target", "<color=#0000ff>Class B</color>");
            }
            else if(EnemyFighterACount > 0)
            {
                Mission.transform.GetChild(1).GetChild(0).GetComponent<TextMeshPro>().text = ((string)MissionData["Mission"]).Replace("Target", "<color=#00ff00>Class A</color>");
            }
        }
        // ZatSBB
        /*if (SpaceZoneNo % 10 == 2 || SpaceZoneNo % 10 == 4 || SpaceZoneNo % 10 == 6)
        {

        } else if (SpaceZoneNo % 10 == 8 || SpaceZoneNo % 10 == 9)
        {

        }*/
        if (EnemyFighterCCount > 0 && NotableEnemiesName.Count < 3)
        {
            if (ChosenEnemyFC.Count > 0)
            {
                foreach (var str in ChosenEnemyFC)
                {
                    NotableEnemiesName.Add("<color=#ff0000>" + str + "</color>");
                    if (NotableEnemiesName.Count >= 3)
                    {
                        break;
                    }
                }
            }
            else
            {
                int count = EnemyFighterCCount;
                string[] idList = ((string)FighterGroupData["EnemiesFighterC"]).Split(",");
                for (int i = idList.Length - 1; i >= 0; i--)
                {
                    if (NotableEnemiesName.Count < 3)
                    {
                        int id = int.Parse(idList[i]);
                        NotableEnemiesName.Add("<color=#ff0000>" + (string)FindObjectOfType<AccessDatabase>().GetDataEnemyById(id)["Name"] + "</color>");
                        ChosenEnemyFC.Add((string)FindObjectOfType<AccessDatabase>().GetDataEnemyById(id)["Name"]);
                    }
                    else
                    {
                        break;
                    }
                    count--;
                    if (count <= 0)
                    {
                        break;
                    }
                }
            }
        }
        if (EnemyFighterBCount > 0 && NotableEnemiesName.Count < 3)
        {
            if (ChosenEnemyFB.Count > 0)
            {
                foreach (var str in ChosenEnemyFB)
                {
                    NotableEnemiesName.Add("<color=#0000ff>" + str + "</color>");
                    if (NotableEnemiesName.Count >= 3)
                    {
                        break;
                    }
                }
            }
            else
            {
                int count = EnemyFighterBCount;
                string[] idList = ((string)FighterGroupData["EnemiesFighterB"]).Split(",");
                for (int i = idList.Length - 1; i >= 0; i--)
                {
                    if (NotableEnemiesName.Count < 3)
                    {
                        int id = int.Parse(idList[i]);
                        NotableEnemiesName.Add("<color=#0000ff>" + (string)FindObjectOfType<AccessDatabase>().GetDataEnemyById(id)["Name"] + "</color>");
                        ChosenEnemyFB.Add((string)FindObjectOfType<AccessDatabase>().GetDataEnemyById(id)["Name"]);
                    }
                    else
                    {
                        break;
                    }
                    count--;
                    if (count <= 0)
                    {
                        break;
                    }
                }
            }
        }
        if (EnemyFighterACount > 0 && NotableEnemiesName.Count < 3)
        {
            if (ChosenEnemyFA.Count > 0)
            {
                foreach (var str in ChosenEnemyFA)
                {
                    NotableEnemiesName.Add("<color=#00ff00>" + str + "</color>");
                    if (NotableEnemiesName.Count >= 3)
                    {
                        break;
                    }
                }
            }
            else
            {
                int count = EnemyFighterACount;
                string[] idList = ((string)FighterGroupData["EnemiesFighterA"]).Split(",");
                for (int i = idList.Length - 1; i >= 0; i--)
                {
                    if (NotableEnemiesName.Count < 3)
                    {
                        int id = int.Parse(idList[i]);
                        NotableEnemiesName.Add("<color=#00ff00>" + (string)FindObjectOfType<AccessDatabase>().GetDataEnemyById(id)["Name"] + "</color>");
                        ChosenEnemyFA.Add((string)FindObjectOfType<AccessDatabase>().GetDataEnemyById(id)["Name"]);
                    }
                    else
                    {
                        break;
                    }
                    count--;
                    if (count <= 0)
                    {
                        break;
                    }
                }
            }
        }
        // Generate Notable Enemies Model
        for (int i = 0; i < NotableEnemiesName.Count; i++)
        {
            GameObject Place = NotableEnemies.transform.GetChild(i + 1).gameObject;
            bool done = false;
                if (!done)
                for (int j = 0; j < EnemiesModel.transform.childCount; j++)
                {

                    if (EnemiesModel.transform.GetChild(j).name.Replace(" ", "").ToLower() == NotableEnemiesName[i].Replace(" ", "").Replace("<color=#0000ff>", "").Replace("<color=#00ff00>", "").Replace("<color=#ff0000>", "").Replace("</color>", "").ToLower())
                    {
                        GameObject go = Instantiate(EnemiesModel.transform.GetChild(j).gameObject, Place.transform.position, Quaternion.identity);
                        go.transform.SetParent(Place.transform);
                        go.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                        go.GetComponent<SpriteRenderer>().sortingOrder = 5003;
                        go.SetActive(true);
                        ListNotableEnemiesGO.Add(go);
                        Place.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = NotableEnemiesName[i];
                        Place.GetComponent<SpaceZoneIntroBox>().ObjectName = EnemiesModel.transform.GetChild(j).name;
                        Place.GetComponent<SpaceZoneIntroBox>().Type = "Enemy";
                        Place.GetComponent<SpaceZoneIntroBox>().GenerateValue();
                        done = true;
                        break;
                    }
                }
            if (!done)
                for (int j = 0; j < SpaceStationModel.transform.childCount; j++)
                {
                    if (SpaceStationModel.transform.GetChild(j).name.Replace(" ", "").Replace("_", "-").ToLower() == NotableEnemiesName[i].Replace(" ", "").ToLower())
                    {
                        GameObject go = Instantiate(SpaceStationModel.transform.GetChild(j).gameObject, Place.transform.position, Quaternion.identity);
                        go.transform.SetParent(Place.transform);
                        go.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                        go.GetComponent<SpriteRenderer>().sortingOrder = 5003;
                        go.SetActive(true);
                        ListNotableEnemiesGO.Add(go);
                        Place.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = NotableEnemiesName[i];
                        Place.GetComponent<SpaceZoneIntroBox>().ObjectName = SpaceStationModel.transform.GetChild(j).name;
                        Place.GetComponent<SpaceZoneIntroBox>().Type = "SpaceStation";
                        Place.GetComponent<SpaceZoneIntroBox>().GenerateValue();
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
                        go.transform.localScale = new Vector3(0.225f, 0.225f, 0.225f);
                        go.GetComponent<SpriteRenderer>().sortingOrder = 5003;
                        go.SetActive(true);
                        ListNotableEnemiesGO.Add(go);
                        Place.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>().text = NotableEnemiesName[i];
                        Place.GetComponent<SpaceZoneIntroBox>().ObjectName = WarshipModel.transform.GetChild(j).name;
                        Place.GetComponent<SpaceZoneIntroBox>().Type = "Warship";
                        Place.GetComponent<SpaceZoneIntroBox>().GenerateValue();
                        done = true;
                        break;
                    }
                }
            Place.SetActive(true);
        }
        // Add Data
        if (PlayerPrefs.GetString("Predict" + PlayerPrefs.GetInt("PlayerID"))=="")
        {
            string Final = ChosenSZD1 + "|" + ChosenEnemySS + "|";
            for (int i = 0; i < ChosenAllyFC.Count; i++)
            {
                if (i == ChosenAllyFC.Count - 1)
                {
                    Final += ChosenAllyFC[i];
                }
                else
                {
                    Final += ChosenAllyFC[i] + "~";
                }
            }
            Final += "|";
            for (int i = 0; i < ChosenAllyFB.Count; i++)
            {
                if (i == ChosenAllyFB.Count - 1)
                {
                    Final += ChosenAllyFB[i];
                }
                else
                {
                    Final += ChosenAllyFB[i] + "~";
                }
            }
            Final += "|";
            for (int i = 0; i < ChosenAllyFA.Count; i++)
            {
                if (i == ChosenAllyFA.Count - 1)
                {
                    Final += ChosenAllyFA[i];
                }
                else
                {
                    Final += ChosenAllyFA[i] + "~";
                }
            }
            Final += "|";
            for (int i = 0; i < ChosenEnemyFC.Count; i++)
            {
                if (i == ChosenEnemyFC.Count - 1)
                {
                    Final += ChosenEnemyFC[i];
                }
                else
                {
                    Final += ChosenEnemyFC[i] + "~";
                }
            }
            Final += "|";
            for (int i = 0; i < ChosenEnemyFB.Count; i++)
            {
                if (i == ChosenEnemyFB.Count - 1)
                {
                    Final += ChosenEnemyFB[i];
                }
                else
                {
                    Final += ChosenEnemyFB[i] + "~";
                }
            }
            Final += "|";
            for (int i = 0; i < ChosenEnemyFA.Count; i++)
            {
                if (i == ChosenEnemyFA.Count - 1)
                {
                    Final += ChosenEnemyFA[i];
                }
                else
                {
                    Final += ChosenEnemyFA[i] + "~";
                }
            }
            PlayerPrefs.SetString("Predict" + PlayerPrefs.GetInt("PlayerID"), Final);
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
        if (DisableColliders.Count > 0)
        {
            foreach (var col in DisableColliders)
            {
                if (col.GetComponent<Collider2D>() != null)
                    col.GetComponent<Collider2D>().enabled = true;
            }
        }
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
