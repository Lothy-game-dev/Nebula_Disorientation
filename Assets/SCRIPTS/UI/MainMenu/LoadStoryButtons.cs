using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadStoryButtons : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public LoadStoryScene Scene;
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
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Function group 1

    private void OnMouseDown()
    {
        FindObjectOfType<SoundSFXGeneratorController>().GenerateSound("ButtonClick");
        if (name=="Delete")
        {
            Scene.CheckBeforeDelete();
        } else if (name=="Onward")
        {
            Scene.CheckOnward();
        }
    }
    #endregion
}
