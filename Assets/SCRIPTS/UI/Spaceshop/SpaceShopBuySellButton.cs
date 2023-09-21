using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShopBuySellButton : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Scene;
    #endregion
    #region NormalVariables
    public int CurrentValue;
    public string ItemName;
    public string ItemNameNoColor;
    public int Quantity;
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
    #region Mouse Check
    private void OnMouseDown()
    {
        // When mouse down, check condition
        if ("Buy".Equals(name))
        {
            if (GetComponent<CursorUnallowed>()!=null)
            {
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                   "Can not buy this item anymore today!\nPlease come back tommorrow!", 5f);
            } else
            if (Scene.GetComponent<SpaceShopScene>().CheckEnoughMoney(CurrentValue))
            {
                FindObjectOfType<NotificationBoardController>().VoidReturnFunction = PerformBuySell;
                FindObjectOfType<NotificationBoardController>().CreateNormalConfirmBoard(Scene.transform.position,
                    "Buy " + Quantity + " " + ItemName + "\nFor " + CurrentValue + " Cash?");
            } else
            {
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                    "Not enough cash for this transaction!\nPlease get some more!", 5f);
            }
        } else if ("Sell".Equals(name))
        {
            FindObjectOfType<NotificationBoardController>().VoidReturnFunction = PerformBuySell;
            FindObjectOfType<NotificationBoardController>().CreateNormalConfirmBoard(Scene.transform.position,
                "sell " + Quantity + "\n" + ItemName + "\nFor " + CurrentValue + " Cash?");
        }
    }

    // If confirm board is click, run this code
    public void PerformBuySell()
    {
        if ("Buy".Equals(name))
        {
            // Check if enough stocks
            int currentStocks = FindObjectOfType<AccessDatabase>().GetStocksPerDayOfConsumable(ItemNameNoColor);
            if (currentStocks == 0)
            {
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                    "Can not buy this item anymore today!\nPlease come back tommorrow!", 5f);
                return;
            } else if (currentStocks < Quantity && currentStocks!=-1)
            {
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                    "Can not buy more than stocks in shop!\nPlease re-input!", 5f);
                return;
            } else
            {
                // case fuel cell: Check total fuel cell 
                if ("fuelcell".Equals(ItemNameNoColor.Replace(" ", "").Replace("-", "").ToLower()))
                {
                    if ((int)FindObjectOfType<AccessDatabase>().GetPlayerInformationById(FindObjectOfType<UECMainMenuController>().PlayerId)["FuelCell"]<=9)
                    {
                        string checkCell = FindObjectOfType<AccessDatabase>().AddFuelCell(FindObjectOfType<UECMainMenuController>().PlayerId);
                        if ("No Data".Equals(checkCell))
                        {
                            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                            "Cannot fetch data about your Fuel Cell!\nPlease contact our email!", 5f);
                        } 
                        else if ("Full".Equals(checkCell))
                        {
                            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                            "Your Fuel Capacity is maxed out.\nCan't purchase another Cell.", 5f);
                        }
                        else if ("Fail".Equals(checkCell))
                        {
                            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                            "Purchased Fail.\nPlease contact our email!", 5f);
                        }
                        else if ("Success".Equals(checkCell))
                        {
                            // Check case for adding ownership
                            string check = FindObjectOfType<AccessDatabase>().AddPurchaseHistory(FindObjectOfType<UECMainMenuController>().PlayerId,
                            12, "Consumable", Quantity, true);
                            if ("Fail".Equals(check))
                            {
                                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                                    "Transaction failed.\nPlease try again.", 5f);
                            }
                            else if ("Success".Equals(check))
                            {
                                // If adding ownership successfully, reduce currency
                                string check2 = FindObjectOfType<AccessDatabase>().DecreaseCurrencyAfterBuy(FindObjectOfType<UECMainMenuController>().PlayerId, CurrentValue, 0);
                                switch (check2)
                                {
                                    case "Not Exist":
                                        FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                                    "Can not fetch data about your pilot.\nplease contact our email.", 5f);
                                        break;
                                    case "Not Enough Cash":
                                        FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                                    "You don't have enough cash!\nPlease get some more.", 5f);
                                        break;
                                    case "Not Enough Shard":
                                        FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                                    "You don't have enough timeless shard!\nPlease get some more.", 5f);
                                        break;
                                    case "Fail":
                                        // if reduce currency fail, reduce ownership
                                        FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                                    "Purchase Failed.\nPlease contact our email.", 5f);
                                        FindObjectOfType<AccessDatabase>().DecreaseOwnershipToItem(FindObjectOfType<UECMainMenuController>().PlayerId,
                                            ItemNameNoColor, "Consumable", Quantity);
                                        break;
                                    case "Success":
                                        // if success, reload data to UI
                                        FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                                    "Purchase Successfully.\n", 5f);
                                        FindObjectOfType<UECMainMenuController>().GetData();
                                        break;
                                }
                            }
                        }
                    } else
                    {
                        FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                            "Your Fuel Capacity is maxed out.\nCan't purchase another Cell.", 5f);
                    }
                }
                else
                {
                    // Check case for adding ownership
                    string check = FindObjectOfType<AccessDatabase>().AddOwnershipToItem(FindObjectOfType<UECMainMenuController>().PlayerId,
                    ItemNameNoColor, "Consumable", Quantity);
                    if ("Not Found".Equals(check))
                    {
                        FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                            "Can not find information about this item.\nplease contact our email.", 5f);
                    }
                    else if ("Fail".Equals(check))
                    {
                        FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                            "Transaction failed.\nPlease try again.", 5f);
                    }
                    else if ("Success".Equals(check))
                    {
                        // If adding ownership successfully, reduce currency
                        string check2 = FindObjectOfType<AccessDatabase>().DecreaseCurrencyAfterBuy(FindObjectOfType<UECMainMenuController>().PlayerId, CurrentValue, 0);
                        switch (check2)
                        {
                            case "Not Exist":
                                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                            "Can not fetch data about your pilot.\nplease contact our email.", 5f);
                                break;
                            case "Not Enough Cash":
                                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                            "You don't have enough cash!\nPlease get some more.", 5f);
                                break;
                            case "Not Enough Shard":
                                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                            "You don't have enough timeless shard!\nPlease get some more.", 5f);
                                break;
                            case "Fail":
                                // if reduce currency fail, reduce ownership
                                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                            "Purchase Failed.\nPlease contact our email.", 5f);
                                FindObjectOfType<AccessDatabase>().DecreaseOwnershipToItem(FindObjectOfType<UECMainMenuController>().PlayerId,
                                    ItemNameNoColor, "Consumable", Quantity);
                                break;
                            case "Success":
                                // if success, reload data to UI
                                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                            "Purchase Successfully.\n", 5f);
                                FindObjectOfType<UECMainMenuController>().GetData();
                                break;
                        }
                    }
                }
            } 
        } else if ("Sell".Equals(name))
        {
            // Check case for adding ownership
            string check = FindObjectOfType<AccessDatabase>().DecreaseOwnershipToItem(FindObjectOfType<UECMainMenuController>().PlayerId,
                ItemNameNoColor, "Consumable", Quantity);
            if ("Fail".Equals(check))
            {
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                    "Transaction failed.\nPlease try again.", 5f);
            }
            else if ("Not Enough Item".Equals(check))
            {
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                    "You don't have enough item to sell!\nPlease buy some more!", 5f);
            }
            else if ("Success".Equals(check))
            {
                // If adding ownership successfully, reduce currency
                string check2 = FindObjectOfType<AccessDatabase>().IncreaseCurrencyAfterSell(FindObjectOfType<UECMainMenuController>().PlayerId, CurrentValue, 0);
                switch (check2)
                {
                    case "Not Exist":
                        FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                    "Can not fetch data about your pilot.\nplease contact our email.", 5f);
                        break;
                    case "Fail":
                        // if reduce currency fail, reduce ownership
                        FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                    "Sold Failed.\nPlease contact our email.", 5f);
                        FindObjectOfType<AccessDatabase>().AddOwnershipToItem(FindObjectOfType<UECMainMenuController>().PlayerId,
                            ItemNameNoColor, "Consumable", Quantity);
                        break;
                    case "Success":
                        // if success, reload data to UI
                        FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                    "Sold Successfully.\n", 5f);
                        FindObjectOfType<UECMainMenuController>().GetData();
                        break;
                }
            }
        }
    }
    #endregion
}
