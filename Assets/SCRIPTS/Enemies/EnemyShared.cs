using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using UnityEngine;

public class EnemyShared : FighterShared
{
    #region Shared Variables
    private FighterMovement fm;
    public EnemyHealthBar HealthBar;
    public EnemyHealthBar ShieldBar;
    public float ScaleOnStatusBoard;
    public GameObject EnemyStatus;
    private StatusBoard Status;
    public GameObject Weapons;
    public Rigidbody2D rb;
    public float changeDirTimer;
    public GameObject BackFire;

    public GameObject PowerModel;
    public int Tier;
    private bool test;
    private Dictionary<string, object> StatsDataDict;
    private Vector2 check;
    private GameObject CurrentLeftWeapon;
    private GameObject CurrentRightWeapon;
    private GameObject CurrentFirstPower;
    private GameObject CurrentSecondPower;
    private bool doneInitWeapon;
    public string weaponName1;
    public string weaponName2;
    public string Power1;
    public string Power2;
    public float Power1CD;
    private float Power1Charging;
    private bool Power1Activation;
    private bool Power1StartCharge;

    public float Power2CD;
    private float Power2Charging;    
    private bool Power2Activation;
    private bool Power2StartCharge;
    private float resetMovetimer;
    private int RandomMove;
    private int RandomRotate;
    public GameObject LeftTarget;
    public GameObject RightTarget;
    public float TargetRange;
    public float DelayBetween2Weap;
    public float DelayTimer;
    public bool LeftFire;
    private int BountyCash;
    private int BountyShard;
    public float TargetRefreshTimer;
    public float FindTargetTimer;
    public float HPScale;
    public float CashBountyScale;
    public GameObject ForceTargetGO;
    public bool Escort;
    public bool isBomb;
    public string Priority;
    public GameObject AimObject;
    private float BombUpdateTimer;
    public GameObject GravTarget;
    public int EnemyID;
    public GameObject NearestTarget;
    #endregion
    #region Shared Functions
    // Set Health to Health Bar
    public void SetHealth()
    {
        HealthBar.SetValue(CurrentHP, MaxHP);
        ShieldBar.SetValue(CurrentBarrier, MaxBarrier);
    }

