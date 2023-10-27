using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpaceZoneSummaryButton : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public string Type;
    public GameObject NextSZInfo;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
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
    #region Function group 1
    // Group all function that serve the same algorithm
    private void OnMouseDown()
    {
        if (Type=="NextSZInfo")
        {
            NextSZInfo.SetActive(true);
        } else if (Type=="Continue")
        {
            Dictionary<string,object> SessionData = FindObjectOfType<AccessDatabase>().GetSessionInfoByPlayerId(PlayerPrefs.GetInt("PlayerID"));
            // Get Next Stage Number, Variant and Hazard
            PlayerPrefs.SetInt("NextStage", (int)SessionData["CurrentStage"] + 1);
            int SpaceZoneNo = (int)SessionData["CurrentStage"] + 1;
            int ChosenVariant;
            int ChosenHazard;
            if (PlayerPrefs.GetInt("Variant")==0)
            {
                Dictionary<string, object> variantData = FindObjectOfType<AccessDatabase>().GetVariantCountsAndBackgroundByStageValue(SpaceZoneNo % 10);
                int VariantCount = (int)variantData["VariantCounts"];
                if (SpaceZoneNo < 51)
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
            } else
            {
                ChosenVariant = PlayerPrefs.GetInt("Variant");
                PlayerPrefs.SetInt("Variant",0);
            }
            if (PlayerPrefs.GetInt("Hazard")==0)
            {
                List<Dictionary<string, object>> ListAvailableHazard = FindObjectOfType<AccessDatabase>().GetAvailableHazards(SpaceZoneNo);
                ChosenHazard = RandomHazardChoose(ListAvailableHazard);
            } else
            {
                ChosenHazard = PlayerPrefs.GetInt("Hazard");
                PlayerPrefs.GetInt("Hazard", 0);
            }
            // Update
            FindObjectOfType<AccessDatabase>().UpdateSessionStageData((int)SessionData["SessionID"], SpaceZoneNo, ChosenHazard, ChosenVariant);
            
            PlayerPrefs.SetString("InitTeleport", "LOTW");
            SceneManager.UnloadSceneAsync("GameplayInterior");
            SceneManager.LoadSceneAsync("GameplayExterior");
        } else if (Type=="BackToUEC")
        {
            Dictionary<string, object> ListData = FindObjectOfType<AccessDatabase>().GetPlayerInformationById(PlayerPrefs.GetInt("PlayerID"));
            if ((int)ListData["FuelCell"] > 1)
            {
                FindObjectOfType<AccessDatabase>().ReduceFuelCell(PlayerPrefs.GetInt("PlayerID"));
                PlayerPrefs.SetString("InitTeleport", "UEC");
                SceneManager.UnloadSceneAsync("GameplayInterior");
                SceneManager.LoadSceneAsync("GameplayExterior");
            }
        }
    }

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
