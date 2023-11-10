using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using UnityEngine;

public class AlliesShared : FighterShared
{
    #region Shared Variables
    private FighterMovement fm;
    public AlliesHealthBar HealthBar;
    public AlliesHealthBar ShieldBar;
    public float ScaleOnStatusBoard;
    public GameObject AllyStatus;
    private StatusBoard Status;
    public GameObject Weapons;
    public Rigidbody2D rb;
    public float changeDirTimer;
    public GameObject PowerModel;
    public LayerMask EnemyLayer;
    public GameObject BackFire;
    public GameObject HeadPos;
    public GameObject SpawnHole;

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
    public float DelayBetween2Weap;
    public float DelayTimer;
    public bool LeftFire;
    public GameObject LeftTarget;
    public GameObject RightTarget;
    public float TargetRange;
    public float TargetRefreshTimer;
    public float FindTargetTimer;
    public float HPScale;
    public bool IsEscorting;
    public Vector3 EscortTargetPosition;
    private int DirMov;
    public bool Escort;
    public GameObject EscortObject;
    public string Priority;
    public GameObject ForceTargetGO;
    public bool IsChasingRightEnemy;
    public GameObject GravTarget;
    public bool Defend;
    public GameObject DefendObject;
    public GameObject NearestTarget;
    public string Class;
    #endregion
    #region Shared Functions
    // Set Health to Health Bar
    public void SetHealth()
    {
        HealthBar.SetValue(CurrentHP, MaxHP);
        ShieldBar.SetValue(CurrentBarrier, MaxBarrier);
    }

