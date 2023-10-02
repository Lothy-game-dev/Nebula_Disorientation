using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShared : FighterShared
{
    #region Shared Variables
    private FighterMovement fm;
    public EnemyHealthBar HealthBar;
    public float ScaleOnStatusBoard;
    public GameObject EnemyStatus;
    private StatusBoard Status;
    public GameObject Weapons;
    public Rigidbody2D rb;
    public float changeDirTimer;
    private bool test;
    private Dictionary<string, object> StatsDataDict;
    private Vector2 check;
    private GameObject CurrentLeftWeapon;
    private GameObject CurrentRightWeapon;
    private bool doneInitWeapon;
    private string weaponName;
    private string Power;
    private float resetMovetimer;
    private int RandomMove;
    private int RandomRotate;
    #endregion
    #region Shared Functions
    // Set Health to Health Bar
    public void SetHealth()
    {
        HealthBar.SetValue(CurrentHP, MaxHP);
    }

    public void UpdateEnemy()
    {
        SetHealth();
        UpdateFighter();
        if (CurrentHP <= 0f)
        {
            Status.StopShowing();
        }
    }

    private void OnMouseOver()
    {
        Status.Timer = 5f;
        Status.StartShowing(gameObject);
    }

    private void OnMouseExit()
    {
        Status.CheckOnDestroy();
    }
    #endregion
    #region Start & Update
    private void Start()
    {
        Status = EnemyStatus.GetComponent<StatusBoard>();
        InitializeFighter();
    }

    private void Update()
    {
        UpdateEnemy();
        if (doneInitWeapon)
        {
            if (LeftWeapon!=null)
            {
                LeftWeapon.GetComponent<Weapons>().AIShootBullet();
            }
            if (RightWeapon!=null)
            {
                RightWeapon.GetComponent<Weapons>().AIShootBullet();
            }
        }
        resetMovetimer -= Time.deltaTime;
        if (resetMovetimer<=0f)
        {
            RandomMove = Random.Range(1, 3);
            RandomRotate = Random.Range(1, 3);
            resetMovetimer = 5f;
        }
        
        if (RandomMove == 1)
        {
            fm.UpMove();
        } 
        else if (RandomMove == 2)
        {
            fm.DownMove();
        } 
        else if (RandomMove == 3)
        {
            fm.NoUpDownMove();
        } 
        if (RandomRotate == 1)
        {
            fm.LeftMove();
        }
        else if (RandomRotate == 2)
        {
            fm.RightMove();
        }
        else if (RandomRotate == 3)
        {
            fm.NoLeftRightMove();
        }
    }
    #endregion
    #region Init Data
    public void InitData(Dictionary<string, object> Data, GameObject Model)
    {
        StatsDataDict = new Dictionary<string, object>();
        fm = GetComponent<FighterMovement>();
        FighterName = (string)Data["Name"];
        StatsDataDict = FindObjectOfType<GlobalFunctionController>().ConvertEnemyStatsToDictionary((string)Data["Stats"]);
        MaxHP = float.Parse((string)StatsDataDict["HP"]);
        // SPD, ROT
        fm.MovingSpeed = float.Parse((string)StatsDataDict["SPD"]);
        fm.RotateSpeed = float.Parse((string)StatsDataDict["ROT"]);
        doneInitWeapon = false;
        weaponName = "";
        string[] checkWeapons = ((string)Data["Weapons"]).Split("|");
        if (checkWeapons.Length > 1)
        {
            weaponName = checkWeapons[Random.Range(0, checkWeapons.Length)];
        } else
        {
            weaponName = checkWeapons[0];
        }
        if (weaponName=="SuicideBombing")
        {

        } else
        {
            FighterAttachedWeapon faw = gameObject.AddComponent<FighterAttachedWeapon>();
            faw.Fighter = gameObject;
            faw.FighterModel = Model;
            FighterModelShared fms = gameObject.AddComponent<FighterModelShared>();
            fms.LeftWeaponPos = Model.GetComponent<FighterModelShared>().LeftWeaponPos;
            fms.RightWeaponPos = Model.GetComponent<FighterModelShared>().RightWeaponPos;
            bool alreadyLeft = false;
            bool alreadyRight = false;
            for (int i = 0; i < Weapons.transform.childCount; i++)
            {
                if (alreadyLeft && alreadyRight)
                {
                    break;
                }
                if (!alreadyLeft && Weapons.transform.GetChild(i).name.Replace(" ", "").ToLower().Equals(weaponName.Replace(" ","").ToLower()))
                {
                    alreadyLeft = true;
                    CurrentLeftWeapon = Weapons.transform.GetChild(i).gameObject;
                }
                if (!alreadyRight && Weapons.transform.GetChild(i).name.Replace(" ", "").ToLower().Equals(weaponName.Replace(" ", "").ToLower()))
                {
                    alreadyRight = true;
                    CurrentRightWeapon = Weapons.transform.GetChild(i).gameObject;
                }
            }
            // Left Weapon
            LeftWeapon = Instantiate(CurrentLeftWeapon, transform.position, Quaternion.identity);
            LeftWeapon.transform.localScale = new Vector2(CurrentLeftWeapon.transform.localScale.x * 2f, CurrentLeftWeapon.transform.localScale.y * 2f);
            LeftWeapon.SetActive(true);
            Weapons LW = LeftWeapon.GetComponent<Weapons>();
            LW.isLeftWeapon = true;
            LW.Fighter = gameObject;
            LW.Aim = FindObjectOfType<GameController>().Player;
            LW.WeaponPosition = null;
            LW.tracking = true;
            LW.EnemyLayer = FindObjectOfType<GameController>().PlayerLayer;
            LW.RotateLimitNegative = float.Parse((string)StatsDataDict["AOFNegative"]);
            LW.RotateLimitPositive = float.Parse((string)StatsDataDict["AOFPositive"]);
            faw.LeftWeapon = LeftWeapon;

            // Right Weapon
            RightWeapon = Instantiate(CurrentRightWeapon, transform.position, Quaternion.identity);
            RightWeapon.transform.localScale = new Vector2(CurrentRightWeapon.transform.localScale.x * 2f, CurrentRightWeapon.transform.localScale.y * 2f);
            RightWeapon.SetActive(true);
            Weapons RW = RightWeapon.GetComponent<Weapons>();
            RW.Fighter = gameObject;
            RW.Aim = FindObjectOfType<GameController>().Player;
            RW.WeaponPosition = null;
            RW.tracking = true;
            RW.EnemyLayer = FindObjectOfType<GameController>().PlayerLayer;
            RW.RotateLimitNegative = float.Parse((string)StatsDataDict["AOFNegative"]);
            RW.RotateLimitPositive = float.Parse((string)StatsDataDict["AOFPositive"]);
            faw.RightWeapon = RightWeapon;


            faw.AttachWeapon();
        }
        
        gameObject.SetActive(true);
        StartCoroutine(WaitForDoneInit());
    }

    private IEnumerator WaitForDoneInit()
    {
        yield return new WaitForSeconds(5f);
        doneInitWeapon = true;
    }
    #endregion
}