    public void UpdateEnemy()
    {
        SetHealth();
        UpdateFighter();
        if (CurrentHP <= 0f)
        {
            Status.StopShowing(gameObject);
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
        ShieldPassive();
    }

    private void Update()
    {
        UpdateEnemy();
        if (isBomb)
        {
            if (doneInitWeapon)
            {
                doneInitWeapon = false;
                CheckTargetEnemy();
            }
            BombUpdateTimer -= Time.deltaTime;
            if (BombUpdateTimer<=0f && CurrentHP > 0)
            {
                BombUpdateTimer = Random.Range(0.1f, 0.2f);
                CheckWSSS();
            }
        } else
        {
            if (doneInitWeapon)
            {
                DelayTimer -= Time.deltaTime;
                /*if (LeftWeapon != null)
                {
                    if (DelayTimer <= 0f && !LeftFire)
                    {
                        LeftFire = true;
                        LeftWeapon.GetComponent<Weapons>().AIShootBullet(Random.Range(-1, 1) * 30);
                        DelayTimer = DelayBetween2Weap;
                    }
                }
                if (RightWeapon != null)
                {
                    if (DelayTimer <= 0f && LeftFire)
                    {
                        LeftFire = false;
                        RightWeapon.GetComponent<Weapons>().AIShootBullet(Random.Range(-1, 1) * 30);
                        DelayTimer = DelayBetween2Weap;
                    }
                }*/
                if (Power1 != "")
                {
                    if (Power1CD <= 0f && CurrentBarrier < MaxBarrier && CurrentBarrier > 0)
                    {
                        UseFirstPower();
                        if (Power1StartCharge)
                        {
                            CheckPower1Charging();
                        }
                    }
                    else
                    {
                        Power1CD -= Time.deltaTime * Random.Range(0.8f, 1.2f);
                    }
                }
                if (Power2 != "")
                {
                    if (Power2CD <= 0f && (LeftTarget != null || RightTarget != null))
                    {
                        UseSecondPower();
                        if (Power2StartCharge)
                        {
                            CheckPower2Charging();
                        }
                    }
                    else
                    {
                        Power2CD -= Time.deltaTime * Random.Range(0.8f, 1.2f);
                    }
                }
            }

            TargetRefreshTimer -= Time.deltaTime;
            if (TargetRefreshTimer <= 0f)
            {
                TargetRefreshTimer = Random.Range(2.5f, 3.5f);
                CheckTargetEnemy();
                if (LeftWeapon != null)
                {
                    LeftWeapon.GetComponent<Weapons>().Aim = LeftTarget;
                }
                if (RightWeapon != null)
                {
                    RightWeapon.GetComponent<Weapons>().Aim = RightTarget;
                }
            }
            if (LeftTarget == null || RightTarget == null)
            {
                FindTargetTimer -= Time.deltaTime;
            }
            if (FindTargetTimer <= 0f)
            {
                FindTargetTimer = Random.Range(2.5f, 3.5f);
                if (LeftTarget == null)
                {
                    TargetLeftEnemy();
                }
                if (RightTarget == null)
                {
                    TargetRightEnemy();
                }
            }
        }
    }
    private void FixedUpdate()
    {

    }
    #endregion
    #region Init Data
    public void InitData(Dictionary<string, object> Data, GameObject Model)
    {
        StatsDataDict = new Dictionary<string, object>();
        fm = GetComponent<FighterMovement>();
        FighterName = (string)Data["Name"];
        // Bounty
        BountyCash = (int)(int.Parse(((string)Data["DefeatReward"]).Split("|")[0]) * CashBountyScale);
        BountyShard = int.Parse(((string)Data["DefeatReward"]).Split("|")[1]);
        StatsDataDict = FindObjectOfType<GlobalFunctionController>().ConvertEnemyStatsToDictionary((string)Data["Stats"]);
        MaxHP = float.Parse((string)StatsDataDict["HP"]) * HPScale;
        // SPD, ROT
        fm.MovingSpeed = float.Parse((string)StatsDataDict["SPD"]);
        fm.RotateSpeed = float.Parse((string)StatsDataDict["ROT"]);
        doneInitWeapon = false;
        BackFire.transform.position = new Vector3(transform.position.x + Model.GetComponent<FighterModelShared>().BackfirePos.x * 30,
            transform.position.y + (-0.4f + Model.GetComponent<FighterModelShared>().BackfirePos.y) * 30,
            BackFire.transform.position.z);
        string[] checkPowers = ((string)Data["Power"]).Split("|");
        if (checkPowers.Length > 1)
        {
            Power1 = checkPowers[0];
            Power2 = checkPowers[1];
        } else if (checkPowers.Length == 1)
        {
            Power1 = checkPowers[0];
            Power2 = "";
        } else
        {
            Power1 = "";
            Power2 = "";
        }
        string[] checkWeapons = ((string)Data["Weapons"]).Split("|");
        if (checkWeapons.Length > 1)
        {
            weaponName1 = checkWeapons[0];
            weaponName2 = checkWeapons[1];
        } else
        {
            weaponName1 = checkWeapons[0];
            weaponName2 = checkWeapons[0];
        }
        if (weaponName1=="SuicideBombing")
        {
            isBomb = true;
            Vector3 HealthPos = fm.HealthBarSlider.transform.position - transform.position;
            Vector3 ShieldPos = fm.ShieldBarSlider.transform.position - transform.position;
            transform.Rotate(new Vector3(0, 0, -270));
            OnFireGO.transform.Rotate(new Vector3(0, 0, 270));
            OnFreezeGO.transform.Rotate(new Vector3(0, 0, 270));
            fm.HealthBarSlider.transform.Rotate(new Vector3(0, 0, 270));
            fm.ShieldBarSlider.transform.Rotate(new Vector3(0, 0, 270));
            fm.CurrentRotateAngle = 270;
            fm.HealthBarSlider.transform.position = transform.position + HealthPos;
            fm.ShieldBarSlider.transform.position = transform.position + ShieldPos;
            HealthBar.Position = HealthPos;
            ShieldBar.Position = ShieldPos;
            BackFire.transform.localPosition *= 2;
            ScaleOnStatusBoard /= 2;
            GetComponent<Rigidbody2D>().mass /= 4;
            Destroy(GetComponent<DecisionRequester>());
            Destroy(GetComponent<EnemyFighterMLAgent>());
            Destroy(GetComponent<BehaviorParameters>());
        } else
        {
            isBomb = false;
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
                if (!alreadyLeft && Weapons.transform.GetChild(i).name.Replace(" ", "").ToLower().Equals(weaponName1.Replace(" ","").ToLower()))
                {
                    alreadyLeft = true;
                    CurrentLeftWeapon = Weapons.transform.GetChild(i).gameObject;
                }
                if (!alreadyRight && Weapons.transform.GetChild(i).name.Replace(" ", "").ToLower().Equals(weaponName2.Replace(" ", "").ToLower()))
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
            LW.Aim = LeftTarget;
            LW.WeaponPosition = null;
            LW.tracking = true;
            LW.EnemyLayer = FindObjectOfType<GameController>().PlayerLayer;
            LW.RotateLimitNegative = float.Parse((string)StatsDataDict["AOFNegative"]);
            LW.RotateLimitPositive = float.Parse((string)StatsDataDict["AOFPositive"]);
            LW.FighterWeaponDamageMod = float.Parse((string)StatsDataDict["DM"]);
            LW.ChangeToZatSprite();
            if (LW.IsThermalType)
            {
                LW.FighterWeaponAoEMod = 0;
            }
            else
                LW.FighterWeaponAoEMod = float.Parse((string)StatsDataDict["AM"]);
            faw.LeftWeapon = LeftWeapon;

            // Right Weapon
            RightWeapon = Instantiate(CurrentRightWeapon, transform.position, Quaternion.identity);
            RightWeapon.transform.localScale = new Vector2(CurrentRightWeapon.transform.localScale.x * 2f, CurrentRightWeapon.transform.localScale.y * 2f);
            RightWeapon.SetActive(true);
            Weapons RW = RightWeapon.GetComponent<Weapons>();
            RW.Fighter = gameObject;
            RW.Aim = RightTarget;
            RW.WeaponPosition = null;
            RW.tracking = true;
            RW.EnemyLayer = FindObjectOfType<GameController>().PlayerLayer;
            RW.RotateLimitNegative = float.Parse((string)StatsDataDict["AOFNegative"]);
            RW.RotateLimitPositive = float.Parse((string)StatsDataDict["AOFPositive"]);
            RW.FighterWeaponDamageMod = float.Parse((string)StatsDataDict["DM"]);
            RW.ChangeToZatSprite();
            if (RW.IsThermalType)
            {
                RW.FighterWeaponAoEMod = 0;
            }
            else
                RW.FighterWeaponAoEMod = float.Parse((string)StatsDataDict["AM"]);
            faw.RightWeapon = RightWeapon;

            faw.AttachWeapon();
            Vector3 HealthPos = fm.HealthBarSlider.transform.position - transform.position;
            Vector3 ShieldPos = fm.ShieldBarSlider.transform.position - transform.position;
            if (!Escort)
            {
                transform.Rotate(new Vector3(0, 0, -270));
                OnFireGO.transform.Rotate(new Vector3(0, 0, 270));
                OnFreezeGO.transform.Rotate(new Vector3(0, 0, 270));
                fm.HealthBarSlider.transform.Rotate(new Vector3(0, 0, 270));
                fm.ShieldBarSlider.transform.Rotate(new Vector3(0, 0, 270));
                fm.CurrentRotateAngle = 270;
            } else
            {
                if (transform.position.y > 0)
                {
                    int n = Random.Range(225, 255);
                    transform.Rotate(new Vector3(0, 0, -n));
                    OnFireGO.transform.Rotate(new Vector3(0, 0, n));
                    OnFreezeGO.transform.Rotate(new Vector3(0, 0, n));
                    fm.HealthBarSlider.transform.Rotate(new Vector3(0, 0, n));
                    fm.ShieldBarSlider.transform.Rotate(new Vector3(0, 0, n));
                    fm.CurrentRotateAngle = n;
                } else
                {
                    int n = Random.Range(15, 45);
                    transform.Rotate(new Vector3(0, 0, -n));
                    OnFireGO.transform.Rotate(new Vector3(0, 0, n));
                    OnFreezeGO.transform.Rotate(new Vector3(0, 0, n));
                    fm.HealthBarSlider.transform.Rotate(new Vector3(0, 0, n));
                    fm.ShieldBarSlider.transform.Rotate(new Vector3(0, 0, n));
                    fm.CurrentRotateAngle = n;
                }
            }
            fm.HealthBarSlider.transform.position = transform.position + HealthPos;
            fm.ShieldBarSlider.transform.position = transform.position + ShieldPos;
            HealthBar.Position = HealthPos;
            ShieldBar.Position = ShieldPos;
            
            // Delay Weapon Fire Check Case
            if (weaponName1 == weaponName2 && !LW.IsThermalType)
            {
                DelayBetween2Weap = (1 / LW.RateOfFire) / 2;
            }
            // Set Target
            TargetRange = LW.Bullet.GetComponent<BulletShared>().MaxEffectiveDistance >= RW.Bullet.GetComponent<BulletShared>().MaxEffectiveDistance ? LW.Bullet.GetComponent<BulletShared>().MaxEffectiveDistance : LW.Bullet.GetComponent<BulletShared>().MaxEffectiveDistance;
            TargetLeftEnemy();
            TargetRightEnemy();
            // Power
            bool alreadyFirst = false;
            bool alreadySecond = false;
            for (int i = 0; i < PowerModel.transform.childCount; i++)
            {
                if (alreadyFirst && alreadySecond)
                {
                    break;
                }
                if (Power1 != "")
                {
                    if (PowerModel.transform.GetChild(i).name.Replace(" ", "").ToLower().Equals(Power1.Replace(" ", "").ToLower()))
                    {
                        alreadyFirst = true;
                        CurrentFirstPower = Instantiate(PowerModel.transform.GetChild(i).gameObject, transform.position, Quaternion.identity);
                        CurrentFirstPower.SetActive(true);
                        CurrentFirstPower.transform.SetParent(transform);
                        CurrentFirstPower.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
                        CurrentFirstPower.GetComponent<Powers>().InitData(Power1);
                    }
                } else
                {
                    alreadyFirst = true;
                }

                if (Power2 != "")
                {
                    if (PowerModel.transform.GetChild(i).name.Replace(" ", "").ToLower().Equals(Power2.Replace(" ", "").ToLower()))
                    {
                        alreadySecond = true;
                        CurrentSecondPower = Instantiate(PowerModel.transform.GetChild(i).gameObject, transform.position, Quaternion.identity);
                        CurrentSecondPower.SetActive(true);
                        CurrentSecondPower.transform.SetParent(transform);
                        CurrentSecondPower.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
                        CurrentSecondPower.GetComponent<Powers>().InitData(Power2);
                    }
                }
                else
                {
                    alreadySecond = true;
                }
            }
            
        }
        
        gameObject.SetActive(true);
        StartCoroutine(WaitForDoneInit());
    }

    public void ShieldPassive()
    {
        float br = 0;
        if (CurrentFirstPower != null)
        {
            if (CurrentFirstPower.GetComponent<Powers>().BR > 0)
            {
                br = CurrentFirstPower.GetComponent<Powers>().BR;
            }
        }
        if (CurrentSecondPower != null)
        {
            if (CurrentSecondPower.GetComponent<Powers>().BR > 0)
            {
                br = CurrentSecondPower.GetComponent<Powers>().BR;
            }
        }
        if (br == 0)
        {
            MaxBarrier = 2000;
        }
        else
        {
            MaxBarrier = float.Parse((string)StatsDataDict["HP"]) * br / 100;
        }
        CurrentBarrier = MaxBarrier;
    }
    private void UseFirstPower()
    {
        if (Power1CD <= 0f && !isFrozen && !isSFBFreeze)
        {
            CurrentFirstPower.GetComponent<Powers>().Fighter = gameObject;
            CurrentFirstPower.GetComponent<Powers>().EnemyLayer = FindObjectOfType<GameController>().PlayerLayer;
            // check if power does not need charge
            //
            if (!CurrentFirstPower.name.Contains("LaserBeam"))
            {
                if (!Power1Activation)
                {
                    Power1Activation = true;
                    // void function to activate power
                    CurrentFirstPower.GetComponent<Powers>().ActivatePower(CurrentFirstPower.name.Replace("(clone)", ""));

                    //Cooldown
                    Power1CD = CurrentFirstPower.GetComponent<Powers>().CD;
                    Power1Activation = false;
                }
            }
            // if power need charge
            //
            else
            {
                if (!Power1Activation)
                {
                    Power1StartCharge = true;
                }
            }
        }
    }

    private void CheckPower1Charging()
    {
        if (Power1Charging < 3f)
        {
            if (Power1Charging == 0)
            {
                CurrentFirstPower.GetComponent<Powers>().BeforeActivating();
            }
            Power1Charging += Time.deltaTime;
        }
        else
        {
            Power1StartCharge = false;
            Power1Activation = true;
            Power1Charging = 0;
            // Void function to activate power
            CurrentFirstPower.GetComponent<Powers>().ActivatePower(CurrentFirstPower.name.Replace("(clone)", ""));

            //Cooldown
            Power1CD = CurrentFirstPower.GetComponent<Powers>().CD;
            Power1Activation = false;
        }
    }

    private void UseSecondPower()
    {
        if (Power2CD <= 0f && !isFrozen && !isSFBFreeze)
        {
            CurrentSecondPower.GetComponent<Powers>().Fighter = gameObject;
            CurrentSecondPower.GetComponent<Powers>().EnemyLayer = FindObjectOfType<GameController>().PlayerLayer;
            // check if power does not need charge
            //
            if (!CurrentSecondPower.name.Contains("LaserBeam"))
            {
                if (!Power2Activation)
                {
                    Power2Activation = true;
                    // void function to activate power
                    CurrentSecondPower.GetComponent<Powers>().ActivatePower(CurrentSecondPower.name.Replace("(clone)", ""));

                    //Cooldown
                    Power2CD = CurrentSecondPower.GetComponent<Powers>().CD;
                    Power2Activation = false;
                }
            }
            // if power need charge
            //
            else
            {
                if (!Power2Activation)
                {
                    Power2StartCharge = true;
                }
            }
        }
    }

    private void CheckPower2Charging()
    {
        if (Power2Charging < 3f)
        {
            if (Power2Charging == 0)
            {
                CurrentSecondPower.GetComponent<Powers>().BeforeActivating();
            }
            Power2Charging += Time.deltaTime;
        }
        else
        {
            Power2StartCharge = false;
            Power2Activation = true;
            Power2Charging = 0;
            // Void function to activate power
            CurrentSecondPower.GetComponent<Powers>().ActivatePower(CurrentSecondPower.name.Replace("(clone)", ""));

            //Cooldown
            Power2CD = CurrentSecondPower.GetComponent<Powers>().CD;
            Power2Activation = false;
        }
    }
    private IEnumerator WaitForDoneInit()
    {
        yield return new WaitForSeconds(0f);
        doneInitWeapon = true;
    }

    public void TargetNearestTarget()
    {
        if (NearestTarget == null)
        {
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 15000f, FindObjectOfType<GameController>().PlayerLayer);
            if (cols.Length > 0)
            {
                GameObject Nearest = cols[0].gameObject;
                float distance = Mathf.Abs((cols[0].transform.position - transform.position).magnitude);
                foreach (var enemy in cols)
                {
                    float distanceTest = Mathf.Abs((enemy.gameObject.transform.position - transform.position).magnitude);
                    if (distanceTest < distance)
                    {
                        distance = distanceTest;
                        Nearest = enemy.gameObject;
                    }
                }
                NearestTarget = Nearest;
            }
        }
    }

