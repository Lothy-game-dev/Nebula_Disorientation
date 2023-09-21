using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryButton : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject Factory;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    private Factory FactoryController;
    public bool AlreadyPurchased;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        FactoryController = Factory.GetComponent<Factory>();
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
        if (GetComponent<CursorUnallowed>() != null)
        {
            if (AlreadyPurchased)
            {
                FindAnyObjectByType<NotificationBoardController>().CreateNormalNotiBoard(Factory.transform.position,
                "You have already bought this fighter!", 5f);
            } else
            {
                FindAnyObjectByType<NotificationBoardController>().CreateNormalNotiBoard(Factory.transform.position,
                "Rank requirement not unmet!", 5f);
            }
        }
        else
        if (FactoryController.EnoughPrice && FactoryController.RankRequired)
        {
            FindAnyObjectByType<NotificationBoardController>().VoidReturnFunction = BuyFighterModel;
            FindAnyObjectByType<NotificationBoardController>().CreateNormalConfirmBoard(Factory.transform.position, "Purchase\n" + FactoryController.ItemName + "\nfor \n" + FactoryController.ItemPriceCash + " cash and " + FactoryController.ItemPriceShard + " shard?");

        }
        else
        {
            if (!FactoryController.EnoughPrice)
            {
                FindAnyObjectByType<NotificationBoardController>().CreateNormalNotiBoard(Factory.transform.position, "You dont have enough money!", 5f);
            } else
            {
                if (!FactoryController.RankRequired)
                {
                    FindAnyObjectByType<NotificationBoardController>().CreateNormalNotiBoard(Factory.transform.position, "Rank requirement not unmet!", 5f);
                }
            }
        }
    }

    private void BuyFighterModel()
    {
        int n = FindObjectOfType<AccessDatabase>().GetCurrentOwnershipWeaponPowerModelByName(FindObjectOfType<UECMainMenuController>().PlayerId,
                FactoryController.ItemName, "Model");
        if (n > 0)
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Factory.transform.position,
                "You have already bought this item!\nPlease contanct our email!", 5f);
        } else if (n==-1)
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Factory.transform.position,
                "Cannot fetch data for " + FactoryController.ItemName + "!\nPlease try again!", 5f);
        } else if (n==0)
        {
            // Check case for adding ownership
            string check = FindObjectOfType<AccessDatabase>().AddOwnershipToItem(FindObjectOfType<UECMainMenuController>().PlayerId,
                FactoryController.ItemName, "Model", 1);
            if ("Not Found".Equals(check))
            {
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Factory.transform.position,
                    "Can not find information about this item.\nplease contact our email.", 5f);
            }
            else if ("Fail".Equals(check))
            {
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Factory.transform.position,
                    "Transaction failed.\nPlease try again.", 5f);
            }
            else if ("Success".Equals(check))
            {
                // If adding ownership successfully, reduce currency
                string check2 = FindObjectOfType<AccessDatabase>().DecreaseCurrencyAfterBuy(FindObjectOfType<UECMainMenuController>().PlayerId, int.Parse(FactoryController.ItemPriceCash), 
                    int.Parse(FactoryController.ItemPriceShard));
                switch (check2)
                {
                    case "Not Exist":
                        FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Factory.transform.position,
                    "Can not fetch data about your pilot.\nplease contact our email.", 5f);
                        FindObjectOfType<AccessDatabase>().DecreaseOwnershipToItem(FindObjectOfType<UECMainMenuController>().PlayerId,
                            FactoryController.ItemName, "Model", 1);
                        break;
                    case "Not Enough Cash":
                        FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Factory.transform.position,
                    "You don't have enough cash!\nPlease get some more.", 5f);
                        FindObjectOfType<AccessDatabase>().DecreaseOwnershipToItem(FindObjectOfType<UECMainMenuController>().PlayerId,
                            FactoryController.ItemName, "Model", 1);
                        break;
                    case "Not Enough Shard":
                        FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Factory.transform.position,
                    "You don't have enough timeless shard!\nPlease get some more.", 5f);
                        FindObjectOfType<AccessDatabase>().DecreaseOwnershipToItem(FindObjectOfType<UECMainMenuController>().PlayerId,
                            FactoryController.ItemName, "Model", 1);
                        break;
                    case "Fail":
                        // if reduce currency fail, reduce ownership
                        FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Factory.transform.position,
                    "Purchase Failed.\nPlease contact our email.", 5f);
                        FindObjectOfType<AccessDatabase>().DecreaseOwnershipToItem(FindObjectOfType<UECMainMenuController>().PlayerId,
                            FactoryController.ItemName, "Model", 1);
                        break;
                    case "Success":
                        // if success, reload data to UI

                        List<string> checkList = FindObjectOfType<AccessDatabase>().GetAllOwnedModel(FindObjectOfType<UECMainMenuController>().PlayerId);
                        if (checkList.Count==1)
                        {
                            string checkStr = FindObjectOfType<AccessDatabase>().AddStarterGiftWeapons(FindObjectOfType<UECMainMenuController>().PlayerId);
                            if (checkStr!="Fail")
                            {
                                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Factory.transform.position,
                                "Congrats!\nYou received <color=#36b37e>Pulse Cannon</color> x" + checkStr + " as starter gift!", 5f);
                            }
                        } else
                        {
                            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Factory.transform.position,
                            "Purchase Successfully.\n(Auto closed in 5 seconds)", 5f);
                        }
                        FindObjectOfType<UECMainMenuController>().GetData();
                        break;
                }
            }
        }
    }
    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    #endregion
}
