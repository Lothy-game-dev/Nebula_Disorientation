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
    public GameObject LoadingScene;
    #endregion
    #region NormalVariables
    private GameObject CurrentScene;
    private GameObject Load;
    private Vector2 Direc;
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
        if (Load != null && Load.activeSelf)
        {
            Load.transform.GetChild(2).Translate(new Vector3(Direc.x, Direc.y, 0) * Time.deltaTime);
        }
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
        GenerateLoadingScene(0.5f);
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
        Direc = new Vector2(Random.Range(-1, 2), Random.Range(-1, 2)) / sec;
        Load = Instantiate(LoadingScene, new Vector3(CurrentScene.transform.position.x, CurrentScene.transform.position.y, LoadingScene.transform.position.z), Quaternion.identity);
        Load.GetComponent<SpriteRenderer>().sortingOrder = 50;
        Load.transform.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 101;
        Load.transform.GetChild(2).GetComponent<SpriteRenderer>().sortingOrder = 100;       
        Load.transform.GetChild(0).GetComponent<LoadingScene>().LoadingTime = sec;
        Load.SetActive(true);
        Destroy(Load, sec);
    }
    #endregion
}
