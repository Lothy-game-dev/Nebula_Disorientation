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
    private AccessDatabase AccDB;

    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject Item;
    public GameObject Content;
    public List<SpriteRenderer> WeaponImage;
    public List<SpriteRenderer> PowerImage;
    public GameObject OtherButton;
    public List<GameObject> ItemStatus;
    public GameObject DescContent;
    public GameObject ItemCash;
    public GameObject ItemTimelessShard;
    public GameObject Rank;
    public GameObject BuyButton;
    public List<GameObject> WeaponStatus;
    public GameObject StatusContent;
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
    public bool CanBuy;
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
    } 

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Function group 1
    // Group all function that serve the same algorithm
    private void OnMouseDown()
    {
        
        if ("Weapon" == gameObject.name)
        {
            ItemStatus = new List<GameObject>();
            DeleteAllChild();
            OtherButton.GetComponent<SpriteRenderer>().color = Color.white;
            gameObject.GetComponent<SpriteRenderer>().color = Color.green;
            for (int i = 0; i < WeaponList.Count; i++)
            {
                GameObject g = Instantiate(Item, Item.transform.position, Quaternion.identity);
                g.transform.SetParent(Content.transform);
                g.transform.localScale = new Vector3(1, 1, 0);
                g.transform.GetChild(1).GetComponent<TMP_Text>().text = WeaponList[i][2];
                g.GetComponent<ArsenalItem>().Id = WeaponList[i][0];
                g.GetComponent<ArsenalItem>().Type = "Weapon";
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
            /*for (int i = 0; i < WeaponStatus.Count; i++)
            {
                GameObject game = Instantiate(WeaponStatus[i], WeaponStatus[i].transform.position, Quaternion.identity);
                game.transform.SetParent(StatusContent.transform);
                game.transform.localScale = WeaponStatus[i].transform.localScale;
                ItemStatus.Add(game.transform.GetChild(1).GetChild(0).gameObject);
                game.SetActive(true);

            }*/
                
        } else
        {
            if ("Power" == gameObject.name)
            {
                DeleteAllChild();
                OtherButton.GetComponent<SpriteRenderer>().color = Color.white;
                gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                for (int i = 0; i < PowerList.Count; i++)
                {
                    GameObject g = Instantiate(Item, Item.transform.position, Quaternion.identity);
                    g.transform.SetParent(Content.transform);
                    g.transform.localScale = new Vector3(1, 1, 0);
                    g.transform.GetChild(1).GetComponent<TMP_Text>().text = PowerList[i][2];
                    g.GetComponent<ArsenalItem>().Id = PowerList[i][0];
                    g.GetComponent<ArsenalItem>().Type = "Power";
                    g.transform.GetChild(0).GetComponent<Image>().sprite = PowerImage[PowerImage.FindIndex(item => PowerList[i][2].Replace(" ", "").ToLower().Contains(item.name.ToLower()))].sprite;
                    g.SetActive(true);
                }
            }
        }
    }
    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    public void DeleteAllChild()
    {
        if (Content.transform.childCount > 0)
        {
            Debug.Log("a");
            for (int i = 0; i < Content.transform.childCount; i++)
            {
                Destroy(Content.transform.GetChild(i).gameObject);
            }
        }
        DescContent.GetComponent<TMP_Text>().text = "";
        ItemCash.GetComponentInChildren<TextMeshPro>().text = "";
        ItemTimelessShard.GetComponentInChildren<TextMeshPro>().text = "";
    }
    #endregion
}
