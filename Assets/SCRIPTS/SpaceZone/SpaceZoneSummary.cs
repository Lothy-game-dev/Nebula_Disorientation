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
    public GameObject Fighter;
    public GameObject HPSlider;
    public GameObject SessionCash;
    public GameObject SessionShard;
    public GameObject FuelEnergy;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    private StatisticController stat;
    public bool DoneLightUp2;
    private float InitASummary;
    private AccessDatabase ad;
    private string Cons;
    private Dictionary<string, int> Consumable;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        Color c3 = GetComponent<SpriteRenderer>().color;
        InitASummary = c3.a;
        c3.a = 0;
        GetComponent<SpriteRenderer>().color = c3;       
        
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (gameObject.activeSelf)
        {
            if (gameObject.GetComponent<SpriteRenderer>().color.a < InitASummary)
            {
                Color c = gameObject.GetComponent<SpriteRenderer>().color;
                c.a += InitASummary / 60;
                gameObject.GetComponent<SpriteRenderer>().color = c;
            }
            else
            {
                if (!DoneLightUp2)
                {
                    DoneLightUp2 = true;
                    gameObject.transform.GetChild(0).gameObject.SetActive(true);
                    gameObject.transform.GetChild(1).gameObject.SetActive(true);
                    gameObject.transform.GetChild(2).gameObject.SetActive(true);
                    gameObject.transform.GetChild(3).gameObject.SetActive(true);
                    gameObject.transform.GetChild(4).gameObject.SetActive(true);
                    gameObject.transform.GetChild(5).gameObject.SetActive(true);
                    gameObject.transform.GetChild(6).gameObject.SetActive(true);
                    gameObject.transform.GetChild(7).gameObject.SetActive(true);
                }
            }
        }
    }
    #endregion
    #region Summarize
    // Group all function that serve the same algorithm
    public void Summarize()
    {
        Time.timeScale = 0;
        stat = FindAnyObjectByType<StatisticController>();
        FindObjectOfType<AccessDatabase>().UpdateSessionCashAndShard(stat.SessionID, true, stat.CurrentCashReward, stat.CurrentShardReward);
        Dictionary<string, object> SessionData = FindObjectOfType<AccessDatabase>().GetSessionInfoByPlayerId(PlayerPrefs.GetInt("PlayerID"));


        Consumable = new Dictionary<string, int>();  
        TotalEnemyDefeated.GetComponent<TextMeshPro>().text = "Enemy Destroyed: <color=#3bccec>" + stat.TotalEnemyDefeated + "</color>";
        TotalDamageDealt.GetComponent<TextMeshPro>().text = "Damage Dealt: <color=#3bccec>" + stat.DamageDealt + "</color>";
        SZNo.GetComponent<TextMeshPro>().text = "Space Zone No. <color=#3bccec>" + stat.CurrentSZNo + "</color> Completed!";
        SessionShard.GetComponent<TextMeshPro>().text = "Session <sprite=0>: <br> <color=#3bccec>" + SessionData["SessionTimelessShard"].ToString() + "</color>";
        SessionCash.GetComponent<TextMeshPro>().text = "Session <sprite=3>: <br> <color=#3bccec>" + SessionData["SessionCash"].ToString() + "</color>";
        CurrentShard.GetComponent<TextMeshPro>().text = "<sprite=0> Collected : <br> <color=#3bccec>" + stat.ShardCollected + "</color> + <color=#3bccec>" + stat.CurrentShardReward + "</color>";
        CurrentCash.GetComponent<TextMeshPro>().text = "<sprite=3> Collected : <br> <color=#3bccec>" + stat.CashCollected + "</color> + <color=#3bccec>" + stat.CurrentCashReward + "</color>";
        FuelEnergy.GetComponent<TextMeshPro>().text = "<sprite=1> Collected : <br> <color=#3bccec>" + stat.TotalEnemyDefeated * 10 + "</color>";
        Timer.GetComponent<TextMeshPro>().text = stat.PlayedTime;

        FuelCell.transform.GetChild(0).GetChild(0).GetComponent<Slider>().maxValue = 10;
        FuelCell.transform.GetChild(0).GetChild(0).GetComponent<Slider>().value = stat.CurrentFuelCell;
      
        CurrentFighterHP.GetComponent<TextMeshPro>().text = "Current Fighter HP (" + Mathf.CeilToInt(stat.CurrentHP / stat.MaxHP * 100) + "%)";
        HPSlider.GetComponent<Slider>().maxValue = Mathf.RoundToInt(stat.MaxHP);
        HPSlider.GetComponent<Slider>().value = Mathf.RoundToInt(stat.CurrentHP);

        // Load data for next
        // Get Next Stage Number, Variant and Hazard
        PlayerPrefs.SetInt("NextStage", (int)SessionData["CurrentStage"] + 1);
        int SpaceZoneNo = (int)SessionData["CurrentStage"] + 1;
        int ChosenVariant;
        int ChosenHazard;
        if (PlayerPrefs.GetInt("Variant") == 0)
        {
            Dictionary<string, object> variantData = FindObjectOfType<AccessDatabase>().GetVariantCountsAndBackgroundByStageValue(SpaceZoneNo % 10);
            int VariantCount = (int)variantData["VariantCounts"];
            if (SpaceZoneNo < 39)
            {
                if (SpaceZoneNo % 10 == 0)
                {
                    ChosenVariant = 2;
                }
                else if (SpaceZoneNo % 10 == 8 || SpaceZoneNo % 10 == 9)
                {
                    ChosenVariant = 1;
                }
                else
                    ChosenVariant = Random.Range(1, 1 + VariantCount);
            }
            else
                ChosenVariant = Random.Range(1, 1 + VariantCount);
        }
        else
        {
            ChosenVariant = PlayerPrefs.GetInt("Variant");
            PlayerPrefs.SetInt("Variant", 0);
        }
        if (PlayerPrefs.GetInt("Hazard") == 0)
        {
            List<Dictionary<string, object>> ListAvailableHazard = FindObjectOfType<AccessDatabase>().GetAvailableHazards(SpaceZoneNo);
            ChosenHazard = RandomHazardChoose(ListAvailableHazard);
        }
        else
        {
            ChosenHazard = PlayerPrefs.GetInt("Hazard");
            PlayerPrefs.GetInt("Hazard", 0);
        }
        // Update
        FindObjectOfType<AccessDatabase>().UpdateSessionStageData((int)SessionData["SessionID"], SpaceZoneNo, ChosenHazard, ChosenVariant);
        FindObjectOfType<AccessDatabase>().AddSessionCurrentSaveData(PlayerPrefs.GetInt("PlayerID"), "LOTW");
        FindObjectOfType<AccessDatabase>().UpdateReduceDurationAllCardByPlayerID(PlayerPrefs.GetInt("PlayerID"));
    }
    #endregion
    #region Random Hazard
    // Group all function that serve the same algorithm
    private int RandomHazardChoose(List<Dictionary<string, object>> dataDict)
    {
        float sum = 0;
        for (int i = 0; i < dataDict.Count; i++)
        {
            sum += (int)dataDict[i]["HazardChance"];
        }
        float n = Random.Range(0, sum);
        for (int i = 0; i < dataDict.Count; i++)
        {
            if (n < (int)dataDict[i]["HazardChance"])
            {
                return (int)dataDict[i]["HazardID"];
            }
            else
            {
                n -= (int)dataDict[i]["HazardChance"];
            }
        }
        return 1;
    }
    #endregion
}
