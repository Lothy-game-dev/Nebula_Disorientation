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
    }

    private void OnEnable()
    {
        // Check teleport to scene: Use when back from UEC
        if (PlayerPrefs.GetFloat("CreateLoading") > 0f)
        {
            // Case from UEC to Option menu
            if (PlayerPrefs.GetInt("ToOption") == 1)
            {
                // Reset data for playerprefs
                PlayerPrefs.SetInt("ToOption", 0);
                ChangeToScene(StartScene, OptionScene);
                CurrentScene = OptionScene;
            }
            // Case from UEC to Encyc menu
            else if (PlayerPrefs.GetInt("ToEncyclopedia") == 1)
            {
                PlayerPrefs.SetInt("ToEncyclopedia", 0);
                ChangeToScene(StartScene, EncyclopediaScene);
                CurrentScene = EncyclopediaScene;
            }
            // Loading
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
        Debug.Log(Load);
        Debug.Log(Load.transform.position);
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

    public void MoveToUECSession(int PlayerID)
    {
        GenerateLoadingScene(1f);
        PlayerPrefs.SetInt("PlayerID", PlayerID);
        transform.GetChild(0).gameObject.SetActive(true);
        StartCoroutine(MoveToUECDelay());
    }

    private IEnumerator MoveToUECDelay()
    {
        yield return new WaitForSeconds(1f);
        PlayerPrefs.SetString("InitTeleport", "UEC");
        SceneManager.LoadSceneAsync("GameplayExterior");
        SceneManager.UnloadSceneAsync("MainMenu");
    }

    public void MoveToLOTWScene(int PlayerID)
    {
        GenerateLoadingScene(1f);
        PlayerPrefs.SetInt("PlayerID", PlayerID);
        transform.GetChild(0).gameObject.SetActive(true);
        StartCoroutine(MoveToLOTWDelay());
    }

    private IEnumerator MoveToLOTWDelay()
    {
        yield return new WaitForSeconds(1f);
        PlayerPrefs.SetString("InitTeleport", "LOTW");
        SceneManager.LoadSceneAsync("GameplayExterior");
        SceneManager.UnloadSceneAsync("MainMenu");
    }

    public void MoveToGameplay(int PlayerID)
    {
        GenerateLoadingScene(1f);
        PlayerPrefs.SetInt("PlayerID", PlayerID);
        transform.GetChild(0).gameObject.SetActive(true);
        StartCoroutine(MoveToGameplayDelay());
    }

    private IEnumerator MoveToGameplayDelay()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadSceneAsync("GameplayInterior");
        SceneManager.UnloadSceneAsync("MainMenu");
    }
    #endregion
}
