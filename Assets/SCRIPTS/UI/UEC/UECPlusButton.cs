using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UECPlusButton : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public string Action;
    public string ConvertFrom;
    public string ConvertTo;
    public float ConvertRate;
    public GameObject Scene;
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
    #region Mouse Check
    private void OnMouseDown()
    {
        if ("Convert".Equals(Action))
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalConvertBoard(Scene.transform.position, ConvertFrom, ConvertTo, ConvertRate);
        } else if ("Recharge".Equals(Action))
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalRechargeBoard(Scene.transform.position);
        }
    }
    private void OnMouseEnter()
    {
        FindObjectOfType<NotificationBoardController>().CreateNormalInformationBoard(gameObject, InfoText);
    }
    private void OnMouseExit()
    {
        FindObjectOfType<NotificationBoardController>().DestroyCurrentInfoBoard();
    }
    #endregion
}
