using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UECSessionStatusBoard : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject StatusBoardRotate;
    public GameObject NameText;
    public GameObject DescriptionText;
    public GameObject StatsText;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    private bool isShowingStats;
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
    #region Function group 1
    // Group all function that serve the same algorithm
    public void GenerateData(string Type, string Item)
    {
        string ItemName = "", ItemDescription = "", ItemStats = "";
        // Retrieve Data
        if (Type.Equals("Weapon"))
        {
            Dictionary<string, object> listData = FindObjectOfType<AccessDatabase>().GetWeaponDataByName(Item);
            if (listData != null)
            {
                ItemName = "<color=" + (string)listData["Color"] + ">" + (string)listData["Name"] + "</color>\n" + (string)listData["Type"];
                ItemDescription = (string)listData["Description"];
                ItemStats = FindObjectOfType<GlobalFunctionController>().ConvertWeaponStatsToString((string)listData["Stats"]);
            }
        }
        else if (Type.Equals("Power"))
        {
            if (Item != null)
            {
                Dictionary<string, object> listData = FindObjectOfType<AccessDatabase>().GetPowerDataByName(Item);
                if (listData != null)
                {
                    ItemName = "<color=" + (string)listData["Color"] + ">" + (string)listData["Name"] + "</color>\n" + (string)listData["Type"];
                    ItemDescription = (string)listData["Description"];
                    ItemStats = FindObjectOfType<GlobalFunctionController>().ConvertPowerStatsToString((string)listData["Stats"]);
                }
            }
        }
        NameText.GetComponent<TextMeshPro>().text = ItemName;
        DescriptionText.GetComponent<TextMeshPro>().text = ItemDescription;
        StatsText.GetComponent<TextMeshPro>().text = ItemStats;
        isShowingStats = false;
    }
    #endregion
    #region Mouse Check
    private void OnMouseDown()
    {
        if (isShowingStats)
        {
            isShowingStats = false;
            StatsText.SetActive(false);
            NameText.SetActive(true);
            DescriptionText.SetActive(true);
        } else
        {
            isShowingStats = true;
            StatsText.SetActive(true);
            NameText.SetActive(false);
            DescriptionText.SetActive(false);
        }
    }
    #endregion
}
