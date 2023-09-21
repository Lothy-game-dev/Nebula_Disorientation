using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Arsenal : UECMenuShared
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
    private Color WeaponBoxColor;
    private Color PowerBoxColor;
    public string ItemName;
    public string RequiredShard;
    public string RequiredCash;
    public GameObject CurrentItem;
    #endregion
    #region Start & Update

    // Start is called before the first frame update
    void Start()
    {
        
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
            g.transform.GetChild(1).GetComponent<TMP_Text>().text = WeaponList[i][2];
            g.GetComponent<ArsenalItem>().Id = WeaponList[i][0];
            g.GetComponent<ArsenalItem>().Type = "Weapon";
            g.GetComponent<ArsenalItem>().ItemStatusList = WeaponStatus;
            g.GetComponent<ArsenalItem>().Content = Content;
            g.GetComponent<ArsenalItem>().ArItemList = WeaponList;
            if (WeaponList[i][2] == "Star Blaster")
            {
                g.transform.GetChild(0).GetComponent<Image>().sprite = WeaponImage[WeaponImage.FindIndex(item => item.name == "Star")].sprite;
            }
            else
            {
                if (WeaponList[i][2].Contains("Nano Flame Thrower"))
                {
                    g.transform.GetChild(0).GetComponent<Image>().sprite = WeaponImage[WeaponImage.FindIndex(item => item.name == "NanoFlame")].sprite;
                }
                else
                {
                    g.transform.GetChild(0).GetComponent<Image>().sprite = WeaponImage[WeaponImage.FindIndex(item => WeaponList[i][2].ToLower().Contains(item.name.ToLower()))].sprite;
                }
            }
            int n = FindObjectOfType<AccessDatabase>().GetCurrentOwnershipWeaponPowerModelByName(FindObjectOfType<UECMainMenuController>().PlayerId,
                g.name, "Weapon");
            if (n!=-1)
            {
                if (n>=2)
                {
                    g.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "2/2";
                } else if (n>=0)
                {
                    g.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = n + "/2";
                }
            } 
            g.SetActive(true);
            // First item choosen
            if (i == 0)
            {
                g.GetComponent<ArsenalItem>().ArsenalInformation(WeaponList, "1");
            }
            LockItem(g, WeaponList[i][8], WeaponList[i][0]);
        }
    }
    #endregion
    #region Locked item will be gray-ed
    public void LockItem(GameObject Game, string RankId, string Id)
    {
        int rankId = 0;
        bool isLocked = false;
        if (RankId != "N/A")
        {
            if ((string)PlayerInformation["Rank"] == "Unranked")
            {
                rankId = 0;
            } else
            {
                rankId = (int)PlayerInformation["RankId"];
            }
            if (rankId < int.Parse(RankId))
            {
                isLocked = true;
                if (CurrentTab == "Weapon")
                {
                    Game.GetComponent<ArsenalItem>().BlackFadeWeapon.SetActive(true);
                    Game.GetComponent<ArsenalItem>().LockedItem = true;
                    Game.GetComponent<ArsenalItem>().ItemPreReq = "";
                    Game.GetComponent<ArsenalItem>().IsRanked = true;
                }
                else if (CurrentTab == "Power")
                {
                    Game.GetComponent<ArsenalItem>().BlackFadePower.SetActive(true);
                    Game.GetComponent<ArsenalItem>().LockedItem = true;
                    Game.GetComponent<ArsenalItem>().ItemPreReq = "";
                    Game.GetComponent<ArsenalItem>().IsRanked = true;
                }

            }
        }
        //If item doesnt have shard value, it cant be buy permanently
        if (!isLocked)
        {
            if (CurrentTab == "Weapon")
            {
                if (WeaponList[int.Parse(Id) - 1][6] == "N.A")
                {
                    isLocked = true;
                    Game.GetComponent<ArsenalItem>().BlackFadeWeapon.SetActive(true);
                    Game.GetComponent<ArsenalItem>().LockedItem = true;
                    Game.GetComponent<ArsenalItem>().IsRanked = false;
                    Game.GetComponent<ArsenalItem>().IsZeroShard = true;
                } 
            }
            else
            {
                if (CurrentTab == "Power")
                {
                    if (PowerList[int.Parse(Id) - 1][6] == "N.A")
                    {
                        isLocked = true;
                        Game.GetComponent<ArsenalItem>().BlackFadeWeapon.SetActive(true);
                        Game.GetComponent<ArsenalItem>().LockedItem = true;
                        Game.GetComponent<ArsenalItem>().IsRanked = false;
                        Game.GetComponent<ArsenalItem>().IsZeroShard = true;
                    }
                }
            }
        }
        // Preq Req
        if (!isLocked)
        {
            string n = FindObjectOfType<AccessDatabase>().CheckWeaponPowerPrereq(FindObjectOfType<UECMainMenuController>().PlayerId, Game.name, CurrentTab);
            if (n!="No Prereq" && n!="Pass")
            {
                isLocked = true;
                if (CurrentTab=="Weapon")
                {
                    Game.GetComponent<ArsenalItem>().BlackFadeWeapon.SetActive(true);
                } else if (CurrentTab=="Power")
                {
                    Game.GetComponent<ArsenalItem>().BlackFadePower.SetActive(true);
                }
                Game.GetComponent<ArsenalItem>().LockedItem = true;
                Game.GetComponent<ArsenalItem>().ItemPreReq = n;
                Game.GetComponent<ArsenalItem>().IsRanked = false;
                Game.GetComponent<ArsenalItem>().IsZeroShard = false;
            } 
        }
        // Already bought
        if (!isLocked)
        {
            if (CurrentTab == "Weapon")
            {
                int n = FindObjectOfType<AccessDatabase>().GetCurrentOwnershipWeaponPowerModelByName(FindObjectOfType<UECMainMenuController>().PlayerId,
                Game.name, CurrentTab);
                if (n >= 2)
                {
                    Game.GetComponent<ArsenalItem>().transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
                    Game.GetComponent<ArsenalItem>().LockedItem = true;
                    Game.GetComponent<ArsenalItem>().ItemPreReq = "";
                    Game.GetComponent<ArsenalItem>().IsRanked = false;
                    Game.GetComponent<ArsenalItem>().IsZeroShard = false;
                }
            } else if (CurrentTab == "Power")
            {
                int n = FindObjectOfType<AccessDatabase>().GetCurrentOwnershipWeaponPowerModelByName(FindObjectOfType<UECMainMenuController>().PlayerId,
                Game.name, CurrentTab);
                if (n >= 1)
                {
                    Game.GetComponent<ArsenalItem>().transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
                    Game.GetComponent<ArsenalItem>().LockedItem = true;
                    Game.GetComponent<ArsenalItem>().ItemPreReq = "";
                    Game.GetComponent<ArsenalItem>().IsRanked = false;
                    Game.GetComponent<ArsenalItem>().IsZeroShard = false;
                }
            }
        }
            
    }
    #endregion
    #region Set data (money,....) /Reset data after exiting
    public void SetData(string Cash, string Shard)
    {
        PCash = Cash;
        PShard = Shard;
        PlayerCash.GetComponent<TextMeshPro>().text = PCash;
        PlayerShard.GetComponent<TextMeshPro>().text = PShard;
        if (CurrentItem!=null)
        {
            if (CurrentTab == "Weapon")
            {
                int n = FindObjectOfType<AccessDatabase>().GetCurrentOwnershipWeaponPowerModelByName(FindObjectOfType<UECMainMenuController>().PlayerId,
                CurrentItem.name, CurrentTab);
                if (n >= 2)
                {
                    CurrentItem.GetComponent<ArsenalItem>().BlackFadeWeapon.SetActive(true);
                    CurrentItem.GetComponent<ArsenalItem>().LockedItem = true;
                    CurrentItem.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = "2/2";
                } else if (n>=0)
                {
                    CurrentItem.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = n + "/2";
                }
            }
            else if (CurrentTab == "Power")
            {
                int n = FindObjectOfType<AccessDatabase>().GetCurrentOwnershipWeaponPowerModelByName(FindObjectOfType<UECMainMenuController>().PlayerId,
                CurrentItem.name, CurrentTab);
                if (n >= 1)
                {
                    CurrentItem.GetComponent<ArsenalItem>().BlackFadePower.SetActive(true);
                    CurrentItem.GetComponent<ArsenalItem>().LockedItem = true;
                }
            }
        }
        ResetDataAfterBuy();
    }
    public void ResetData()
    {
        // reset all properties of the box (color, sortingOrder)
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
        } else
        {
            for (int i = 0; i < WeaponStatus.Count; i++)
            {
                WeaponStatus[i].GetComponent<TextMeshPro>().text = "";
            }
        }
    }
    public void SetFirstData()
    {
        WeaponList = FindAnyObjectByType<AccessDatabase>().GetAllArsenalWeapon();
        PowerList = FindAnyObjectByType<AccessDatabase>().GetAllPower();
        PlayerId = FindAnyObjectByType<AccessDatabase>().GetCurrentSessionPlayerId();
        PlayerInformation = FindAnyObjectByType<AccessDatabase>().GetPlayerInformationById(PlayerId);
        PCash = PlayerInformation["Cash"].ToString();
        PShard = PlayerInformation["TimelessShard"].ToString();
        PlayerCash.GetComponent<TextMeshPro>().text = PlayerInformation["Cash"].ToString();
        PlayerShard.GetComponent<TextMeshPro>().text = PlayerInformation["TimelessShard"].ToString();
        WeaponBoxColor = Content.transform.parent.parent.parent.parent.GetComponent<SpriteRenderer>().color;
        PowerBoxColor = OtherContent.transform.parent.parent.parent.parent.GetComponent<SpriteRenderer>().color;
        FirstContent();
        CurrentTab = "Weapon";
    }
    #endregion
    #region Start animation when enter or exit the scene 
    public override void OnEnterAnimation()
    {
        GetComponent<BackgroundBrieflyMoving>().enabled = true;
        transform.GetChild(0).GetComponent<Rigidbody2D>().simulated = true;
        FindObjectOfType<MainMenuCameraController>().GenerateLoadingSceneAtPos(transform.position, 1f);
        SetFirstData();
    }

    public override void OnExitAnimation()
    {
        GetComponent<BackgroundBrieflyMoving>().enabled = false;
        transform.GetChild(0).GetComponent<Rigidbody2D>().simulated = false;
        ResetData();
    }
    #endregion
    #region Reset Data After Buy
    public void ResetDataAfterBuy()
    {
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
                g.transform.GetChild(1).GetComponent<TMP_Text>().text = WeaponList[i][2];
                g.GetComponent<ArsenalItem>().Id = WeaponList[i][0];
                g.GetComponent<ArsenalItem>().Type = "Weapon";
                g.GetComponent<ArsenalItem>().ItemStatusList = WeaponStatus;
                g.GetComponent<ArsenalItem>().Content = Content;
                g.GetComponent<ArsenalItem>().ArItemList = WeaponList;
                if (WeaponList[i][2] == "Star Blaster")
                {
                    g.transform.GetChild(0).GetComponent<Image>().sprite = WeaponImage[WeaponImage.FindIndex(item => item.name == "Star")].sprite;
                }
                else
                {
                    if (WeaponList[i][2].Contains("Nano Flame Thrower"))
                    {
                        g.transform.GetChild(0).GetComponent<Image>().sprite = WeaponImage[WeaponImage.FindIndex(item => item.name == "NanoFlame")].sprite;
                    }
                    else
                    {
                        g.transform.GetChild(0).GetComponent<Image>().sprite = WeaponImage[WeaponImage.FindIndex(item => WeaponList[i][2].ToLower().Contains(item.name.ToLower()))].sprite;
                    }
                }
                int n = FindObjectOfType<AccessDatabase>().GetCurrentOwnershipWeaponPowerModelByName(FindObjectOfType<UECMainMenuController>().PlayerId,
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
                    g.GetComponent<ArsenalItem>().ArsenalInformation(WeaponList, "1");

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
                    g.transform.GetChild(1).GetComponent<TMP_Text>().text = PowerList[i][2];
                    g.GetComponent<ArsenalItem>().Id = PowerList[i][0];
                    g.GetComponent<ArsenalItem>().Type = "Power";
                    g.GetComponent<ArsenalItem>().ItemStatusList = PowerStatus;
                    g.GetComponent<ArsenalItem>().Content = OtherContent;
                    g.GetComponent<ArsenalItem>().ArItemList = PowerList;
                    g.transform.GetChild(0).GetComponent<Image>().sprite = PowerButton.GetComponent<ArsenalButton>()
                        .PowerImage[PowerButton.GetComponent<ArsenalButton>().PowerImage.FindIndex(item => PowerList[i][2].Replace(" ", "").ToLower().Contains(item.name.ToLower()))].sprite;
                    g.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                    g.SetActive(true);
                    LockItem(g, PowerList[i][8], PowerList[i][0]);
                    if (i == 0)
                    {
                        g.GetComponent<ArsenalItem>().ArsenalInformation(PowerList, "1");
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
