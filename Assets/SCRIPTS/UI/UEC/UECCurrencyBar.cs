using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UECCurrencyBar : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public string InfoText;
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
    #region MouseCheck
    private void OnMouseEnter()
    {
        FindObjectOfType<NotificationBoardController>().CreateNormalInformationBoard(gameObject, 
            name.Contains("FuelCell") ? InfoText + FindObjectOfType<UECController>().RegenFuelTime : InfoText);
    }

    private void OnMouseExit()
    {
        FindObjectOfType<NotificationBoardController>().DestroyCurrentInfoBoard();
    }
    #endregion
}
