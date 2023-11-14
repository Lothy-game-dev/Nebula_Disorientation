using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadOutTextRed : MonoBehaviour
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
    private float ResetColorTimer;
    private bool isWhite;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        GetComponent<TextMeshPro>().color = Color.white;
        isWhite = false;
        ResetColorTimer = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (ResetColorTimer <= 0f)
        {
            if (isWhite)
            {
                isWhite = false;
                ResetColorTimer = 1f;
            } else
            {
                isWhite = true;
                ResetColorTimer = 1f;
            }
        } else
        {
            ResetColorTimer -= Time.deltaTime;
            if (isWhite)
            {
                Color c = GetComponent<TextMeshPro>().color;
                c.g += Time.deltaTime;
                c.b += Time.deltaTime;
                GetComponent<TextMeshPro>().color = c;
            } else
            {
                Color c = GetComponent<TextMeshPro>().color;
                c.g -= Time.deltaTime;
                c.b -= Time.deltaTime;
                GetComponent<TextMeshPro>().color = c;
            }
        }
    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    #endregion
}
