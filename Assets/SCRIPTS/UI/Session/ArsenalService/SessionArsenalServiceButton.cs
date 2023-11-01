using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionArsenalServiceButton : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject ArsenalServiceController;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public bool CanBeRepaired;
    public bool isEnoughMoney;
    public int HealPercent;
    private SessionArsenalService service;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        service = ArsenalServiceController.GetComponent<SessionArsenalService>();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Check mouse
    // Group all function that serve the same algorithm
    private void OnMouseDown()
    {
        if (GetComponent<CursorUnallowed>() == null)
        {
            if (CanBeRepaired && isEnoughMoney)
            {
                FindAnyObjectByType<NotificationBoardController>().VoidReturnFunction = Repair;
                FindAnyObjectByType<NotificationBoardController>().CreateNormalConfirmBoard(ArsenalServiceController.transform.position, "Do you want to repair?");
            } else
            {
                if (!isEnoughMoney)
                {
                    FindObjectOfType<SoundSFXGeneratorController>().GenerateSound("EconomyInsuff");
                    FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(ArsenalServiceController.transform.position,
                    "insufficient money.", 5f);
                }
            }
        }
    }
    #endregion
    #region Repair 
    // Group all function that serve the same algorithm
    public void Repair()
    {
        int amount = service.CurrentHP + (service.MaxHP * HealPercent / 100);
        if (amount > service.MaxHP)
        {
            amount = service.MaxHP;
        }
        string check = FindAnyObjectByType<AccessDatabase>().RepairService(service.SessionID, amount);
        switch (check)
        {
            case "Fail":
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(ArsenalServiceController.transform.position,
                "Purchase Failed.\nPlease contact our email.", 5f); break;
            case "Success":
                FindObjectOfType<SoundSFXGeneratorController>().GenerateSound("Repair");
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(ArsenalServiceController.transform.position,
                "Repair Successfully!.", 5f); 
                FindAnyObjectByType<AccessDatabase>().DecreaseCurrencyAfterBuyForSession(service.SessionID, service.PriceToRepair);
                service.ResetAfterRepair();
                break;
        }       
    }
    #endregion
}
