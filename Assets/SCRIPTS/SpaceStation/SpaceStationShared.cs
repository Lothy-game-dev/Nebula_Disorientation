using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using UnityEngine;

public class SpaceStationShared : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject StatusBoard;
    public GameObject Barrier;
    public GameObject BarrierBreak;
    public GameObject HealEffect;
    public GameObject Explosion;
    public GameObject Flash;
    public SpaceStationHealthBar HPBar;
    public SpaceStationHealthBar ShieldBar;
    public NNModel MLBrain; 
    public bool isEnemy;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public float CurrentHP;
    public float MaxHP;
    public float CurrentBarrier;
    public float MaxBarrier;
    public float AuraRange;
    public List<string> MainWeapon;
    public List<string> SupWeapon;
    public GameObject Weapons;
    private GameObject Target;
    private float TargetRefreshTimer;
    private List<GameObject> SpWps;
    private List<GameObject> MainWps;
    private float FindTargetTimer;
    private bool doneInitWeapon;
    private float DelayTimer;
    public LayerMask MainWeaponTarget;
    public LayerMask SupWeaponTarget;
    public LayerMask HealTarget;
    private float ResetHealTimer;
    private StatusBoard Status;
    private float BarrierEffectDelay;
    private float BarrierRegenTimer;
    private float BarrierRegenAmount;
    private float BarrierRegenDelay;
    private bool AlreadyDestroy;
    public Dictionary<GameObject, int> WSSSDict;
    public Dictionary<GameObject, GameObject> SpWeaponTargets;
    public int Order;
    public bool isHit;
    private bool HitByPlayer;
    private int BountyCash;
    private int BountyShard;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        FindAnyObjectByType<WSSSDetected>().DectectWSSS();
        WSSSDict = FindAnyObjectByType<WSSSDetected>().PrioritizeDict;
        Order = WSSSDict[gameObject];
        ShieldBar.SetValue(CurrentBarrier, MaxBarrier, true);
        HPBar.SetValue(CurrentHP, MaxHP, true);
        if (GetComponent<AudioSource>() != null)
        {
            AudioSource aus = gameObject.AddComponent<AudioSource>();
            aus.spatialBlend = 1;
            aus.rolloffMode = AudioRolloffMode.Linear;
            aus.maxDistance = 2000;
            aus.minDistance = 1000;
            aus.priority = 256;
            aus.dopplerLevel = 0;
            aus.spread = 360;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (doneInitWeapon)
        {
            DelayTimer -= Time.deltaTime;
            for (int i = 0; i < MainWps.Count; i++)
            {
                if (MainWps[i] != null)
                {
                    if (DelayTimer <= 0f)
                    {
                        MainWps[i].GetComponent<Weapons>().isCharging = true;
                        MainWps[i].GetComponent<Weapons>().Fireable = true;
                        DelayTimer = MainWps[i].GetComponent<Weapons>().Cooldown;
                    }
                }
            }

            for (int i = 0; i < SpWps.Count; i++)
            {
                if (SpWps[i] != null)
                {
                    SpWps[i].GetComponent<Weapons>().WSShootBullet();
                }
            }
        }
        //Reset Target
        TargetRefreshTimer -= Time.deltaTime;
        if (TargetRefreshTimer <= 0f)
        {
            TargetRefreshTimer = Random.Range(0.5f, 1f);

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
                    MainWps[i].GetComponent<Weapons>().Aim = Target;
                }
            }

        }


        if (Target == null || SpWeaponTargets.ContainsValue(null))
        {
            FindTargetTimer -= Time.deltaTime;
        }
        if (FindTargetTimer <= 0f)
        {
            FindTargetTimer = Random.Range(0.5f, 1f);
            for (int i = 0; i < MainWps.Count; i++)
            {
                MainWeaponTargetEnemy(MainWps[i]);
            }

            for (int i = 0; i < SpWps.Count; i++)
            {
                SpWeaponTargets[SpWps[i]] = TargetEnemy(SpWps[i]);
            }

        }

        CheckBarrierAndHealth();
        

    }
    private void FixedUpdate()
    {
        ResetHealTimer -= Time.fixedDeltaTime;
        if (ResetHealTimer <= 0f)
        {
            HealTheAlly();
            ResetHealTimer = 1f;
        }
    }
    #endregion
    #region Init Data
    // Group all function that serve the same algorithm
    public void InitData(GameObject model, Dictionary<string, object> data)
    {
        HealEffect.SetActive(true);
        MaxHP = float.Parse(data["BaseHP"].ToString());
        AuraRange = float.Parse(data["AuraRange"].ToString());
        CurrentHP = MaxHP;
        if (isEnemy)
        {
            string Bounty = (string)data["Bounty"];
            BountyCash = int.Parse(Bounty.Split("|")[0]);
            BountyShard = int.Parse(Bounty.Split("|")[1]);
        }

        /*HPBar.SetPostion(new Vector3(transform.position.x + model.GetComponent<SpaceStationModelShared>().HPBarPosition.x, transform.position.y + model.GetComponent<SpaceStationModelShared>().HPBarPosition.y, transform.position.z));*/ 
        //Main Weapon
        if (data["MainWeapon"].ToString().Contains("|"))
        {
            string[] weapons = data["MainWeapon"].ToString().Split("|");
            for (int i = 0; i < weapons.Length; i++)
            {
                MainWeapon.Add(weapons[i]);
            }
        }
        else
        {
            MainWeapon.Add(data["MainWeapon"].ToString());
        }

        MainWps = new List<GameObject>();
        Vector2 WPPos = model.GetComponent<SpaceStationModelShared>().MainWeaponPosition;
        for (int i = 0; i < MainWeapon.Count; i++)
        {
            for (int j = 0; j < Weapons.transform.childCount; j++)
            {
                //Find model
                if (Weapons.transform.GetChild(j).name.Replace(" ", "").ToLower().Contains(MainWeapon[i].Replace(" ", "").ToLower()))
                {
                    GameObject main = Instantiate(Weapons.transform.GetChild(j).gameObject, new Vector3(transform.position.x + WPPos.x, transform.position.y + WPPos.y, Weapons.transform.GetChild(i).position.z), Quaternion.identity);
                    main.transform.SetParent(gameObject.transform);
                    main.transform.localScale = new Vector2(0.5f, 0.5f);
                    main.SetActive(true);

                    Weapons wp = main.GetComponent<Weapons>();
                    wp.Fighter = gameObject;
                    wp.EnemyLayer = MainWeaponTarget;
                    wp.tracking = true;
                    wp.isMainWeapon = true;
                    wp.isSpaceStation = true;
                    MainWps.Add(main);
                }
            }
        }


        //Sup Weapon
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

        List<Vector2> SupWPPos = model.GetComponent<SpaceStationModelShared>().SupportWeaponPosition;
        SpWps = new List<GameObject>();
        for (int i = 0; i < SupWeapon.Count; i++)
        {
            for (int j = 0; j < Weapons.transform.childCount; j++)
            {
                //Find model
                if (Weapons.transform.GetChild(j).name.Replace(" ", "").ToLower().Contains(SupWeapon[i].Replace(" ", "").ToLower()))
                {
                    GameObject sup = Instantiate(Weapons.transform.GetChild(j).gameObject, new Vector3(transform.position.x + SupWPPos[i].x, transform.position.y + SupWPPos[i].y, Weapons.transform.GetChild(i).position.z), Quaternion.identity);
                    sup.transform.SetParent(gameObject.transform);
                    sup.transform.localScale = new Vector2(0.5f, 0.5f);
                    sup.SetActive(true);


                    Weapons wp = sup.GetComponent<Weapons>();
                    wp.Fighter = gameObject;
                    wp.EnemyLayer = SupWeaponTarget;
                    wp.tracking = true;
                    wp.isMainWeapon = false;
                    wp.isSpaceStation = true;

                    /*sup.AddComponent<BehaviorParameters>();
                    sup.GetComponent<BehaviorParameters>().BrainParameters.VectorObservationSize = 4;

                    sup.GetComponent<BehaviorParameters>().BehaviorType = BehaviorType.InferenceOnly;
                    sup.GetComponent<BehaviorParameters>().Model = MLBrain;
                    sup.GetComponent<BehaviorParameters>().BehaviorName = "AllyFire23091012";
                    ActionSpec act = sup.GetComponent<BehaviorParameters>().BrainParameters.ActionSpec;
                    act.NumContinuousActions = 1;
                    act.BranchSizes = null;
                    sup.GetComponent<BehaviorParameters>().BrainParameters.ActionSpec = act;
                    sup.AddComponent<WSSupportWeaponMLAgent>();
                    sup.AddComponent<DecisionRequester>();*/
                    SpWps.Add(sup);
                }
            }

        }
        SpWeaponTargets = new Dictionary<GameObject, GameObject>();             
        for (int i = 0; i < SpWps.Count; i++)
        {
            SpWeaponTargets.Add(SpWps[i], TargetEnemy(SpWps[i]));
        }
        for (int i = 0; i < MainWps.Count; i++)
        {
            MainWeaponTargetEnemy(MainWps[i]);
        }
        doneInitWeapon = true;
        gameObject.SetActive(true);

    }
    
    #endregion
    #region Target
    // Group all function that serve the same algorithm
    private GameObject TargetEnemy(GameObject weapon)
    {        
        GameObject game = null;
        BulletShared bul = weapon.GetComponent<Weapons>().Bullet.GetComponent<BulletShared>();
        Collider2D[] cols = Physics2D.OverlapCircleAll(weapon.transform.position, bul.MaxEffectiveDistance, (weapon.GetComponent<Weapons>().isMainWeapon == true ? MainWeaponTarget : SupWeaponTarget));
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
                        float distanceTest = Mathf.Abs((enemy.transform.position - weapon.transform.position).magnitude);
                        if (distanceTest < distance)
                        {
                            distance = distanceTest;
                            Nearest = enemy.gameObject;
                        }
                    }
                    else
                    {
                        continue;
                    }
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
            game = Nearest;
        }
        return game;
    }

    private void CheckTargetEnemy(GameObject weapon)
    {
        if (!weapon.GetComponent<Weapons>().isMainWeapon)
        {
            BulletShared bul = weapon.GetComponent<Weapons>().Bullet.GetComponent<BulletShared>();
            for (int i = 0; i < SpWeaponTargets.Count; i++)
            {
                if (SpWeaponTargets[weapon] != null && (Mathf.Abs((SpWeaponTargets[weapon].transform.position - weapon.transform.position).magnitude) > bul.MaxEffectiveDistance || SpWeaponTargets[weapon].layer == LayerMask.NameToLayer("Untargetable")))
                {
                    weapon.GetComponent<Weapons>().Aim = null;
                    SpWeaponTargets[weapon] = null;
                }
            }
        }
        else
        {
            BulletShared bul = weapon.GetComponent<Weapons>().Bullet.GetComponent<BulletShared>();           
            if (Target != null && (Mathf.Abs((Target.transform.position - weapon.transform.position).magnitude) > bul.MaxEffectiveDistance || Target.layer == LayerMask.NameToLayer("Untargetable")))
            {
                weapon.GetComponent<Weapons>().Aim = null;
                Target = null;
            }
            
        }
    }

    public GameObject MainWeaponTargetEnemy(GameObject weapon)
    {
        GameObject game = null;
        BulletShared bul = weapon.GetComponent<Weapons>().Bullet.GetComponent<BulletShared>();
        Collider2D[] cols = Physics2D.OverlapCircleAll(weapon.transform.position, bul.MaxEffectiveDistance, (weapon.GetComponent<Weapons>().isMainWeapon == true ? MainWeaponTarget : SupWeaponTarget));
        if (cols.Length > 0)
        {
            GameObject Nearest = null;
            foreach (var enemy in cols)
            {
                // If weapon is main weap, target enemy ws/ss only
                if (weapon.GetComponent<Weapons>().isMainWeapon)
                {
                    if (enemy.gameObject.tag == "BossEnemy" || enemy.gameObject.tag == "AlliesBossFighter")
                    {
                        Nearest = enemy.gameObject;
                        float distance = Mathf.Abs((enemy.transform.position - weapon.transform.position).magnitude);
                        float distanceTest = Mathf.Abs((enemy.gameObject.transform.position - weapon.transform.position).magnitude);
                        if (distanceTest < distance)
                        {
                            distance = distanceTest;
                            Nearest = enemy.gameObject;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

            }
            game = Nearest;
            Target = game;
        }
        return game;
    }
    #endregion
    #region Heal ally
    public void HealTheAlly()
    {
   
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 700f, HealTarget);
        if (cols.Length > 0)
        {
            foreach (var ally in cols)
            {
                if (ally.gameObject != gameObject)
                {
                    if (ally.GetComponent<FighterShared>() != null)
                    {
                        float MaxHP = ally.GetComponent<FighterShared>().MaxHP;
                        float CurrentHP = ally.GetComponent<FighterShared>().CurrentHP;
                        if (CurrentHP < MaxHP)
                        {
                            ally.GetComponent<FighterShared>().ReceiveHealing(MaxHP * 1/100);
                        } else
                        {
                            ally.GetComponent<FighterShared>().CurrentHP = MaxHP;
                        }
                    } else
                    {
                        if (ally.GetComponent<WSShared>() != null)
                        {
                            float MaxWSHP = ally.GetComponent<WSShared>().MaxHP;
                            float CurrentWSHP = ally.GetComponent<WSShared>().CurrentHP;
                            if (CurrentHP < MaxHP)
                            {
                                ally.GetComponent<WSShared>().ReceiveHealing(MaxWSHP * 1 / 100);
                            }
                            else
                            {
                                ally.GetComponent<WSShared>().CurrentHP = CurrentWSHP;
                            }
                        }
                    }
                }
            }
        }

        
    }
    #endregion
    #region check mouse
    /*private void OnMouseOver()
    {
        Status.Timer = 5f;
        Status.StartShowing(gameObject);
    }
    private void OnMouseExit()
    {
        Status.CheckOnDestroy();
    }*/
    #endregion
    #region Receive Damage
    public void ReceiveBombingDamage(float Damage, Vector2 BulletHitPos)
    {
        float RealDamage = Damage;
        if (CurrentBarrier > 0)
        {
            Camera.main.GetComponent<GameplaySoundSFXController>().GenerateSound("BarrierHit", gameObject);
            if (CurrentBarrier > RealDamage)
            {
                CurrentBarrier -= RealDamage;
                ShieldBar.SetValue(CurrentBarrier, MaxBarrier, true);
                if (BarrierEffectDelay <= 0f)
                {
                    BarrierEffectDelay = 0.25f;
                    GameObject br = Instantiate(Barrier, BulletHitPos, Quaternion.identity);
                    br.SetActive(true);
                    br.transform.SetParent(transform);
                    Destroy(br, 0.25f);
                }
                BarrierRegenTimer = 90f;
                BarrierRegenAmount = 2500f;
            }
            else
            {
                float finalDamage = (RealDamage - CurrentBarrier);
                CurrentBarrier = 0;
                ShieldBar.SetValue(CurrentBarrier, MaxBarrier, true);
                if (BarrierEffectDelay <= 0f)
                {
                    BarrierEffectDelay = 0.25f;
                    GameObject br = Instantiate(Barrier, BulletHitPos, Quaternion.identity);
                    br.SetActive(true);
                    br.transform.SetParent(transform);
                    Destroy(br, 0.25f);
                }
                BarrierRegenTimer = 90f;
                BarrierRegenAmount = 2500f;
                CurrentHP -= finalDamage;
                HPBar.SetValue(CurrentHP, MaxHP, true);
            }
        }
        else
        {
            Camera.main.GetComponent<GameplaySoundSFXController>().GenerateSound("WSSSHit", gameObject);
            if (BarrierEffectDelay <= 0f)
            {
                BarrierEffectDelay = 0.25f;
                GameObject BRBreak = Instantiate(BarrierBreak, BulletHitPos, Quaternion.identity);
                BRBreak.SetActive(true);
                BRBreak.transform.SetParent(transform);
                Destroy(BRBreak, 0.5f);
            }
            if (CurrentHP >= RealDamage)
            {
                CurrentHP -= RealDamage;
                HPBar.SetValue(CurrentHP, MaxHP, true);
            }
            else
            {
                CurrentHP = 0;
            }
        }
    }
    public void ReceiveBulletDamage(float Damage, GameObject Bullet, Vector2 BulletHitPos)
    {
        float RealDamage = 0;
        if (Bullet.GetComponent<BulletShared>().WeaponShoot.GetComponent<Weapons>().Fighter.GetComponent<PlayerFighter>()!=null)
        {
            HitByPlayer = true;
        }
        if (Bullet.GetComponent<UsualKineticBullet>() != null)
        {
            if (!Bullet.GetComponent<UsualKineticBullet>().isGravitationalLine)
            {
                RealDamage = Damage * 70 / 100f;
            }
            else
            {
                RealDamage = Damage;
            }
        }
        else
        {
            RealDamage = Damage * 5 / 100f;
        }
        if (CurrentBarrier > 0)
        {
            Camera.main.GetComponent<GameplaySoundSFXController>().GenerateSound("BarrierHit", gameObject);
            if (CurrentBarrier > RealDamage)
            {
                CurrentBarrier -= RealDamage;
                ShieldBar.SetValue(CurrentBarrier, MaxBarrier, true);
                if (BarrierEffectDelay <= 0f)
                {
                    BarrierEffectDelay = 0.25f;
                    GameObject br = Instantiate(Barrier, BulletHitPos, Quaternion.identity);
                    br.SetActive(true);
                    br.transform.SetParent(transform);
                    Destroy(br, 0.25f);
                }
                BarrierRegenTimer = 90f;
                BarrierRegenAmount = 2500f;
            } else
            {
                float finalDamage = (RealDamage - CurrentBarrier);
                CurrentBarrier = 0;
                ShieldBar.SetValue(CurrentBarrier, MaxBarrier, true);
                if (BarrierEffectDelay <= 0f)
                {
                    BarrierEffectDelay = 0.25f;
                    GameObject br = Instantiate(Barrier, BulletHitPos, Quaternion.identity);
                    br.SetActive(true);
                    br.transform.SetParent(transform);
                    Destroy(br, 0.25f);
                }
                BarrierRegenTimer = 90f;
                BarrierRegenAmount = 2500f;
                CurrentHP -= finalDamage;
                HPBar.SetValue(CurrentHP, MaxHP, true);
            }
        }
        else
        {
            Camera.main.GetComponent<GameplaySoundSFXController>().GenerateSound("WSSSHit", gameObject);
            if (BarrierEffectDelay <= 0f)
            {
                BarrierEffectDelay = 0.25f;
                GameObject BRBreak = Instantiate(BarrierBreak, BulletHitPos, Quaternion.identity);
                BRBreak.SetActive(true);
                BRBreak.transform.SetParent(transform);
                Destroy(BRBreak, 0.5f);
            }
            if (CurrentHP >= RealDamage)
            {
                CurrentHP -= RealDamage;
                HPBar.SetValue(CurrentHP, MaxHP, true);
            }
            else
            {
                CurrentHP = 0;
            }
        }
    }

    public void ReceivePowerDamage(float Damage, GameObject FighterCast, Vector2 BulletHitPos)
    {
        float RealDamage = Damage * 50f / 100;
        if (FighterCast.GetComponent<PlayerFighter>()!=null)
        {
            HitByPlayer = true;
        }
        if (CurrentBarrier > 0)
        {
            Camera.main.GetComponent<GameplaySoundSFXController>().GenerateSound("BarrierHit", gameObject);
            if (CurrentBarrier > RealDamage)
            {
                CurrentBarrier -= RealDamage;
                ShieldBar.SetValue(CurrentBarrier, MaxBarrier, true);
                if (BarrierEffectDelay <= 0f)
                {
                    BarrierEffectDelay = 0.25f;
                    GameObject br = Instantiate(Barrier, BulletHitPos, Quaternion.identity);
                    br.SetActive(true);
                    br.transform.SetParent(transform);
                    Destroy(br, 0.25f);
                }
                BarrierRegenTimer = 120f;
                BarrierRegenAmount = 2500f;
            }
            else
            {
                float afterDamage = (RealDamage - CurrentBarrier);
                CurrentBarrier = 0;
                ShieldBar.SetValue(CurrentBarrier, MaxBarrier, true);
                if (BarrierEffectDelay <= 0f)
                {
                    BarrierEffectDelay = 0.25f;
                    GameObject br = Instantiate(Barrier, BulletHitPos, Quaternion.identity);
                    br.SetActive(true);
                    br.transform.SetParent(transform);
                    Destroy(br, 0.25f);
                }
                BarrierRegenTimer = 120f;
                BarrierRegenAmount = 2500f;
                CurrentHP -= afterDamage;
                HPBar.SetValue(CurrentHP, MaxHP, true);
            }
        }
        else
        {
            Camera.main.GetComponent<GameplaySoundSFXController>().GenerateSound("WSSSHit", gameObject);
            if (BarrierEffectDelay <= 0f)
            {
                BarrierEffectDelay = 0.25f;
                GameObject BRBreak = Instantiate(BarrierBreak, BulletHitPos, Quaternion.identity);
                BRBreak.SetActive(true);
                BRBreak.transform.SetParent(transform);
                Destroy(BRBreak, 0.5f);
            }
            if (CurrentHP >= RealDamage)
            {
                CurrentHP -= RealDamage;
                isHit = true;
                HPBar.SetValue(CurrentHP, MaxHP, isHit);
            }
            else
            {
                CurrentHP = 0;
            }
        }
    }
    #endregion
    #region Check barrier and health
    public void CheckBarrierAndHealth()
    {
        //Check barrier and regen barrier
        /*BarrierRegenTimer -= Time.deltaTime;
        BarrierRegenDelay -= Time.deltaTime;
        BarrierEffectDelay -= Time.deltaTime;
        if (BarrierRegenTimer <= 0f)
        {
            if (BarrierRegenDelay <= 0f && CurrentBarrier < MaxBarrier)
            {
                if (CurrentBarrier <= MaxBarrier - BarrierRegenAmount)
                {
                    CurrentBarrier += BarrierRegenAmount;
                    BarrierRegenDelay = 1f;
                }
                else
                {
                    CurrentBarrier = MaxBarrier;
                    BarrierRegenDelay = 1f;
                }
            }

        }*/

        if (CurrentHP <= 0)
        {
            if (!AlreadyDestroy)
            {
                Camera.main.GetComponent<GameplaySoundSFXController>().GenerateSound("SSExplo", gameObject);
                AlreadyDestroy = true;
                StartCoroutine(DestroySelf());

            }
        }
    }

    private IEnumerator DestroySelf()
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }
        Explosion.transform.localScale = transform.localScale / 1.5f;
        Explosion.GetComponent<SpriteRenderer>().sortingOrder = 3;
       /* GetComponent<SpriteRenderer>().color = Color.black;*/
        GetComponent<PolygonCollider2D>().enabled = false;
        for (int i = 0; i < 10; i++)
        {
            GameObject expl = Instantiate(Explosion, transform.position, Quaternion.identity);
            expl.SetActive(true);
            Destroy(expl, 0.3f);
            yield return new WaitForSeconds(0.05f);
            GameObject expl2 = Instantiate(Explosion, new Vector3(transform.position.x + Random.Range(100, 700), transform.position.y + Random.Range(100, 700), transform.position.z), Quaternion.identity);
            expl2.SetActive(true);
            Destroy(expl2, 0.3f);
            yield return new WaitForSeconds(0.05f);
            GameObject expl3 = Instantiate(Explosion, new Vector3(transform.position.x - Random.Range(100, 700), transform.position.y + Random.Range(100, 700), transform.position.z), Quaternion.identity);
            expl3.SetActive(true);
            Destroy(expl3, 0.3f);
            yield return new WaitForSeconds(0.05f);
            GameObject expl4 = Instantiate(Explosion, new Vector3(transform.position.x - Random.Range(100, 700), transform.position.y - Random.Range(100, 700), transform.position.z), Quaternion.identity);
            expl4.SetActive(true);
            Destroy(expl4, 0.3f);
            yield return new WaitForSeconds(0.05f);
            GameObject expl5 = Instantiate(Explosion, new Vector3(transform.position.x + Random.Range(100, 700), transform.position.y - Random.Range(100, 700), transform.position.z), Quaternion.identity);
            expl5.SetActive(true);
            Destroy(expl5, 0.3f);
        }
        if (isEnemy)
        {
            if (HitByPlayer)
            {
                FindObjectOfType<GameplayInteriorController>().AddCashAndShard(BountyCash, BountyShard);
            }
            FindObjectOfType<SpaceZoneMission>().EnemySpaceStationDestroy();
        } else
        {
            FindObjectOfType<SpaceZoneMission>().AllySpaceStationDestroy();
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
        bf.transform.SetParent(Flash.transform.parent);
        bf.SetActive(true);
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
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 1f);
        Destroy(Fade);
        
    }
    #endregion
}
