using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WSShared : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
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
    private Dictionary<string, object> WsStat;
    private WSMovement WM;
    private float resetMovetimer;
    private int RandomMove;
    private int RandomRotate;
    public List<string> MainWeapon;
    public List<string> SupWeapon;
    public GameObject Weapons;
    public GameObject BackFire;
    private GameObject LeftTarget;
    private GameObject RightTarget;
    private float TargetRefreshTimer;
    private List<GameObject> SpWps;
    private List<GameObject> MainWps;
    private float FindTargetTimer;
    private bool doneInitWeapon;
    private float[] DelayTimer;
    private float DelayTimer1;
    private float ChargingTime;
    private float SpawnTimer;
    public GameObject AllyModel;
    private GameObject ChosenModel;
    public GameObject AllyTemplate;
    private int[] FightersId;
    private int Count;
    private int IDCount;
    private bool Spawned;
    private float BarrierEffectDelay;
    private float BarrierRegenTimer;
    private float BarrierRegenAmount;
    private float BarrierRegenDelay;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        CurrentHP = MaxHP;
        FightersId = new int[3] { 2, 3, 4 };
        Count = 0;
        IDCount = 0;
        SpawnTimer = 10f;
    }

    // Update is called once per frame
    void Update()
    {
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
                        DelayTimer[i] = 5f;
                    }
                }
            }

            DelayTimer1 -= Time.deltaTime;
            for (int i = 0; i < SpWps.Count; i++)
            {
                if (SpWps[i] != null)
                {                 
                   SpWps[i].GetComponent<Weapons>().WSShootBullet();                  
                }
            }
        }
            // Call function and timer only if possible
            resetMovetimer -= Time.deltaTime;
        if (resetMovetimer <= 0f)
        {
            RandomMove = Random.Range(1, 3);
            RandomRotate = Random.Range(1, 4);
            resetMovetimer = 2f;
        }

        if (RandomMove == 1)
        {
            WM.UpMove();
        }
        else if (RandomMove == 2)
        {
            WM.DownMove();
        }
        else if (RandomMove == 3)
        {
            WM.NoUpDownMove();
        }
        if (RandomRotate == 1)
        {
            WM.LeftMove();
        }
        else if (RandomRotate == 2)
        {
            WM.RightMove();
        }
        else if (RandomRotate == 3)
        {
            WM.NoLeftRightMove();

        }
        TargetRefreshTimer -= Time.deltaTime;
        if (TargetRefreshTimer <= 0f)
        {
            TargetRefreshTimer = Random.Range(2.5f, 3.5f);
            CheckTargetEnemy();

            for (int i = 0; i < SpWps.Count; i++)
            {
                if (SpWps[i] != null && SpWps[i].GetComponent<Weapons>() != null)
                {
                    SpWps[i].GetComponent<Weapons>().Aim = TargetLeftEnemy(SpWps[i]);
                }
                /*if (RightWeapon != null)
                {
                    RightWeapon.GetComponent<Weapons>().Aim = RightTarget;
                }*/
            }

            for (int i = 0; i < MainWps.Count; i++)
            {
                if (MainWps[i] != null && MainWps[i].GetComponent<Weapons>() != null)
                {
                    MainWps[i].GetComponent<Weapons>().Aim = TargetLeftEnemy(MainWps[i]);
                }
                /*if (RightWeapon != null)
                {
                    RightWeapon.GetComponent<Weapons>().Aim = RightTarget;
                }*/
            }

        }
        if (MainWps[0].GetComponent<Weapons>() == null)
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
                Animation.SetBool("isSpawn", true);
                SpawnFighter(FightersId[IDCount], Count);
                Spawned = true;
                IDCount++;
                Count++;
                SpawnTimer = 10f;
                if (IDCount == 3)
                {
                    IDCount = 0;
                }
               
            }
        }

        //Check barrier and regen barrier
        BarrierRegenTimer -= Time.deltaTime;
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

        }
        /*if (LeftTarget == null || RightTarget == null)
        {
            FindTargetTimer -= Time.deltaTime;
        }
        if (FindTargetTimer <= 0f)
        {
            FindTargetTimer = Random.Range(2.5f, 3.5f);
            if (LeftTarget == null)
            {
                //TargetLeftEnemy(1000);
            }
            if (RightTarget == null)
            {
                TargetRightEnemy();
            }
        }*/

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
        for (int i = 0; i < MainWeapon.Count; i++)
        {
            for (int j = 0; j < Weapons.transform.childCount; j++)
            {
                //Find model
                if (MainWeapon[i].Replace(" ", "").ToLower() == Weapons.transform.GetChild(j).name.Replace(" ","").ToLower()) 
                {
                    GameObject main = Instantiate(Weapons.transform.GetChild(j).gameObject, new Vector3(transform.position.x + WPPos[i].x, transform.position.y + WPPos[i].y, Weapons.transform.GetChild(i).position.z), Quaternion.identity);
                    main.transform.SetParent(gameObject.transform);
                    main.SetActive(true);

                    if (MainWeapon[i] != "CarrierHatch")
                    {
                        main.transform.localScale = new Vector2(0.5f, 0.5f);
                        Weapons wp = main.GetComponent<Weapons>();
                        wp.Fighter = gameObject;
                        wp.Aim = LeftTarget;
                        wp.EnemyLayer = MainWeaponTarget;
                        wp.tracking = true;
                        wp.isMainWeapon = true;
                        wp.RotateLimitPositive = 180;
                        wp.RotateLimitNegative = -180;
                    } else
                    {
                        main.transform.localScale = new Vector2(0.25f, 0.25f);
                    }
                    MainWps.Add(main);


                }
            }           
        }

        DelayTimer = new float[MainWps.Count];

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

        List<Vector2> SupWPPos = model.GetComponent<WarshipModelShared>().SupWeaponPos;
        //TargetLeftEnemy(1000);
        /*TargetRightEnemy();*/
        SpWps = new List<GameObject>();
        for (int i = 0; i < SupWeapon.Count; i++)
        {
            for (int j = 0; j < Weapons.transform.childCount; j++)
            {
                //Find model
                if (SupWeapon[i].Replace(" ", "").ToLower() == Weapons.transform.GetChild(j).name.Replace(" ", "").ToLower())
                {
                    GameObject sup = Instantiate(Weapons.transform.GetChild(j).gameObject, new Vector3(transform.position.x + SupWPPos[i].x, transform.position.y + SupWPPos[i].y, Weapons.transform.GetChild(i).position.z), Quaternion.identity);
                    sup.transform.SetParent(gameObject.transform);
                    sup.transform.localScale = new Vector2(0.5f, 0.5f);
                    sup.SetActive(true);


                    Weapons wp = sup.GetComponent<Weapons>();
                    wp.Fighter = gameObject;
                    wp.Aim = LeftTarget;
                    wp.EnemyLayer = SupWeaponTarget;
                    wp.tracking = true;
                    wp.RotateLimitPositive = 360;
                    wp.RotateLimitNegative = -360;
                    wp.isMainWeapon = false;

                    SpWps.Add(sup);
                }
            }
            
        }
        doneInitWeapon = true;
        gameObject.SetActive(true);
    }
    #endregion
    #region Receive Damage
    public void ReceiveBulletDamage(float Damage, GameObject Bullet)
    {
        float RealDamage = 0;
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
            if (CurrentBarrier > RealDamage)
            {
                CurrentBarrier -= RealDamage;
                if (BarrierEffectDelay <= 0f)
                {
                    BarrierEffectDelay = 0.25f;
                    GameObject br = Instantiate(Barrier, transform.position, Quaternion.identity);
                    br.transform.localScale = gameObject.transform.localScale * 4f;
                    br.SetActive(true);
                    br.transform.SetParent(transform);
                    Destroy(br, 0.25f);
                }
                BarrierRegenTimer = 90f;
                BarrierRegenAmount = 2500f;
            }
            else
            {
                CurrentBarrier = 0;
                if (BarrierEffectDelay <= 0f)
                {
                    BarrierEffectDelay = 0.25f;
                    GameObject br = Instantiate(Barrier, transform.position, Quaternion.identity);
                    br.transform.localScale = gameObject.transform.localScale * 4f;
                    br.SetActive(true);
                    br.transform.SetParent(transform);
                    Destroy(br, 0.25f);
                }
                BarrierRegenTimer = 90f;
                BarrierRegenAmount = 2500f;
                CurrentHP -= (RealDamage - CurrentBarrier);
            }
        }
        else
        {
            if (BarrierEffectDelay <= 0f)
            {
                BarrierEffectDelay = 0.25f;
                GameObject BRBreak = Instantiate(BarrierBreak, transform.position, Quaternion.identity);
                BRBreak.transform.localScale = gameObject.transform.localScale * 4f;
                BRBreak.SetActive(true);
                BRBreak.transform.SetParent(transform);
                Destroy(BRBreak, 0.5f);
            }
            if (CurrentHP >= RealDamage)
            {
                CurrentHP -= RealDamage;
            }
            else
            {
                CurrentHP = 0;
            }
        }
    }

    public void ReceivePowerDamage(float Damage)
    {
        float RealDamage = Damage * 50f / 100;
        if (CurrentHP>=RealDamage)
        {
            CurrentHP -= RealDamage;
        } else
        {
            CurrentHP = 0;
        }
    }
    #endregion
    #region Target
    private GameObject TargetLeftEnemy(GameObject weapon)
    {
        GameObject game = null;
        BulletShared bul = weapon.GetComponent<Weapons>().Bullet.GetComponent<BulletShared>();
        Collider2D[] cols = Physics2D.OverlapCircleAll(weapon.transform.position, bul.MaxEffectiveDistance, (weapon.GetComponent<Weapons>().isMainWeapon == true ? MainWeaponTarget : SupWeaponTarget));
        if (cols.Length > 0)
        {
            GameObject Nearest = cols[0].gameObject;
            float distance = Mathf.Abs((cols[0].transform.position - weapon.transform.position).magnitude);
            foreach (var enemy in cols)
            {
                float distanceTest = Mathf.Abs((enemy.gameObject.transform.position - weapon.transform.position).magnitude);
                if (distanceTest < distance)
                {
                    distance = distanceTest;
                    Nearest = enemy.gameObject;
                }
            }
            LeftTarget = Nearest;
            game = Nearest;
        }
        return game;
    }

    private void TargetRightEnemy()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 1000f, FindObjectOfType<GameController>().PlayerLayer);
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
        for (int i = 0; i < SpWps.Count; i++)
        {
            if (LeftTarget != null && (Mathf.Abs((LeftTarget.transform.position - transform.position).magnitude) > 800f || LeftTarget.layer == LayerMask.NameToLayer("Untargetable")))
            {
                LeftTarget = null;
                SpWps[i].GetComponent<Weapons>().Aim = null;
            }
        }
        /*if (RightTarget != null && (Mathf.Abs((RightTarget.transform.position - transform.position).magnitude) > TargetRange || RightTarget.layer == LayerMask.NameToLayer("Untargetable")))
        {
            RightTarget = null;
            RightWeapon.GetComponent<Weapons>().Aim = null;
        }*/
    }
    #endregion
    #region Spawn

    public void SpawnFighter(int id, int count)
    {
        Dictionary<string, object> DataDict;
        if (gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            DataDict = FindObjectOfType<AccessDatabase>().GetDataEnemyById(id);
        } else
        {
            DataDict = FindObjectOfType<AccessDatabase>().GetDataAlliesById(id);
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
        Ally.name = ChosenModel.name + " |" + MainWps[0].transform.position.x + " - " + MainWps[0].transform.position.y + " - " + count;
        Ally.GetComponent<SpriteRenderer>().sprite = ChosenModel.GetComponent<SpriteRenderer>().sprite;
        Ally.GetComponent<AlliesShared>().HPScale = (gameObject.layer == LayerMask.NameToLayer("Enemy") ? FindObjectOfType<SpaceZoneGenerator>().EnemyMaxHP : FindObjectOfType<SpaceZoneGenerator>().AllyMaxHP);
        Ally.GetComponent<AlliesShared>().InitData(DataDict, ChosenModel);
        
    }

    #endregion
}

