using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Controller used to generate Fighter as start of each stage.
public class FighterController : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Weapons;
    public GameObject PlayerFighter;
    public GameObject LeftWeaponGO;
    public GameObject RightWeaponGO;
    public GameObject Aim;
    public GameObject LeftOverheatBar;
    public GameObject RightOverheatBar;
    public GameObject LeftOverheatImage;
    public GameObject RightOverheatImage;
    public GameObject LeftReloadBar;
    public GameObject RightReloadBar;
    public GameObject FirstPowerImage;
    public GameObject SecondPowerImage;
    public GameObject FighterModel;
    public GameObject PowerModel;
    public GameObject ConsumableModel;
    public LayerMask EnemyLayer;
    public List<GameObject> ConsumableImages;
    public LOTWEffect LOTWEffect;
    #endregion
    #region NormalVariables
    public string DatabaseModel;
    public string DatabaseLeftWeapon;
    public string DatabaseRightWeapon;
    public string DatabaseFirstPower;
    public string DatabaseSecondPower;
    public Dictionary<string, int> DatabaseConsumables;
    public GameObject CurrentModel;
    public GameObject CurrentLeftWeapon;
    public GameObject CurrentRightWeapon;
    public GameObject RightWeaponPosition;
    public GameObject LeftWeaponPosition;
    private Dictionary<string, object> dataDict;
    private GameObject FirstPower;
    private GameObject SecondPower;
    private int CurrentHP;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        // Retrieve Datas from DB to get inits data
        InitData();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Initialize Datas
    private void SetDataFromDB()
    {
        DatabaseConsumables = new Dictionary<string, int>();
        dataDict = FindObjectOfType<AccessDatabase>().GetSessionInfoByPlayerId(PlayerPrefs.GetInt("PlayerID"));
        DatabaseModel = (string)dataDict["Model"];
        DatabaseLeftWeapon = (string)dataDict["LeftWeapon"];
        DatabaseRightWeapon = (string)dataDict["RightWeapon"];
        DatabaseFirstPower = (string)dataDict["FirstPower"];
        DatabaseSecondPower = (string)dataDict["SecondPower"];
        string Consumables = (string)dataDict["Consumables"];
        if (Consumables.Length>0)
        {
            string[] ListCons = Consumables.Split("|");
            for (int i=0;i<ListCons.Length;i++)
            {
                DatabaseConsumables.Add(ListCons[i].Split("-")[0], int.Parse(ListCons[i].Split("-")[1]));
            }
        }
        CurrentHP = (int)dataDict["SessionCurrentHP"];
        PlayerFighter.GetComponent<PlayerFighter>().FighterName = DatabaseModel;

    }
    private void InitData()
    {
        PlayerFighter.SetActive(false);
        LOTWEffect.InitLOTW();
        // Get Data from DB
        SetDataFromDB();
        // Get Model from model list
        for (int i=0;i<FighterModel.transform.childCount;i++)
        {
            if (FighterModel.transform.GetChild(i).name.Replace(" ","").ToLower().Equals(DatabaseModel.Replace(" ", "").ToLower()))
            {
                CurrentModel = FighterModel.transform.GetChild(i).gameObject;
                break;
            }
        }

       
        // Change sprite on the model
        PlayerFighter.GetComponent<SpriteRenderer>().sprite = CurrentModel.GetComponent<SpriteRenderer>().sprite;
        // Calculate Weapon Pos based on Model
        PlayerFighter.transform.GetChild(0).position =
            new Vector3(PlayerFighter.transform.position.x + CurrentModel.GetComponent<FighterModelShared>().LeftWeaponPos.x * 30,
            PlayerFighter.transform.position.y + CurrentModel.GetComponent<FighterModelShared>().LeftWeaponPos.y * 30,
            PlayerFighter.transform.GetChild(0).position.z);
        PlayerFighter.transform.GetChild(1).position =
            new Vector3(PlayerFighter.transform.position.x + CurrentModel.GetComponent<FighterModelShared>().RightWeaponPos.x * 30,
            PlayerFighter.transform.position.y + CurrentModel.GetComponent<FighterModelShared>().RightWeaponPos.y * 30,
            PlayerFighter.transform.GetChild(1).position.z);
        LeftWeaponPosition = PlayerFighter.transform.GetChild(0).gameObject;
        RightWeaponPosition = PlayerFighter.transform.GetChild(1).gameObject;
        // Change Back Fire Pos Based on model
        PlayerFighter.transform.GetChild(2).position =
            new Vector3(PlayerFighter.transform.position.x + CurrentModel.GetComponent<FighterModelShared>().BackfirePos.x * 30,
            PlayerFighter.transform.position.y + (-0.4f + CurrentModel.GetComponent<FighterModelShared>().BackfirePos.y) * 30,
            PlayerFighter.transform.GetChild(2).position.z);
        // Get Current Weapon
        bool alreadyLeft = false;
        bool alreadyRight = false;
        for (int i=0;i<Weapons.transform.childCount;i++)
        {
            if (alreadyLeft && alreadyRight)
            {
                break;  
            }
            if (!alreadyLeft && Weapons.transform.GetChild(i).name.Replace(" ","").ToLower().Equals(DatabaseLeftWeapon.Replace(" ","").ToLower()))
            {
                alreadyLeft = true;
                CurrentLeftWeapon = Weapons.transform.GetChild(i).gameObject;
            }
            if (!alreadyRight && Weapons.transform.GetChild(i).name.Replace(" ", "").ToLower().Equals(DatabaseRightWeapon.Replace(" ", "").ToLower()))
            {
                alreadyRight = true;
                CurrentRightWeapon = Weapons.transform.GetChild(i).gameObject;
            }
        }
        // Instantiate Weapon and ready to aim
        GameObject LeftWeapon = Instantiate(CurrentLeftWeapon, LeftWeaponPosition.transform.position, Quaternion.identity);
        GameObject RightWeapon = Instantiate(CurrentRightWeapon, RightWeaponPosition.transform.position, Quaternion.identity);
        LeftWeapon.transform.localScale = new Vector2(LeftWeapon.transform.localScale.x * 2f, LeftWeapon.transform.localScale.y * 2f);
        RightWeapon.transform.localScale = new Vector2(RightWeapon.transform.localScale.x * 2f, RightWeapon.transform.localScale.y * 2f);
        LeftWeaponGO = LeftWeapon;
        RightWeaponGO = RightWeapon;
        PlayerFighter.GetComponent<PlayerFighter>().LeftWeapon = LeftWeaponGO;
        PlayerFighter.GetComponent<PlayerFighter>().RightWeapon = RightWeaponGO;
        LeftWeapon.SetActive(true);
        Weapons LW = LeftWeapon.GetComponent<Weapons>();
        LW.isLeftWeapon = true;
        LW.Fighter = PlayerFighter;
        LW.Aim = Aim;
        LW.WeaponPosition = LeftWeaponPosition;
        LW.tracking = true;
        LW.EnemyLayer = EnemyLayer;
        LW.ControllerMain = PlayerFighter.GetComponent<PlayerFighter>().ControllerMain;
        LW.LOTWEffect = LOTWEffect;
        RightWeapon.SetActive(true);
        Weapons RW = RightWeapon.GetComponent<Weapons>();
        RW.Fighter = PlayerFighter;
        RW.Aim = Aim;
        RW.WeaponPosition = RightWeaponPosition;
        RW.tracking = true;
        RW.EnemyLayer = EnemyLayer;
        RW.ControllerMain = PlayerFighter.GetComponent<PlayerFighter>().ControllerMain;
        RW.LOTWEffect = LOTWEffect;
        Aim.GetComponent<TargetCursor>().LeftWeapon = LeftWeapon;
        Aim.GetComponent<TargetCursor>().RightWeapon = RightWeapon;
        LeftWeapon.GetComponent<Weapons>().OverHeatImage = LeftOverheatImage;
        RightWeapon.GetComponent<Weapons>().OverHeatImage = RightOverheatImage;
        // Set left right weapon to overheat bar
        LeftOverheatBar.GetComponent<OverheatBar>().Weapon = LeftWeapon.GetComponent<Weapons>();
        RightOverheatBar.GetComponent<OverheatBar>().Weapon = RightWeapon.GetComponent<Weapons>();
        Vector2 ClonePosLeft = new Vector2();
        float LeftClonseScale = 1;
        float RightClonseScale = 1;
        Vector2 ClonePosRight = new Vector2();
        if (CurrentLeftWeapon.name.ToLower().Contains("blastcannon"))
        {
            ClonePosLeft = new Vector2(LeftOverheatImage.transform.position.x, LeftOverheatImage.transform.position.y - 10);
            LeftClonseScale = 16f;
        } else
        {
            ClonePosLeft = LeftOverheatImage.transform.position;
            LeftClonseScale = 20f;
        }
        if (CurrentRightWeapon.name.ToLower().Contains("blastcannon"))
        {
            ClonePosRight = new Vector2(RightOverheatImage.transform.position.x, RightOverheatImage.transform.position.y - 10);
            RightClonseScale = 16f;
        }
        else
        {
            ClonePosRight = RightOverheatImage.transform.position;
            RightClonseScale = 20f;
        }
        GameObject LeftWeaponIcon = Instantiate(CurrentLeftWeapon, ClonePosLeft, Quaternion.identity);
        GameObject RightWeaponIcon = Instantiate(CurrentRightWeapon, ClonePosRight, Quaternion.identity);
        LeftWeaponIcon.SetActive(true);
        LeftWeaponIcon.GetComponent<SpriteRenderer>().sprite = LeftWeaponIcon.GetComponent<Weapons>().IconSprite;
        LeftWeaponIcon.GetComponent<Weapons>().enabled = false;
        LeftWeaponIcon.transform.localScale =
            new Vector3(LeftWeaponIcon.transform.localScale.x * LeftClonseScale/4,
            LeftWeaponIcon.transform.localScale.y * LeftClonseScale/4,
            LeftWeaponIcon.transform.localScale.z);
        LeftWeaponIcon.GetComponent<SpriteRenderer>().sortingOrder = 201;
        Color lc = LeftWeaponIcon.GetComponent<SpriteRenderer>().color;
        lc.a = 200 / 255f;
        LeftWeaponIcon.GetComponent<SpriteRenderer>().color = lc;
        LeftWeaponIcon.transform.SetParent(LeftOverheatImage.transform);
        LeftOverheatImage.GetComponent<HUDCreateInfoBoard>().Text.Add(FindObjectOfType<AccessDatabase>().GetItemRealName(CurrentLeftWeapon.name, "Weapon"));
        LeftOverheatImage.GetComponent<HUDCreateInfoBoard>().TopBottomLeftRight.Add("Bottom");
        LeftOverheatImage.GetComponent<HUDShowRange>().Range = CurrentLeftWeapon.GetComponent<Weapons>().
            Bullet.GetComponent<BulletShared>().MaximumDistance > 0 ?
            CurrentLeftWeapon.GetComponent<Weapons>().
            Bullet.GetComponent<BulletShared>().MaximumDistance :
            CurrentLeftWeapon.GetComponent<Weapons>().
            Bullet.GetComponent<BulletShared>().MaxEffectiveDistance;
        RightWeaponIcon.SetActive(true);
        RightWeaponIcon.GetComponent<SpriteRenderer>().sprite = RightWeaponIcon.GetComponent<Weapons>().IconSprite;
        RightWeaponIcon.GetComponent<Weapons>().enabled = false;
        RightWeaponIcon.transform.localScale =
            new Vector3(RightWeaponIcon.transform.localScale.x * RightClonseScale /4,
            RightWeaponIcon.transform.localScale.y * RightClonseScale/4,
            RightWeaponIcon.transform.localScale.z);
        RightWeaponIcon.GetComponent<SpriteRenderer>().sortingOrder = 201;
        RightWeaponIcon.transform.SetParent(RightOverheatImage.transform);
        RightOverheatImage.GetComponent<HUDCreateInfoBoard>().Text.Add(FindObjectOfType<AccessDatabase>().GetItemRealName(CurrentRightWeapon.name, "Weapon"));
        RightOverheatImage.GetComponent<HUDCreateInfoBoard>().TopBottomLeftRight.Add("Bottom");
        Color rc = RightWeaponIcon.GetComponent<SpriteRenderer>().color;
        rc.a = 200 / 255f;
        RightWeaponIcon.GetComponent<SpriteRenderer>().color = rc;
        // Set Reload Bar
        LeftWeapon.GetComponent<Weapons>().ReloadBar = LeftReloadBar;
        RightWeapon.GetComponent<Weapons>().ReloadBar = RightReloadBar;
        RightOverheatImage.GetComponent<HUDShowRange>().Range = CurrentRightWeapon.GetComponent<Weapons>().
            Bullet.GetComponent<BulletShared>().MaximumDistance > 0 ?
            CurrentRightWeapon.GetComponent<Weapons>().
            Bullet.GetComponent<BulletShared>().MaximumDistance :
            CurrentRightWeapon.GetComponent<Weapons>().
            Bullet.GetComponent<BulletShared>().MaxEffectiveDistance;


        // set 1st power 
        if (DatabaseFirstPower != null && DatabaseFirstPower !="")
        {
            for (int i = 0; i < PowerModel.transform.childCount; i++)
            {
                if (PowerModel.transform.GetChild(i).name.Equals(DatabaseFirstPower))
                {
                    FirstPower = Instantiate(PowerModel.transform.GetChild(i).gameObject, FirstPowerImage.transform.GetChild(0).position, Quaternion.identity);
                    FirstPower.SetActive(true);
                    FirstPower.transform.SetParent(PlayerFighter.transform);
                    FirstPowerImage.GetComponent<HUDCreateInfoBoard>().Text[0] = FindObjectOfType<AccessDatabase>().GetItemRealName(DatabaseFirstPower, "Power");
                    FirstPowerImage.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = FirstPower.GetComponent<SpriteRenderer>().sprite;
                    FirstPowerImage.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 201;
                    FirstPowerImage.transform.GetChild(0).gameObject.SetActive(true);
                    FirstPower.GetComponent<SpriteRenderer>().enabled = false;
                    PlayerFighter.GetComponent<PlayerFighter>().FirstPower = FirstPower;
                    FirstPower.GetComponent<Powers>().EnemyLayer = EnemyLayer;
                    FirstPower.GetComponent<Powers>().LOTWEffect = LOTWEffect;
                    FirstPower.GetComponent<Powers>().InitData(FirstPower.name);
                    break;
                }
            }
        } else
        {
            Destroy(FirstPowerImage.GetComponent<HUDCreateInfoBoard>());
            FirstPowerImage.transform.GetChild(1).GetChild(0).GetComponent<Image>().fillAmount = 1;
        }           
        //set 2nd power
        if (DatabaseSecondPower != null && DatabaseSecondPower != "")
        {
            for (int i = 0; i < PowerModel.transform.childCount; i++)
            {
                if (PowerModel.transform.GetChild(i).name.Equals(DatabaseSecondPower))
                {
                    SecondPower = Instantiate(PowerModel.transform.GetChild(i).gameObject, SecondPowerImage.transform.GetChild(0).position, Quaternion.identity);
                    SecondPower.SetActive(true);
                    SecondPower.transform.SetParent(PlayerFighter.transform);
                    SecondPowerImage.GetComponent<HUDCreateInfoBoard>().Text[0] = FindObjectOfType<AccessDatabase>().GetItemRealName(DatabaseSecondPower, "Power");
                    SecondPowerImage.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = SecondPower.GetComponent<SpriteRenderer>().sprite;
                    SecondPowerImage.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 201;
                    SecondPowerImage.transform.GetChild(0).gameObject.SetActive(true);
                    SecondPower.GetComponent<SpriteRenderer>().enabled = false;
                    PlayerFighter.GetComponent<PlayerFighter>().SecondPower = SecondPower;
                    SecondPower.GetComponent<Powers>().EnemyLayer = EnemyLayer;
                    SecondPower.GetComponent<Powers>().LOTWEffect = LOTWEffect;
                    SecondPower.GetComponent<Powers>().InitData(SecondPower.name);
                    break;

                }
            }
        }
        else
        {
            Destroy(SecondPowerImage.GetComponent<HUDCreateInfoBoard>());
            SecondPowerImage.transform.GetChild(1).GetChild(0).GetComponent<Image>().fillAmount = 1;
        }
        //set consumable
        int checkCount = 0;
        if (DatabaseConsumables.Count > 0)
        {
            int count = 0;
            List<GameObject> ConsumableName = new List<GameObject>();

            foreach (var itemKey in DatabaseConsumables)
            {
                checkCount++;
                for (int i = 0; i < ConsumableModel.transform.childCount; i++)
                {                  
                    if (itemKey.Key == (ConsumableModel.transform.GetChild(i).name))
                    {
                        GameObject cons = Instantiate(ConsumableModel.transform.GetChild(i).gameObject, ConsumableImages[count].transform.GetChild(0).position, Quaternion.identity);
                        cons.SetActive(true);
                        cons.transform.SetParent(PlayerFighter.transform);
                        ConsumableImages[count].GetComponent<HUDCreateInfoBoard>().Text[0] = FindObjectOfType<AccessDatabase>().GetItemRealName(ConsumableModel.transform.GetChild(i).name, "Consumable");
                        cons.transform.localScale = new Vector2(ConsumableImages[count].transform.GetChild(0).localScale.x, ConsumableImages[count].transform.GetChild(0).localScale.y);
                        cons.GetComponent<SpriteRenderer>().sortingOrder = 201;
                        cons.GetComponent<Consumable>().InitData(ConsumableModel.transform.GetChild(i).name);
                        ConsumableName.Add(cons);
                        count++;
                    }
                }
            }

            PlayerFighter.GetComponent<PlayerFighter>().Consumables = DatabaseConsumables;
            PlayerFighter.GetComponent<PlayerFighter>().ConsumableObject = ConsumableName;
        }
        for (int i = checkCount; i < 4; i++)
        {
            ConsumableImages[i].GetComponent<HUDCreateInfoBoard>().Text[0] = "";
            ConsumableImages[i].transform.GetChild(1).GetChild(0).GetComponent<Image>().fillAmount = 1;
        }


        // Set all stats data 
        SetStatsData();
    }

    private void SetStatsData()
    {
        // Model stats
        string ModelStats = FindObjectOfType<AccessDatabase>().GetFighterStatsByName(DatabaseModel);
        if (ModelStats!="Fail")
        {
            //HP-n|SPD-n|ROT-n|AOF-n,n|DM-n|AM-n|PM-n
            Dictionary<string, object> statsDict = FindObjectOfType<GlobalFunctionController>().ConvertModelStatsToDictionaryForGameplay(ModelStats);
            PlayerFighter.GetComponent<PlayerFighter>().MaxHP = int.Parse((string)statsDict["HP"]) * LOTWEffect.LOTWMaxHPScale;
            PlayerFighter.GetComponent<PlayerFighter>().CurrentHP = (int)CurrentHP;
            PlayerFighter.GetComponent<FighterShared>().LOTWEffect = LOTWEffect;
            PlayerFighter.GetComponent<PlayerMovement>().MovingSpeed = int.Parse((string)statsDict["SPD"]) * LOTWEffect.LOTWMoveSpeedScale;
            PlayerFighter.GetComponent<PlayerMovement>().RotateSpeed = float.Parse((string)statsDict["ROT"]);
            PlayerFighter.GetComponent<PlayerMovement>().LOTWEffect = LOTWEffect;

            LeftWeaponGO.GetComponent<Weapons>().RotateLimitNegative = int.Parse((string)statsDict["AOFNegative"]);
            LeftWeaponGO.GetComponent<Weapons>().RotateLimitPositive = int.Parse((string)statsDict["AOFPositive"]);
            LeftWeaponGO.GetComponent<Weapons>().RateOfFire *= LOTWEffect.LOTWWeaponROFScale;
            RightWeaponGO.GetComponent<Weapons>().RotateLimitNegative = int.Parse((string)statsDict["AOFNegative"]);
            RightWeaponGO.GetComponent<Weapons>().RotateLimitPositive = int.Parse((string)statsDict["AOFPositive"]);
            RightWeaponGO.GetComponent<Weapons>().RateOfFire *= LOTWEffect.LOTWWeaponROFScale;

            if (int.Parse((string)statsDict["AOFPositive"]) < 180)
            {
                PlayerFighter.GetComponent<PlayerFighter>().LeftRange.transform.RotateAround(PlayerFighter.transform.position, Vector3.back, int.Parse((string)statsDict["AOFPositive"]));
                PlayerFighter.GetComponent<PlayerFighter>().RightRange.transform.RotateAround(PlayerFighter.transform.position, Vector3.back, int.Parse((string)statsDict["AOFNegative"]));
            } else
            {
                PlayerFighter.GetComponent<PlayerFighter>().LeftRange.transform.localScale = new Vector3(0, 0, 0);
                PlayerFighter.GetComponent<PlayerFighter>().RightRange.transform.localScale = new Vector3(0, 0, 0);
            }

            LeftWeaponGO.GetComponent<Weapons>().FighterWeaponDamageMod = float.Parse((string)statsDict["DM"]);
            if (!LeftWeaponGO.GetComponent<Weapons>().IsThermalType)
            {
                LeftWeaponGO.GetComponent<Weapons>().FighterWeaponAoEMod = float.Parse((string)statsDict["AM"]);
            } else
            {
                LeftWeaponGO.GetComponent<Weapons>().FighterWeaponAoEMod = 1;
            }
            RightWeaponGO.GetComponent<Weapons>().FighterWeaponDamageMod = float.Parse((string)statsDict["DM"]);
            if (!RightWeaponGO.GetComponent<Weapons>().IsThermalType)
            {
                RightWeaponGO.GetComponent<Weapons>().FighterWeaponAoEMod = float.Parse((string)statsDict["AM"]);
            }
            else
            {
                RightWeaponGO.GetComponent<Weapons>().FighterWeaponAoEMod = 1;
            }
        }
        PlayerFighter.SetActive(true);
    }
    #endregion
}
