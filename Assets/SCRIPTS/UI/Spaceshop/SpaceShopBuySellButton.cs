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
            if (Scene.GetComponent<SpaceShopScene>().CheckEnoughMoney(CurrentValue))
            {
                FindObjectOfType<NotificationBoardController>().VoidReturnFunction = PerformBuySell;
                FindObjectOfType<NotificationBoardController>().CreateNormalConfirmBoard(Scene.transform.position,
                    "Do you want to buy " + Quantity + " " + ItemName + " For " + CurrentValue + " Cash?");
            } else
            {
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                    "Not enough cash for this transaction!\nPlease get some more!", 5f);
            }
        } else if ("Sell".Equals(name))
        {
            FindObjectOfType<NotificationBoardController>().VoidReturnFunction = PerformBuySell;
            FindObjectOfType<NotificationBoardController>().CreateNormalConfirmBoard(Scene.transform.position,
                "Do you want to sell " + Quantity + " " + ItemName + " For " + CurrentValue + " Cash?");
        }
    }

    // If confirm board is click, run this code
    public void PerformBuySell()
    {
        if ("Buy".Equals(name))
        {
            // Check case for adding ownership
            Debug.Log(FindObjectOfType<UECMainMenuController>().PlayerId + "," + ItemNameNoColor + "," + Quantity);
            string check = FindObjectOfType<AccessDatabase>().AddOwnershipToItem(FindObjectOfType<UECMainMenuController>().PlayerId,
                ItemNameNoColor, "Consumable", Quantity);
            if ("Not Found".Equals(check))
            {
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                    "Can not find information about this item.\nplease contact our email.", 5f);
            } else if ("Fail".Equals(check))
            {
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                    "Transaction failed.\nPlease try again.", 5f);
            } else if ("Success".Equals(check))
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
                        FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                    "Purchase Failed.\nPlease contact our email.", 5f);
                        break;
                    case "Success":
                        FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                    "Purchase Successfully.\n(Auto closed in 5 seconds)", 5f);
                        Scene.GetComponent<SpaceShopScene>().SetData();
                        break;
                }
            }
        }
    }
    #endregion
}
