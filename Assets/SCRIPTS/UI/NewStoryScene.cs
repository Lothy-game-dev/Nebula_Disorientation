using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewStoryScene : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public InputField NameField;
    #endregion
    #region NormalVariables
    private bool startWriting;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        startWriting = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (Input.GetKeyDown(KeyCode.Return))
        {
            NameField.ActivateInputField();
        }
    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    #endregion
}