    public void UpdateAlly()
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
        Status.Timer = 3f;
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
        Status = AllyStatus.GetComponent<StatusBoard>();
        InitializeFighter();
        ShieldPassive();
        LeftFire = false;
    }

    private void Update()
    {
        UpdateAlly();
        if (IsEscorting)
        {
            fm.UpMove();
            if ((EscortTargetPosition - transform.GetChild(5).position).magnitude < 100f)
            {
                DoneEscorting();
            }
            if (!CheckEscortPath())
            {
                GameObject EscortGo = new GameObject();
                EscortGo.transform.position = EscortTargetPosition;
                CheckIsUpOrDownMovement(transform.GetChild(5).gameObject,EscortGo,BackFire);
                Destroy(EscortGo);
                if (DirMov == 1)
                {
                    fm.LeftMove();
                } else if (DirMov ==-1)
                {
                    fm.RightMove();
                } else if (DirMov==0)
                {
                    fm.NoLeftRightMove();
                }
            }
        } else
        {
            if (doneInitWeapon)
            {
                DelayTimer -= Time.deltaTime;
                /*if (LeftWeapon != null)
                {
                    if (DelayTimer < 0f && !LeftFire)
                    {
                        LeftFire = true;
                        LeftWeapon.GetComponent<Weapons>().AIShootBullet();
                        DelayTimer = DelayBetween2Weap;
                    }
                }
                if (RightWeapon != null)
                {
                    if (DelayTimer < 0f && LeftFire)
                    {
                        LeftFire = false;
                        RightWeapon.GetComponent<Weapons>().AIShootBullet();
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
                TargetRefreshTimer = Defend? Random.Range(0.5f, 1.5f) : Random.Range(2.5f, 3.5f);
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
                FindTargetTimer = Defend ? Random.Range(0.5f, 1.5f) : Random.Range(2.5f, 3.5f);;
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
    #endregion
    #region Init Data
    public void InitData(Dictionary<string, object> Data, GameObject Model)
    {
        StatsDataDict = new Dictionary<string, object>();
        fm = GetComponent<FighterMovement>();
        FighterName = (string)Data["Name"];
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
        }
        else if (checkPowers.Length == 1)
        {
            Power1 = checkPowers[0];
            Power2 = "";
        }
        else
        {
            Power1 = "";
            Power2 = "";
        }
        string[] checkWeapons = ((string)Data["Weapons"]).Split("|");
        if (checkWeapons.Length > 1)
        {
            weaponName1 = checkWeapons[0];
            weaponName2 = checkWeapons[1];
        }
        else
        {
            weaponName1 = checkWeapons[0];
            weaponName2 = checkWeapons[0];
        }
        if (weaponName1 == "Transport")
        {
            IsEscorting = true;
            Vector3 HealthPos = fm.HealthBarSlider.transform.position - transform.position;
            Vector3 ShieldPos = fm.ShieldBarSlider.transform.position - transform.position;
            EscortTargetPosition = new Vector3(Random.Range(3500, 4900), Random.Range(-4900, -3500), 0);
            float angle = Vector2.Angle(transform.GetChild(5).position - BackFire.transform.position, EscortTargetPosition - transform.position);
            transform.Rotate(new Vector3(0, 0, -angle));
            OnFireGO.transform.Rotate(new Vector3(0, 0, angle));
            OnFreezeGO.transform.Rotate(new Vector3(0, 0, angle));
            fm.HealthBarSlider.transform.Rotate(new Vector3(0, 0, angle));
            fm.ShieldBarSlider.transform.Rotate(new Vector3(0, 0, angle));
            fm.HealthBarSlider.transform.position = transform.position + HealthPos;
            fm.ShieldBarSlider.transform.position = transform.position + ShieldPos;
            HealthBar.Position = HealthPos;
            ShieldBar.Position = ShieldPos;
            fm.CurrentRotateAngle = angle;
            BackFire.transform.localPosition *= 3;
            ScaleOnStatusBoard /= 3;
            GetComponent<Rigidbody2D>().mass = 3;
            Destroy(GetComponent<DecisionRequester>());
            Destroy(GetComponent<AlliesFighterMLAgent>());
            Destroy(GetComponent<BehaviorParameters>());
        }
        else
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
                if (!alreadyLeft && Weapons.transform.GetChild(i).name.Replace(" ", "").ToLower().Equals(weaponName1.Replace(" ", "").ToLower()))
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
            LW.EnemyLayer = EnemyLayer;
            LW.RotateLimitNegative = float.Parse((string)StatsDataDict["AOFNegative"]);
            LW.RotateLimitPositive = float.Parse((string)StatsDataDict["AOFPositive"]);
            LW.FighterWeaponDamageMod = float.Parse((string)StatsDataDict["DM"]);
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
            RW.EnemyLayer = EnemyLayer;
            RW.RotateLimitNegative = float.Parse((string)StatsDataDict["AOFNegative"]);
            RW.RotateLimitPositive = float.Parse((string)StatsDataDict["AOFPositive"]);
            RW.FighterWeaponDamageMod = float.Parse((string)StatsDataDict["DM"]);
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
                transform.Rotate(new Vector3(0, 0, -90));
                OnFireGO.transform.Rotate(new Vector3(0, 0, 90));
                OnFreezeGO.transform.Rotate(new Vector3(0, 0, 90));
                fm.HealthBarSlider.transform.Rotate(new Vector3(0, 0, 90));
                fm.ShieldBarSlider.transform.Rotate(new Vector3(0, 0, 90));
                fm.CurrentRotateAngle = 90;
            } else
            {
                transform.Rotate(new Vector3(0, 0, -135));
                OnFireGO.transform.Rotate(new Vector3(0, 0, 135));
                OnFreezeGO.transform.Rotate(new Vector3(0, 0, 135));
                fm.HealthBarSlider.transform.Rotate(new Vector3(0, 0, 135));
                fm.ShieldBarSlider.transform.Rotate(new Vector3(0, 0, 135));
                fm.CurrentRotateAngle = 135;
                EscortSSTP();
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
            TargetRange = LW.Bullet.GetComponent<BulletShared>().MaxEffectiveDistance <= RW.Bullet.GetComponent<BulletShared>().MaxEffectiveDistance ? LW.Bullet.GetComponent<BulletShared>().MaxEffectiveDistance : RW.Bullet.GetComponent<BulletShared>().MaxEffectiveDistance;
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
                }
                else
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
            ShieldPassive();
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
        Debug.Log(FighterName + " " + br);
        CurrentBarrier = MaxBarrier;
    }
    private void UseFirstPower()
    {
        if (Power1CD <= 0f && !isFrozen && !isSFBFreeze)
        {
            CurrentFirstPower.GetComponent<Powers>().Fighter = gameObject;
            CurrentFirstPower.GetComponent<Powers>().EnemyLayer = EnemyLayer;
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
            CurrentSecondPower.GetComponent<Powers>().EnemyLayer = EnemyLayer;
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
        yield return new WaitForSeconds(0);
        doneInitWeapon = true;
    }

    public void TargetLeftEnemy()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, TargetRange, EnemyLayer);
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
                        if (enemy.GetComponent<WSShared>()!=null || enemy.GetComponent<SpaceStationShared>()!=null)
                        {
                            if (Nearest.GetComponent<WSShared>() != null || Nearest.GetComponent<SpaceStationShared>() != null)
                            {
                                if (distanceTest < distance)
                                {
                                    distance = distanceTest;
                                    Nearest = enemy.gameObject;
                                }
                            } else
                            {
                                distance = distanceTest;
                                Nearest = enemy.gameObject;
                            }
                        } else
                        {
                            if (Nearest.GetComponent<WSShared>() == null && Nearest.GetComponent<SpaceStationShared>() == null)
                            if (distanceTest < distance)
                            {
                                distance = distanceTest;
                                Nearest = enemy.gameObject;
                            }
                        }
                    }
                } else
                if (distanceTest < distance)
                {
                    distance = distanceTest;
                    Nearest = enemy.gameObject;
                }
            }
            if (weaponName1.Contains("Gravitational") && ForceTargetGO == null && (Nearest.GetComponent<WSShared>() != null || Nearest.GetComponent<SpaceStationShared>() != null))
            {
                GravTarget = Nearest;
            }
            LeftTarget = Nearest;
        }
    }

    public void TargetNearestTarget()
    {
        if (NearestTarget==null)
        {
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 15000f, EnemyLayer);
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

    public void TargetRightEnemy()
    {

        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, TargetRange, EnemyLayer);
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

    public void InitTargetEnemy()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 15000f, EnemyLayer);
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
                else if (Priority == "SSTP")
                {
                    float distanceTest = Mathf.Abs((enemy.gameObject.transform.position - transform.position).magnitude);
                    if (enemy.name.Contains("SSTP"))
                    {
                        if (!Nearest.name.Contains("SSTP"))
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
            ForceTargetGO = Nearest;
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
        else if (DamageDealer != null)
        {
            LeftTarget = DamageDealer;
            LeftWeapon.GetComponent<Weapons>().Aim = DamageDealer;
            RightTarget = DamageDealer;
            RightWeapon.GetComponent<Weapons>().Aim = DamageDealer;
            check = false;
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
        if (Escort && EscortObject==null) 
        EscortSSTP();
        else if (Defend && DefendObject == null)
        {
            DefendStation();
        }
    }

    private void EscortSSTP()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 5000f, FindObjectOfType<GameController>().PlayerLayer);
        if (cols.Length > 0)
        {
            GameObject Nearest = cols[0].gameObject;
            float distance = Mathf.Abs((cols[0].transform.position - transform.position).magnitude);
            foreach (var escort in cols)
            {
                if (escort.name.Contains("SSTP"))
                {
                    EscortObject = escort.gameObject;
                    break;
                }
            }
        }
    }

    private void DefendStation()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 10000f, FindObjectOfType<GameController>().PlayerLayer);
        if (cols.Length > 0)
        {
            GameObject Nearest = cols[0].gameObject;
            float distance = Mathf.Abs((cols[0].transform.position - transform.position).magnitude);
            foreach (var escort in cols)
            {
                if (escort.GetComponent<SpaceStationShared>()!=null || escort.GetComponent<WSShared>()!=null)
                {
                    DefendObject = escort.gameObject;
                    break;
                }
            }
        }
    }

    private bool CheckEscortPath()
    {
        float TopToBottom = (transform.GetChild(5).position - BackFire.transform.position).magnitude;
        float TopToTarget = (EscortTargetPosition - transform.GetChild(5).position).magnitude;
        float BottomToTarget = (EscortTargetPosition - BackFire.transform.position).magnitude;
        return TopToTarget + TopToBottom - BottomToTarget < 100f;
    }

    private void CheckIsUpOrDownMovement(GameObject target, GameObject Head, GameObject Back)
    {
        Vector2 HeadToTarget = target.transform.position - Head.transform.position;
        Vector2 MovingVector = Head.transform.position - Back.transform.position;
        float angle = Vector2.Angle(HeadToTarget, MovingVector);
        float DistanceNew = Mathf.Cos(angle * Mathf.Deg2Rad) * HeadToTarget.magnitude;
        Vector2 TempPos = new Vector2(Back.transform.position.x, Back.transform.position.y) + MovingVector / MovingVector.magnitude * (MovingVector.magnitude + DistanceNew);
        Vector2 CheckPos = new Vector2(target.transform.position.x, target.transform.position.y) + (TempPos - new Vector2(target.transform.position.x, target.transform.position.y)) * 2;
        if (Head.transform.position.x == Back.transform.position.x)
        {
            if (Head.transform.position.y > Back.transform.position.y)
            {
                if (target.transform.position.x < Back.transform.position.x)
                {
                    DirMov = -1;
                }
                else if (target.transform.position.x == Back.transform.position.x)
                {
                    DirMov = 0;
                }
                else
                {
                    DirMov = 1;
                }
            }
            else
            {
                if (target.transform.position.x < Head.transform.position.x)
                {
                    DirMov = 1;
                }
                else if (target.transform.position.x == Head.transform.position.x)
                {
                    DirMov = 0;
                }
                else
                {
                    DirMov = -1;
                }
            }
        }
        else if (Head.transform.position.y == Back.transform.position.y)
        {
            if (Head.transform.position.x > Back.transform.position.x)
            {
                if (target.transform.position.y > Head.transform.position.y)
                {
                    DirMov -= 1;
                }
                else if (target.transform.position.y == Head.transform.position.y)
                {
                    DirMov = 0;
                }
                else
                {
                    DirMov = 1;
                }
            }
            else
            {
                if (target.transform.position.y > Head.transform.position.y)
                {
                    DirMov = 1;
                }
                else if (target.transform.position.y == Head.transform.position.y)
                {
                    DirMov = 0;
                }
                else
                {
                    DirMov -= 1;
                }
            }
        }
        else if (Head.transform.position.x > Back.transform.position.x)
        {
            if (CheckPos.y < target.transform.position.y)
            {
                DirMov = -1;
            }
            else
            {
                DirMov = 1;
            }
        }
        else
        {
            if (CheckPos.y < target.transform.position.y)
            {
                DirMov = 1;
            }
            else
            {
                DirMov = -1;
            }
        }

    }

    private void DoneEscorting()
    {
        FindObjectOfType<SpaceZoneMission>().AllyEscortDone();
        GameObject SpawnEffect = Instantiate(SpawnHole, transform.position, Quaternion.identity);
        SpawnEffect.SetActive(true);
        Destroy(SpawnEffect, 1.5f);
        Destroy(gameObject);
    }
    #endregion
}
