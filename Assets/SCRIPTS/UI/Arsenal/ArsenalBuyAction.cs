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
    public string PreReqName;
    public bool isRanked;
    public bool isZeroShard;
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
        if (GetComponent<CursorUnallowed>() != null)
        {
            if (isZeroShard)
            {
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(ArsenalItem.transform.position,
                        "You can only buy this " + ArsenalItem.GetComponent<Arsenal>().CurrentTab + " during a session.", 5f);
            } else
            {
                if (isRanked)
                {
                    FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(ArsenalItem.transform.position,
                        "Rank requirement not unmet!", 5f);
                }
                else if (PreReqName != null && PreReqName != "")
                {
                    FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(ArsenalItem.transform.position,
                        "You need to own " + PreReqName + " in order to buy this " + ArsenalItem.GetComponent<Arsenal>().CurrentTab + "!", 5f);
                }
                else
                {
                    FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(ArsenalItem.transform.position,
                        "You have purchased this " + ArsenalItem.GetComponent<Arsenal>().CurrentTab + "!", 5f);
                }
            }           
        } else
        // check the conditions before buying 
        if (Ar.EnoughPrice && Ar.RankRequired)
        {
            FindAnyObjectByType<NotificationBoardController>().VoidReturnFunction = BuyArsenalItem;
            if (!Ar.IsInSession)
            {
                FindAnyObjectByType<NotificationBoardController>().CreateNormalConfirmBoard(ArsenalItem.transform.position, "Are you sure you wanna buy " + Ar.ItemName + " for " + Ar.RequiredShard + " shard?");
            }
            else
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
                    FindAnyObjectByType<NotificationBoardController>().CreateNormalNotiBoard(ArsenalItem.transform.position, "Rank requirement not unmet!", 5f);
                }
            }
        }
    }
    #endregion
    #region Buy function
    // Group all function that serve the same algorithm
    public void BuyArsenalItem()
    {
        int n = FindObjectOfType<AccessDatabase>().GetCurrentOwnershipWeaponPowerModelByName(FindObjectOfType<UECMainMenuController>().PlayerId,
                Ar.ItemName, Ar.CurrentTab);
        bool checkcase = false;
        if (Ar.CurrentTab == "Weapon")
        {
            if (n >= 2)
            {
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(ArsenalItem.transform.position,
                "You have already bought this item!\nPlease contanct our email!", 5f);
            }
            else if (n == -1)
            {
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(ArsenalItem.transform.position,
                    "Cannot fetch data for " + Ar.ItemName + "!\nPlease try again!", 5f);
            }
            else if (n >= 0) checkcase = true;
        }
        else if (Ar.CurrentTab == "Power")
        {
            if (n >= 1)
            {
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(ArsenalItem.transform.position,
                "You have already bought this item!\nPlease contanct our email!", 5f);
            }
            else if (n == -1)
            {
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(ArsenalItem.transform.position,
                    "Cannot fetch data for " + Ar.ItemName + "!\nPlease try again!", 5f);
            }
            else if (n == 0) checkcase = true;
        }
        if (checkcase)
        {
            // Check case for adding ownership
            string check = FindObjectOfType<AccessDatabase>().AddOwnershipToItem(FindObjectOfType<UECMainMenuController>().PlayerId,
                Ar.ItemName, Ar.CurrentTab, 1);
            if ("Not Found".Equals(check))
            {
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(ArsenalItem.transform.position,
                    "Can not find information about this item.\nplease contact our email.", 5f);
            }
            else if ("Fail".Equals(check))
            {
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(ArsenalItem.transform.position,
                    "Transaction failed.\nPlease try again.", 5f);
            }
            else if ("Success".Equals(check))
            {
                // If adding ownership successfully, reduce currency
                string check2 = FindObjectOfType<AccessDatabase>().DecreaseCurrencyAfterBuy(FindObjectOfType<UECMainMenuController>().PlayerId, 
                    0, int.Parse(Ar.RequiredShard));
                switch (check2)
                {
                    case "Not Exist":
                        FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(ArsenalItem.transform.position,
                    "Can not fetch data about your pilot.\nplease contact our email.", 5f);
                        FindObjectOfType<AccessDatabase>().DecreaseOwnershipToItem(FindObjectOfType<UECMainMenuController>().PlayerId,
                            Ar.ItemName, Ar.CurrentTab, 1);
                        break;
                    case "Not Enough Cash":
                        FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(ArsenalItem.transform.position,
                    "You don't have enough cash!\nPlease get some more.", 5f);
                        FindObjectOfType<AccessDatabase>().DecreaseOwnershipToItem(FindObjectOfType<UECMainMenuController>().PlayerId,
                            Ar.ItemName, Ar.CurrentTab, 1);
                        break;
                    case "Not Enough Shard":
                        FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(ArsenalItem.transform.position,
                    "You don't have enough timeless shard!\nPlease get some more.", 5f);
                        FindObjectOfType<AccessDatabase>().DecreaseOwnershipToItem(FindObjectOfType<UECMainMenuController>().PlayerId,
                            Ar.ItemName, Ar.CurrentTab, 1);
                        break;
                    case "Fail":
                        // if reduce currency fail, reduce ownership
                        FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(ArsenalItem.transform.position,
                    "Purchase Failed.\nPlease contact our email.", 5f);
                        FindObjectOfType<AccessDatabase>().DecreaseOwnershipToItem(FindObjectOfType<UECMainMenuController>().PlayerId,
                            Ar.ItemName, Ar.CurrentTab, 1);
                        break;
                    case "Success":
                        // if success, reload data to UI
                        FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(ArsenalItem.transform.position,
                    "Purchase Successfully.\n(Auto closed in 5 seconds)", 5f);
                        FindObjectOfType<UECMainMenuController>().GetData();
                        break;
                }
            }
        }
    }
    #endregion
}
