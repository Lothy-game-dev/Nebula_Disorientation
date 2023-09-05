using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCameraController : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject StartScene;
    public MainMenuButtons[] MainMenuButtons;
    #endregion
    #region NormalVariables
    private GameObject CurrentScene;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        CurrentScene = StartScene;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Scene Control
    public void ChangeToScene(GameObject SceneGO)
    {
        if (SceneGO!=null)
        {
            CurrentScene = SceneGO;
        } else
        {
            CurrentScene = StartScene;
        }
        transform.position = new Vector3(CurrentScene.transform.position.x, CurrentScene.transform.position.y,transform.position.z);
        MainMenuSceneShared m = SceneGO.GetComponent<MainMenuSceneShared>();
        if (m != null)
        {
            m.StartAnimation(); 
        }
    }
    public void BackToMainMenu()
    {
        if (CurrentScene!=StartScene)
        {
            CurrentScene = StartScene;
        }
        transform.position = new Vector3(CurrentScene.transform.position.x, CurrentScene.transform.position.y, transform.position.z);
        foreach (var but in MainMenuButtons)
        {
            but.EnterView();
        }
    }
    #endregion
}
