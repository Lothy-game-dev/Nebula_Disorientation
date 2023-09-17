using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadOutStatusBoard : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public GameObject StatusBoardRotate;
    public GameObject NameText;
    public GameObject StatsText;
    public GameObject InfoText;
    public string Type;
    #endregion
    #region NormalVariables
    private AccessDatabase ad;
    private string Item;
    private string ItemName;
    private string ItemDescription;
    private string ItemStats;
    private bool isShowingDes;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void OnEnable()
    {
        // Initialize variables
        GetComponent<Collider2D>().enabled = false;
        ad = FindObjectOfType<AccessDatabase>();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Mouse Check
    private void OnMouseDown()
    {
        // On mouse down: Disable collider and rotate
        // THen show data
        GetComponent<Collider2D>().enabled = false;
        NameText.SetActive(false);
        StatsText.SetActive(false);
        InfoText.SetActive(false);
        StartCoroutine(RotateStatusBoard());
    }
    #endregion
    #region Animation
    private IEnumerator RotateStatusBoard()
    {
        // Rotate the board through y 
        // Then show data
        for (int i=0;i<18;i++)
        {
            StatusBoardRotate.transform.Rotate(new Vector3(0, 10, 0));
            yield return new WaitForSeconds(1/18f);
        }
        GetComponent<Collider2D>().enabled = true;
        if (isShowingDes)
        {
            isShowingDes = false;
            StatsText.SetActive(true);
        } else
        {
            isShowingDes = true;
            NameText.SetActive(true);
            InfoText.SetActive(true);
        }
    }
    #endregion
    #region Set Data
    public void SetData(string itemName)
    {
        ad = FindObjectOfType<AccessDatabase>();
        Item = itemName;
        GetComponent<Collider2D>().enabled = true;
        bool spin = false;
        if (ItemName != null && itemName != "") { spin = true; }
        else
        {
            ItemName = "!!!ERROR!!!";
            ItemDescription = "404! Can not found Data related to this " + Type + "!";
            ItemStats = "404! Can not found Data related to this " + Type + "!";
        }
        // Retrieve Data
        if (Type.Equals("Weapon"))
        {
            Dictionary<string, object> listData = ad.GetWeaponDataByName(Item);
            if (listData != null)
            {
                ItemName = "<color=" + (string)listData["Color"] + ">" + (string)listData["Name"] + "</color>\n" + (string)listData["Type"];
                ItemDescription = (string)listData["Description"];
                ItemStats = FindObjectOfType<GlobalFunctionController>().ConvertWeaponStatsToString((string)listData["Stats"]);
            }
        } else if (Type.Equals("Power"))
        {
            Dictionary<string, object> listData = ad.GetPowerDataByName(Item);
            if (listData != null)
            {
                ItemName = "<color=" + (string)listData["Color"] + ">" + (string)listData["Name"] + "</color>\n" + (string)listData["Type"];
                ItemDescription = (string)listData["Description"];
                ItemStats = FindObjectOfType<GlobalFunctionController>().ConvertPowerStatsToString((string)listData["Stats"]);
            }
        }
        NameText.GetComponent<TextMeshPro>().text = ItemName;
        InfoText.GetComponent<TextMeshPro>().text = ItemDescription;
        StatsText.GetComponent<TextMeshPro>().text = ItemStats;
        if (spin)
        {
            isShowingDes = false;
            GetComponent<Collider2D>().enabled = false;
            NameText.SetActive(false);
            StatsText.SetActive(false);
            InfoText.SetActive(false);
            StartCoroutine(RotateStatusBoard());
        }
        else
        {
            isShowingDes = true;
            GetComponent<Collider2D>().enabled = true;
            NameText.SetActive(true);
            InfoText.SetActive(true);
            StatsText.SetActive(false);
        }

    }
    #endregion
}
