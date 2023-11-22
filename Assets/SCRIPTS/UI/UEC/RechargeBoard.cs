using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RechargeBoard : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    #endregion
    #region NormalVariables
    public int CurrentShardRecharging;
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
    #region Recharge
    public void Recharge()
    {
        // Recharge
        if (CurrentShardRecharging>0)
        {
            /*InsertData();*/
            ServiceUnavailable();
        } else
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
                "Recharging Failed!\nPlease select one option first!", 5f);
        }   
    }
    private void InsertData()
    {
        int id = FindObjectOfType<UECMainMenuController>().PlayerId;
        string check = FindObjectOfType<AccessDatabase>().RechargeTimelessShard(id, CurrentShardRecharging);
        if ("Fail".Equals(check))
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
                "Recharging Failed!\nPlease try again!", 5f);
        } else if ("No Exist".Equals(check))
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
                "Cannot fetch Pilot's Data!\nPlease log-in again.", 5f);
        } else if ("Success".Equals(check)) {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
                CurrentShardRecharging.ToString() + " <sprite index='0'>\nRecharged Successfully." +
                "\nEnjoy!", 5f);
            FindObjectOfType<UECMainMenuController>().GetData();
            FindAnyObjectByType<AccessDatabase>().UpdateEconomyStatistic(id, CurrentShardRecharging, 0, "ConvertShard");
            Destroy(transform.parent.gameObject);
        }
    }

    private void ServiceUnavailable()
    {
        FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
                "Service currently unavailable!\nPlease try again later!", 5f);
        Destroy(transform.parent.gameObject);
    }
    #endregion
}
