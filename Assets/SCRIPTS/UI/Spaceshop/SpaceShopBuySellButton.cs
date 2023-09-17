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

    public void PerformBuySell()
    {
        Debug.Log(name + " - " + Quantity + " " + name + ":" + CurrentValue);
    }
    #endregion
}
