using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuCameraController : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject StartScene;
    public MainMenuButtons[] MainMenuButtons;
    public GameObject LoadingScene;
    #endregion
    #region NormalVariables
    private GameObject CurrentScene;
    private GameObject Load;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Awake()
    {
        // Initialize variables
        CurrentScene = StartScene;
    }

    // Update is called once per frame
    void Update()
    {
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

    public void GenerateLoadingScene(float sec)
    {
        Load = Instantiate(LoadingScene, new Vector3(CurrentScene.transform.position.x, CurrentScene.transform.position.y, LoadingScene.transform.position.z), Quaternion.identity);
        Load.GetComponent<SpriteRenderer>().sortingOrder = 50;
        Load.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 101;
        Load.transform.GetChild(2).GetComponent<SpriteRenderer>().sortingOrder = 100;       
        Load.transform.GetChild(0).GetComponent<LoadingScene>().LoadingTime = sec;
        Load.SetActive(true);
    }

    public void MoveToUEC()
    {
        SceneManager.LoadSceneAsync("UECMainMenu");
        SceneManager.UnloadSceneAsync("MainMenu");
    }
    #endregion
}
