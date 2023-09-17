using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArsenalItem : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject Arsenal;
    public GameObject BuyButton;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public string Id;
    public string Type;
    private Arsenal ar;
    private Dictionary<string, object> Status;
    private Dictionary<string, object> RankSys;
    private string PriceColor;
    private string RankColor;
    public List<GameObject> ItemStatusList;
    public GameObject Content;
    public bool LockedItem;

    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        ar = Arsenal.GetComponent<Arsenal>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion
    #region Show information when click item
    // Group all function that serve the same algorithm
    private void OnMouseDown()
    {
        
        if (Type == "Weapon")
        {
            CheckCurrentItem(Id);
            ArsenalInformation(ar.WeaponList);
            ar.ItemId = int.Parse(Id);
            ar.ItemType = Type;
        } else
        {
            if (Type == "Power")
            {
                CheckCurrentItem(Id);
                ArsenalInformation(ar.PowerList);
                ar.ItemId = int.Parse(Id);
                ar.ItemType = Type;
            }
        }
    }
    
    #endregion
    #region Show Arsenal (weapon,power) information
    // Group all function that serve the same algorithm
    private void ArsenalInformation(List<List<string>> ItemList)
    {
        RankSys = FindAnyObjectByType<AccessDatabase>().GetRankById(int.Parse(ItemList[int.Parse(Id) - 1][8]));
        if (Type == "Weapon")
        {
            Status = FindAnyObjectByType<GlobalFunctionController>().ConvertWeaponStatsToDictionary(ItemList[int.Parse(Id) - 1][4]);
        } else
        {
            Status = FindAnyObjectByType<GlobalFunctionController>().ConvertPowerStatsToDictionary(ItemList[int.Parse(Id) - 1][4]);

        }
        Debug.Log(Status.Count);
        for (int i = 0; i < ItemStatusList.Count; i++)
        {
            Debug.Log("abcd");           
            if (!Status.ContainsKey(ItemStatusList[i].name))
            {
                ItemStatusList[i].GetComponent<TextMeshPro>().text = "N/A";
            } else
            {
                ItemStatusList[i].GetComponent<TextMeshPro>().text = (string)Status[ItemStatusList[i].name];
            }
        }
        //ar.DescContent.GetComponent<TMP_Text>().text = ItemList[int.Parse(Id) - 1][3];
        // check if enough timeless shard
        if (int.Parse(ar.PShard) < int.Parse(ItemList[int.Parse(Id) - 1][6]))
        {
            PriceColor = "red";
            ar.EnoughPrice = false;
        }
        else
        {
            if (int.Parse(ar.PShard) == 0)
            {
                ar.EnoughPrice = false;
                PriceColor = "red";
            }
            else
            {
                PriceColor = "green";
                ar.EnoughPrice = true;
            }
        }
        // check if enough cash
        if (int.Parse(ar.PCash) < int.Parse(ItemList[int.Parse(Id) - 1][5]))
        {
            PriceColor = "red";
            ar.EnoughPrice = false;
        }
        else
        {
            PriceColor = "green";
            ar.EnoughPrice = true;
        }
        //check rank required
        if ((string)ar.PlayerInformation["Rank"] == (string)RankSys["RankName"])
        {
            RankColor = "green";
            ar.RankRequired = true;
        }
        else
        {
            RankColor = "red";
            ar.RankRequired = false;
        }
        ar.ItemTimelessShard.GetComponentInChildren<TextMeshPro>().text = "<color=" + PriceColor + ">" + ItemList[int.Parse(Id) - 1][6] + "</color>";
        ar.ItemCash.GetComponentInChildren<TextMeshPro>().text = "<color=" + PriceColor + ">" + ItemList[int.Parse(Id) - 1][5] + "</color>";
        ar.Rank.GetComponentInChildren<TextMeshPro>().text = "<color=" + RankColor + ">Rank Required</color><br><color=" + (string)RankSys["RankTier"] + ">" + (string)RankSys["RankName"] + "</color>";
        StartCoroutine(TextRunning(ItemList[int.Parse(Id) - 1][3]));
        //change the color of buy button if item is locked
        Color c = BuyButton.transform.GetChild(0).GetComponent<TextMeshPro>().color;
        if (LockedItem)
        {
            c.a = 0.5f;
        }
        else
        {
            c.a = 1f;
        }
        BuyButton.transform.GetChild(0).GetComponent<TextMeshPro>().color = c;
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
        ar.DescContent.GetComponent<TMP_Text>().text = "";
        gameObject.GetComponent<Collider2D>().enabled = false;
        for (int i = 0; i < text.Length; i++)
        {
            ar.DescContent.GetComponent<TMP_Text>().text += text.Substring(i, 1);
            yield return new WaitForSeconds(0.05f);
        }
        gameObject.GetComponent<Collider2D>().enabled = true;
    }
    #endregion
}
