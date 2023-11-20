using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryScene : UECMenuShared
{

    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public Factory FactoryController;
    public GameObject Tutorial;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public bool FromLoadout;
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
    // Group all function that serve the same algorithm
    #endregion
    #region Animation
    // Group all function that serve the same algorithm
    public override void OnEnterAnimation()
    {
        GetComponent<BackgroundBrieflyMoving>().enabled = true;
        transform.GetChild(0).GetComponent<Rigidbody2D>().simulated = true;
        FindObjectOfType<MainMenuCameraController>().GenerateLoadingSceneAtPos(transform.position, 1f);
        FactoryController.SetFirstData();      
        Tutorial.SetActive(true);
        if (FromLoadout)
        {
            Tutorial.GetComponent<FirstTimeTutorial>().Section = 0;
            FromLoadout = false;
        }
    }

    public override void OnExitAnimation()
    {
        GetComponent<BackgroundBrieflyMoving>().enabled = false;
        transform.GetChild(0).GetComponent<Rigidbody2D>().simulated = false;
        Tutorial.SetActive(false);
        FactoryController.ResetData();
    }
    #endregion
}
