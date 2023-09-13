using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArsenalItem : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject Arsenal;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public string Id;
    public string Type;
    private Arsenal ar;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        ar = Arsenal.GetComponent<Arsenal>();
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
        Debug.Log(FindAnyObjectByType<GlobalFunctionController>().ConvertWeaponStatsToString(ar.WeaponList[int.Parse(Id) - 1][4]));
    }
    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    #endregion
}
