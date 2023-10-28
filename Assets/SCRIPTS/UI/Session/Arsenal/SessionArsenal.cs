using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionArsenal : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    private AccessDatabase AccDB;

    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject Item;
    public GameObject Content;
    public List<SpriteRenderer> WeaponImage;
    public List<GameObject> WeaponStatus;
    public List<GameObject> PowerStatus;
    public GameObject DescContent;
    public GameObject ItemCash;
    public GameObject ItemTimelessShard;
    public GameObject Rank;
    public GameObject PlayerCash;
    public GameObject PlayerShard;
    public bool IsInSession;
    public GameObject OtherContent;
    public GameObject PowerButton;
    public GameObject WeaponButton;
    public GameObject BuyButton;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public List<List<string>> WeaponList;
    public List<List<string>> PowerList;
    public Dictionary<string, object> PlayerInformation;
    public Dictionary<string, object> SessionPlayerInformation;
    private int PlayerId;
    public bool EnoughPrice;
    public bool RankRequired;
    public string CurrentTab;
    public int ItemId;
    public string ItemType;
    private Dictionary<string, object> Status;
    private Dictionary<string, object> RankSys;
    public string PCash;
    public string PShard;
    public Coroutine OldCoroutine;
    public Coroutine CurrentCoroutine;
    private Color WeaponBoxColor;
    private Color PowerBoxColor;
    public string ItemName;
    public string RequiredShard;
    public string RequiredCash;
    public GameObject CurrentItem;
    public string ItemTierColor;
    public bool isReset;
    private Vector2 InitWeaponBoxPos;
    private Vector2 InitPowerBoxPos;
    private Vector2 InitWeaponBtnPos;
    private Vector2 InitPowerBtnPos;
    #endregion
    #region Start & Update

    // Start is called before the first frame update
    void Start()
    {
        SetFirstData();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Generate first weapon category
    private void FirstContent()
    {
        CurrentTab = "Weapon";
        for (int i = 0; i < WeaponList.Count; i++)
        {
            GameObject g = Instantiate(Item, Item.transform.position, Quaternion.identity);
            g.name = WeaponList[i][2];
            g.transform.SetParent(Content.transform);
            g.transform.localScale = new Vector3(1, 1, Item.transform.position.z);
            g.transform.GetChild(1).GetComponent<TMP_Text>().text = "<color=" + WeaponList[i][9].ToUpper() + ">" + WeaponList[i][2] + "</color>";
            g.GetComponent<SessionArsenalItem>().Id = WeaponList[i][0];
            g.GetComponent<SessionArsenalItem>().Type = "Weapon";
            g.GetComponent<SessionArsenalItem>().ItemStatusList = WeaponStatus;
            g.GetComponent<SessionArsenalItem>().Content = Content;
            g.GetComponent<SessionArsenalItem>().ArItemList = WeaponList;
            // set item image
            if (WeaponList[i][2] == "Star Blaster")
            {
                g.transform.GetChild(0).GetComponent<Image>().sprite = WeaponImage[WeaponImage.FindIndex(item => item.name == "Star")].sprite;
            }
            else
            {
                if (WeaponList[i][2].Contains("Nano Flame Thrower") || WeaponList[i][2].Contains("Freezing Blaster"))
                {
                    g.transform.GetChild(0).GetComponent<Image>().sprite = WeaponImage[WeaponImage.FindIndex(item => item.name == "NanoFlame")].sprite;
                }
                else
                {
                    g.transform.GetChild(0).GetComponent<Image>().sprite = WeaponImage[WeaponImage.FindIndex(item => WeaponList[i][2].ToLower().Contains(item.name.ToLower()))].sprite;
                }
            }
            // check weapon has picked in Loadout
            int n = FindObjectOfType<AccessDatabase>().GetCurrentOwnershipWeaponPowerModelByName(PlayerPrefs.GetInt("PlayerID"),
                g.name, "Weapon");
            if (n != -1)
            {
                if (n >= 2)
                {
                    g.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "2/2";
                }
                else if (n >= 0)
                {
                    g.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = n + "/2";
                }
            }           
            LockItem(g, WeaponList[i][8], WeaponList[i][0]);
            g.SetActive(true);
            // First item choosen
            if (i == 0)
            {
                g.GetComponent<SessionArsenalItem>().ArsenalInformation(WeaponList, "1");   
            }
        }
    }
    #endregion
    #region Locked item will be gray-ed
    public void LockItem(GameObject Game, string RankId, string Id)
    {
        int rankId = 0;
        bool isLocked = false;
        // check rank req
        if (!isLocked)
        {
            if (RankId != "N/A")
            {
                if ((string)PlayerInformation["Rank"] == "Unranked")
                {
                    rankId = 0;
                }
                else
                {
                    rankId = (int)PlayerInformation["RankId"];
                }
                // lock item if its rank requirement is higher than player's rank
                if (rankId < int.Parse(RankId))
                {
                    isLocked = true;
                    if (CurrentTab == "Weapon")
                    {
                        Game.GetComponent<SessionArsenalItem>().BlackFadeWeapon.SetActive(true);
                        Game.GetComponent<SessionArsenalItem>().LockedItem = true;
                        Game.GetComponent<SessionArsenalItem>().ItemPreReq = "";
                        Game.GetComponent<SessionArsenalItem>().IsRanked = true;
                    }
                    else if (CurrentTab == "Power")
                    {
                        Game.GetComponent<SessionArsenalItem>().BlackFadePower.SetActive(true);
                        Game.GetComponent<SessionArsenalItem>().LockedItem = true;
                        Game.GetComponent<SessionArsenalItem>().ItemPreReq = "";
                        Game.GetComponent<SessionArsenalItem>().IsRanked = true;
                    }

                }
            }
        }
        // Preq Req
        if (!isLocked)
        {
            string n = FindObjectOfType<AccessDatabase>().CheckWeaponPowerPrereq(PlayerPrefs.GetInt("PlayerID"), Game.name, CurrentTab);
            int m = FindObjectOfType<AccessDatabase>().GetCurrentOwnershipWeaponPowerModelByName(PlayerPrefs.GetInt("PlayerID"),
                Game.name, CurrentTab);
            if (n != "No Prereq" && n != "Pass" && m < 1)
            {
                isLocked = true;
                if (CurrentTab == "Weapon")
                {
                    Game.GetComponent<SessionArsenalItem>().BlackFadeWeapon.SetActive(true);
                }
                else if (CurrentTab == "Power")
                {
                    Game.GetComponent<SessionArsenalItem>().BlackFadePower.SetActive(true);
                }
                Game.GetComponent<SessionArsenalItem>().LockedItem = true;
                Game.GetComponent<SessionArsenalItem>().ItemPreReq = n;
                Game.GetComponent<SessionArsenalItem>().IsRanked = false;
                Game.GetComponent<SessionArsenalItem>().IsZeroShard = false;
            }
        }
        // Already bought
        if (!isLocked)
        {
            if (CurrentTab == "Weapon")
            {
                int n = FindObjectOfType<AccessDatabase>().GetCurrentOwnershipWeaponPowerModelByName(PlayerPrefs.GetInt("PlayerID"),
                Game.name, CurrentTab);
                if (n >= 2)
                {
                    Game.GetComponent<SessionArsenalItem>().transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
                    Game.GetComponent<SessionArsenalItem>().LockedItem = true;
                    Game.GetComponent<SessionArsenalItem>().ItemPreReq = "";
                    Game.GetComponent<SessionArsenalItem>().IsRanked = false;
                    Game.GetComponent<SessionArsenalItem>().IsZeroShard = false;

                }
            }
            else if (CurrentTab == "Power")
            {
                int n = FindObjectOfType<AccessDatabase>().GetCurrentOwnershipWeaponPowerModelByName(PlayerPrefs.GetInt("PlayerID"),
                Game.name, CurrentTab);
                if (n >= 1)
                {
                    Game.GetComponent<SessionArsenalItem>().transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
                    Game.GetComponent<SessionArsenalItem>().LockedItem = true;
                    Game.GetComponent<SessionArsenalItem>().ItemPreReq = "";
                    Game.GetComponent<SessionArsenalItem>().IsRanked = false;
                    Game.GetComponent<SessionArsenalItem>().IsZeroShard = false;

                }
            }
        }



    }
    #endregion
    #region Set data (money,....) /Reset data after exiting
    public void SetData(string Cash)
    {
        PCash = Cash;
        PlayerCash.GetComponent<TextMeshPro>().text = PCash;
        if (CurrentItem != null)
        {
            // check owned item
            if (CurrentTab == "Weapon")
            {
                int n = FindObjectOfType<AccessDatabase>().GetCurrentOwnershipWeaponPowerModelByName(PlayerPrefs.GetInt("PlayerID"),
                CurrentItem.name, CurrentTab);
                if (n >= 2)
                {
                    CurrentItem.GetComponent<SessionArsenalItem>().BlackFadeWeapon.SetActive(true);
                    CurrentItem.GetComponent<SessionArsenalItem>().LockedItem = true;
                    CurrentItem.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "2/2";
                }
                else if (n >= 0)
                {
                    CurrentItem.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = n + "/2";
                }
            }
            else if (CurrentTab == "Power")
            {
                int n = FindObjectOfType<AccessDatabase>().GetCurrentOwnershipWeaponPowerModelByName(PlayerPrefs.GetInt("PlayerID"),
                CurrentItem.name, CurrentTab);
                if (n >= 1)
                {
                    CurrentItem.GetComponent<SessionArsenalItem>().BlackFadePower.SetActive(true);
                    CurrentItem.GetComponent<SessionArsenalItem>().LockedItem = true;
                }
            }
        }
        ResetDataAfterBuy();
    }
    public void ResetData()
    {


        if (CurrentCoroutine != null)
        {
            StopCoroutine(CurrentCoroutine);
        }

        if (Content.transform.parent.parent.parent.parent.GetComponent<Rigidbody2D>() != null)
        {
            Destroy(Content.transform.parent.parent.parent.parent.GetComponent<Rigidbody2D>());
        }
        if (OtherContent.transform.parent.parent.parent.parent.GetComponent<Rigidbody2D>() != null)
        {
            Destroy(OtherContent.transform.parent.parent.parent.parent.GetComponent<Rigidbody2D>());
        }
        if (WeaponButton.GetComponent<Rigidbody2D>() != null)
        {
            WeaponButton.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }
        if (PowerButton.GetComponent<Rigidbody2D>() != null)
        {
            PowerButton.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }
        PowerButton.GetComponent<Collider2D>().enabled = true;
        WeaponButton.GetComponent<Collider2D>().enabled = true;
        // reset all properties of the box (color, sortingOrder)     
        CurrentTab = "Weapon";
        Content.transform.parent.parent.parent.parent.position = InitWeaponBoxPos;
        OtherContent.transform.parent.parent.parent.parent.position = InitPowerBoxPos;
        WeaponButton.transform.position = InitWeaponBtnPos;
        PowerButton.transform.position = InitPowerBtnPos;

        Content.transform.parent.parent.parent.parent.GetComponent<SpriteRenderer>().color = WeaponBoxColor;
        OtherContent.transform.parent.parent.parent.parent.GetComponent<SpriteRenderer>().color = PowerBoxColor;
        Content.transform.parent.parent.parent.parent.GetComponent<SpriteRenderer>().sortingOrder = 3;
        OtherContent.transform.parent.parent.parent.parent.GetComponent<SpriteRenderer>().sortingOrder = 2;

        WeaponButton.GetComponent<SpriteRenderer>().sortingOrder = 4;
        PowerButton.GetComponent<SpriteRenderer>().sortingOrder = 2;
        WeaponButton.GetComponent<SpriteRenderer>().color = WeaponBoxColor;
        PowerButton.GetComponent<SpriteRenderer>().color = PowerBoxColor;

        WeaponButton.GetComponentInChildren<TextMeshPro>().sortingOrder = 5;
        PowerButton.GetComponentInChildren<TextMeshPro>().sortingOrder = 3;
        WeaponButton.GetComponentInChildren<TextMeshPro>().color = WeaponBoxColor;
        PowerButton.GetComponentInChildren<TextMeshPro>().color = PowerBoxColor;
        // check it is existed before deleting
        if (OtherContent.transform.childCount > 0)
        {
            for (int i = 0; i < OtherContent.transform.childCount; i++)
            {
                Destroy(OtherContent.transform.GetChild(i).gameObject);
            }
        }
        if (Content.transform.childCount > 0)
        {
            for (int i = 0; i < Content.transform.childCount; i++)
            {
                Destroy(Content.transform.GetChild(i).gameObject);
            }
        }

        //reset the information
        if (CurrentTab == "Power")
        {
            if (PowerStatus[0].transform.parent.parent.gameObject.activeSelf)
            {
                PowerStatus[0].transform.parent.parent.parent.gameObject.SetActive(false);
                WeaponStatus[0].transform.parent.parent.parent.gameObject.SetActive(true);

                for (int i = 0; i < PowerStatus.Count; i++)
                {
                    PowerStatus[i].GetComponent<TextMeshPro>().text = "";
                }
            }
        }
        else
        {
            for (int i = 0; i < WeaponStatus.Count; i++)
            {
                WeaponStatus[i].GetComponent<TextMeshPro>().text = "";
            }
        }


    }
    public void SetFirstData()
    {
        Content.transform.parent.parent.parent.parent.gameObject.SetActive(true);
        OtherContent.transform.parent.parent.parent.parent.gameObject.SetActive(true);
        PowerButton.SetActive(true);
        WeaponButton.SetActive(true);
        WeaponList = FindAnyObjectByType<AccessDatabase>().GetAllArsenalWeapon();
        PowerList = FindAnyObjectByType<AccessDatabase>().GetAllPower();
        PlayerId = FindAnyObjectByType<AccessDatabase>().GetCurrentSessionPlayerId();
        PlayerInformation = FindAnyObjectByType<AccessDatabase>().GetPlayerInformationById(PlayerPrefs.GetInt("PlayerID"));
        SessionPlayerInformation = FindAnyObjectByType<AccessDatabase>().GetSessionInfoByPlayerId(PlayerPrefs.GetInt("PlayerID"));
        PCash = SessionPlayerInformation["SessionCash"].ToString();
        PlayerCash.GetComponent<TextMeshPro>().text = SessionPlayerInformation["SessionCash"].ToString();     
        InitWeaponBoxPos = Content.transform.parent.parent.parent.parent.position;
        InitPowerBoxPos = OtherContent.transform.parent.parent.parent.parent.position;
        InitWeaponBtnPos = WeaponButton.transform.position;
        InitPowerBtnPos = PowerButton.transform.position;
        WeaponBoxColor = Content.transform.parent.parent.parent.parent.GetComponent<SpriteRenderer>().color;
        PowerBoxColor = OtherContent.transform.parent.parent.parent.parent.GetComponent<SpriteRenderer>().color;

        CurrentTab = "Weapon";
        FirstContent();
    }
    #endregion
    #region Start animation when enter or exit the scene 
    /*public override void OnEnterAnimation()
    {
        GetComponent<BackgroundBrieflyMoving>().enabled = true;
        transform.GetChild(0).GetComponent<Rigidbody2D>().simulated = true;
        FindObjectOfType<MainMenuCameraController>().GenerateLoadingSceneAtPos(transform.position, 1f);
        SetFirstData();
    }*/

    /*public override void OnExitAnimation()
    {
        GetComponent<BackgroundBrieflyMoving>().enabled = false;
        transform.GetChild(0).GetComponent<Rigidbody2D>().simulated = false;
        ResetData();
    }*/
    #endregion
    #region Reset Data After Buy
    public void ResetDataAfterBuy()
    {
        SessionPlayerInformation = FindAnyObjectByType<AccessDatabase>().GetSessionInfoByPlayerId(PlayerPrefs.GetInt("PlayerID"));
        PCash = SessionPlayerInformation["SessionCash"].ToString();
        PlayerCash.GetComponent<TextMeshPro>().text = SessionPlayerInformation["SessionCash"].ToString();
        if ("Weapon" == CurrentTab)
        {
            DeleteAllChild();
            //Generate item
            for (int i = 0; i < WeaponList.Count; i++)
            {
                GameObject g = Instantiate(Item, Item.transform.position, Quaternion.identity);
                g.name = WeaponList[i][2];
                g.transform.SetParent(Content.transform);
                g.transform.localScale = new Vector3(1, 1, 0);
                g.transform.GetChild(1).GetComponent<TMP_Text>().text = "<color=" + WeaponList[i][9].ToUpper() + ">" + WeaponList[i][2] + "</color>";
                g.GetComponent<SessionArsenalItem>().Id = WeaponList[i][0];
                g.GetComponent<SessionArsenalItem>().Type = "Weapon";
                g.GetComponent<SessionArsenalItem>().ItemStatusList = WeaponStatus;
                g.GetComponent<SessionArsenalItem>().Content = Content;
                g.GetComponent<SessionArsenalItem>().ArItemList = WeaponList;
                if (WeaponList[i][2] == "Star Blaster")
                {
                    g.transform.GetChild(0).GetComponent<Image>().sprite = WeaponImage[WeaponImage.FindIndex(item => item.name == "Star")].sprite;
                }
                else
                {
                    if (WeaponList[i][2].Contains("Nano Flame Thrower") || WeaponList[i][2].Contains("Freezing Blaster"))
                    {
                        g.transform.GetChild(0).GetComponent<Image>().sprite = WeaponImage[WeaponImage.FindIndex(item => item.name == "NanoFlame")].sprite;
                    }
                    else
                    {
                        g.transform.GetChild(0).GetComponent<Image>().sprite = WeaponImage[WeaponImage.FindIndex(item => WeaponList[i][2].ToLower().Contains(item.name.ToLower()))].sprite;
                    }
                }
                int n = FindObjectOfType<AccessDatabase>().GetCurrentOwnershipWeaponPowerModelByName(PlayerPrefs.GetInt("PlayerID"),
                g.name, "Weapon");
                if (n != -1)
                {
                    if (n >= 2)
                    {
                        g.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "2/2";
                    }
                    else if (n >= 0)
                    {
                        g.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = n + "/2";
                    }
                }
                LockItem(g, WeaponList[i][8], WeaponList[i][0]);
                g.SetActive(true);
                if (i == 0)
                {
                    g.GetComponent<SessionArsenalItem>().ArsenalInformation(WeaponList, "1");

                }

            }
        }
        else
        {
            if ("Power" == CurrentTab)
            {
                DeleteAllChild();
                //Generate item
                for (int i = 0; i < PowerList.Count; i++)
                {
                    GameObject g = Instantiate(Item, Item.transform.position, Quaternion.identity);
                    g.name = PowerList[i][2];
                    g.transform.SetParent(OtherContent.transform);
                    g.transform.localScale = new Vector3(1, 1, 0);
                    g.transform.GetChild(1).GetComponent<TMP_Text>().text = "<color=" + PowerList[i][9].ToUpper() + ">" + PowerList[i][2] + "</color>";
                    g.GetComponent<SessionArsenalItem>().Id = PowerList[i][0];
                    g.GetComponent<SessionArsenalItem>().Type = "Power";
                    g.GetComponent<SessionArsenalItem>().ItemStatusList = PowerStatus;
                    g.GetComponent<SessionArsenalItem>().Content = OtherContent;
                    g.GetComponent<SessionArsenalItem>().ArItemList = PowerList;
                    g.transform.GetChild(0).GetComponent<Image>().sprite = PowerButton.GetComponent<SessionArsenalButton>()
                        .PowerImage[PowerButton.GetComponent<SessionArsenalButton>().PowerImage.FindIndex(item => PowerList[i][2].Replace(" ", "").ToLower() == (item.name.ToLower()))].sprite;
                    g.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                    g.SetActive(true);
                    LockItem(g, PowerList[i][8], PowerList[i][0]);
                    if (i == 0)
                    {
                        g.GetComponent<SessionArsenalItem>().ArsenalInformation(PowerList, "1");
                    }
                }

            }
        }
    }

    public void DeleteAllChild()
    {
        // check it is existed before deleting
        if (OtherContent.transform.childCount > 0)
        {
            for (int i = 0; i < OtherContent.transform.childCount; i++)
            {
                Destroy(OtherContent.transform.GetChild(i).gameObject);
            }
        }
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
