using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonalAreaItem : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject PersonalArea;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    private PersonalArea PAController;
    public string ItemId;
    public bool isLocked;
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
    #region check mouse
    // Group all function that serve the same algorithm
    private void OnMouseDown()
    {
        PAController = PersonalArea.GetComponent<PersonalArea>();
        PAController.ShowRankInfo(ItemId);
    }
    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    #endregion
}
