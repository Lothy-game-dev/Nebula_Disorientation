using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncycCategory : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject Encyc;
    public string[] GroupName;
    public string Type;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    private EncycMenu Menu;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        Menu = Encyc.GetComponent<EncycMenu>();
        
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Check mouse
    // Group all function that serve the same algorithm
    private void OnMouseDown()
    {
        Menu.CurrentItem = gameObject;
        Menu.GenerateEncyc(Type, GroupName);
    }
    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    #endregion
}
