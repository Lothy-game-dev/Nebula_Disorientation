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
    public GameObject Text;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    private OptionMenu OptionMenu;
    private string FpsCounter;
    public bool isClick;
    private List<string> ResolList;
    private Vector3 InitScale;
    private float ExpectedScale;    
    private bool alreadySelect;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        OptionMenu = OptionBoard.GetComponent<OptionMenu>();
        ResolList = new List<string>() {"FullScreen", "1920x1080", "1600x900" ,"1280x720" };
        InitScale = transform.localScale;
        ExpectedScale = transform.localScale.x * 1.1f;
    }

    // Update is called once per frame
    void Update()
    { 
        // Call function and timer only if possible
    }
    #endregion
    #region Change the setting when click
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
            FindAnyObjectByType<AccessDatabase>().UpdateOptionSetting(Mathf.RoundToInt(float.Parse(OptionMenu.Master)),
                                    Mathf.RoundToInt(float.Parse(OptionMenu.Music)), Mathf.RoundToInt(float.Parse(OptionMenu.Sound)), Mathf.RoundToInt(float.Parse(OptionMenu.FpsCounter)), OptionMenu.Resol);
            OptionMenu.IsSaved = true;
            Camera.main.GetComponent<SoundController>().CheckSoundVolumeByDB();
        }
    }
    #endregion
    private void OnMouseEnter()
    {
        alreadySelect = false;
    }
    //Highlight the button when the mouse over
    private void OnMouseOver()
    {
        if ("SaveButton".Equals(gameObject.name))
        {
            Color c = GetComponent<SpriteRenderer>().color;
            c.r -= 0.01f;
            c.b -= 0.01f;
            GetComponent<SpriteRenderer>().color = c;
            GetComponent<SpriteRenderer>().sortingOrder = 3;
            Color c2 = Text.GetComponent<SpriteRenderer>().color;
            c2.r -= 0.01f;
            c2.g -= 0.01f;
            c2.b -= 0.01f;
            Text.GetComponent<SpriteRenderer>().color = c2;
            Text.GetComponent<SpriteRenderer>().sortingOrder = 4;
            if (transform.localScale.x < ExpectedScale)
            {
                transform.localScale = new Vector3
                    (transform.localScale.x + InitScale.x * 0.002f,
                    transform.localScale.y + InitScale.y * 0.002f,
                    transform.localScale.z + InitScale.z * 0.002f);
            }
        }
    }
    //Opposite to the above function
    private void OnMouseExit()
    {
        if (!alreadySelect && "SaveButton".Equals(gameObject.name))
        {
            Color c = GetComponent<SpriteRenderer>().color;
            c.r = 1;
            c.b = 1;
            GetComponent<SpriteRenderer>().color = c;
            GetComponent<SpriteRenderer>().sortingOrder = 3;
            Color c2 = Text.GetComponent<SpriteRenderer>().color;
            c2.r = 1;
            c2.g = 1;
            c2.b = 1;
            Text.GetComponent<SpriteRenderer>().color = c2;
            Text.GetComponent<SpriteRenderer>().sortingOrder = 4;
            transform.localScale = new Vector3(InitScale.x, InitScale.y, InitScale.z);
        }
    }

}
