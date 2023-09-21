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
    public GameObject BuyButton;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public string Id;
    public bool AlreadyPurchased;
    private Factory Fac;
    private Dictionary<string, object> Status;
    private Dictionary<string, object> ItemPrice;
    private Dictionary<string, object> RankSys;
    private string CashColor;
    private string ShardColor;
    private string RankColor;
    public List<GameObject> ItemStatusList;
    public GameObject Content;
    public bool LockedItem;
    private Coroutine currentCoroutine;
    public List<List<string>> FacItemList;
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


    }
    #endregion
    #region Show information when choose item
    // Group all function that serve the same algorithm
    private void OnMouseDown()
    {
        CheckCurrentItem(Id);
        FighterInformation(FacItemList, Id);
    }


    #endregion
    #region Show Fighter information
    // Group all function that serve the same algorithm
    public void FighterInformation(List<List<string>> ItemList, string ItemID)
    {
        Fac = Factory.GetComponent<Factory>();
        //convert the price x | y => Cash = x, Shard = y
        ItemPrice = FindAnyObjectByType<GlobalFunctionController>().ConvertModelPriceIntoTwoTypePrice(ItemList[int.Parse(ItemID) - 1][4]);
        // Check rank, if rank = null => that item can buy without rank
        if (ItemList[int.Parse(ItemID) - 1][5] == "N/A")
        {
            RankSys = FindAnyObjectByType<AccessDatabase>().GetRankById(0);
        }
        else
        {
            RankSys = FindAnyObjectByType<AccessDatabase>().GetRankById(int.Parse(ItemList[int.Parse(ItemID) - 1][5]));
        }
        //convert Fighter model stat => dictionary
        Status = FindAnyObjectByType<GlobalFunctionController>().ConvertModelStatsToDictionary(ItemList[int.Parse(ItemID) - 1][3]);

        //show stat info for each stat
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
        if (int.Parse(Fac.PShard) < int.Parse((string)ItemPrice["Timeless"]))
        {
            ShardColor = "red";
            Fac.EnoughPrice = false;
        }
        else
        {         
            if (int.Parse(Fac.PShard) == 0)
            {                
                Fac.EnoughPrice = false;
                ShardColor = "red";
            }
            else
            {
                ShardColor = "green";
                Fac.EnoughPrice = true;
            }
        }     
        // check if have enough cash
        if (int.Parse(Fac.PCash) < int.Parse((string)ItemPrice["Cash"]))
        {
            CashColor = "red";
            Fac.EnoughPrice = false;
        }
        else
        {
            CashColor = "green";
            Fac.EnoughPrice = true;
        }       
        // check rank required
        if ((int)Fac.PlayerInformation["RankId"] < int.Parse((string)RankSys["RankId"]))
        {
            RankColor = "red";
            Fac.RankRequired = false;
        } else
        {

            RankColor = "green";
            Fac.RankRequired = true;
        }
        
        //set information like item shard, cash, rank
        Fac.ItemTimelessShard.GetComponentInChildren<TextMeshPro>().text = "<color=" + ShardColor + ">" + (string)ItemPrice["Timeless"] + "</color>";
        Fac.ItemCash.GetComponentInChildren<TextMeshPro>().text = "<color=" + CashColor + ">" + (string)ItemPrice["Cash"] + "</color>";
        Fac.Rank.GetComponentInChildren<TextMeshPro>().text = "<color=" + RankColor + ">Rank Required</color><br><color=" + (string)RankSys["RankTier"] + ">" + (string)RankSys["RankName"] + "</color>";
        
        Fac.ItemName = ItemList[int.Parse(ItemID) - 1][1];
        Fac.ItemPriceCash = (string)ItemPrice["Cash"];
        Fac.ItemPriceShard = (string)ItemPrice["Timeless"];
        Fac.ItemId = int.Parse(ItemList[int.Parse(ItemID) - 1][0]);

        //change the color of buy button if item is locked
        Color c = BuyButton.transform.GetChild(0).GetComponent<TextMeshPro>().color;
        if (LockedItem)
        {
            c.a = 0.5f;
            if (BuyButton.GetComponent<CursorUnallowed>()==null)
            {
                BuyButton.AddComponent<CursorUnallowed>();
            }
        } else
        {
            c.a = 1f;
            if (BuyButton.GetComponent<CursorUnallowed>() != null)
            {
                Destroy(BuyButton.GetComponent<CursorUnallowed>());
            }
        }
        BuyButton.transform.GetChild(0).GetComponent<TextMeshPro>().color = c;
        BuyButton.GetComponent<FactoryButton>().AlreadyPurchased = AlreadyPurchased;
    }
    #endregion
    #region Check current item on mouse down
    private void CheckCurrentItem(string id)
    {
        // the item becomes green when we choose
        for (int i = 0; i < Content.transform.childCount; i++)
        {
            Color c = Content.transform.GetChild(i).GetComponent<Image>().color;
            c.r = 1f;
            c.g = 1f;
            c.b = 1f;
            c.a = 0.58f;
            Content.transform.GetChild(i).GetComponent<Image>().color = c;
        }
        Content.transform.GetChild(int.Parse(Id) - 1).GetComponent<Image>().color = Color.green;
        Factory.GetComponent<Factory>().CurrentChosen = gameObject;
    }

    public void LockCurrentItem()
    {
        LockedItem = true;
        transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
        Color c = BuyButton.transform.GetChild(0).GetComponent<TextMeshPro>().color;
        c.a = 0.5f;
        if (BuyButton.GetComponent<CursorUnallowed>() == null)
        {
            BuyButton.AddComponent<CursorUnallowed>();
        }
        BuyButton.transform.GetChild(0).GetComponent<TextMeshPro>().color = c;
        BuyButton.GetComponent<FactoryButton>().AlreadyPurchased = AlreadyPurchased;
    }
    #endregion
    #region Text animation
    private IEnumerator TextRunning(string text)
    {
        Fac.DescContent.GetComponent<TMP_Text>().text = "";
        //Use substring to take each word in the text and put it back
        for (int i = 0; i < text.Length; i++)
        {
            Fac.DescContent.GetComponent<TMP_Text>().text += text.Substring(i, 1);
            yield return new WaitForSeconds(0.05f);
        }

    }
    public void StartTextRunning(string text)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }
        if (Fac.OldCoroutine != null)
        {
            Debug.Log("Old");
            StopCoroutine(Fac.OldCoroutine);
        }

        currentCoroutine = StartCoroutine(TextRunning(text));
        Fac.OldCoroutine = currentCoroutine;


    }
    #endregion
}
    