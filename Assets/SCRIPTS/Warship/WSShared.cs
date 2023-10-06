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

    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public float CurrentHP;
    public float MaxHP;
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

    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        CurrentHP = MaxHP;
    }

    // Update is called once per frame
    void Update()
    {
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
        Vector2 BFScale = model.GetComponent<WarshipModelShared>().BackFireScale;
        for (int i = 0; i < BFpos.Count; i++)
        {
            GameObject game = Instantiate(BackFire, new Vector3(transform.position.x+BFpos[i].x, transform.position.y+BFpos[i].y, BackFire.transform.position.z), Quaternion.identity);
            game.transform.SetParent(gameObject.transform);
            game.transform.Rotate(new Vector3(0, 0, 180));
            game.name = "BackFire" + i;
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
                    main.transform.localScale = new Vector2(1f, 1f);
                    main.SetActive(true);
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

        List<Vector2> SupWPPos = model.GetComponent<WarshipModelShared>().SupWeaponPos;
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
                }
            }
            
        }
        gameObject.SetActive(true);
    }
    #endregion
    #region Receive Damage
    public void ReceiveBulletDamage(float Damage, GameObject Bullet)
    {
        float RealDamage = 0;
        if (Bullet.GetComponent<UsualKineticBullet>()!=null)
        {
            if (!Bullet.GetComponent<UsualKineticBullet>().isGravitationalLine)
            {
                RealDamage = Damage * 70 / 100f;
            } else
            {
                RealDamage = Damage;
            }
        }
        else
        {
            RealDamage = Damage * 5 / 100f;
        }
        if (CurrentHP>= RealDamage)
        {
            CurrentHP -= RealDamage;
        } else
        {
            CurrentHP = 0;
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
}

