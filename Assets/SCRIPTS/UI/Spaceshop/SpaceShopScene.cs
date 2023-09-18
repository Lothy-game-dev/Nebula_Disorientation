using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpaceShopScene : UECMenuShared
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject Cash;
    public GameObject ListItem;
    #endregion
    #region NormalVariables
    private int CurrentCash;
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
    #region Animation
    public override void OnEnterAnimation()
    {
        GetComponent<BackgroundBrieflyMoving>().enabled = true;
        transform.GetChild(0).GetComponent<Rigidbody2D>().simulated = true;
        ListItem.SetActive(true);
        FindObjectOfType<MainMenuCameraController>().GenerateLoadingSceneAtPos(transform.position, 1f);
        CurrentCash = 0;
        SetData();
    }

    public override void OnExitAnimation()
    {
        GetComponent<BackgroundBrieflyMoving>().enabled = false;
        transform.GetChild(0).GetComponent<Rigidbody2D>().simulated = false;
        ListItem.SetActive(false);
    }
    #endregion
    #region Data
    public void SetData()
    {
        // Set Data to items in UI
        Dictionary<string, object> ListData = FindObjectOfType<AccessDatabase>()
            .GetPlayerInformationById(FindObjectOfType<UECMainMenuController>().PlayerId);
        CurrentCash = (int)ListData["Cash"];
        Cash.transform.GetChild(1).GetComponent<TextMeshPro>().text = CurrentCash.ToString();
        transform.GetChild(2).GetComponent<SpaceShopInformation>().SetInfoForStockAndOwned();
        ListItem.GetComponent<SpaceShopListItem>().UpdateItemStocks();
    }

    public bool CheckEnoughMoney(int cash)
    {
        return cash <= CurrentCash;
    }
    #endregion
}
