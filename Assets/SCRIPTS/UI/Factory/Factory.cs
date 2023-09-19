using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Factory : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject Item;
    public GameObject Content;
    public List<SpriteRenderer> FighterImage;
    public List<GameObject> FighterStatus;
    public GameObject DescContent;
    public GameObject ItemCash;
    public GameObject ItemTimelessShard;
    public GameObject Rank;
    public GameObject PlayerCash;
    public GameObject PlayerShard;
    public bool IsInSession;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public List<List<string>> FighterList;
    public Dictionary<string, object> PlayerInformation;
    private int PlayerId;
    public bool EnoughPrice;
    public bool RankRequired;
    public bool Locked;
    public int ItemId;
    public string ItemName;
    public string ItemPriceCash;
    public string ItemPriceShard;
    private Dictionary<string, object> Status;
    private Dictionary<string, object> RankSys;
    private Dictionary<string, object> ItemPrice;
    public string PCash;
    public string PShard;
    public Coroutine OldCoroutine;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        FighterList = FindAnyObjectByType<AccessDatabase>().GetAllFighter();
        PlayerId = FindAnyObjectByType<AccessDatabase>().GetCurrentSessionPlayerId();
        PlayerInformation = FindAnyObjectByType<AccessDatabase>().GetPlayerInformationById(PlayerId);
        PCash = PlayerInformation["Cash"].ToString();
        PShard = PlayerInformation["TimelessShard"].ToString();
        PlayerCash.GetComponent<TextMeshPro>().text = PCash;
        PlayerShard.GetComponent<TextMeshPro>().text = PShard;
        FirstContent();
    }

    // Update is called once per frame
    void Update()
    {

        // Call function and timer only if possible
    }
    #endregion
    #region Generate fighter list and choose the first one
    private void FirstContent()
    {
        // clone the template and set image of model
        for (int i = 0; i < FighterList.Count; i++)
        {
            GameObject g = Instantiate(Item, Item.transform.position, Quaternion.identity);
            g.name = FighterList[i][1];
            g.transform.SetParent(Content.transform);
            g.transform.GetChild(1).GetComponent<TMP_Text>().text = FighterList[i][1];
            g.transform.localScale = new Vector2(1, 1);
            g.GetComponent<FactoryItem>().Id = FighterList[i][0];
            g.GetComponent<FactoryItem>().ItemStatusList = FighterStatus;
            g.GetComponent<FactoryItem>().Content = Content;
            if (FighterList[i][1] == "SSS-MKL")
            {
                g.transform.GetChild(0).GetComponent<Image>().sprite = FighterImage[FighterImage.FindIndex(item => item.name == "SSSL")].sprite;
            }
            else
            {
                if (FighterList[i][1].Contains("UEC29-MKL"))
                {
                    g.transform.GetChild(0).GetComponent<Image>().sprite = FighterImage[FighterImage.FindIndex(item => item.name == "UEC29L")].sprite;
                }
                else
                {
                    g.transform.GetChild(0).GetComponent<Image>().sprite = FighterImage[FighterImage.FindIndex(item => FighterList[i][1].ToLower().Contains(item.name.ToLower()))].sprite;
                }
            }
            LockItem(g, FighterList[i][5]);
            g.SetActive(true);
        }

        // First item choosen and set the information 
        Status = FindAnyObjectByType<GlobalFunctionController>().ConvertModelStatsToDictionary(FighterList[0][3]);
        if (FighterList[0][5] == "N/A")
        {
            RankSys = FindAnyObjectByType<AccessDatabase>().GetRankById(1);
        }
        else
        {
            RankSys = FindAnyObjectByType<AccessDatabase>().GetRankById(int.Parse(FighterList[0][5]));
        }
        ItemPrice = FindAnyObjectByType<GlobalFunctionController>().ConvertModelPriceIntoTwoTypePrice(FighterList[0][4]);
        Content.transform.GetChild(0).GetComponent<Image>().color = Color.green;
        for (int i = 0; i < FighterStatus.Count; i++)
        {
            if (!Status.ContainsKey(FighterStatus[i].name))
            {
                FighterStatus[i].GetComponent<TextMeshPro>().text = "N/A";
            }
            else
            {
                FighterStatus[i].GetComponent<TextMeshPro>().text = (string)Status[FighterStatus[i].name];
            }
        }
        EnoughPrice = true;
        RankRequired = true;
        ItemId = int.Parse(FighterList[0][0]);
        ItemTimelessShard.GetComponentInChildren<TextMeshPro>().text = "<color=green>" + (string)ItemPrice["Timeless"] + "</color>";
        DescContent.GetComponent<TMP_Text>().text = FighterList[0][2];
        ItemCash.GetComponentInChildren<TextMeshPro>().text = "<color=green>" + (string)ItemPrice["Cash"] + "</color>";
        Rank.GetComponentInChildren<TextMeshPro>().text = "<color=green>Rank Required</color><br><color=" + (string)RankSys["RankTier"] + ">" + (string)RankSys["RankName"] + "</color>";
        ItemName = FighterList[0][1];
        ItemPriceCash = (string)ItemPrice["Cash"];
        ItemPriceShard = (string)ItemPrice["Timeless"];
    }
    #endregion
    #region Lock item
    // Group all function that serve the same algorithm
    public void LockItem(GameObject Game, string RankId)
    {
        int rankId = 0;
        bool isLocked = false;
        if (RankId != "N/A")
        {
            if ((string)PlayerInformation["Rank"] == "Unranked")
            {
                rankId = 1;
            }
            else
            {
                rankId = (int)PlayerInformation["RankId"];
            }
            if (rankId < int.Parse(RankId))
            {
                isLocked = true;
                Game.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                Game.GetComponent<FactoryItem>().LockedItem = true;
            }
        }
        if (!isLocked)
        {
            int n = FindObjectOfType<AccessDatabase>().GetCurrentOwnershipWeaponPowerModelByName(FindObjectOfType<UECMainMenuController>().PlayerId,
                Game.name, "Model");
            if (n>0)
            {
                isLocked = true;
                Game.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                Game.GetComponent<FactoryItem>().LockedItem = true;
            }
        }
    }
    #endregion
    #region Set data (money,....)
    public void SetData(string Cash, string Shard)
    {
        PCash = Cash;
        PShard = Shard;
        PlayerCash.GetComponent<TextMeshPro>().text = PCash;
        PlayerShard.GetComponent<TextMeshPro>().text = PShard;
    }

    #endregion
    #region Reset data when clicking back button
    public void ResetData()
    {

    }
    #endregion

}
