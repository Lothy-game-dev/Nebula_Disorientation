using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OptionButton : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject OptionBoard;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    private OptionMenu OptionMenu;
    private string FpsCounter;
    public bool isClick;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        OptionMenu = OptionBoard.GetComponent<OptionMenu>();
    }

    // Update is called once per frame
    void Update()
    { 
        // Call function and timer only if possible
    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    private void OnMouseDown()
    {
        if ("LeftButton".Equals(gameObject.name))
        {
            OptionMenu.IsClicked = true;
            if (("30").Equals(OptionMenu.FpsCounter))
            {
                OptionMenu.FpsCounter = "90";
            } else
            {
                int down = int.Parse(OptionMenu.FpsCounter) - 30;
                OptionMenu.FpsCounter = down.ToString();
            }

        } else
        {
            if ("RightButton".Equals(gameObject.name))
            {
                OptionMenu.IsClicked = true;
                if (("90").Equals(OptionMenu.FpsCounter))
                {
                    OptionMenu.FpsCounter = "30";
                }
                else
                {
                    int down = int.Parse(OptionMenu.FpsCounter) + 30;
                    OptionMenu.FpsCounter = down.ToString();
                }
            }
        }
    }
    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    #endregion
}
