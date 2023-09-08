using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UECMainMenuController : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public UECController controller;
    #endregion
    #region NormalVariables
    public int PlayerId;
    private AccessDatabase ad;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        ad = FindObjectOfType<AccessDatabase>();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Retrieve Data
    public void GetData()
    {
        PlayerId = ad.GetCurrentSessionPlayerId();
        Dictionary<string,object> ListData = ad.GetPlayerInformationById(PlayerId);
        if (ListData!=null)
        {
            controller.SetDataToView(ListData);
        } else
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
                "Can not find your pilot's data.\n Please try again or contact to our email.", 5f);
        }
    }
    #endregion
}
