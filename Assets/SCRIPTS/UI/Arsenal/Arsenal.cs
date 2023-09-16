using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Arsenal : MonoBehaviour
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
    public List<SpriteRenderer> WeaponImage;
    public List<GameObject> WeaponStatus;
    public List<GameObject> PowerStatus;
    public GameObject DescContent;
    public GameObject ItemCash;
    public GameObject ItemTimelessShard;
    public GameObject Rank;
    public GameObject PlayerCash;
    public GameObject PlayerShard;
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
    #endregion
    #region Start & Update

    // Start is called before the first frame update
    void Start()
    {
        WeaponList = FindAnyObjectByType<AccessDatabase>().GetAllArsenalWeapon();
        PowerList = FindAnyObjectByType<AccessDatabase>().GetAllPower();
        PlayerId = FindAnyObjectByType<AccessDatabase>().GetCurrentSessionPlayerId();
        PlayerInformation = FindAnyObjectByType<AccessDatabase>().GetPlayerInformationById(PlayerId);
        
        if (gameObject.name == "Arsenal")
        {
            PlayerCash.GetComponent<TextMeshPro>().text = PlayerInformation["Cash"].ToString();
            PlayerShard.GetComponent<TextMeshPro>().text = PlayerInformation["TimelessShard"].ToString();
        }
        CurrentTab = "Weapon";
        FirstContent();
    } 

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region
    private void FirstContent()
    {
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
            g.SetActive(true);
        }

        // First item choosen
        FirstItemChoosen(WeaponList, WeaponStatus, "Weapon", Content);



    }
    #endregion
    #region
    public void FirstItemChoosen(List<List<string>> ItemList, List<GameObject> StatusList, string Type, GameObject ContentType)
    {
        if (Type == "Weapon")
        {
            Status = FindAnyObjectByType<GlobalFunctionController>().ConvertWeaponStatsToDictionary(ItemList[0][4]);
        }
        else
        {
            Status = FindAnyObjectByType<GlobalFunctionController>().ConvertPowerStatsToDictionary(ItemList[0][4]);

        }
        RankSys = FindAnyObjectByType<AccessDatabase>().GetRankById(int.Parse(ItemList[0][8]));
        ContentType.transform.GetChild(0).GetComponent<Image>().color = Color.green;
        for (int i = 0; i < StatusList.Count; i++)
        {
            if (!Status.ContainsKey(StatusList[i].name))
            {
                StatusList[i].GetComponent<TextMeshPro>().text = "N/A";
            }
            else
            {
                StatusList[i].GetComponent<TextMeshPro>().text = (string)Status[StatusList[i].name];
            }
        }
        EnoughPrice = true;
        RankRequired = true;
        ItemId = int.Parse(ItemList[0][0]);
        ItemType = Type;
        ItemTimelessShard.GetComponentInChildren<TextMeshPro>().text = "<color=green>" + ItemList[0][6] + "</color>";
        DescContent.GetComponent<TMP_Text>().text = ItemList[0][3];
        ItemCash.GetComponentInChildren<TextMeshPro>().text = "<color=green>" + ItemList[0][5] + "</color>";
        Rank.GetComponentInChildren<TextMeshPro>().text = "<color=green>Rank Required</color><br><color=" + (string)RankSys["RankTier"] + ">" + (string)RankSys["RankName"] + "</color>";
    }
    #endregion
}
