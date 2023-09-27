using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers : MonoBehaviour
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
    public GameObject Fighter;
    private Dictionary<string, object> Power;
    private Dictionary<string, object> PowerStats;
    public float DPH;
    public float AoH;
    public float AoE;
    public float Velocity;
    public float Range;
    public float Duration;
    public float CD;
    public float BR;
    public float BRx;
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
                BR = float.Parse(PowerStats["BR"].ToString());
                Duration = float.Parse(PowerStats["Dur"].ToString());
                CD = float.Parse(PowerStats["CD"].ToString());
                if (PowerStats.ContainsKey("BRx"))
                {
                    BRx = float.Parse(PowerStats["BRx"].ToString());
                } break;
            case "Offensive":
                DPH = float.Parse(PowerStats["DPH"].ToString());
                AoH = float.Parse(PowerStats["AOH"].ToString());
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
        InitData(name);
        if (name.Contains("Wormhole"))
        {
            gameObject.GetComponent<Wormhole>().GenerateWormhole();
        }
        
    }
    #endregion
}
