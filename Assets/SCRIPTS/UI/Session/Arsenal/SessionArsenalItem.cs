using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionArsenalItem : MonoBehaviour
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
    public GameObject BlackFadeWeapon;
    public GameObject BlackFadePower;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public string Id;
    public string Type;
    private SessionArsenal ar;
    private Dictionary<string, object> Status;
    private Dictionary<string, object> RankSys;
    private string CashColor;
    private string ShardColor;
    private string RankColor;
    public List<GameObject> ItemStatusList;
    public GameObject Content;
    public bool LockedItem;
    private int RankId;
    private Coroutine currentCoroutine;
    public List<List<string>> ArItemList;
    private bool GeneratingText;
    private string TextGenerated;
    public string ItemPreReq;
    public bool IsRanked;
    public bool IsOwned;
    private string ItemReq;
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
    #region Show information when click item
    // Group all function that serve the same algorithm
    private void OnMouseDown()
    {

        if (Type == "Weapon")
        {
            Arsenal.GetComponent<SessionArsenal>().CurrentItem = gameObject;
            ArsenalInformation(ArItemList, Id);
            ar.ItemId = int.Parse(Id);
            ar.ItemType = Type;
        }
        else
        {
            if (Type == "Power")
            {
                Arsenal.GetComponent<SessionArsenal>().CurrentItem = gameObject;
                ArsenalInformation(ArItemList, Id);
                ar.ItemId = int.Parse(Id);
                ar.ItemType = Type;
            }
        }
    }

    #endregion
    #region Show Arsenal (weapon,power) information
    // Group all function that serve the same algorithm
    public void ArsenalInformation(List<List<string>> ItemList, string ItemID)
    {
        ar = Arsenal.GetComponent<SessionArsenal>();
        CheckCurrentItem(ItemID);
        // get the item rank 
        RankSys = FindAnyObjectByType<AccessDatabase>().GetRankById(int.Parse(ItemList[int.Parse(ItemID) - 1][8]), int.Parse(ar.PlayerInformation["SupremeWarriorNo"].ToString()));
        // convert stat to dictionary depend on type like weapon or power
        if (Type == "Weapon")
        {
            Status = FindAnyObjectByType<GlobalFunctionController>().ConvertWeaponStatsToDictionary(ItemList[int.Parse(ItemID) - 1][4]);
        }
        else
        {
            Status = FindAnyObjectByType<GlobalFunctionController>().ConvertPowerStatsToDictionary(ItemList[int.Parse(ItemID) - 1][4]);

        }
        //show stat info for each stat
        for (int i = 0; i < ItemStatusList.Count; i++)
        {
            if (!Status.ContainsKey(ItemStatusList[i].name))
            {
                ItemStatusList[i].GetComponent<TextMeshPro>().text = "N/A";
            }
            else
            {
                if (ItemStatusList[i].name == "BRx")
                {
                    ItemStatusList[i].GetComponent<TextMeshPro>().text = "X" + (string)Status[ItemStatusList[i].name];
                }
                else
                {
                    ItemStatusList[i].GetComponent<TextMeshPro>().text = (string)Status[ItemStatusList[i].name];
                }
            }
        }

        // check if we r in session to buy item by cash
        if (ar.IsInSession)
        {
            // check if have enough cash
            if (int.Parse(ar.PCash) < int.Parse(ItemList[int.Parse(ItemID) - 1][5]))
            {
                CashColor = "red";
                ar.EnoughPrice = false;
                LockedItem = true;
            }
            else
            {
                CashColor = "green";
                ar.EnoughPrice = true;
            }
        }
        else
        {
            CashColor = "grey";
            // check if enough timeless shard
            if (ItemList[int.Parse(ItemID) - 1][6] == "N.A")
            {
                ShardColor = "grey";
                ar.EnoughPrice = false;
            }
            else
            {
                if (int.Parse(ar.PShard) < int.Parse(ItemList[int.Parse(ItemID) - 1][6]))
                {
                    ShardColor = "red";
                    ar.EnoughPrice = false;
                }
                else
                {
                    if (int.Parse(ar.PShard) == 0)
                    {
                        ar.EnoughPrice = false;
                        ShardColor = "red";
                    }
                    else
                    {
                        ShardColor = "green";
                        ar.EnoughPrice = true;
                    }
                }
            }

        }
        //ar.ItemTimelessShard.GetComponentInChildren<TextMeshPro>().text = "<color=" + ShardColor + ">" + ItemList[int.Parse(Id) - 1][6] + "</color>";
        ar.ItemCash.GetComponentInChildren<TextMeshPro>().text = "<color=" + CashColor + ">" + ItemList[int.Parse(Id) - 1][5] + "</color>";
        ar.Rank.GetComponentInChildren<TextMeshPro>().text = "<color=grey>Rank Required</color><br><color=grey>" + (string)RankSys["RankName"] + "</color>";

        //Check item req
        ItemReq = "";
        if (ItemList[int.Parse(ItemID) - 1][7] != "N.A")
        {
            ItemReq = "<br><br>Prerequisite "+ ar.CurrentTab +": <color=#3bccec>" + ItemList[(int.Parse(ItemList[int.Parse(ItemID) - 1][7])) - 1][2] + "</color>";
        }
        StartTextRunning(ItemList[int.Parse(ItemID) - 1][3]);

        LockItem();

        ar.ItemName = ItemList[int.Parse(ItemID) - 1][2];
        ar.RequiredCash = ItemList[int.Parse(ItemID) - 1][5];
        ar.RequiredShard = ItemList[int.Parse(ItemID) - 1][6];
        ar.ItemTierColor = ItemList[int.Parse(ItemID) - 1][9];

    }
    #endregion
    #region Check current item on mouse down
    public void CheckCurrentItem(string id)
    {
        Debug.Log(Content.transform.GetChild(int.Parse(id) - 1));
        for (int i = 0; i < Content.transform.childCount; i++)
        {
            Color c = Content.transform.GetChild(i).GetComponent<Image>().color;
            c.r = 1f;
            c.g = 1f;
            c.b = 1f;
            c.a = 0.58f;
            Content.transform.GetChild(i).GetComponent<Image>().color = c;
        }
        Content.transform.GetChild(int.Parse(id) - 1).GetComponent<Image>().color = Color.green;
    }
    #endregion
    #region Text animation
    private IEnumerator TextRunning(string text)
    {
        ar.DescContent.GetComponent<TMP_Text>().text = "";
        //Use substring to take each word in the text and put it back
        for (int i = 0; i < text.Length; i++)
        {
            ar.DescContent.GetComponent<TMP_Text>().text += text.Substring(i, 1);
            yield return new WaitForSeconds(0.05f);
        }
        ar.DescContent.GetComponent<TMP_Text>().text += ItemReq;

    }

    public void LockItem()
    {
        //change the color of buy button if item is locked
        Color c = BuyButton.transform.GetChild(0).GetComponent<TextMeshPro>().color;
        if (LockedItem)
        {
            c.a = 0.5f;
            if (BuyButton.GetComponent<CursorUnallowed>() == null)
            {
                BuyButton.AddComponent<CursorUnallowed>();
            }
        }
        else
        {
            c.a = 1f;
            if (BuyButton.GetComponent<CursorUnallowed>() != null)
            {
                Destroy(BuyButton.GetComponent<CursorUnallowed>());
            }
        }
        BuyButton.transform.GetChild(0).GetComponent<TextMeshPro>().color = c;
        BuyButton.GetComponent<SessionArsenalBuyButton>().PreReqName = ItemPreReq;
        BuyButton.GetComponent<SessionArsenalBuyButton>().isEnoughMoney = ar.EnoughPrice;
        BuyButton.GetComponent<SessionArsenalBuyButton>().ItemId = Id;
        BuyButton.GetComponent<SessionArsenalBuyButton>().isOwned = IsOwned;
    }
    public void StartTextRunning(string text)
    {
        if (ar.OldCoroutine != null)
        {
            StopCoroutine(ar.OldCoroutine);
        }
        ar.OldCoroutine = StartCoroutine(TextRunning(text));
    }
    #endregion
}
