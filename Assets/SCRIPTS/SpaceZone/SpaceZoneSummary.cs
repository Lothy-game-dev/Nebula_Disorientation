using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpaceZoneSummary : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject TotalEnemyDefeated;
    public GameObject TotalDamageDealt;
    public GameObject CurrentFighterHP;
    public GameObject CurrentShard;
    public GameObject CurrentCash;
    public GameObject FuelCell;
    public GameObject Timer;
    public GameObject SZNo;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    private StatisticController stat;  
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
    #region Summarize
    // Group all function that serve the same algorithm
    public void Summarize()
    {
        stat = FindAnyObjectByType<StatisticController>();
        TotalEnemyDefeated.GetComponent<TextMeshPro>().text = "Enemy Destroyed: " + stat.TotalEnemyDefeated;
        TotalDamageDealt.GetComponent<TextMeshPro>().text = "Damage Dealt: " + stat.DamageDealt;
        CurrentFighterHP.GetComponent<TextMeshPro>().text = "Current Fighter HP: " + Mathf.RoundToInt(stat.CurrentHP) + "/" + Mathf.RoundToInt(stat.MaxHP);
        SZNo.GetComponent<TextMeshPro>().text = "Space Zone No." + stat.CurrentSZNo + " Completed!";
        CurrentShard.GetComponent<TextMeshPro>().text = "Current Shard: " + stat.CurrentShard;
        CurrentCash.GetComponent<TextMeshPro>().text = "Current Cash: " + stat.CurrentCash;
        Timer.GetComponent<TextMeshPro>().text = stat.PlayedTime;

        FuelCell.transform.GetChild(0).GetChild(0).GetComponent<Slider>().maxValue = 10;
        FuelCell.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value = stat.CurrentFuelCell;
        Time.timeScale = 0;
    }
    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    #endregion
}
