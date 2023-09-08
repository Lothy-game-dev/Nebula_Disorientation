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
    private List<string> ResolList;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        OptionMenu = OptionBoard.GetComponent<OptionMenu>();
        ResolList = new List<string>() {"FullScreen", "1920x1080", "1600x900" ,"1280x720" };
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

        int index = ResolList.IndexOf(OptionMenu.Resol);
        if (("LeftResolButton").Equals(gameObject.name)) {
            OptionMenu.IsClicked = true;
            if ("FullScreen".Equals(OptionMenu.Resol))
            {
                OptionMenu.Resol = ResolList[2];
            } else
            {
                OptionMenu.Resol = ResolList[index-1];
            }
        } else
        {
            if (("RightResolButton").Equals(gameObject.name))
            {
                OptionMenu.IsClicked = true;
                if ("1280x720".Equals(OptionMenu.Resol))
                {
                    OptionMenu.Resol = ResolList[0];
                }
                else
                {
                    OptionMenu.Resol = ResolList[index + 1];
                }
            }
        }
        if ("SaveButton".Equals(gameObject.name))
        {
            Debug.Log(float.Parse(OptionMenu.Master));
            FindAnyObjectByType<AccessDatabase>().UpdateOptionSetting(Mathf.RoundToInt(float.Parse(OptionMenu.Master)),
                                    Mathf.RoundToInt(float.Parse(OptionMenu.Music)), Mathf.RoundToInt(float.Parse(OptionMenu.Sound)), Mathf.RoundToInt(float.Parse(OptionMenu.FpsCounter)), OptionMenu.Resol);
            Debug.Log("1");
        }
    }
    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    #endregion
}
