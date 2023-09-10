using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EncycMenu : MonoBehaviour
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
    public GameObject ItemImage;
    public GameObject ItemName;
    public GameObject ItemTier;
    public string[] Category;
    public GameObject CategoryContent;
    public GameObject CateTemplate;
    public ScrollRect Scroll;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public List<List<string>> WeaponList;
    public List<List<string>> FighterList;
    public List<List<string>> PowerList;
    private List<GameObject> OthersCategory;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        WeaponList = FindAnyObjectByType<AccessDatabase>().GetAllArsenalWeapon();
        FighterList = FindAnyObjectByType<AccessDatabase>().GetAllFighter();
        PowerList = FindAnyObjectByType<AccessDatabase>().GetAllPower();

        for (int i = 0; i < Category.Length; i++)
        {
            GameObject g = Instantiate(CateTemplate, CateTemplate.transform.position, Quaternion.identity);
            g.transform.SetParent(CategoryContent.transform);
            g.transform.GetChild(0).GetComponent<TMP_Text>().text = Category[i];
            g.SetActive(true);
            

        }

    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Function group 1
    private void OnMouseDown()
    {
    
        if ("Weapon".Equals(gameObject.GetComponentInChildren<TMP_Text>().text))
        {
            DeleteAllChild();
            ChangeColorWhenChoosen("Weapon");
            for (int i = 0; i < WeaponList.Count; i++)
            {
                GameObject game = Instantiate(Item, Item.transform.position, Quaternion.identity);
                game.transform.SetParent(Content.transform);
                game.transform.localScale = new Vector3(2, 7, 0);
                game.transform.GetChild(0).GetComponent<TMP_Text>().text = WeaponList[i][2];
                game.GetComponent<EncycButton>().Type = "Weapon";
                game.GetComponent<EncycButton>().Id = int.Parse(WeaponList[i][0]);
                game.SetActive(true);
            }
        } else
        {
            if ("Fighter".Equals(gameObject.GetComponentInChildren<TMP_Text>().text))
            {
                DeleteAllChild();
                ChangeColorWhenChoosen("Fighter");
                for (int i = 0; i < FighterList.Count; i++)
                {
                    GameObject game = Instantiate(Item, Item.transform.position, Quaternion.identity);
                    game.transform.SetParent(Content.transform);
                    game.transform.localScale = new Vector3(2, 7, 0);
                    game.transform.GetChild(0).GetComponent<TMP_Text>().text = FighterList[i][1];
                    game.GetComponent<EncycButton>().Type = "Fighter";
                    game.GetComponent<EncycButton>().Id = int.Parse(FighterList[i][0]);
                    game.SetActive(true);
                }
            } else
            {
                if ("WS & SS".Equals(gameObject.GetComponentInChildren<TMP_Text>().text))
                {
                    DeleteAllChild();
                    ChangeColorWhenChoosen("WS & SS");

                } else
                {
                    if ("Power & Consumable".Equals(gameObject.GetComponentInChildren<TMP_Text>().text))
                    {
                        DeleteAllChild();
                        ChangeColorWhenChoosen("Power & Consumable");
                        for (int i = 0; i < PowerList.Count; i++)
                        {
                            GameObject game = Instantiate(Item, Item.transform.position, Quaternion.identity);
                            game.transform.SetParent(Content.transform);
                            game.transform.localScale = new Vector3(2, 7, 0);
                            game.transform.GetChild(0).GetComponent<TMP_Text>().text = PowerList[i][2];
                            game.GetComponent<EncycButton>().Type = "Power";
                            game.GetComponent<EncycButton>().Id = int.Parse(PowerList[i][0]);
                            game.SetActive(true);
                        }
                    }
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
            ItemName.GetComponent<TMP_Text>().text = "";
            ItemTier.GetComponent<TMP_Text>().text = "";
            ItemImage.GetComponent<SpriteRenderer>().sprite = null;
           
        }
    }
    #endregion
    #region
    public void ChangeColorWhenChoosen(string ObjectName)
    {

        for (int i = 0; i < CategoryContent.transform.childCount; i++)
        {
            if (ObjectName != CategoryContent.transform.GetChild(i).GetComponentInChildren<TMP_Text>().text) 
            {
                CategoryContent.transform.GetChild(i).GetComponent<Image>().color = Color.white;

            } else
            {
                CategoryContent.transform.GetChild(i).GetComponent<Image>().color = Color.green;
            }
        }
    }
    #endregion
}
