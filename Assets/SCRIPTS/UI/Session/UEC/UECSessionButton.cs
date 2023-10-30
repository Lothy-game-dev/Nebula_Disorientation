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
        if (Type=="Continue")
        {
            Dictionary<string, object> SessionData = FindObjectOfType<AccessDatabase>().GetSessionInfoByPlayerId(PlayerPrefs.GetInt("PlayerID"));
            // Get Next Stage Number, Variant and Hazard
            int SpaceZoneNo = (int)SessionData["CurrentStage"] + 1;
            int ChosenVariant;
            int ChosenHazard;
            if (PlayerPrefs.GetInt("Variant") == 0)
            {
                Dictionary<string, object> variantData = FindObjectOfType<AccessDatabase>().GetVariantCountsAndBackgroundByStageValue(SpaceZoneNo % 10);
                int VariantCount = (int)variantData["VariantCounts"];
                if (SpaceZoneNo < 51)
                {
                    if (SpaceZoneNo % 10 == 0)
                    {
                        ChosenVariant = 2;
                    }
                    else if (SpaceZoneNo % 10 == 8 || SpaceZoneNo % 10 == 9)
                    {
                        ChosenVariant = 1;
                    }
                    else
                        ChosenVariant = Random.Range(1, 1 + VariantCount);
                }
                else
                    ChosenVariant = Random.Range(1, 1 + VariantCount);
            }
            else
            {
                ChosenVariant = PlayerPrefs.GetInt("Variant");
                PlayerPrefs.SetInt("Variant", 0);
            }
            if (PlayerPrefs.GetInt("Hazard") == 0)
            {
                List<Dictionary<string, object>> ListAvailableHazard = FindObjectOfType<AccessDatabase>().GetAvailableHazards(SpaceZoneNo);
                ChosenHazard = RandomHazardChoose(ListAvailableHazard);
            }
            else
            {
                ChosenHazard = PlayerPrefs.GetInt("Hazard");
                PlayerPrefs.GetInt("Hazard", 0);
            }
            // Update
            FindObjectOfType<AccessDatabase>().UpdateSessionStageData((int)SessionData["SessionID"], SpaceZoneNo, ChosenHazard, ChosenVariant);

            Controller.GenerateBlackFadeClose(1f, 3f);
            StartCoroutine(MoveToLOTW());
        } else if (Type=="NextSZInfo")
        {
            NextSZInfo.SetActive(true);
            foreach (var col in DisableColliders)
            {
                col.GetComponent<Collider2D>().enabled = false;
            }
        } else if (Type=="SaveQuit")
        {
            // Quit To MainMenu
            PlayerPrefs.SetFloat("CreateLoading", 1f);
            SceneManager.UnloadSceneAsync("GameplayExterior");
            SceneManager.LoadSceneAsync("MainMenu");
        } else if (Type=="Retreat")
        {
            Dictionary<string, object> Data = FindObjectOfType<AccessDatabase>().GetPlayerInformationById(PlayerPrefs.GetInt("PlayerID"));
            if ((int)Data["FuelCell"] > 1)
            {
                FindObjectOfType<AccessDatabase>().ReduceFuelCell(PlayerPrefs.GetInt("PlayerID"));
                // End Session
                PlayerPrefs.SetString("isFailed", "F");
                SceneManager.LoadSceneAsync("SessionSummary");
                SceneManager.UnloadSceneAsync("GameplayExterior");
            } else
            {
                FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(Scene.transform.position,
                    "Insufficient\nFuel Core.\nCannot retreat!", 3f);
            }
        }
    }

    private IEnumerator MoveToLOTW()
    {
        yield return new WaitForSeconds(1.5f);
        Scene.GetComponent<BackgroundBrieflyMoving>().enabled = false;
        Controller.GenerateBlackFadeOpen(LOTW.transform.position, 1.5f);
        Controller.ChangeToScene(LOTW);
    }

    private int RandomHazardChoose(List<Dictionary<string, object>> dataDict)
    {
        float sum = 0;
        for (int i = 0; i < dataDict.Count; i++)
        {
            sum += (int)dataDict[i]["HazardChance"];
        }
        float n = Random.Range(0, sum);
        for (int i = 0; i < dataDict.Count; i++)
        {
            if (n < (int)dataDict[i]["HazardChance"])
            {
                return (int)dataDict[i]["HazardID"];
            }
            else
            {
                n -= (int)dataDict[i]["HazardChance"];
            }
        }
        return 1;
    }
    #endregion
}
