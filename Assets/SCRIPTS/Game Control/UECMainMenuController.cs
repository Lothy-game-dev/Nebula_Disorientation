using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UECMainMenuController : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject MainCamera;
    public UECController controller;
    public GameObject[] DisableUponActive;
    public Factory FactoryController;
    public Arsenal ArsenalController;
    public PersonalArea PAController;
    #endregion
    #region NormalVariables
    public int PlayerId;
    private AccessDatabase ad;
    public int Playedtime;
    public bool isStart;
    public bool isCount;
    private DateTime StartTime;
    public int BuyAmount;
    public int CashSpent;
    public int ShardSpent;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    private void Awake()
    {
        Camera.main.transparencySortAxis = new Vector3(0, 0, 1);
        isStart = false;
    }
    void OnEnable()
    {
        // Initialize variables
        ad = FindObjectOfType<AccessDatabase>();      
        ExitAnimationAllScene();
        GetData();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
        if (isStart)
        {
            SetTimer(StartTime);
        }
    }
    #endregion
    #region Retrieve Data
    public void GetData()
    {
        PlayerPrefs.SetInt("PlayerID", 0);
        PlayerId = ad.GetCurrentSessionPlayerId();
        Dictionary<string,object> ListData = ad.GetPlayerInformationById(PlayerId);
        if (ListData!=null)
        {
            isStart = true; 
            StartTime = DateTime.Now;
            controller.SetDataToView(ListData);
            FactoryController.SetData(ListData["Cash"].ToString(), ListData["TimelessShard"].ToString());
            ArsenalController.SetData(ListData["Cash"].ToString(), ListData["TimelessShard"].ToString());
            PAController.SetData(ListData);
            // Temp
            FindObjectOfType<SpaceShopScene>().SetData();
        } else
        {
            FindObjectOfType<NotificationBoardController>().CreateNormalNotiBoard(transform.position,
                "Can not find your pilot's data.\n Please try again or contact to our email.", 5f);
        }
    }
    #endregion
    #region Teleport To Scene
    public void TeleportToScene(GameObject FromScene,GameObject ToScene)
    {
        if (FromScene.GetComponent<UECMenuShared>()!=null)
        {
            FromScene.GetComponent<UECMenuShared>().OnExitAnimation();
        }
        MainCamera.transform.position = new Vector3(ToScene.transform.position.x, ToScene.transform.position.y, MainCamera.transform.position.z);
        if (ToScene.GetComponent<UECMenuShared>() != null)
        {
            ToScene.GetComponent<UECMenuShared>().OnEnterAnimation();
        }
    }

    public void ExitAnimationAllScene()
    {
        foreach (var scene in DisableUponActive)
        {
            scene.GetComponent<UECMenuShared>().OnExitAnimation();
        }
    }
    #endregion
    #region Start Play Timer
    public void SetTimer(DateTime startTime)
    {
        int oldTime = Playedtime;
        DateTime myDateTime = DateTime.Now;
        TimeSpan currentTimePlayed = myDateTime - startTime;
        Playedtime = (int)currentTimePlayed.TotalMinutes;
        if (oldTime < Playedtime)
        {
            isCount = true;
        }
    }
    #endregion
}