    public void TargetLeftEnemy()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, TargetRange, FindObjectOfType<GameController>().PlayerLayer);
        if (cols.Length > 0)
        {
            GameObject Nearest = cols[0].gameObject;
            float distance = Mathf.Abs((cols[0].transform.position - transform.position).magnitude);
            foreach (var enemy in cols)
            {
                if (enemy == ForceTargetGO)
                {
                    Nearest = enemy.gameObject;
                    break;
                }
                float distanceTest = Mathf.Abs((enemy.gameObject.transform.position - transform.position).magnitude);
                if (Priority == null || Priority == "" || (!Priority.Contains("WS") && !Priority.Contains("SS")))
                {
                    if (weaponName1.Contains("Gravitational"))
                    {
                        if (enemy.GetComponent<WSShared>() != null || enemy.GetComponent<SpaceStationShared>() != null)
                        {
                            if (Nearest.GetComponent<WSShared>() != null || Nearest.GetComponent<SpaceStationShared>() != null)
                            {
                                if (distanceTest < distance)
                                {
                                    distance = distanceTest;
                                    Nearest = enemy.gameObject;
                                }
                            }
                            else
                            {
                                distance = distanceTest;
                                Nearest = enemy.gameObject;
                            }
                        }
                        else
                        {
                            if (Nearest.GetComponent<WSShared>() == null && Nearest.GetComponent<SpaceStationShared>() == null)
                                if (distanceTest < distance)
                                {
                                    distance = distanceTest;
                                    Nearest = enemy.gameObject;
                                }
                        }
                    }
                }
                else
                if (distanceTest < distance)
                {
                    distance = distanceTest;
                    Nearest = enemy.gameObject;
                }
            }
            LeftTarget = Nearest;
        }
    }
    
    public void InitTargetEnemy()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 15000f, FindObjectOfType<GameController>().PlayerLayer);
        if (cols.Length > 0)
        {
            GameObject Nearest = cols[0].gameObject;
            float distance = Mathf.Abs((cols[0].transform.position - transform.position).magnitude);
            foreach (var enemy in cols)
            {
                if (Priority == "WSSS")
                {
                    float distanceTest = Mathf.Abs((enemy.gameObject.transform.position - transform.position).magnitude);
                    if (enemy.GetComponent<WSShared>() != null || enemy.GetComponent<SpaceStationShared>() != null)
                    {
                        if (Nearest.GetComponent<WSShared>() != null || Nearest.GetComponent<SpaceStationShared>() != null)
                        {
                            if (distanceTest < distance)
                            {
                                distance = distanceTest;
                                Nearest = enemy.gameObject;
                            }
                        }
                        else
                        {
                            distance = distanceTest;
                            Nearest = enemy.gameObject;
                        }
                    }
                }
                else if (Priority == "WS")
                {
                    float distanceTest = Mathf.Abs((enemy.gameObject.transform.position - transform.position).magnitude);
                    if (enemy.GetComponent<WSShared>() != null)
                    {
                        if (Nearest.GetComponent<WSShared>() != null)
                        {
                            if (distanceTest < distance)
                            {
                                distance = distanceTest;
                                Nearest = enemy.gameObject;
                            }
                        }
                        else
                        {
                            distance = distanceTest;
                            Nearest = enemy.gameObject;
                        }
                    }
                }
                else if (Priority == "SS")
                {
                    if (enemy.GetComponent<SpaceStationShared>() != null)
                    {
                        Nearest = enemy.gameObject;
                        break;
                    }
                }
            }
            if (weaponName1.Contains("Gravitational") && ForceTargetGO == null && (Nearest.GetComponent<WSShared>() != null || Nearest.GetComponent<SpaceStationShared>() != null))
            {
                GravTarget = Nearest;
            }
            ForceTargetGO = Nearest;
        }
    }

    public void TargetRightEnemy()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, TargetRange, FindObjectOfType<GameController>().PlayerLayer);
        if (cols.Length > 0)
        {
            GameObject Nearest = cols[0].gameObject;
            float distance = Mathf.Abs((cols[0].transform.position - transform.position).magnitude);
            foreach (var enemy in cols)
            {
                if (enemy == ForceTargetGO)
                {
                    Nearest = enemy.gameObject;
                    break;
                }
                float distanceTest = Mathf.Abs((enemy.gameObject.transform.position - transform.position).magnitude);
                if (Priority == null || Priority == "" || (!Priority.Contains("WS") && !Priority.Contains("SS")))
                {
                    if (weaponName2.Contains("Gravitational"))
                    {
                        if (enemy.GetComponent<WSShared>() != null || enemy.GetComponent<SpaceStationShared>() != null)
                        {
                            if (Nearest.GetComponent<WSShared>() != null || Nearest.GetComponent<SpaceStationShared>() != null)
                            {
                                if (distanceTest < distance)
                                {
                                    distance = distanceTest;
                                    Nearest = enemy.gameObject;
                                }
                            }
                            else
                            {
                                distance = distanceTest;
                                Nearest = enemy.gameObject;
                            }
                        }
                        else
                        {
                            if (Nearest.GetComponent<WSShared>() == null && Nearest.GetComponent<SpaceStationShared>() == null)
                                if (distanceTest < distance)
                                {
                                    distance = distanceTest;
                                    Nearest = enemy.gameObject;
                                }
                        }
                    }
                }
                else
                if (distanceTest < distance)
                {
                    distance = distanceTest;
                    Nearest = enemy.gameObject;
                }
            }
            if (weaponName2.Contains("Gravitational") && ForceTargetGO == null && (Nearest.GetComponent<WSShared>() != null || Nearest.GetComponent<SpaceStationShared>() != null))
            {
                GravTarget = Nearest;
            }
            RightTarget = Nearest;
        }
    }

    private void CheckTargetEnemy()
    {
        bool check = false;
        if (Priority != null && Priority != "")
        {
            if (ForceTargetGO == null)
                InitTargetEnemy();
            else
            {
                if ((ForceTargetGO.transform.position - transform.position).magnitude <= TargetRange)
                {
                    if (LeftTarget != ForceTargetGO)
                    {
                        LeftTarget = null;
                        LeftWeapon.GetComponent<Weapons>().Aim = null;
                    }
                    if (RightTarget != ForceTargetGO)
                    {
                        RightTarget = null;
                        RightWeapon.GetComponent<Weapons>().Aim = null;
                    }
                    check = true;
                }
            }
        }
        else if (GravTarget != null)
        {
            check = true;
            if (weaponName1.Contains("Gravitational"))
            {
                if (LeftTarget != GravTarget)
                {
                    LeftTarget = null;
                    LeftWeapon.GetComponent<Weapons>().Aim = null;
                }
            }
            if (weaponName2.Contains("Gravitational"))
            {
                if (RightTarget != GravTarget)
                {
                    RightTarget = null;
                    RightWeapon.GetComponent<Weapons>().Aim = null;
                }
            }
        }
        else if (weaponName1.Contains("Gravitational") || weaponName2.Contains("Gravitational"))
        {
            if (weaponName1.Contains("Gravitational"))
            {
                LeftTarget = null;
                LeftWeapon.GetComponent<Weapons>().Aim = null;
            }
            else
            {
                check = true;
            }
            if (weaponName2.Contains("Gravitational"))
            {
                RightTarget = null;
                RightWeapon.GetComponent<Weapons>().Aim = null;
            }
            else
            {
                check = true;
            }
        }
        else check = true;
        if (check)
        {
            if (LeftTarget != null && (Mathf.Abs((LeftTarget.transform.position - transform.position).magnitude) > TargetRange || LeftTarget.layer == LayerMask.NameToLayer("Untargetable")))
            {
                LeftTarget = null;
                LeftWeapon.GetComponent<Weapons>().Aim = null;
            }
            if (RightTarget != null && (Mathf.Abs((RightTarget.transform.position - transform.position).magnitude) > TargetRange || RightTarget.layer == LayerMask.NameToLayer("Untargetable")))
            {
                RightTarget = null;
                RightWeapon.GetComponent<Weapons>().Aim = null;
            }
        }
    }

    private void CheckWSSS()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.zero);
        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.GetComponent<WSShared>() != null)
                {
                    GameObject Explo = Instantiate(Explosion, transform.position, Quaternion.identity);
                    Explo.transform.localScale *= 3;
                    Explo.GetComponent<SpriteRenderer>().sortingOrder = 10;
                    Explo.SetActive(true);
                    Destroy(Explo, 0.3f);
                    hit.collider.gameObject.GetComponent<WSShared>().ReceiveBulletDamage(hit.collider.gameObject.GetComponent<WSShared>().MaxHP * 5 / 100f, null, true, transform.position);
                    Destroy(gameObject);
                }
                else if (hit.collider.gameObject.GetComponent<SpaceStationShared>() != null)
                {
                    GameObject Explo = Instantiate(Explosion, transform.position, Quaternion.identity);
                    Explo.transform.localScale *= 3;
                    Explo.GetComponent<SpriteRenderer>().sortingOrder = 10;
                    Explo.SetActive(true);
                    Destroy(Explo, 0.3f);
                    hit.collider.gameObject.GetComponent<SpaceStationShared>().ReceiveBombingDamage(hit.collider.gameObject.GetComponent<SpaceStationShared>().MaxHP * 5 / 100f, transform.position);
                    Destroy(gameObject);
                }
            }
        }
    }
    #endregion
    #region Bounty
    public void AddBounty()
    {
        FindObjectOfType<GameplayInteriorController>().AddCashAndShard(BountyCash, BountyShard, gameObject);       
    }
    #endregion
}
