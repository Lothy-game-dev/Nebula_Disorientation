using System.Collections;
using System.Collections.Generic;
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
    public LayerMask EnemyLayer;
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
    public GameObject CurrentFirstPower;
    public GameObject CurrentSecondPower;
    private Dictionary<string, object> dataDict;
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
            for (int i=1;i<=ListCons.Length;i++)
            {
                DatabaseConsumables.Add(ListCons[i].Split("-")[0], int.Parse(ListCons[i].Split("-")[1]));
            }
        }
    }
    private void InitData()
    {
        PlayerFighter.SetActive(false);
        // Get Data from DB
        SetDataFromDB();
        // Get Model from model list
        for (int i=0;i<FighterModel.transform.childCount;i++)
        {
            if (FighterModel.transform.GetChild(i).name.Equals(DatabaseModel))
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
        RightWeapon.SetActive(true);
        Weapons RW = RightWeapon.GetComponent<Weapons>();
        RW.Fighter = PlayerFighter;
        RW.Aim = Aim;
        RW.WeaponPosition = RightWeaponPosition;
        RW.tracking = true;
        RW.EnemyLayer = EnemyLayer;
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
        LeftWeaponIcon.GetComponent<Weapons>().enabled = false;
        LeftWeaponIcon.transform.localScale =
            new Vector3(LeftWeaponIcon.transform.localScale.x * LeftClonseScale,
            LeftWeaponIcon.transform.localScale.y * LeftClonseScale,
            LeftWeaponIcon.transform.localScale.z);
        LeftWeaponIcon.GetComponent<SpriteRenderer>().sortingOrder = 201;
        Color lc = LeftWeaponIcon.GetComponent<SpriteRenderer>().color;
        lc.a = 200 / 255f;
        LeftWeaponIcon.GetComponent<SpriteRenderer>().color = lc;
        LeftWeaponIcon.transform.SetParent(LeftOverheatImage.transform);
        LeftOverheatImage.GetComponent<HUDCreateInfoBoard>().Text.Add(CurrentLeftWeapon.name);
        LeftOverheatImage.GetComponent<HUDCreateInfoBoard>().TopBottomLeftRight.Add("Bottom");
        LeftOverheatImage.GetComponent<HUDShowRange>().Range = CurrentLeftWeapon.GetComponent<Weapons>().
            Bullet.GetComponent<BulletShared>().MaximumDistance > 0 ?
            CurrentLeftWeapon.GetComponent<Weapons>().
            Bullet.GetComponent<BulletShared>().MaximumDistance :
            CurrentLeftWeapon.GetComponent<Weapons>().
            Bullet.GetComponent<BulletShared>().MaxEffectiveDistance;
        RightWeaponIcon.SetActive(true);
        RightWeaponIcon.GetComponent<Weapons>().enabled = false;
        RightWeaponIcon.transform.localScale =
            new Vector3(RightWeaponIcon.transform.localScale.x * RightClonseScale,
            RightWeaponIcon.transform.localScale.y * RightClonseScale,
            RightWeaponIcon.transform.localScale.z);
        RightWeaponIcon.GetComponent<SpriteRenderer>().sortingOrder = 201;
        RightWeaponIcon.transform.SetParent(RightOverheatImage.transform);
        RightOverheatImage.GetComponent<HUDCreateInfoBoard>().Text.Add(CurrentRightWeapon.name);
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


        // set power 
        if (CurrentFirstPower!=null)
        {
            GameObject FirstPower = Instantiate(CurrentFirstPower, FirstPowerImage.transform.GetChild(0).position, Quaternion.identity);
            FirstPower.SetActive(true);
            FirstPower.transform.SetParent(FirstPowerImage.transform);
            FirstPower.transform.localScale = new Vector2(FirstPowerImage.transform.GetChild(0).localScale.x, FirstPowerImage.transform.GetChild(0).localScale.y);
            FirstPower.GetComponent<SpriteRenderer>().sortingOrder = 201;
            PlayerFighter.GetComponent<PlayerFighter>().FirstPower = FirstPower;
            FirstPower.GetComponent<Powers>().InitData(FirstPower.name);
        }
        if (CurrentSecondPower!=null)
        {
            GameObject SecondPower = Instantiate(CurrentSecondPower, SecondPowerImage.transform.GetChild(0).position, Quaternion.identity);
            SecondPower.SetActive(true);
            SecondPower.transform.SetParent(SecondPowerImage.transform);
            SecondPower.transform.localScale = new Vector2(SecondPowerImage.transform.GetChild(0).localScale.x, SecondPowerImage.transform.GetChild(0).localScale.y);
            SecondPower.GetComponent<SpriteRenderer>().sortingOrder = 201;
            PlayerFighter.GetComponent<PlayerFighter>().SecondPower = SecondPower;
            SecondPower.GetComponent<Powers>().InitData(SecondPower.name);
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
            PlayerFighter.GetComponent<PlayerFighter>().MaxHP = int.Parse((string)statsDict["HP"]);
            PlayerFighter.GetComponent<PlayerMovement>().MovingSpeed = int.Parse((string)statsDict["SPD"]);
            PlayerFighter.GetComponent<PlayerMovement>().RotateSpeed = float.Parse((string)statsDict["ROT"]);
            LeftWeaponGO.GetComponent<Weapons>().RotateLimitNegative = int.Parse((string)statsDict["AOFNegative"]);
            LeftWeaponGO.GetComponent<Weapons>().RotateLimitPositive = int.Parse((string)statsDict["AOFPositive"]);
            RightWeaponGO.GetComponent<Weapons>().RotateLimitNegative = int.Parse((string)statsDict["AOFNegative"]);
            RightWeaponGO.GetComponent<Weapons>().RotateLimitPositive = int.Parse((string)statsDict["AOFPositive"]);
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
