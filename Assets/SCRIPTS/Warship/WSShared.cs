using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using UnityEngine;

public class WSShared : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    private StatisticController Statistic;
    private GameController Controller;
    private WSMovement wm;
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public LayerMask MainWeaponTarget;
    public LayerMask SupWeaponTarget;
    public Animator Animation;
    public GameObject Barrier;
    public GameObject BarrierBreak;
    public GameObject Flash;
    public GameObject Explosion;
    public bool IsEnemy;
    public NNModel MLBrain;
    public SpaceZoneHazardEnvironment HazEnv;
    public GameObject WarshipStatus;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public float CurrentHP;
    public float MaxHP;
    public float CurrentBarrier;
    public float MaxBarrier;
    public float BaseSpeed;
    public float RotationSpd;
    public string WarshipName;
    public string Tier;
    private Dictionary<string, object> WsStat;
    private WSMovement WM;
    private float resetMovetimer;
    private int RandomMove;
    private int RandomRotate;
    public List<string> MainWeapon;
    public List<string> SupWeapon;
    public GameObject Weapons;
    public GameObject BackFire;
    private float TargetRefreshTimer;
    public List<GameObject> SpWps;
    public List<GameObject> MainWps;
    public float FindTargetTimer;
    public float FindMainTargetTimer;
    private bool doneInitWeapon;
    private float[] DelayTimer;
    private float SpawnTimer;
    public GameObject AllyModel;
    private GameObject ChosenModel;
    public GameObject AllyTemplate;
    private List<int> FightersId;
    private int Count;
    private int[] EnemiesTier;
    private bool Spawned;
    private float BarrierEffectDelay;
    private float BarrierRegenTimer;
    private float BarrierRegenAmount;
    private float BarrierRegenDelay;
    private bool AlreadyDestroy;
    public Dictionary<GameObject, GameObject> MainTarget;
    public float TargetRange;
    public float SPTargetRange;
    public GameObject Head;
    public WSHealthBar HPBar;
    public WSHealthBar ShieldBar;
    private bool isHit;
    private Dictionary<GameObject, int> WSSSDict;
    public int Order;
    private bool HitByPlayer;
    private int BountyCash;
    private int BountyShard;
    public Dictionary<GameObject , GameObject> SpWeaponTargets;
    private float Distance;
    private bool isFighting;
    public bool isStation;
    private GameObject Killer;
    public GameObject NearestTarget;
    private float PlayerDamageReceive;
    public List<GameObject> Targets;
    private StatusBoard Status;
    private GameplayInteriorController controller;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        CurrentHP = MaxHP;
        Count = 0;
        SpawnTimer = 10f;
        FightersId = (gameObject.layer == LayerMask.NameToLayer("Enemy") ? FindAnyObjectByType<SpaceZoneGenerator>().EnemyFighterIDs : FindAnyObjectByType<SpaceZoneGenerator>().AllyFighterIDs);
        Debug.Log(FightersId.Count);
        EnemiesTier = FindObjectOfType<SpaceZoneGenerator>().EnemiesTier;
        FindAnyObjectByType<WSSSDetected>().DectectWSSS();
        WSSSDict = FindAnyObjectByType<WSSSDetected>().PrioritizeDict;
        wm = GetComponent<WSMovement>();
        Order = WSSSDict[gameObject];
        isFighting = false;
        Controller = FindObjectOfType<GameController>();
        Statistic = FindAnyObjectByType<StatisticController>();
        ShieldBar.SetValue(CurrentBarrier, MaxBarrier, true);
        HPBar.SetValue(CurrentHP, MaxHP, true);
        Status = WarshipStatus.GetComponent<StatusBoard>();
        controller = FindAnyObjectByType<GameplayInteriorController>();
    }

    // Update is called once per frame
    void Update()
    {
        Targets = new List<GameObject>(SpWeaponTargets.Values);
        // Weapon action
        if (doneInitWeapon)
        {
            for (int i = 0; i < MainWps.Count; i++)
            {
                DelayTimer[i] -= Time.deltaTime;
                if (MainWps[i] != null && MainWps[i].GetComponent<Weapons>() != null)
                {
                    if (DelayTimer[i] <= 0f)
                    {
                        MainWps[i].GetComponent<Weapons>().isCharging = true;
                        MainWps[i].GetComponent<Weapons>().Fireable = true;
                        DelayTimer[i] = MainWps[i].GetComponent<Weapons>().Cooldown;
                    }
                }
            }

            /*for (int i = 0; i < SpWps.Count; i++)
            {
                if (SpWps[i] != null)
                {
                    SpWps[i].GetComponent<Weapons>().WSShootBullet();
                }
            }*/
        }
        // Call function and timer only if possible

        //Reset Target
        TargetRefreshTimer -= Time.deltaTime;
        if (TargetRefreshTimer <= 0f)
        {
            TargetRefreshTimer = 0.25f;

            for (int i = 0; i < SpWps.Count; i++)
            {
                CheckTargetEnemy(SpWps[i]);
                if (SpWps[i] != null && SpWps[i].GetComponent<Weapons>() != null)
                {
                    SpWps[i].GetComponent<Weapons>().Aim = SpWeaponTargets[SpWps[i]];
                }
            }
            for (int i = 0; i < MainWps.Count; i++)
            {
                CheckTargetEnemy(MainWps[i]);
                if (MainWps[i] != null && MainWps[i].GetComponent<Weapons>() != null)
                {
                    MainWps[i].GetComponent<Weapons>().Aim = MainTarget[MainWps[i]];
                }
            }

        }

        try
        {
            foreach (var target in SpWeaponTargets)
            {
                target.Value.GetComponent<Transform>();
            }
        }
        catch (MissingReferenceException)
        {
            FindTargetTimer -= Time.deltaTime;
        }
        catch (System.NullReferenceException)
        {
            FindTargetTimer -= Time.deltaTime;
        }

        if (FindTargetTimer <= 0f)
        {
            FindTargetTimer = 0.25f;
            for (int i = 0; i < SpWps.Count; i++)
            {
                SpWeaponTargets[SpWps[i]] = TargetEnemy(SpWps[i]);
            }
        }

        try
        {
            foreach (var target in MainTarget)
            {
                target.Value.GetComponent<Transform>();
            }
        }
        catch (MissingReferenceException)
        {
            FindMainTargetTimer -= Time.deltaTime;
        }
        catch (System.NullReferenceException)
        {
            FindMainTargetTimer -= Time.deltaTime;
        }

        if (FindMainTargetTimer <= 0f)
        {
            FindMainTargetTimer = 0.25f;
            for (int i = 0; i < MainWps.Count; i++)
            {
                MainWeaponTargetEnemy(MainWps[i]);
            }
        }

        // This is for the non-weapon warship 
        if (MainWps[0].GetComponent<Weapons>() == null && FightersId.Count > 0)
        {
            
            SpawnTimer -= Time.deltaTime;
            Animation = MainWps[0].GetComponent<Animator>();    
            if (Spawned)
            {
                Animation.SetBool("isSpawn", false);
                Spawned = false;
            }
            if (SpawnTimer < 0f)
            {
                Debug.Log("Spawn");
                int random = Random.Range(0, FightersId.Count);
                Animation.SetBool("isSpawn", true);
                SpawnFighter(FightersId[random], EnemiesTier[random]);
                Spawned = true;
                Count++;
                SpawnTimer = 10f;
                              
            }
        }
        CheckBarrierAndHealth();
        if (HazEnv != null)
        {
            if (HazEnv.HazardID == 5 || HazEnv.HazardID == 6)
            {
                CheckForHazard();
            }
        }
    }
    #endregion
    #region Init data
    // Group all function that serve the same algorithm
    public void InitData(Dictionary<string, object> data, GameObject model)
    {
        WM = GetComponent<WSMovement>();
        WsStat = new Dictionary<string, object>();
        WsStat = FindAnyObjectByType<GlobalFunctionController>().ConvertWarshipStatToDictionary(data["WarshipStat"].ToString());
        MaxHP = float.Parse(WsStat["HP"].ToString());
        BaseSpeed = float.Parse(WsStat["SPD"].ToString());
        RotationSpd = float.Parse(WsStat["ROT"].ToString());
        WM.RotateSpeed = RotationSpd;
        WM.MovingSpeed = BaseSpeed;
        if (IsEnemy)
        {
            string Bounty = (string)data["Bounty"];
            if (Bounty!="")
            {
                BountyCash = int.Parse(Bounty.Split("|")[0]);
                BountyShard = int.Parse(Bounty.Split("|")[1]);
            }
        }

        WarshipName = name;
        switch (data["Tier"].ToString())
        {
            case "#4c9aff": Tier = "Tier II"; break;
            case "#bf2600": Tier = "Tier I"; break;
        }
        //Set head object
        Vector2 headpos = model.GetComponent<WarshipModelShared>().HeadPosition;
        GameObject headclone = Instantiate(Head, new Vector3(transform.position.x + headpos.x, transform.position.y + headpos.y, transform.position.z), Quaternion.identity);
        headclone.transform.SetParent(gameObject.transform);
        WM.HeadObject = headclone;

        //Set Hp bar slider position
        HPBar.SetPostion(model.GetComponent<WarshipModelShared>().HealthBarPosiiton);
        ShieldBar.SetPostion(model.GetComponent<WarshipModelShared>().HealthBarPosiiton);

        //BackFire
        List<Vector2> BFpos = model.GetComponent<WarshipModelShared>().BackFirePos;
        List<Vector2> BFScale = model.GetComponent<WarshipModelShared>().BackFireScale;
        for (int i = 0; i < BFpos.Count; i++)
        {
            BackFire.transform.localScale = BFScale[i];
            GameObject game = Instantiate(BackFire, new Vector3(transform.position.x+BFpos[i].x, transform.position.y+BFpos[i].y, BackFire.transform.position.z), Quaternion.identity);
            game.transform.SetParent(gameObject.transform);
            game.transform.Rotate(new Vector3(0, 0, 180));
            game.name = "BackFire " + i;
            game.GetComponent<SpriteRenderer>().sortingOrder = model.GetComponent<WarshipModelShared>().BackFireSortingOrder[i];
            game.SetActive(false);
            WM.BackFires.Add(game);
        }
        //Main Weapon
        if (data["MainWeapon"].ToString().Contains("|"))
        {
            string[] weapons = data["MainWeapon"].ToString().Split("|");
            for (int i = 0; i < weapons.Length; i++)
            {
                MainWeapon.Add(weapons[i]);
            }
        } else
        {
            MainWeapon.Add(data["MainWeapon"].ToString());
        }

        MainWps = new List<GameObject>();
        List<Vector2> WPPos = model.GetComponent<WarshipModelShared>().MainWeaponPos;
        Vector2 WPScale = model.GetComponent<WarshipModelShared>().MainWeaponScale;
        for (int i = 0; i < MainWeapon.Count; i++)
        {
            for (int j = 0; j < Weapons.transform.childCount; j++)
            {
                //Find model
                if (Weapons.transform.GetChild(j).name.Replace(" ", "").ToLower().Contains(MainWeapon[i].Replace(" ", "").ToLower())) 
                {
                    GameObject main = Instantiate(Weapons.transform.GetChild(j).gameObject, new Vector3(transform.position.x + WPPos[i].x, transform.position.y + WPPos[i].y, Weapons.transform.GetChild(i).position.z), Quaternion.identity);
                    main.transform.SetParent(gameObject.transform);
                    main.SetActive(true);

                    if (MainWeapon[i] != "Carrier Hatch")
                    {
                        main.transform.localScale = WPScale;
                        Weapons wp = main.GetComponent<Weapons>();
                        wp.Fighter = gameObject;
                        wp.Aim = null;
                        wp.EnemyLayer = MainWeaponTarget;
                        wp.tracking = true;
                        wp.isMainWeapon = true;
                        TargetRange = wp.GetComponent<Weapons>().Bullet.GetComponent<BulletShared>().MaxEffectiveDistance;
                    } else
                    {
                        main.transform.localScale = WPScale;
                    }
                    MainWps.Add(main);


                }
            }           
        }

        DelayTimer = new float[MainWps.Count];

        //Sup Weapon
        Debug.Log(data["SupWeapon"].ToString());
        if (data["SupWeapon"].ToString().Contains("|"))
        {
            string[] weapons = data["SupWeapon"].ToString().Split("|");
            for (int i = 0; i < weapons.Length; i++)
            {
                SupWeapon.Add(weapons[i]);
            }
        }
        else
        {
            SupWeapon.Add(data["SupWeapon"].ToString());
        }

        List<Vector2> SupWPPos = model.GetComponent<WarshipModelShared>().SupWeaponPos;
        //TargetLeftEnemy(1000);
        /*TargetRightEnemy();*/
        SpWps = new List<GameObject>();
        for (int i = 0; i < SupWeapon.Count; i++)
        {
            for (int j = 0; j < Weapons.transform.childCount; j++)
            {
                //Find model
                if (Weapons.transform.GetChild(j).name.Replace(" ", "").Replace((IsEnemy ? "Zaturi" : "UEC"), "").ToLower() == (SupWeapon[i].Replace(" ", "").ToLower()))
                {
                    GameObject sup = Instantiate(Weapons.transform.GetChild(j).gameObject, new Vector3(transform.position.x + SupWPPos[i].x, transform.position.y + SupWPPos[i].y, Weapons.transform.GetChild(i).position.z), Quaternion.identity);
                    sup.transform.SetParent(gameObject.transform);
                    sup.transform.localScale = new Vector2(0.7f, 0.7f);
                    sup.SetActive(true);


                    Weapons wp = sup.GetComponent<Weapons>();
                    wp.Fighter = gameObject;
                    wp.Aim = null;
                    wp.EnemyLayer = SupWeaponTarget;
                    wp.tracking = true;
                    wp.RotateLimitPositive = 360;
                    wp.RotateLimitNegative = -360;
                    wp.isMainWeapon = false;

                    sup.AddComponent<BehaviorParameters>();
                    sup.GetComponent<BehaviorParameters>().BrainParameters.VectorObservationSize = 4;
                    
                    sup.GetComponent<BehaviorParameters>().BehaviorType = BehaviorType.InferenceOnly;
                    sup.GetComponent<BehaviorParameters>().Model = MLBrain;
                    sup.GetComponent<BehaviorParameters>().BehaviorName = "AllyFire23091012";
                    ActionSpec act = sup.GetComponent<BehaviorParameters>().BrainParameters.ActionSpec;
                    act.NumContinuousActions = 1;
                    act.BranchSizes = null;
                    sup.GetComponent<BehaviorParameters>().BrainParameters.ActionSpec = act;
                    sup.AddComponent<WSSupportWeaponMLAgent>();
                    sup.AddComponent<DecisionRequester>();

                    if (i == 0)
                    {
                        SPTargetRange = wp.GetComponent<Weapons>().Bullet.GetComponent<BulletShared>().MaxEffectiveDistance;
                    }
                    SpWps.Add(sup);
                    break;
                }
            }
            
        }
        SpWeaponTargets = new Dictionary<GameObject, GameObject>();
        MainTarget = new Dictionary<GameObject, GameObject>();
        for (int i = 0; i < MainWps.Count; i++)
        {
            MainWeaponTargetEnemy(MainWps[i]);
        }
        for (int i = 0; i < SpWps.Count; i++)
        {
            SpWeaponTargets.Add(SpWps[i], TargetEnemy(SpWps[i]));           
        }
        if (IsEnemy)
        {
            Vector3 HealthBarPos = WM.HPSlider.transform.position - transform.position;
            Vector3 ShieldBarPos = WM.ShieldSlider.transform.position - transform.position;
            float HPDistance = Mathf.Abs((model.GetComponent<WarshipModelShared>().HPBarHorizontalPosition - model.GetComponent<WarshipModelShared>().HealthBarPosiiton).magnitude);
            WM.HPDistance = HPDistance;
            transform.Rotate(new Vector3(0, 0, -270));
            WM.CurrentRotateAngle += 270;
            WM.HPSlider.transform.Rotate(new Vector3(0, 0, 270));
            WM.HPSlider.transform.position = transform.position + new Vector3(HealthBarPos.x, HealthBarPos.y - HPDistance - 150, HealthBarPos.z);
            HPBar.Position = new Vector3(HealthBarPos.x, HealthBarPos.y - HPDistance - 150, HealthBarPos.z);
            WM.ShieldSlider.transform.Rotate(new Vector3(0, 0, 270));
            WM.ShieldSlider.transform.position = transform.position + new Vector3(ShieldBarPos.x, ShieldBarPos.y - HPDistance - 150, ShieldBarPos.z);
            ShieldBar.Position = new Vector3(ShieldBarPos.x, ShieldBarPos.y - HPDistance - 150, ShieldBarPos.z);
            WM.HPBarVerticalPos = model.GetComponent<WarshipModelShared>().HealthBarPosiiton;
        } else
        {      
            Vector3 HealthBarPos = WM.HPSlider.transform.position - transform.position;
            Vector3 ShieldBarPos = WM.ShieldSlider.transform.position - transform.position;
            float HPDistance = Mathf.Abs((model.GetComponent<WarshipModelShared>().HPBarHorizontalPosition - model.GetComponent<WarshipModelShared>().HealthBarPosiiton).magnitude);
            WM.HPDistance = HPDistance;
            transform.Rotate(new Vector3(0, 0, -90));
            WM.CurrentRotateAngle += 90;
            WM.HPSlider.transform.Rotate(new Vector3(0, 0, 90));
            WM.HPSlider.transform.position = transform.position + new Vector3(HealthBarPos.x, HealthBarPos.y - HPDistance - 150, HealthBarPos.z);
            HPBar.Position = new Vector3(HealthBarPos.x, HealthBarPos.y - HPDistance - 150, HealthBarPos.z);
            WM.ShieldSlider.transform.Rotate(new Vector3(0, 0, 90));
            WM.ShieldSlider.transform.position = transform.position + new Vector3(ShieldBarPos.x, ShieldBarPos.y - HPDistance - 150, ShieldBarPos.z);
            ShieldBar.Position = new Vector3(ShieldBarPos.x, ShieldBarPos.y - HPDistance - 150, ShieldBarPos.z);
            WM.HPBarVerticalPos = model.GetComponent<WarshipModelShared>().HealthBarPosiiton;
        }

        doneInitWeapon = true;
        gameObject.SetActive(true);
    }
    #endregion
    #region Receive Damage
    public void ReceiveBulletDamage(float Damage, GameObject Bullet, bool isMainWeapon, Vector2 BulletHitPos)
    {
        float RealDamage = 0;
        if (Bullet!=null && Bullet.GetComponent<BulletShared>().WeaponShoot.GetComponent<Weapons>().Fighter.GetComponent<PlayerFighter>()!=null)
        {
            HitByPlayer = true;
        }
       
        if (!isMainWeapon)
        {
            if (Bullet.GetComponent<UsualKineticBullet>() != null)
            {
                if (!Bullet.GetComponent<UsualKineticBullet>().isGravitationalLine)
                {
                    RealDamage = Damage * 1 / 100f;
                }
                else
                {
                    RealDamage = Damage;
                }
            }
            else
            {
                RealDamage = Damage * 1 / 100f;
            }
        }
        else
        {
            RealDamage = Damage;
        }
        RealDamage *= HazEnv.HazardNDAllDamageScale;
        if (HitByPlayer)
        {
            Statistic.DamageDealt += RealDamage;
            PlayerDamageReceive += RealDamage;
        }
        if (CurrentBarrier > 0)
        {
            Camera.main.GetComponent<GameplaySoundSFXController>().GenerateSound("BarrierHit", gameObject);
            if (CurrentBarrier > RealDamage)
            {
                CurrentBarrier -= RealDamage;
                ShieldBar.SetValue(CurrentBarrier, MaxBarrier, true);                
                BarrierEffectDelay = 0.25f;
                GameObject br = Instantiate(Barrier, BulletHitPos, Quaternion.identity);
                br.SetActive(true);
                br.transform.SetParent(transform);
                Destroy(br, 0.25f);

            }
            else
            {
                float afterDamage = (RealDamage - CurrentBarrier);
                CurrentBarrier = 0;
                ShieldBar.SetValue(CurrentBarrier, MaxBarrier, true);              
       
                GameObject br = Instantiate(Barrier, BulletHitPos, Quaternion.identity);                
                br.SetActive(true);
                br.transform.SetParent(transform);
                Destroy(br, 0.25f);
                

                CurrentHP -= afterDamage;
                HPBar.SetValue(CurrentHP, MaxHP, true);
            }
        }
        else
        {
            Camera.main.GetComponent<GameplaySoundSFXController>().GenerateSound("WSSSHit", gameObject);
                            
            GameObject BRBreak = Instantiate(BarrierBreak, BulletHitPos, Quaternion.identity);
            BRBreak.SetActive(true);
            BRBreak.transform.SetParent(transform);
            Destroy(BRBreak, 0.5f);
            
            if (CurrentHP >= RealDamage)
            {
                CurrentHP -= RealDamage;
                isHit = true;
                HPBar.SetValue(CurrentHP, MaxHP, isHit);
            }
            else
            {
                CurrentHP = 0;
                if (Bullet != null)
                {
                    Killer = Bullet.GetComponent<BulletShared>().WeaponShoot.GetComponent<Weapons>().Fighter;
                }
                if (Killer == Controller.Player)
                {
                    Statistic.Warship += 1;
                    Statistic.TotalEnemyDefeated += 1;
                    Statistic.KillBossEnemy = true;
                }
            }
        }
    }

    public void ReceiveTrueDamage(float Damage, Vector2 BulletHitPos)
    {
        float RealDamage = Damage;
        RealDamage *= HazEnv.HazardNDAllDamageScale;
        if (CurrentBarrier > 0)
        {
            Camera.main.GetComponent<GameplaySoundSFXController>().GenerateSound("BarrierHit", gameObject);
            if (CurrentBarrier > RealDamage)
            {
                CurrentBarrier -= RealDamage;
                ShieldBar.SetValue(CurrentBarrier, MaxBarrier, true);
                GameObject br = Instantiate(Barrier, BulletHitPos, Quaternion.identity);
                br.SetActive(true);
                br.transform.SetParent(transform);
                Destroy(br, 0.25f);               
            }
            else
            {
                float afterDamage = (RealDamage - CurrentBarrier);
                CurrentBarrier = 0;
                ShieldBar.SetValue(CurrentBarrier, MaxBarrier, true);

 
                GameObject br = Instantiate(Barrier, BulletHitPos, Quaternion.identity);
                br.SetActive(true);
                br.transform.SetParent(transform);
                Destroy(br, 0.25f);
                

                CurrentHP -= afterDamage;
                HPBar.SetValue(CurrentHP, MaxHP, true);
            }
        }
        else
        {
            Camera.main.GetComponent<GameplaySoundSFXController>().GenerateSound("WSSSHit", gameObject);
              
            GameObject BRBreak = Instantiate(BarrierBreak, BulletHitPos, Quaternion.identity);
            BRBreak.SetActive(true);
            BRBreak.transform.SetParent(transform);
            Destroy(BRBreak, 0.5f);
            
            if (CurrentHP >= RealDamage)
            {
                CurrentHP -= RealDamage;
                isHit = true;
                HPBar.SetValue(CurrentHP, MaxHP, isHit);
            }
            else
            {
                CurrentHP = 0;
                if (Killer == Controller.Player)
                {
                    Statistic.Warship += 1;
                    Statistic.TotalEnemyDefeated += 1;
                    Statistic.KillBossEnemy = true;
                }
            }
        }
    }

    public void ReceivePowerDamage(float Damage, GameObject Power, GameObject FighterCast, Vector2 BulletHitPos)
    {
        float RealDamage = Damage * 10f /100;
        RealDamage *= HazEnv.HazardNDAllDamageScale;
        if (FighterCast.GetComponent<PlayerFighter>()!=null)
        {
            HitByPlayer = true;
        }
        if (HitByPlayer)
        {
            FindAnyObjectByType<StatisticController>().DamageDealt += Damage;
        }
        if (CurrentBarrier > 0)
        {
            Camera.main.GetComponent<GameplaySoundSFXController>().GenerateSound("BarrierHit", gameObject);
            if (CurrentBarrier > RealDamage)
            {
                CurrentBarrier -= RealDamage;
                ShieldBar.SetValue(CurrentBarrier, MaxBarrier, true);

                GameObject br = Instantiate(Barrier, BulletHitPos, Quaternion.identity);
                br.SetActive(true);
                br.transform.SetParent(transform);
                Destroy(br, 0.25f);

            }
            else
            {
                float afterDamage = (RealDamage - CurrentBarrier);
                CurrentBarrier = 0;
                ShieldBar.SetValue(CurrentBarrier, MaxBarrier, true);
                GameObject br = Instantiate(Barrier, BulletHitPos, Quaternion.identity);
                br.SetActive(true);
                br.transform.SetParent(transform);
                Destroy(br, 0.25f);
 
                CurrentHP -= afterDamage;
                HPBar.SetValue(CurrentHP, MaxHP, true);
            }
        }
        else
        {
            Camera.main.GetComponent<GameplaySoundSFXController>().GenerateSound("WSSSHit", gameObject);
          
            GameObject BRBreak = Instantiate(BarrierBreak, BulletHitPos, Quaternion.identity);
            BRBreak.SetActive(true);
            BRBreak.transform.SetParent(transform);
            Destroy(BRBreak, 0.5f);
            
            if (CurrentHP >= RealDamage)
            {
                CurrentHP -= RealDamage;
                isHit = true;
                HPBar.SetValue(CurrentHP, MaxHP, isHit);
            }
            else
            {
                CurrentHP = 0;
                if (Power != null)
                {
                    Killer = Power.GetComponent<Powers>().Fighter;
                }
                if (Killer == Controller.Player)
                {
                    Statistic.Warship += 1;
                    Statistic.TotalEnemyDefeated += 1;
                    Statistic.KillBossEnemy = true;
                }
            }
        }
    }
    #endregion
    #region Target
    public GameObject TargetEnemy(GameObject weapon)
    {        
        GameObject game = null;
        BulletShared bul = weapon.GetComponent<Weapons>().Bullet.GetComponent<BulletShared>();
        Collider2D[] cols = Physics2D.OverlapCircleAll(weapon.transform.position, (bul.MaximumDistance == 0 ? bul.MaxEffectiveDistance + 100 : bul.MaximumDistance + 100), (weapon.GetComponent<Weapons>().isMainWeapon == true ? MainWeaponTarget : SupWeaponTarget));
        if (cols.Length > 0)
        {
            // Find the nearest first, if a fighter is found, target it instead
            GameObject Nearest = cols[0].gameObject;
            float distance = Mathf.Abs((cols[0].transform.position - weapon.transform.position).magnitude);
            foreach (var enemy in cols)
            {
                if (!weapon.name.Contains("GravitationalArtillery"))
                {
                    if (enemy.GetComponent<FighterShared>() != null)
                    {
                        Nearest = enemy.gameObject;
                        break;                       
                    }
                    else
                    {
                        float distanceTest = Mathf.Abs((enemy.transform.position - weapon.transform.position).magnitude);
                        if (distanceTest < distance)
                        {
                            distance = distanceTest;
                            Nearest = enemy.gameObject;
                        }
                    }
                } else
                {
                    if (enemy.GetComponent<WSShared>() != null)
                    {
                        Nearest = enemy.gameObject;
                        break;
                    } else
                    {
                        float distanceTest = Mathf.Abs((enemy.transform.position - weapon.transform.position).magnitude);
                        if (distanceTest < distance)
                        {
                            distance = distanceTest;
                            Nearest = enemy.gameObject;
                        }
                    }
                }
            }
            game = Nearest;
        }
        return game;
    }

    public GameObject MainWeaponTargetEnemy(GameObject weapon)
    {
        GameObject game = null;
        Collider2D[] cols = Physics2D.OverlapCircleAll(weapon.transform.position, (isFighting == true ? TargetRange : 10000f), MainWeaponTarget);
        if (cols.Length > 0)
        {
            foreach (var enemy in cols)
            {
                // If weapon is main weap, target enemy ws/ss only
                if (weapon.GetComponent<Weapons>() != null && weapon.GetComponent<Weapons>().isMainWeapon)
                {
                    if (enemy.gameObject.tag == "BossEnemy" ||  enemy.gameObject.tag == "AlliesBossFighter")
                    {
                        GameObject Nearest = enemy.gameObject;
                        float distance = Mathf.Abs((Nearest.transform.position - weapon.transform.position).magnitude);
                        float distanceTest = Mathf.Abs((enemy.gameObject.transform.position - weapon.transform.position).magnitude);
                        if (distanceTest < distance)
                        {
                            distance = distanceTest;
                            Nearest = enemy.gameObject;
                        }
                        game = Nearest;
                        Distance = distanceTest;
                    } else
                    {
                        continue;
                    }
                }

            }
            // If the distance between 2 ws is too far, expand the search area to 10000f,
            // and will turn back to the weapon range if 2 ws are in the weapon range
            if (Distance <= TargetRange)
            {
                isFighting = true;
            }
            
        }
        MainTarget[weapon] = game;
        return game;
    }
    public void TargetNearestTarget()
    {
        if (NearestTarget == null)
        {
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 15000f, SupWeaponTarget);
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

    private void CheckTargetEnemy(GameObject weapon)
    {
        if (weapon.GetComponent<Weapons>() != null)
        {
            if (!weapon.GetComponent<Weapons>().isMainWeapon)
            {
                BulletShared bul = weapon.GetComponent<Weapons>().Bullet.GetComponent<BulletShared>();
                for (int i = 0; i < SpWeaponTargets.Count; i++)
                {
                    if (SpWeaponTargets[weapon] != null && (Mathf.Abs((SpWeaponTargets[weapon].transform.position - weapon.transform.position).magnitude) > (bul.MaximumDistance == 0 ? bul.MaxEffectiveDistance + 100 : bul.MaximumDistance + 100) || SpWeaponTargets[weapon].layer == LayerMask.NameToLayer("Untargetable")))
                    {                   
                        weapon.GetComponent<Weapons>().Aim = null;
                        SpWeaponTargets[weapon] = null;
                    }
                }
            } else
            {            
                BulletShared bul = weapon.GetComponent<Weapons>().Bullet.GetComponent<BulletShared>();
                for (int i = 0; i < MainTarget.Count; i++)
                {
                    if (MainTarget[weapon] != null && (Mathf.Abs((MainTarget[weapon].transform.position - weapon.transform.position).magnitude) > (bul.MaximumDistance == 0 ? bul.MaxEffectiveDistance + 100 : bul.MaximumDistance + 100) || MainTarget[weapon].layer == LayerMask.NameToLayer("Untargetable")))
                    {
                        weapon.GetComponent<Weapons>().Aim = null;
                        MainTarget[weapon] = null;
                    }
                }           
            }
        }
    }
    #endregion
    #region Spawn

    public void SpawnFighter(int id , int tier)
    {
        Dictionary<string, object> DataDict;
        float HPScale = 1;
        float BountyScale = 1;
        if (gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            DataDict = FindObjectOfType<AccessDatabase>().GetDataEnemyById(id);
            HPScale = FindObjectOfType<SpaceZoneGenerator>().EnemyMaxHP;
            BountyScale = FindObjectOfType<SpaceZoneGenerator>().EnemyBountyScale;          
        } else
        {
            DataDict = FindObjectOfType<AccessDatabase>().GetDataAlliesById(id);
            HPScale = FindObjectOfType<SpaceZoneGenerator>().AllyMaxHP;
        }
        // Get Model
        for (int i = 0; i < AllyModel.transform.childCount; i++)    
        {
            if (AllyModel.transform.GetChild(i).name.Replace(" ", "").ToLower().Equals(((string)DataDict["Name"]).Replace(" ", "").ToLower()))
            {
                ChosenModel = AllyModel.transform.GetChild(i).gameObject;
            }
        }
        GameObject Ally = Instantiate(AllyTemplate, MainWps[0].transform.position, Quaternion.identity);
        if (id >= 13)
        {
            Ally.tag = "AlliesEliteFighter";
        }
        if (id == 1)
        {
            Ally.transform.localScale *= 3;
        }
        AudioSource aus = Ally.AddComponent<AudioSource>();
        //aus.clip = SpawnSoundEffect;
        aus.spatialBlend = 1;
        aus.rolloffMode = AudioRolloffMode.Linear;
        aus.maxDistance = 1000;
        aus.minDistance = 500;
        aus.priority = 256;
        aus.dopplerLevel = 0;
        aus.spread = 360;
        Destroy(aus, 4f);
        Ally.name = ChosenModel.name + " |" + MainWps[0].transform.position.x + " - " + MainWps[0].transform.position.y + " - " + Count;
        if (gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Ally.GetComponent<SpriteRenderer>().sprite = ChosenModel.GetComponent<SpriteRenderer>().sprite;
            Ally.GetComponent<EnemyShared>().HPScale = HPScale;
            Ally.GetComponent<EnemyShared>().CashBountyScale = BountyScale;
            Ally.GetComponent<EnemyShared>().Tier = 2;
            Ally.GetComponent<EnemyShared>().InitData(DataDict, ChosenModel);
        } else
        {
            Ally.GetComponent<SpriteRenderer>().sprite = ChosenModel.GetComponent<SpriteRenderer>().sprite;
            Ally.GetComponent<AlliesShared>().HPScale = HPScale; 
            Ally.GetComponent<AlliesShared>().Class = "B";
            Ally.GetComponent<AlliesShared>().InitData(DataDict, ChosenModel);

        }
        
    }

    #endregion
    #region Receive healing
    public void ReceiveHealing(float HealAmount)
    {
        CurrentHP += HealAmount;
    }
    #endregion
    #region Check barrier and health
    public void CheckBarrierAndHealth()
    {
        if (!controller.isEnding)
        {
            if (CurrentHP <= 0)
            {
                if (!AlreadyDestroy)
                {
                    Camera.main.GetComponent<GameplaySoundSFXController>().GenerateSound("WSExplo", gameObject);
                    AlreadyDestroy = true;
                    if (IsEnemy)
                    {
                        FindObjectOfType<SpaceZoneMission>().EnemyWarshipDestroy();
                    }
                    else
                    {
                        if (isStation)
                        {
                            FindObjectOfType<SpaceZoneMission>().AllySpaceStationDestroy();
                        }
                        else
                            FindObjectOfType<SpaceZoneMission>().AllyWarshipDestroy();
                    }
                    StartCoroutine(DestroySelf());
                }
            }
        }
    }

    private IEnumerator DestroySelf()
    {       
        Explosion.transform.localScale = transform.localScale * 3;
        Explosion.GetComponent<SpriteRenderer>().sortingOrder = 3;
        /* GetComponent<SpriteRenderer>().color = Color.black;*/
        GetComponent<PolygonCollider2D>().enabled = false;
        GetComponent<WSMovement>().enabled = false;
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (gameObject.transform.GetChild(i).gameObject.GetComponent<Weapons>() != null)
            {
                gameObject.transform.GetChild(i).gameObject.GetComponent<Weapons>().enabled = false;
            } else
            {
                gameObject.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < 10; i++)
        {
            GameObject expl = Instantiate(Explosion, transform.position, Quaternion.identity);
            expl.SetActive(true);
            Destroy(expl, 0.6f);
            yield return new WaitForSeconds(0.05f);
            GameObject expl2 = Instantiate(Explosion, new Vector3(transform.position.x + Random.Range(100, 300), transform.position.y + Random.Range(100, 350), transform.position.z), Quaternion.identity);
            expl2.SetActive(true);
            Destroy(expl2, 0.6f);
            yield return new WaitForSeconds(0.05f);
            GameObject expl3 = Instantiate(Explosion, new Vector3(transform.position.x - Random.Range(100, 300), transform.position.y + Random.Range(100, 350), transform.position.z), Quaternion.identity);
            expl3.SetActive(true);
            Destroy(expl3, 0.6f);
            yield return new WaitForSeconds(0.05f);
            GameObject expl4 = Instantiate(Explosion, new Vector3(transform.position.x - Random.Range(100, 300), transform.position.y - Random.Range(100, 350), transform.position.z), Quaternion.identity);
            expl4.SetActive(true);
            Destroy(expl4, 0.6f);
            yield return new WaitForSeconds(0.05f);
            GameObject expl5 = Instantiate(Explosion, new Vector3(transform.position.x + Random.Range(100, 300), transform.position.y - Random.Range(100, 350), transform.position.z), Quaternion.identity);
            expl5.SetActive(true);
            Destroy(expl5, 0.6f);
        }
    

        if (IsEnemy)
        {
            // Bounty
            if (HitByPlayer || PlayerDamageReceive > MaxHP * 5 / 100f)
            {
                FindObjectOfType<GameplayInteriorController>().AddCashAndShard(BountyCash, BountyShard, gameObject);
            }
        }
        
        GenerateFlash(Flash.transform.parent.position, 0.5f, 1f);


    }
    #endregion
    #region Flash
    public void GenerateFlash(Vector2 pos, float delay, float duration)
    {

        GameObject bf = Instantiate(Flash, new Vector3(pos.x, pos.y, Flash.transform.position.z), Quaternion.identity);
        Color c = bf.GetComponent<SpriteRenderer>().color;
        c.a = 1;
        bf.GetComponent<SpriteRenderer>().color = c;
        bf.transform.SetParent(Flash.transform.parent.transform);
        bf.SetActive(true);
        GetComponent<LimitRendering>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {   
           gameObject.transform.GetChild(i).gameObject.SetActive(false);           
        }
        StartCoroutine(FlashOpen(bf, delay, duration));

    }

    private IEnumerator FlashOpen(GameObject Fade, float delay, float duration)
    {
        yield return new WaitForSeconds(delay);       
        for (int i = 0; i < 50; i++)
        {
            Color c = Fade.GetComponent<SpriteRenderer>().color;
            c.a -= 1 / 50f;
            Fade.GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(duration / 50f);
        }
        Destroy(Fade);
        Destroy(gameObject);

    }
    #endregion
    #region Check layer
    public void CheckWhichIsOnTop()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.zero);
        foreach (var x in hits )
        {
            if (x.collider.gameObject != gameObject)
            {
                if (x.collider.GetComponent<WSShared>() != null)
                {                
                    if (WSSSDict[gameObject] > WSSSDict[x.collider.gameObject])
                    {
                        Debug.Log(name + "is under" + x.collider.name);
                    } else
                    {                        
                        Debug.Log(name + "is on top" + x.collider.name);
                    }
                }
            }
        }
    }
    #endregion
    #region Check mouse
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
    #region Hazard
    private void CheckForHazard()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, (wm.HeadObject.transform.position - transform.position).magnitude, wm.HazardMask);
        if (cols.Length > 0)
        {
            foreach (var col in cols)
            {
                if (col.name.Contains("RS"))
                {
                    ReceiveTrueDamage(MaxHP * 10 / 100f, col.transform.position);
                    col.GetComponent<Collider2D>().enabled = false;
                    col.GetComponent<SpaceZoneStar>().currentHP = 0;
                }
            }
        }
    }
    #endregion
}

