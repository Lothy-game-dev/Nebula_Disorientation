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
        if (FactoryController.EnoughPrice && FactoryController.RankRequired)
        {
            FindAnyObjectByType<NotificationBoardController>().VoidReturnFunction = BuyFighterModel;
            FindAnyObjectByType<NotificationBoardController>().CreateNormalConfirmBoard(Factory.transform.position, "Are you sure you wanna buy " + FactoryController.ItemName + " for " + FactoryController.ItemPriceCash + " cash or " + FactoryController.ItemPriceShard + " shard?");

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
                    FindAnyObjectByType<NotificationBoardController>().CreateNormalNotiBoard(Factory.transform.position, "Rank requirement unmet!", 5f);
                }
            }
        }
    }

    private void BuyFighterModel()
    {
        Debug.Log(FactoryController.ItemId);
    }
    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    #endregion
}
