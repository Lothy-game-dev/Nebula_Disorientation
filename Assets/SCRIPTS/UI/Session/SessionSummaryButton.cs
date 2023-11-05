using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionSummaryButton : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject SSController;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    private AccessDatabase ad;
    private SessionSummary SessionSum;
    private bool isLocked;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        ad = FindAnyObjectByType<AccessDatabase>();
        SessionSum = SSController.GetComponent<SessionSummary>();
        isLocked = true;       
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (name == "EndButton")
        {          
            if ((SessionSum.ShardCollected || SessionSum.ShardAmount == 0) && (SessionSum.CashCollected || SessionSum.CashAmount == 0))
            {
                transform.GetChild(1).gameObject.SetActive(false);
                isLocked = false;
            }           
        }

        if (SessionSum.CashAmount == 0 && name == "CashButton")
        {
            transform.GetChild(1).gameObject.SetActive(true);
        }

        if (SessionSum.ShardAmount == 0 && name == "ShardButton")
        {   
            transform.GetChild(1).gameObject.SetActive(true);
        }
    }
    #endregion
    #region Check mouse
    // Group all function that serve the same algorithm
    private void OnMouseDown()
    {
        FindObjectOfType<SoundSFXGeneratorController>().GenerateSound("ButtonClick");
        string check = "";
        switch(name)
        {
            case "ShardButton":
                // Collect shard
                if (!SessionSum.ShardCollected && SessionSum.ShardAmount > 0)
                {
                    check = ad.RechargeTimelessShard(PlayerPrefs.GetInt("PlayerID"), SessionSum.ShardAmount);
                    gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = "Collected";
                    SessionSum.Shard.transform.GetChild(0).GetComponent<TextMeshPro>().text = SessionSum.ShardAmount + " <sprite index='0'>";
                    SessionSum.ShardCollected = true;
                } 
                break;
            case "CashButton":
                // Collect cash
                if (!SessionSum.CashCollected && SessionSum.CashAmount > 0)
                {
                    check = ad.UpdateCash(PlayerPrefs.GetInt("PlayerID"), SessionSum.CashAmount);
                    gameObject.transform.GetChild(0).GetComponent<TextMeshPro>().text = "Collected";
                    SessionSum.Cash.transform.GetChild(0).GetComponent<TextMeshPro>().text = SessionSum.CashAmount + " <sprite index='3'>";
                    SessionSum.CashCollected = true;
                }
                break;
            case "EndButton": 
                // Show notification to collect all the reward
                if (isLocked)
                {
                    FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(SSController.transform.position,
                "Please collect all the reward before ending the session!", 5f);
                } else
                {
                    //End Session                
                    if ((SessionSum.ShardCollected || SessionSum.ShardAmount == 0) && (SessionSum.CashCollected || SessionSum.CashAmount == 0))
                    {
                        if (!SessionSum.isFailed)
                        {
                            Dictionary<string, int> Data = FindObjectOfType<AccessDatabase>().GetSessionOwnedConsumables(PlayerPrefs.GetInt("PlayerID"));
                            foreach (var item in Data)
                            {
                                FindObjectOfType<AccessDatabase>().AddOwnershipToItem(PlayerPrefs.GetInt("PlayerID"), item.Key,
                                    "Consumable", item.Value);
                                FindObjectOfType<AccessDatabase>().DecreaseSessionOwnershipToItem(PlayerPrefs.GetInt("PlayerID"), item.Key,
                                    "Consumable", item.Value);
                            }
                        }
                        check = ad.EndSession(PlayerPrefs.GetInt("PlayerID"));
                        SceneManager.LoadSceneAsync("UECMainMenu");
                        SceneManager.UnloadSceneAsync("SessionSummary");
                    }                  
                }
                break;
        }
        if (check != "")
        {
            switch(check)
            {              
                case "Not Exist":
                    FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(SSController.transform.position,
                "Can not fetch data about your pilot.\nplease contact our email.", 5f); break;
                case "Fail":                
                    FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(SSController.transform.position,
                "Collect Failed.\nPlease contact our email.", 5f); break;
            }
        }
    }
    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    private void OnApplicationQuit()
    {
        ad.EndSession(PlayerPrefs.GetInt("PlayerID"));
    }
    #endregion
}
