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
    public GameObject CurrentChosen;
    public string ItemTierColor;
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
    #region Generate fighter list and choose the first one
    public void FirstContent()
    {
        // clone the template and set image of model
        for (int i = 0; i < FighterList.Count; i++)
        {
            GameObject g = Instantiate(Item, Item.transform.position, Quaternion.identity);
            g.name = FighterList[i][1];
            g.transform.SetParent(Content.transform);
            g.transform.GetChild(1).GetComponent<TMP_Text>().text = "<color=" + FighterList[i][6].ToUpper() + ">" + FighterList[i][1] + "</color>";
            g.transform.localScale = new Vector2(1, 1);
            g.GetComponent<FactoryItem>().Id = FighterList[i][0];
            g.GetComponent<FactoryItem>().ItemStatusList = FighterStatus;
            g.GetComponent<FactoryItem>().Content = Content;
            g.GetComponent<FactoryItem>().FacItemList = FighterList;
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
            g.SetActive(true);
            LockItem(g, FighterList[i][5]);
            if (i == 0)
            {
                g.GetComponent<FactoryItem>().FighterInformation(FighterList, "1");
            }
        }

       
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
                Game.GetComponent<FactoryItem>().AlreadyPurchased = false;
            }
        }
        if (!isLocked)
        {
            int n = FindObjectOfType<AccessDatabase>().GetCurrentOwnershipWeaponPowerModelByName(FindObjectOfType<UECMainMenuController>().PlayerId,
                Game.name, "Model");
            if (n>0)
            {
                isLocked = true;
                Game.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                Game.GetComponent<FactoryItem>().LockedItem = true;
                Game.GetComponent<FactoryItem>().AlreadyPurchased = true;
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
        int n = FindObjectOfType<AccessDatabase>().GetCurrentOwnershipWeaponPowerModelByName(FindObjectOfType<UECMainMenuController>().PlayerId,
                ItemName, "Model");
        if (n>0)
        {
            if (CurrentChosen!=null && CurrentChosen.GetComponent<FactoryItem>()!=null)
            {
                Debug.Log(CurrentChosen.GetComponent<FactoryItem>().AlreadyPurchased);
                CurrentChosen.GetComponent<FactoryItem>().LockCurrentItem();
            }
        }
    }
    public void SetFirstData()
    {
        FighterList = FindAnyObjectByType<AccessDatabase>().GetAllFighter();
        PlayerId = FindAnyObjectByType<AccessDatabase>().GetCurrentSessionPlayerId();
        PlayerInformation = FindAnyObjectByType<AccessDatabase>().GetPlayerInformationById(PlayerId);
        PCash = PlayerInformation["Cash"].ToString();
        PShard = PlayerInformation["TimelessShard"].ToString();
        PlayerCash.GetComponent<TextMeshPro>().text = PCash;
        PlayerShard.GetComponent<TextMeshPro>().text = PShard;
        FirstContent();
    }


    #endregion
    #region Reset data when clicking back button
    public void ResetData()
    {
        if (Content.transform.childCount > 0)
        {
            for (int i = 0; i < Content.transform.childCount; i++)
            {
                Destroy(Content.transform.GetChild(i).gameObject);
            }
        }
    }
    #endregion

}
