using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDCreateInfoBoard : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public List<GameObject> CreatePos;
    public List<string> Text;
    public List<string> TopBottomLeftRight;
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
        if (CreatePos == null)
        CreatePos = new List<GameObject>();
        if (Text==null)
        Text = new List<string>();
        if (TopBottomLeftRight==null)
        TopBottomLeftRight = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    private void OnMouseEnter()
    {
        for (int i=0;i<Text.Count;i++)
        {
            FindObjectOfType<NotificationBoardController>().
                CreateHUDSmallInfoBoard(CreatePos[i], Text[i], TopBottomLeftRight[i]);
        }
    }
    private void OnMouseExit()
    {
        FindObjectOfType<NotificationBoardController>().DestroyAllCurrentHUDSmallBoard();
    }

    private void OnDisable()
    {
        FindObjectOfType<NotificationBoardController>().DestroyAllCurrentHUDSmallBoard();
    }
    #endregion
}
