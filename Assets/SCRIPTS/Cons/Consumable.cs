using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour
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
    public float Cooldown;
    public float Duration;
    public string Effect;
    public GameObject Fighter;
    private Dictionary<string, object> ConsDict;
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
    }
    #endregion
    #region Init data
    // Group all function that serve the same algorithm
    public void InitData(string name)
    {
        ConsDict = FindAnyObjectByType<AccessDatabase>().GetConsumableDataByName(name);
        Cooldown = float.Parse(ConsDict["Cooldown"].ToString());
        Duration = float.Parse(ConsDict["Duration"].ToString());
        Effect = (string)ConsDict["Effect"];
    }
    #endregion
    #region Activate cons
    // Group all function that serve the same algorithm?
    public void ActivateConsumable()
    {
        switch (Effect.Split("-")[0])
        {
            case "RED": gameObject.GetComponent<WingShield>().WingmanShieldEffect(); break;
            case "AER": gameObject.GetComponent<EngineBooster>().BoosterEffect(); break;
            case "RMH": gameObject.GetComponent<AutoRepair>().ActivateAutoRepair(); break;
            case "INV": gameObject.GetComponent<NanoCoat>().NanoCoatActivated(); break;
            case "FC": break;
        }
    }
    #endregion
}
