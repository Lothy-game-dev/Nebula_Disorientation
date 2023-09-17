using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FactoryItem : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject Factory;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public string Id;
    public string Type;
    private Factory Fac;
    private Dictionary<string, object> Status;
    private Dictionary<string, object> ItemPrice;
    private Dictionary<string, object> RankSys;
    private string PriceColor;
    private string RankColor;
    public List<GameObject> ItemStatusList;
    public GameObject Content;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        Fac = Factory.GetComponent<Factory>();
    }

    // Update is called once per frame
    void Update()
    {


    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    private void OnMouseDown()
    {
        CheckCurrentItem(Id);
        FighterInformation(Fac.FighterList);
    }


    #endregion
    #region Show Fighter information
    // Group all function that serve the same algorithm
    private void FighterInformation(List<List<string>> ItemList)
    {
        ItemPrice = FindAnyObjectByType<GlobalFunctionController>().ConvertModelPriceIntoTwoTypePrice(ItemList[int.Parse(Id) - 1][4]);
        if (ItemList[int.Parse(Id) - 1][5] == "N/A")
        {
            RankSys = FindAnyObjectByType<AccessDatabase>().GetRankById(1);
        }
        else
        {
            RankSys = FindAnyObjectByType<AccessDatabase>().GetRankById(int.Parse(ItemList[int.Parse(Id) - 1][5]));
        }
        Status = FindAnyObjectByType<GlobalFunctionController>().ConvertModelStatsToDictionary(ItemList[int.Parse(Id) - 1][3]);
        for (int i = 0; i < ItemStatusList.Count; i++)
        {
            if (!Status.ContainsKey(ItemStatusList[i].name))
            {
                ItemStatusList[i].GetComponent<TextMeshPro>().text = "N/A";
            } else
            {
                ItemStatusList[i].GetComponent<TextMeshPro>().text = (string)Status[ItemStatusList[i].name];
            }
        }
        // check if have enough Timeless shard
        if ((int)Fac.PlayerInformation["TimelessShard"] < int.Parse((string)ItemPrice["Timeless"]))
        {
            PriceColor = "red";
            Fac.EnoughPrice = false;
        }
        else
        {         
            if ((int)Fac.PlayerInformation["TimelessShard"] == 0)
            {                
                Fac.EnoughPrice = false;
                PriceColor = "red";
            }
            else
            {
                PriceColor = "green";
                Fac.EnoughPrice = true;
            }
        }
        // check if have enough cash
        if ((int)Fac.PlayerInformation["Cash"] < int.Parse((string)ItemPrice["Cash"]))
        {
            PriceColor = "red";
            Fac.EnoughPrice = false;
        }
        else
        {
            PriceColor = "green";
            Fac.EnoughPrice = true;
        }
        // check rank required
        if ((string)Fac.PlayerInformation["Rank"] == (string)RankSys["RankName"])
        {
            RankColor = "green";     
            Fac.RankRequired = true;
        }
        else
        {
            RankColor = "red";
            Fac.RankRequired = false;
        }
        Fac.ItemTimelessShard.GetComponentInChildren<TextMeshPro>().text = "<color=" + PriceColor + ">" + (string)ItemPrice["Timeless"] + "</color>";
        Fac.ItemCash.GetComponentInChildren<TextMeshPro>().text = "<color=" + PriceColor + ">" + (string)ItemPrice["Cash"] + "</color>";
        Fac.Rank.GetComponentInChildren<TextMeshPro>().text = "<color=" + RankColor + ">Rank Required</color><br><color=" + (string)RankSys["RankTier"] + ">" + (string)RankSys["RankName"] + "</color>";
        StartCoroutine(TextRunning(ItemList[int.Parse(Id) - 1][2]));
    }
    #endregion
    #region Check current item on mouse down
    private void CheckCurrentItem(string id)
    {
        for (int i = 0; i < Content.transform.childCount; i++)
        {
            Content.transform.GetChild(i).GetComponent<Image>().color = Color.white;
        }
        Content.transform.GetChild(int.Parse(Id) - 1).GetComponent<Image>().color = Color.green;
    }
    #endregion
    #region
    private IEnumerator TextRunning(string text)
    {     
        Fac.DescContent.GetComponent<TMP_Text>().text = "";
        gameObject.GetComponent<Collider2D>().enabled = false;
        for (int i = 0; i < text.Length; i++)
        {
            Fac.DescContent.GetComponent<TMP_Text>().text += text.Substring(i, 1);
            yield return new WaitForSeconds(0.05f);
        }
        gameObject.GetComponent<Collider2D>().enabled = true;
    }
    #endregion
}
