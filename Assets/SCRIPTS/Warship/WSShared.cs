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
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
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
    public void InitData(Dictionary<string, object> data)
    {
        WM = GetComponent<WSMovement>();
        WsStat = new Dictionary<string, object>();
        WsStat = FindAnyObjectByType<GlobalFunctionController>().ConvertWarshipStatToDictionary(data["WarshipStat"].ToString());
        MaxHP = float.Parse(WsStat["HP"].ToString());
        BaseSpeed = float.Parse(WsStat["SPD"].ToString());
        RotationSpd = float.Parse(WsStat["ROT"].ToString());
        WM.RotateSpeed = RotationSpd;
        WM.MovingSpeed = BaseSpeed;

        //Weapon
        for (int i = 0; i < MainWeapon.Count; i++)
        {
            for (int j = 0; j < Weapons.transform.childCount; j++)
            {

            }
        }
    }
    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    #endregion
}

