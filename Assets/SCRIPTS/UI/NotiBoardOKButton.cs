using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotiBoardOKButton : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject NotiBoard;
    #endregion
    #region NormalVariables
    public bool MoveToUEC;
    private float currentTimer;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        currentTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        currentTimer += Time.deltaTime;
    }
    #endregion
    #region Mouse Check
    private void OnMouseDown()
    {
        if ("OKButton".Equals(name) || "CancelButton".Equals(name))
        {
            Destroy(NotiBoard);
        } else if ("CertainButton".Equals(name))
        {
            FindObjectOfType<NotificationBoardController>().ConfirmOnConfirmationBoard();
            Destroy(NotiBoard);
        }
    }

    private void OnDestroy()
    {
        if ("OKButton".Equals(name) && MoveToUEC)
        {
            FindObjectOfType<MainMenuCameraController>().MoveToUEC();
        }   
    }
    #endregion
}
