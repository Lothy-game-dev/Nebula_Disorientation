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
    private StatisticController stat;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        stat = FindAnyObjectByType<StatisticController>();
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
            stat.UpdateSessionPlaytime(stat.SessionID, stat.SessionPlayTime);
            PlayerPrefs.SetString("InitTeleport", "LOTW");
            SceneManager.UnloadSceneAsync("GameplayInterior");
            SceneManager.LoadSceneAsync("GameplayExterior");
        } else if (Type=="BackToUEC")
        {
            Dictionary<string, object> ListData = FindObjectOfType<AccessDatabase>().GetPlayerInformationById(PlayerPrefs.GetInt("PlayerID"));
            if ((int)ListData["FuelCell"] >= 1)
            {
                FindObjectOfType<GameplayInteriorController>().GenerateBlackFadeClose(1f);
                StartCoroutine(MoveToUEC());
            }
        }
    }

    private IEnumerator MoveToUEC()
    {
        yield return new WaitForSeconds(1f);
        FindObjectOfType<AccessDatabase>().ReduceFuelCell(PlayerPrefs.GetInt("PlayerID"));
        FindObjectOfType<AccessDatabase>().AddSessionCurrentSaveData(PlayerPrefs.GetInt("PlayerID"), "UEC");
        PlayerPrefs.SetString("InitTeleport", "UEC");
        SceneManager.UnloadSceneAsync("GameplayInterior");
        SceneManager.LoadSceneAsync("GameplayExterior");
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
