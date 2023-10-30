using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpaceShopSessionScene : MonoBehaviour
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
    void OnEnable()
    {
        // Initialize variables
        ListItem.SetActive(true);
        CurrentCash = 0;
        SetData();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Data
    public void SetData()
    {
        // Set Data to items in UI
        Dictionary<string, object> ListData = FindObjectOfType<AccessDatabase>()
            .GetSessionInfoByPlayerId(PlayerPrefs.GetInt("PlayerID"));
        CurrentCash = (int)ListData["SessionCash"];
        Cash.transform.GetChild(1).GetComponent<TextMeshPro>().text = CurrentCash.ToString();
        transform.GetChild(2).GetComponent<SpaceShopSessionInformation>().SetInfoForStockAndOwned();
        ListItem.GetComponent<SpaceShopSessionListItem>().UpdateItemStocks();
    }

    public bool CheckEnoughMoney(int cash)
    {
        return cash <= CurrentCash;
    }
    #endregion
}
