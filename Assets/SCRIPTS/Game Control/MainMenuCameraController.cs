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
    public GameObject OptionScene;
    public GameObject EncyclopediaScene;
    #endregion
    #region NormalVariables
    private GameObject CurrentScene;
    private GameObject Load;
    private InitializeDatabase db;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Awake()
    {
        db = GetComponent<InitializeDatabase>();
        //db.DropDatabase();
        db.Initialization();
        // Initialize variables
        CurrentScene = StartScene;
    }

    private void OnEnable()
    {
        // Check teleport to scene: Use when back from UEC
        if (PlayerPrefs.GetFloat("CreateLoading") > 0f)
        {
            if (PlayerPrefs.GetInt("ToOption") == 1)
            {
                PlayerPrefs.SetInt("ToOption", 0);
                ChangeToScene(StartScene, OptionScene);
                CurrentScene = OptionScene;
            } else if (PlayerPrefs.GetInt("ToEncyclopedia") == 1)
            {
                PlayerPrefs.SetInt("ToEncyclopedia", 0);
                ChangeToScene(StartScene, EncyclopediaScene);
                CurrentScene = EncyclopediaScene;
            }
            GenerateLoadingScene(PlayerPrefs.GetFloat("CreateLoading"));
            PlayerPrefs.SetFloat("CreateLoading", 0f);
        }
    }
    // Update is called once per frame
    void Update()
    {
    }
    #endregion
    #region Scene Control
    public void ChangeToScene(GameObject FromScene, GameObject SceneGO)
    {
        MainMenuSceneShared m = FromScene.GetComponent<MainMenuSceneShared>();
        if (m != null)
        {
            m.EndAnimation();
        }
        if (SceneGO!=null)
        {
            CurrentScene = SceneGO;
        } else
        {
            CurrentScene = StartScene;
        }
        transform.position = new Vector3(CurrentScene.transform.position.x, CurrentScene.transform.position.y,transform.position.z);
        MainMenuSceneShared m2 = SceneGO.GetComponent<MainMenuSceneShared>();
        if (m2 != null)
        {
            m2.StartAnimation(); 
        }
    }
    public void BackToMainMenu(GameObject FromScene)
    {
        MainMenuSceneShared m = FromScene.GetComponent<MainMenuSceneShared>();
        if (m != null)
        {
            m.EndAnimation();
        }
        if (CurrentScene!=StartScene)
        {
            CurrentScene = StartScene;
        }
        MainMenuSceneShared m2 = CurrentScene.GetComponent<MainMenuSceneShared>();
        if (m2 != null)
        {
            m2.StartAnimation();
        }
        transform.position = new Vector3(CurrentScene.transform.position.x, CurrentScene.transform.position.y, transform.position.z);
        foreach (var but in MainMenuButtons)
        {
            but.EnterView();
        }
    }

    public void GenerateLoadingScene(float sec)
    {
        if (CurrentScene==null)
        {
            CurrentScene = StartScene;
        }
        Load = Instantiate(LoadingScene, 
            new Vector3(CurrentScene.transform.position.x, 
            CurrentScene.transform.position.y, 
            LoadingScene.transform.position.z), Quaternion.identity);
        Load.GetComponent<SpriteRenderer>().sortingOrder = 50;
        Load.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 101;
        Load.transform.GetChild(2).GetComponent<SpriteRenderer>().sortingOrder = 100;       
        Load.transform.GetChild(0).GetComponent<LoadingScene>().LoadingTime = sec;
        Load.SetActive(true);
    }

    public void GenerateLoadingSceneAtPos(Vector2 Pos, float sec)
    {
        Load = Instantiate(LoadingScene,
            new Vector3(Pos.x, Pos.y,
            LoadingScene.transform.position.z), Quaternion.identity);
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
