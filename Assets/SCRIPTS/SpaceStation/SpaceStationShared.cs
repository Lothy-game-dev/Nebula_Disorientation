using System.Collections;
using System.Collections.Generic;
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
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public float CurrentHP;
    public float MaxHP;
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
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        Status = StatusBoard.GetComponent<StatusBoard>(); 
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
                        DelayTimer = 5f;
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
        TargetRefreshTimer -= Time.deltaTime;
        if (TargetRefreshTimer <= 0f)
        {
            TargetRefreshTimer = Random.Range(2.5f, 3.5f);
            //CheckTargetEnemy();

            for (int i = 0; i < SpWps.Count; i++)
            {
                CheckTargetEnemy(SpWps[i], Target);
                if (SpWps[i] != null)
                {
                    SpWps[i].GetComponent<Weapons>().Aim = TargetEnemy(SpWps[i]);
                }
                /*if (RightWeapon != null)
                {
                    RightWeapon.GetComponent<Weapons>().Aim = RightTarget;
                }*/
            }

            for (int i = 0; i < MainWps.Count; i++)
            {
                CheckTargetEnemy(MainWps[i], Target);
                if (MainWps[i] != null)
                {
                    MainWps[i].GetComponent<Weapons>().Aim = TargetEnemy(MainWps[i]);
                }
                /*if (RightWeapon != null)
                {
                    RightWeapon.GetComponent<Weapons>().Aim = RightTarget;
                }*/
            }

        }
       
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
        MaxHP = float.Parse(data["BaseHP"].ToString());
        AuraRange = float.Parse(data["AuraRange"].ToString());
        CurrentHP = MaxHP;
        GetComponent<CapsuleCollider2D>().size = model.GetComponent<SpaceStationModelShared>().Size;
        GetComponent<CapsuleCollider2D>().offset = model.GetComponent<SpaceStationModelShared>().Offset;
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
                if (MainWeapon[i].Replace(" ", "").ToLower() == Weapons.transform.GetChild(j).name.Replace(" ", "").ToLower())
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
                if (SupWeapon[i].Replace(" ", "").ToLower() == Weapons.transform.GetChild(j).name.Replace(" ", "").ToLower())
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
                    SpWps.Add(sup);
                }
            }

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
            game = Nearest;
            Target = game;
        }
        return game;
    }

    private void CheckTargetEnemy(GameObject weapon, GameObject target)
    {
        BulletShared bul = weapon.GetComponent<Weapons>().Bullet.GetComponent<BulletShared>();
        for (int i = 0; i < SpWps.Count; i++)
        {
            if (target != null && (Mathf.Abs((target.transform.position - weapon.transform.position).magnitude) > bul.MaxEffectiveDistance || target.layer == LayerMask.NameToLayer("Untargetable")))
            {
                target = null;
                weapon.GetComponent<Weapons>().Aim = null;
            }
        }
    }
    #endregion
    #region Heal ally
    public void HealTheAlly()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 300f, HealTarget);
        if (cols.Length > 0)
        {
            foreach (var ally in cols)
            {
                if (ally.gameObject != gameObject)
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
                }
            }
        }
    }
    #endregion
    #region check mouse
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
}
