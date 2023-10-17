using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalAreaCollectButton : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject PersonalArea;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    private PersonalArea PAController;
    public DateTime ResetDateTime;
    public DateTime CollectedTime;
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
    #region Check mouse
    // Group all function that serve the same algorithm
    private void OnMouseDown()
    {      
        PAController = PersonalArea.GetComponent<PersonalArea>();
        if (PAController.isUnranked)
        {
            FindAnyObjectByType<NotificationBoardController>().CreateNormalNotiBoard(PersonalArea.transform.position,
                "You are still unranked. Please try again later!", 5f);
        } else
        {
            if (!PAController.IsCollected)
            {
                PAController.IsCollected = true;
                CollectDailyIncome();
            } else
            {
                FindAnyObjectByType<NotificationBoardController>().CreateNormalNotiBoard(PersonalArea.transform.position,
                        "You have already collected salary!", 5f);
                
            }
        }
    }
    #endregion
    #region Collect salary
    public void CollectDailyIncome()
    {
        string check = FindAnyObjectByType<AccessDatabase>().CollectSalary(FindObjectOfType<UECMainMenuController>().PlayerId, (int)PAController.PlayerInformation["DailyIncome"]);
        switch (check)
        {
            case "Not Exist":
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(PersonalArea.transform.position,
                    "Can not fetch data about your pilot.\nplease contact our email.", 5f); break;
            case "Fail":
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(PersonalArea.transform.position,
                    "Collect Failed.\nPlease try again later.", 5f); break;
            case "Success":
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(PersonalArea.transform.position,
                    "Collect Successfully.\nSee you tomorrow!.", 5f);
                FindObjectOfType<UECMainMenuController>().GetData();
                gameObject.AddComponent<CursorUnallowed>();
                FindAnyObjectByType<AccessDatabase>().SalaryCollected(FindObjectOfType<UECMainMenuController>().PlayerId);
                FindAnyObjectByType<AccessDatabase>().UpdateEconomyStatistic(FindObjectOfType<UECMainMenuController>().PlayerId, 0, (int)PAController.PlayerInformation["DailyIncome"], "ReceiveSalary");
                gameObject.transform.GetChild(1).gameObject.SetActive(true);
                break;
        }
    }
    #endregion

}
