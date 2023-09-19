using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArsenalBuyAction : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject ArsenalItem;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    private Arsenal Ar;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        Ar = ArsenalItem.GetComponent<Arsenal>();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Buy Action
    // Group all function that serve the same algorithm
    private void OnMouseDown()
    {
        // check the conditions before buying 
        if (Ar.EnoughPrice && Ar.RankRequired)
        {
            FindAnyObjectByType<NotificationBoardController>().VoidReturnFunction = BuyArsenalItem;
            if (!Ar.IsInSession)
            {
                FindAnyObjectByType<NotificationBoardController>().CreateNormalConfirmBoard(ArsenalItem.transform.position, "Are you sure you wanna buy " + Ar.ItemName + " for "+  Ar.RequiredShard + " shard?");
            } else
            {
                FindAnyObjectByType<NotificationBoardController>().CreateNormalConfirmBoard(ArsenalItem.transform.position, "Are you sure you wanna buy " + Ar.ItemName + " for " + Ar.RequiredCash + " cash?");
            }
            
        }
        else
        {
            if (!Ar.EnoughPrice)
            {
                FindAnyObjectByType<NotificationBoardController>().CreateNormalNotiBoard(ArsenalItem.transform.position, "You dont have enough money!", 5f);
            }
            else
            {
                if (!Ar.RankRequired)
                {
                    FindAnyObjectByType<NotificationBoardController>().CreateNormalNotiBoard(ArsenalItem.transform.position, "Rank requirement unmet!", 5f);
                }
            }
        }


    }
    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    public void BuyArsenalItem()
    {
        Debug.Log(Ar.ItemId + "|" + Ar.ItemType);
    }
    #endregion
}
