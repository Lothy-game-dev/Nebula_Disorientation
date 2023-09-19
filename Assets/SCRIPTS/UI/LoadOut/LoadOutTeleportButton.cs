using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadOutTeleportButton : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject FromScene;
    public GameObject ToScene;
    public GameObject Bar;
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
    #region Mouse
    private void OnMouseDown()
    {
        if (Bar.GetComponent<LoadOutPowerBar>() != null)
        {
            Bar.GetComponent<LoadOutPowerBar>().OnBackgroundMouseDown();
        }
        else if (Bar.GetComponent<LoadOutConsumables>() != null)
        {
            Bar.GetComponent<LoadOutConsumables>().OnBackgroundMouseDown();
        }
        FindObjectOfType<UECMainMenuController>().TeleportToScene(FromScene, ToScene);
    }
    #endregion
}
