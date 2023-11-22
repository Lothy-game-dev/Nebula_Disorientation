using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PersonalAreaRename : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public TMP_InputField TextInput;
    public 
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
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
    #region Rename action
    // Group all function that serve the same algorithm
    public void RenameAction()
    {
        if (TextInput.text.Length < 6)
        {
            FindAnyObjectByType<NotificationBoardController>().CreateNormalNotiBoard(gameObject.transform.position,
               "Pilot Name must have at least 6 characters.", 5f);
        } else
        {
            int PlayerId = FindAnyObjectByType<UECMainMenuController>().PlayerId;
            string check = FindAnyObjectByType<AccessDatabase>().UpdatePlayerProfileName(PlayerId, TextInput.text);
            switch (check)
            {
                case "Not Exist":
                    FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(gameObject.transform.position,
                        "Can not fetch data about your pilot.\nplease contact our email.", 5f); break;
                case "Fail":
                    FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(gameObject.transform.position,
                        "Renaming Failed!\nPlease try again later!", 5f); break;
                case "Success":
                    FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(gameObject.transform.position,
                        "Successfully Renamed.", 5f);
                    FindObjectOfType<UECMainMenuController>().GetData();
                    Destroy(gameObject.transform.parent.gameObject);
                    break;
            }
        }
    }
    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    #endregion
}
