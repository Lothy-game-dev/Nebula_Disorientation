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
    public GameObject BlackFade;
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
        if (db!=null)
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
        if (CurrentScene == null)
            CurrentScene = StartScene;
        GameObject bf = Instantiate(BlackFade, new Vector3(CurrentScene.transform.position.x, CurrentScene.transform.position.y, BlackFade.transform.position.z), Quaternion.identity);
        Color c = bf.GetComponent<SpriteRenderer>().color;
        c.a = 1;
        bf.GetComponent<SpriteRenderer>().color = c;
        bf.SetActive(true);
        StartCoroutine(BlackFadeOpen(bf, sec));
    }

    private IEnumerator BlackFadeOpen(GameObject Fade, float duration)
    {
        for (int i=0;i<50;i++)
        {
            Color c = Fade.GetComponent<SpriteRenderer>().color;
            c.a -= 1/50f;
            Fade.GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(duration / 50f);
        }
        Destroy(Fade);
    }
    public void GenerateLoadingSceneAtPos(Vector2 pos, float sec)
    {
        GameObject bf = Instantiate(BlackFade, new Vector3(pos.x, pos.y, BlackFade.transform.position.z), Quaternion.identity);
        Color c = bf.GetComponent<SpriteRenderer>().color;
        c.a = 1;
        bf.GetComponent<SpriteRenderer>().color = c;
        bf.SetActive(true);
        StartCoroutine(BlackFadeOpen(bf, sec));
    }

    public void GenerateBlackFadeWaitAtPos(Vector2 pos, float sec, float Wait)
    {
        GameObject bf = Instantiate(BlackFade, new Vector3(pos.x, pos.y, BlackFade.transform.position.z), Quaternion.identity);
        Color c = bf.GetComponent<SpriteRenderer>().color;
        c.a = 1;
        bf.GetComponent<SpriteRenderer>().color = c;
        bf.SetActive(true);
        StartCoroutine(BlackFadeWait(bf, sec, Wait));
    }

    private IEnumerator BlackFadeWait(GameObject Fade, float duration, float wait)
    {
        yield return new WaitForSeconds(wait);
        for (int i = 0; i < 50; i++)
        {
            Color c = Fade.GetComponent<SpriteRenderer>().color;
            c.a -= 1 / 50f;
            Fade.GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(duration / 50f);
        }
        Destroy(Fade);
    }

    public void GenerateBlackFadeClose(float duration, float wait)
    {
        GameObject bf = Instantiate(BlackFade, new Vector3(transform.position.x, transform.position.y, BlackFade.transform.position.z), Quaternion.identity);
        Color c = bf.GetComponent<SpriteRenderer>().color;
        c.a = 0;
        bf.GetComponent<SpriteRenderer>().color = c;
        bf.SetActive(true);
        StartCoroutine(BlackFadeClose(bf, duration, wait));
    }

    private IEnumerator BlackFadeClose(GameObject Fade, float duration, float wait)
    {
        for (int i = 0; i < 50; i++)
        {
            Color c = Fade.GetComponent<SpriteRenderer>().color;
            c.a += 1 / 50f;
            Fade.GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(duration / 50f);
        }
        yield return new WaitForSeconds(wait);
        Destroy(Fade);
    }

    public void MoveToUEC()
    {
        GenerateBlackFadeClose(1f,4f);
        StartCoroutine(MoveToUECDelay());
    }

    private IEnumerator MoveToUECDelay()
    {
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadSceneAsync("UECMainMenu");
        SceneManager.UnloadSceneAsync("MainMenu");
    }

    public void MoveToUECSession(int PlayerID)
    {
        GenerateLoadingScene(1f);
        PlayerPrefs.SetInt("PlayerID", PlayerID);
        transform.GetChild(0).gameObject.SetActive(true);
        StartCoroutine(MoveToUECTime());
    }

    private IEnumerator MoveToUECTime()
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
