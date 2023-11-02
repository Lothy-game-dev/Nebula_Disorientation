using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EncycMenu : MainMenuSceneShared
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
    public GameObject ItemDesc;
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
    public List<List<string>> EnemyList;
    public List<List<string>> ConsumList;
    public List<List<string>> WarshipList;
    public List<List<string>> SStationList;
    public List<List<string>> DmgElementList;
    public List<List<string>> AttributeList;
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
    #region Generate list for each category
    private void OnMouseDown()
    {
    
        if ("Weapon".Equals(gameObject.GetComponentInChildren<TMP_Text>().text))
        {
            ResetAll();
            ChangeColorWhenChoosen("Weapon");
            for (int i = 0; i < WeaponList.Count; i++)
            {
                GameObject game = Instantiate(Item, Item.transform.position, Quaternion.identity);
                game.transform.SetParent(Content.transform);
                game.transform.localScale = new Vector3(2, 7, 0);
                game.transform.GetChild(0).GetComponent<TMP_Text>().text = WeaponList[i][2];
                game.GetComponent<EncycButton>().Type = "Weapon";
                game.GetComponent<EncycButton>().Id = int.Parse(WeaponList[i][0]);
                game.name = WeaponList[i][2];
                game.SetActive(true);
            }
        } else
        {
            if ("Fighter".Equals(gameObject.GetComponentInChildren<TMP_Text>().text))
            {
                ResetAll();
                ChangeColorWhenChoosen("Fighter");
                for (int i = 0; i < FighterList.Count + EnemyList.Count; i++)
                {
                    GameObject game = Instantiate(Item, Item.transform.position, Quaternion.identity);
                    game.transform.SetParent(Content.transform);
                    game.transform.localScale = new Vector3(2, 7, 0);
                    if (i < FighterList.Count)
                    {
                        game.transform.GetChild(0).GetComponent<TMP_Text>().text = FighterList[i][1];
                        game.GetComponent<EncycButton>().Type = "Fighter";
                        game.GetComponent<EncycButton>().Id = int.Parse(FighterList[i][0]);
                        game.name = FighterList[i][1];
                    } else
                    {
                        game.transform.GetChild(0).GetComponent<TMP_Text>().text = EnemyList[i - FighterList.Count][1];
                        game.GetComponent<EncycButton>().Type = "Enemy";
                        game.GetComponent<EncycButton>().Id = int.Parse(EnemyList[i - FighterList.Count][0]);
                        game.name = EnemyList[i - FighterList.Count][1];

                    }
                    game.SetActive(true);
                }
            } else
            {
                if ("WS & SS".Equals(gameObject.GetComponentInChildren<TMP_Text>().text))
                {
                    ResetAll();
                    ChangeColorWhenChoosen("WS & SS");
                    Debug.Log(WarshipList.Count);
                    for (int i = 0; i < WarshipList.Count + SStationList.Count; i++)
                    {
                        GameObject game = Instantiate(Item, Item.transform.position, Quaternion.identity);
                        game.transform.SetParent(Content.transform);
                        game.transform.localScale = new Vector3(2, 7, 0);
                        if (i < WarshipList.Count)
                        {
                            game.transform.GetChild(0).GetComponent<TMP_Text>().text = WarshipList[i][1];
                            game.GetComponent<EncycButton>().Type = "Warship";
                            game.GetComponent<EncycButton>().Id = int.Parse(WarshipList[i][0]);
                            game.name = WarshipList[i][1];

                        }
                        else
                        {
                            game.transform.GetChild(0).GetComponent<TMP_Text>().text = SStationList[i - WarshipList.Count][1];
                            game.GetComponent<EncycButton>().Type = "SpaceStation";
                            game.GetComponent<EncycButton>().Id = int.Parse(SStationList[i - WarshipList.Count][0]);
                            game.name = SStationList[i - WarshipList.Count][1];
                        }
                        game.SetActive(true);
                    }

                } else
                {
                    if ("Power & Consumable".Equals(gameObject.GetComponentInChildren<TMP_Text>().text))
                    {
                        ResetAll();
                        ChangeColorWhenChoosen("Power & Consumable");
                        for (int i = 0; i < PowerList.Count + ConsumList.Count; i++)
                        {
                            GameObject game = Instantiate(Item, Item.transform.position, Quaternion.identity);
                            game.transform.SetParent(Content.transform);
                            game.transform.localScale = new Vector3(2, 7, 0);
                            if (i < PowerList.Count)
                            {
                                game.transform.GetChild(0).GetComponent<TMP_Text>().text = PowerList[i][2];
                                game.GetComponent<EncycButton>().Type = "Power";
                                game.GetComponent<EncycButton>().Id = int.Parse(PowerList[i][0]);
                                game.name = PowerList[i][2];

                            } else
                            {
                                game.transform.GetChild(0).GetComponent<TMP_Text>().text = ConsumList[i - PowerList.Count][1];
                                game.GetComponent<EncycButton>().Type = "Consumable";
                                game.GetComponent<EncycButton>().Id = int.Parse(ConsumList[i - PowerList.Count][0]);
                                game.name = ConsumList[i - PowerList.Count][1];
                            }
                            game.SetActive(true);
                        } 
                    } else
                    {
                        if ("Damage Element".Equals(gameObject.GetComponentInChildren<TMP_Text>().text))
                        {
                            ResetAll();
                            ChangeColorWhenChoosen("Damage Element");
                            for (int i = 0; i < DmgElementList.Count; i++)
                            {
                                GameObject game = Instantiate(Item, Item.transform.position, Quaternion.identity);
                                game.transform.SetParent(Content.transform);
                                game.transform.localScale = new Vector3(2, 7, 0);
                                game.transform.GetChild(0).GetComponent<TMP_Text>().text = DmgElementList[i][1];
                                game.GetComponent<EncycButton>().Type = "DMG";
                                game.GetComponent<EncycButton>().Id = int.Parse(DmgElementList[i][0]);
                                game.name = DmgElementList[i][1];
                                game.SetActive(true);
                            }
                        } else
                        {
                            if ("Attribute".Equals(gameObject.GetComponentInChildren<TMP_Text>().text))
                            {
                                ResetAll();
                                ChangeColorWhenChoosen("Attribute");
                                for (int i = 0; i < AttributeList.Count; i++)
                                {
                                    GameObject game = Instantiate(Item, Item.transform.position, Quaternion.identity);
                                    game.transform.SetParent(Content.transform);
                                    game.transform.localScale = new Vector3(2, 7, 0);
                                    game.transform.GetChild(0).GetComponent<TMP_Text>().text = AttributeList[i][1];
                                    game.GetComponent<EncycButton>().Type = "ATT";
                                    game.GetComponent<EncycButton>().Id = int.Parse(AttributeList[i][0]);
                                    game.name = AttributeList[i][1];
                                    game.SetActive(true);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    #endregion
    #region Delete the item before generating a new list
    // Group all function that serve the same algorithm
    public void ResetAll()
    {
        if (Content.transform.childCount > 0)
        {
            for (int i = 0; i < Content.transform.childCount; i++)
            {
                Destroy(Content.transform.GetChild(i).gameObject);
            }
            ItemName.GetComponent<TMP_Text>().text = "";
            ItemTier.GetComponent<TMP_Text>().text = "";
            if (ItemImage.transform.parent.childCount > 1)
            {
                Destroy(ItemImage.transform.parent.GetChild(1).gameObject);
            } else
            {
                ItemImage.GetComponent<SpriteRenderer>().sprite = null;
            }
            ItemDesc.GetComponent<TMP_Text>().text = "";


        }
    }
    #endregion
    #region Change color when choosen
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
    #region Animation
    public override void StartAnimation()
    {
        GetComponent<BackgroundBrieflyMoving>().enabled = true;
        transform.GetChild(0).GetComponent<Rigidbody2D>().simulated = true;
        GetData();
    }

    public override void EndAnimation()
    {
        GetComponent<BackgroundBrieflyMoving>().enabled = false;
        transform.GetChild(0).GetComponent<Rigidbody2D>().simulated = false;
        ResetData();
    }
    #endregion
    #region ResetData and GetData
    public void GetData()
    {
        WeaponList = FindAnyObjectByType<AccessDatabase>().GetAllArsenalWeapon();
        FighterList = FindAnyObjectByType<AccessDatabase>().GetAllFighter();
        PowerList = FindAnyObjectByType<AccessDatabase>().GetAllPower();
        EnemyList = FindAnyObjectByType<AccessDatabase>().GetAllEnemy();
        ConsumList = FindAnyObjectByType<AccessDatabase>().GetAllConsumable();
        WarshipList = FindAnyObjectByType<AccessDatabase>().GetAllWarship();
        SStationList = FindAnyObjectByType<AccessDatabase>().GetAllSpaceStation();
        DmgElementList = FindAnyObjectByType<AccessDatabase>().GetAllDMGElement();
        AttributeList = FindAnyObjectByType<AccessDatabase>().GetAllAttribute();
        for (int i = 0; i < Category.Length; i++)
        {
            GameObject g = Instantiate(CateTemplate, CateTemplate.transform.position, Quaternion.identity);
            g.transform.SetParent(CategoryContent.transform);
            g.transform.GetChild(0).GetComponent<TMP_Text>().text = Category[i];
            g.SetActive(true);
        }
        ChangeColorWhenChoosen("Fighter");
        for (int i = 0; i < FighterList.Count + EnemyList.Count; i++)
        {
            GameObject game = Instantiate(Item, Item.transform.position, Quaternion.identity);
            game.transform.SetParent(Content.transform);
            game.transform.localScale = new Vector3(2, 7, 0);
            if (i < FighterList.Count)
            {
                game.transform.GetChild(0).GetComponent<TMP_Text>().text = FighterList[i][1];
                game.GetComponent<EncycButton>().Type = "Fighter";
                game.GetComponent<EncycButton>().Id = int.Parse(FighterList[i][0]);
                game.name = FighterList[i][1];
                if (i == 0)
                {
                    game.GetComponent<EncycButton>().ShowTheCurrentItem("Fighter");
                }
            }
            else
            {
                game.transform.GetChild(0).GetComponent<TMP_Text>().text = EnemyList[i - FighterList.Count][1];
                game.GetComponent<EncycButton>().Type = "Enemy";
                game.GetComponent<EncycButton>().Id = int.Parse(EnemyList[i - FighterList.Count][0]);
                game.name = EnemyList[i - FighterList.Count][1];

            }
            game.SetActive(true);
        }
    }
    public void ResetData()
    {
        if (CategoryContent.transform.childCount > 0)
        {
            for (int i = 1; i < CategoryContent.transform.childCount; i++)
            {
                Destroy(CategoryContent.transform.GetChild(i).gameObject);
            }
        }
        ResetAll();
    }
    #endregion
}
