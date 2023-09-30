using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    private AudioSource sound;
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public LayerMask EnemyLayer;
    public GameObject Effect;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public GameObject Fighter;
    private Dictionary<string, object> Power;
    private Dictionary<string, object> PowerStats;
    public float DPH;
    public int AoH;
    public float AoE;
    public float Velocity;
    public float Range;
    public float Duration;
    public float CD;
    public float BR;
    public float BRx;
    public AudioClip SoundEffect;
    public AudioClip ChargingSoundEffect;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        /*InitData();*/
        
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
       

    }
    #endregion
    #region Init Data
    public void InitData(string name)
    {
        //Get power stat from DB
        Power = FindAnyObjectByType<AccessDatabase>().GetPowerDataByName(name.Replace("(Clone)", ""));
        PowerStats = FindAnyObjectByType<GlobalFunctionController>().ConvertPowerStatsToDictionary(Power["Stats"].ToString());
        switch (Power["Type"].ToString())
        {
            case "Defensive":
                if (PowerStats.ContainsKey("BR"))
                {
                    BR = float.Parse(PowerStats["BR"].ToString());
                }              
                Duration = float.Parse(PowerStats["Dur"].ToString());
                CD = float.Parse(PowerStats["CD"].ToString());
                if (PowerStats.ContainsKey("BRx"))
                {
                    BRx = float.Parse(PowerStats["BRx"].ToString());
                } break;
            case "Offensive":
                DPH = float.Parse(PowerStats["DPH"].ToString());
                AoH = int.Parse(PowerStats["AOH"].ToString());
                AoE = float.Parse(PowerStats["AOE"].ToString());
                Velocity = float.Parse(PowerStats["V"].ToString());
                Range = float.Parse(PowerStats["R"].ToString());
                Duration = float.Parse(PowerStats["Dur"].ToString());
                CD = float.Parse(PowerStats["CD"].ToString());
                break;
            case "Movement":
                Range = float.Parse(PowerStats["R"].ToString());
                CD = float.Parse(PowerStats["CD"].ToString());
                break;
        }


    }
    #endregion
    #region Activate power
    // Group all function that serve the same algorithm
    public void ActivatePower(string name)
    {
        gameObject.AddComponent<AudioSource>();
        sound = GetComponent<AudioSource>();
        InitData(name);
        if (name.Contains("Wormhole"))
        {          
            gameObject.GetComponent<Wormhole>().GenerateWormhole();
        } else
        {
            if (name.Contains("LaserBeam"))
            {
                gameObject.GetComponent<LaserBeam>().isFire = true;
            } else
            {
                if (name.Contains("RocketBurst"))
                {
                    gameObject.GetComponent<RocketBurst>().GenerateRocket();
                } else
                {
                    if (name.Contains("Barrier"))
                    {
                        gameObject.GetComponent<Barrier>().GenBarrier();
                    }
                }
            }
        }      
    }

    public void BeforeActivating()
    {
        gameObject.AddComponent<AudioSource>();
        sound = GetComponent<AudioSource>();
        gameObject.GetComponent<LaserBeam>().Charging();
    }
    #endregion
    #region Sound Effect
    public void PlaySound(AudioClip sfx)
    {
        sound.clip = sfx;
        sound.loop = false;
        sound.Play();
        sound.volume = 0.35f;
    }  
    public void EndSound()
    {
        sound.clip = null;
    }
    #endregion
    #region Calculate pos
    public Vector2 CalculatePos(float range)
    {
        float angle = Fighter.GetComponent<PlayerMovement>().CurrentRotateAngle;
        float x = 0, y = 0;
        if (angle < 0) angle = angle % 360 + 360;
        if (angle >= 360) angle = angle % 360;
        if (angle >= 0 && angle <= 90)
        {
            x = Mathf.Abs(Mathf.Sin(angle * Mathf.Deg2Rad) * range);
            y = Mathf.Abs(Mathf.Cos(angle * Mathf.Deg2Rad) * range);
        }
        else if (angle > 90 && angle <= 180)
        {
            x = Mathf.Abs(Mathf.Sin((180 - angle) * Mathf.Deg2Rad) * range);
            y = -Mathf.Abs(Mathf.Cos((180 - angle) * Mathf.Deg2Rad) * range);
        }
        else if (angle > 180 && angle <= 270)
        {
            x = -Mathf.Abs(Mathf.Sin((angle - 180) * Mathf.Deg2Rad) * range);
            y = -Mathf.Abs(Mathf.Cos((angle - 180) * Mathf.Deg2Rad) * range);
        }
        else if (angle > 270 && angle < 360)
        {
            x = -Mathf.Abs(Mathf.Sin((360 - angle) * Mathf.Deg2Rad) * range);
            y = Mathf.Abs(Mathf.Cos((360 - angle) * Mathf.Deg2Rad) * range);
        }
        return new Vector2(x, y);

    }
    #endregion
    
}
