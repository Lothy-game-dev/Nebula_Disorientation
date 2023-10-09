using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlliesShared : FighterShared
{
    #region Shared Variables
    private FighterMovement fm;
    public AlliesHealthBar HealthBar;
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
    private bool LeftFire;
    public GameObject LeftTarget;
    public GameObject RightTarget;
    private float TargetRange;
    public float TargetRefreshTimer;
    public float FindTargetTimer;
    public float HPScale;
    private bool IsEscorting;
    private Vector3 EscortTargetPosition;
    private int DirMov;
    #endregion
    #region Shared Functions
    // Set Health to Health Bar
    public void SetHealth()
    {
        HealthBar.SetValue(CurrentHP, MaxHP);
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
        Status = AllyStatus.GetComponent<StatusBoard>();
        InitializeFighter();
        LeftFire = false;
    }

    private void Update()
    {
        UpdateAlly();
        if (IsEscorting)
        {
            fm.UpMove();
            if ((EscortTargetPosition - transform.GetChild(5).position).magnitude < 5f)
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
                if (LeftWeapon != null)
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
                }
                if (Power1 != "")
                {
                    if (Power1CD <= 0f && CurrentBarrier < MaxBarrier)
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
            /*
            resetMovetimer -= Time.deltaTime;
            if (resetMovetimer <= 0f)
            {
                RandomMove = Random.Range(1, 3);
                RandomRotate = Random.Range(1, 4);
                resetMovetimer = 2f;
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
            }*/
            TargetRefreshTimer -= Time.deltaTime;
            if (TargetRefreshTimer <= 0f)
            {
                TargetRefreshTimer = /*Random.Range(2.5f, 3.5f);*/0f;
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
                FindTargetTimer = /*Random.Range(2.5f, 3.5f);*/0f;
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
            EscortTargetPosition = new Vector3(Random.Range(3500, 4900), Random.Range(-4900, -3500), 0);
            float angle = Vector2.Angle(transform.GetChild(5).position - BackFire.transform.position, EscortTargetPosition - transform.position);
            transform.Rotate(new Vector3(0, 0, -angle));
            OnFireGO.transform.Rotate(new Vector3(0, 0, angle));
            OnFreezeGO.transform.Rotate(new Vector3(0, 0, angle));
            fm.HealthBarSlider.transform.Rotate(new Vector3(0, 0, angle));
            fm.CurrentRotateAngle = angle;
            BackFire.transform.localPosition *= 3;
        }
        else
        {
            transform.Rotate(new Vector3(0, 0, -90));
            OnFireGO.transform.Rotate(new Vector3(0, 0, 90));
            OnFreezeGO.transform.Rotate(new Vector3(0, 0, 90));
            fm.HealthBarSlider.transform.Rotate(new Vector3(0, 0, 90));
            fm.CurrentRotateAngle = 90;
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
        }

        gameObject.SetActive(true);
        StartCoroutine(WaitForDoneInit());
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
        yield return new WaitForSeconds(Random.Range(1, 10));
        doneInitWeapon = true;
    }

    private void TargetLeftEnemy()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, TargetRange, EnemyLayer);
        if (cols.Length>0)
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
            LeftTarget = Nearest;
        }
    }

    private void TargetRightEnemy()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, TargetRange, EnemyLayer);
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
            RightTarget = Nearest;
        }
    }

    private void CheckTargetEnemy()
    {
        if (LeftTarget!=null && (Mathf.Abs((LeftTarget.transform.position - transform.position).magnitude)>TargetRange || LeftTarget.layer == LayerMask.NameToLayer("Untargetable")))
        {
            LeftTarget = null;
            LeftWeapon.GetComponent<Weapons>().Aim = null;
        }
        if (RightTarget != null && (Mathf.Abs((RightTarget.transform.position - transform.position).magnitude) > TargetRange  || RightTarget.layer == LayerMask.NameToLayer("Untargetable")))
        {
            RightTarget = null;
            RightWeapon.GetComponent<Weapons>().Aim = null;
        }
    }

    private bool CheckEscortPath()
    {
        float TopToBottom = (transform.GetChild(5).position - BackFire.transform.position).magnitude;
        float TopToTarget = (EscortTargetPosition - transform.GetChild(5).position).magnitude;
        float BottomToTarget = (EscortTargetPosition - BackFire.transform.position).magnitude;
        return TopToTarget + TopToBottom - BottomToTarget < 3f;
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
        Destroy(gameObject);
    }
    #endregion
}
