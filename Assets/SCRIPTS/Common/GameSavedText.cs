using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameSavedText : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public float FadingCountDown;
    public bool AlreadySetCountDown;
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
        if (AlreadySetCountDown)
        {
            if (FadingCountDown >0)
            {
                if (Time.timeScale!=0)
                {
                    FadingCountDown -= Time.deltaTime;
                } else
                FadingCountDown -= 1 / 60f;
            } else
            {
                Color c = GetComponent<TextMeshPro>().color;
                if (c.a > 0)
                {
                    if (Time.timeScale != 0)
                    {
                        c.a -= Time.deltaTime;
                    }
                    else
                        c.a -= 1 / 60f;
                    GetComponent<TextMeshPro>().color = c;
                } else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    #endregion
}
