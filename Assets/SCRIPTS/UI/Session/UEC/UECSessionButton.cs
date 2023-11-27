using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UECSessionButton : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public string Type;
    public GameObject NextSZInfo;
    public GameObject[] DisableColliders;
    public GameObject LOTW;
    public GameObject Scene;
    public GameplayExteriorController Controller;
    #endregion
    #region NormalVariables
    private float InitScaleX;
    private float InitScaleY;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        InitScaleX = transform.localScale.x;
        InitScaleY = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Mouse
    // Group all function that serve the same algorithm
    private void OnMouseEnter()
    {
        transform.localScale = new Vector3(InitScaleX * 1.05f, InitScaleY * 1.05f, transform.localScale.z);
        
    }

    private void OnMouseOver()
    {
        if (Type == "Continue")
        {
            transform.GetChild(0).Rotate(new Vector3(0, 0, 2f));
            transform.GetChild(1).Rotate(new Vector3(0, 0, -2f));
        }
    }

    private void OnMouseExit()
    {
        transform.localScale = new Vector3(InitScaleX, InitScaleY, transform.localScale.z);

    }

    private void OnMouseDown()
    {
        FindObjectOfType<SoundSFXGeneratorController>().GenerateSound("ButtonClick");
        if (Type=="Continue")
        {
            FindObjectOfType<NotificationBoardController>().VoidReturnFunction = Continue;
            FindObjectOfType<NotificationBoardController>().CreateNormalConfirmBoard(Scene.transform.position,
                "Ready to continue?");
 
        } else if (Type=="NextSZInfo")
        {
            NextSZInfo.SetActive(true);
            foreach (var col in DisableColliders)
            {
                if (col.GetComponent<Collider2D>() != null)
                    col.GetComponent<Collider2D>().enabled = false;
            }
        } else if (Type=="SaveQuit")
        {
            // Quit To MainMenu
            FindAnyObjectByType<StatisticController>().UpdateSessionPlaytime();
            PlayerPrefs.SetFloat("CreateLoading", 1f);
            SceneManager.UnloadSceneAsync("GameplayExterior");
            SceneManager.LoadSceneAsync("MainMenu");
        } else if (Type=="Retreat")
        {
            FindObjectOfType<NotificationBoardController>().VoidReturnFunction = Retreat;
            FindObjectOfType<NotificationBoardController>().CreateNormalConfirmBoard(Scene.transform.position,
                "Retreat already?");
        }
    }

    public void Continue()
    {
        FindAnyObjectByType<StatisticController>().UpdateSessionPlaytime();
        Controller.GenerateBlackFadeClose(0.5f, 3f);
        StartCoroutine(MoveToLOTW());       
    }

    public void Retreat()
    {
        Dictionary<string, object> Data = FindObjectOfType<AccessDatabase>().GetSessionInfoByPlayerId(PlayerPrefs.GetInt("PlayerID"));
        if ((int)Data["SessionFuelCore"] >= 1)
        {
            FindAnyObjectByType<StatisticController>().UpdateSessionPlaytime();
            FindObjectOfType<AccessDatabase>().UpdateSessionFuelCore(PlayerPrefs.GetInt("PlayerID"), false);
            FindObjectOfType<GameplayExteriorController>().GenerateLoadingScene(2f);
            FindObjectOfType<GameplayExteriorController>().GenerateBlackFadeClose(1f, 4f);
            StartCoroutine(MoveToSessionSum());
        }
        else
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                "Insufficient\nFuel Core.\nCannot retreat!", 3f);
        }
    }

    private IEnumerator MoveToLOTW()
    {
        yield return new WaitForSeconds(1f);
        Scene.GetComponent<BackgroundBrieflyMoving>().enabled = false;
        FindObjectOfType<AccessDatabase>().AddSessionCurrentSaveData(PlayerPrefs.GetInt("PlayerID"), "LOTW");
        Controller.GenerateLoadingScene(2f);
        Controller.ChangeToScene(LOTW);
        gameObject.SetActive(false);
        Scene.SetActive(false);
    }
    
    private IEnumerator MoveToSessionSum()
    {
        yield return new WaitForSeconds(2f);
        // End Session
        PlayerPrefs.SetString("isFailed", "F");
        SceneManager.LoadSceneAsync("SessionSummary");
        SceneManager.UnloadSceneAsync("GameplayExterior");
    }

    private IEnumerator MoveToGameplay()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadSceneAsync("GameplayInterior");
        SceneManager.UnloadSceneAsync("GameplayExterior");
    }
    #endregion
}
