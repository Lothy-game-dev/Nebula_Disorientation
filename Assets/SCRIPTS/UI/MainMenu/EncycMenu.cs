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
    public List<List<string>> EnemyList;
    public List<List<string>> ConsumList;
    public List<List<string>> WarshipList;
    public List<List<string>> SStationList;
    public List<List<string>> DmgElementList;
    public List<List<string>> AttributeList;
    public List<string> Story;
    public List<string> StoryName;
    public GameObject CurrentItem;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        WeaponList = FindAnyObjectByType<AccessDatabase>().GetAllArsenalWeapon();
        FighterList = FindAnyObjectByType<AccessDatabase>().GetAllFighter();
        PowerList = FindAnyObjectByType<AccessDatabase>().GetAllPower();
        EnemyList = FindAnyObjectByType<AccessDatabase>().GetAllEnemy();
        ConsumList = FindAnyObjectByType<AccessDatabase>().GetAllConsumable();
        WarshipList = FindAnyObjectByType<AccessDatabase>().GetAllWarship();
        SStationList = FindAnyObjectByType<AccessDatabase>().GetAllSpaceStation();
        DmgElementList = FindAnyObjectByType<AccessDatabase>().GetAllDMGElement();
        AttributeList = FindAnyObjectByType<AccessDatabase>().GetAllAttribute();

    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Generate list for each category
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
    public void ChangeColorWhenChoosen()
    {
        for (int i = 0; i < CategoryContent.transform.childCount; i++)
        {
            CategoryContent.transform.GetChild(i).GetComponent<SpriteRenderer>().color = new Color(54/255f ,140/255f, 224/255f, 1);
        }
        CurrentItem.GetComponent<SpriteRenderer>().color = Color.green;
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
        string[] text = new string[3] { "Story", "Attributes - Fighters", "Attributes - Weapons & Powers" };
        CurrentItem = CategoryContent.transform.GetChild(0).gameObject;
        GenerateEncyc("Basic", text);
    }
    public void ResetData()
    {
        
        ResetAll();
    }
    #endregion
    #region Gen encyc
    public void GenerateEncyc(string Type, string[] GroupName)
    {
        ResetAll();
        ChangeColorWhenChoosen();
        int count = 0;
        switch (Type)
        {
            case "Basic": 
                for (int i = 0; i < Story.Count; i++)
                {
                    // Create group
                    if (i == 0 || i == 12)
                    {
                        GameObject group = Instantiate(Item.transform.GetChild(0).gameObject, Item.transform.GetChild(0).gameObject.transform.position, Quaternion.identity);
                        group.transform.SetParent(Content.transform);
                        group.transform.localScale = new Vector2(1, 1);
                        group.transform.GetChild(0).GetComponent<TMP_Text>().text = GroupName[count];
                        group.name = GroupName[count];
                        group.SetActive(true);
                        count++;
                    }
                    //Create item 
                    GameObject g = Instantiate(Item.transform.GetChild(1).gameObject, Item.transform.GetChild(1).gameObject.transform.position, Quaternion.identity);
                    g.transform.SetParent(Content.transform);
                    g.transform.GetChild(0).GetComponent<TMP_Text>().text = StoryName[i];
                    g.name = StoryName[i];
                    g.transform.localScale = Item.transform.GetChild(1).gameObject.transform.localScale;
                    g.GetComponent<EncycButton>().Id = i + 1;
                    g.GetComponent<EncycButton>().Type = "STR";
                    g.SetActive(true);
                }
                for (int i = 0; i < AttributeList.Count; i++)
                {
                    // Create group
                    if (i == 0 || i == 11)
                    {
                        GameObject group = Instantiate(Item.transform.GetChild(0).gameObject, Item.transform.GetChild(0).gameObject.transform.position, Quaternion.identity);
                        group.transform.SetParent(Content.transform);
                        group.transform.localScale = new Vector2(1, 1);
                        group.transform.GetChild(0).GetComponent<TMP_Text>().text = GroupName[count];
                        group.name = GroupName[count];
                        group.SetActive(true);
                        count++;
                    }
                    //Create item 
                    GameObject g = Instantiate(Item.transform.GetChild(1).gameObject, Item.transform.GetChild(1).gameObject.transform.position, Quaternion.identity);
                    g.transform.SetParent(Content.transform);
                    g.transform.GetChild(0).GetComponent<TMP_Text>().text = AttributeList[i][1];
                    g.name = AttributeList[i][1];
                    g.transform.localScale = Item.transform.GetChild(1).gameObject.transform.localScale;
                    g.GetComponent<EncycButton>().Id = i + 1;
                    g.GetComponent<EncycButton>().Type = "ATT";
                    g.SetActive(true);
                }
                break;
            case "Vehicle":
                // Fighter
                for (int i = 0; i < FighterList.Count + EnemyList.Count; i++)
                {
                    // Create group
                    if (i == 0)
                    {
                        GameObject group = Instantiate(Item.transform.GetChild(0).gameObject, Item.transform.GetChild(0).gameObject.transform.position, Quaternion.identity);
                        group.transform.SetParent(Content.transform);
                        group.transform.localScale = new Vector2(1, 1);
                        group.transform.GetChild(0).GetComponent<TMP_Text>().text = GroupName[count];
                        group.name = GroupName[count];
                        group.SetActive(true);
                        count++;                      
                    }
                    //Create item 
                    GameObject game = Instantiate(Item.transform.GetChild(1).gameObject, Item.transform.GetChild(1).gameObject.transform.position, Quaternion.identity);
                    game.transform.SetParent(Content.transform);
                    game.transform.localScale = Item.transform.GetChild(1).gameObject.transform.localScale;
                    if (i == 0)
                    {
                        game.transform.GetChild(0).GetComponent<TMP_Text>().text = "SSTP";
                        game.GetComponent<EncycButton>().Type = "Fighter";
                        game.GetComponent<EncycButton>().Id = 666;
                        game.name = "SSTP";
                    } else
                    {
                        if (i < FighterList.Count + 1)
                        {
                            game.transform.GetChild(0).GetComponent<TMP_Text>().text = FighterList[i - 1][1];
                            game.GetComponent<EncycButton>().Type = "Fighter";
                            game.GetComponent<EncycButton>().Id = int.Parse(FighterList[i - 1][0]);                        
                            game.name = FighterList[i - 1][1];
                        
                        }
                        else
                        {
                            game.transform.GetChild(0).GetComponent<TMP_Text>().text = EnemyList[i - FighterList.Count][1];
                            game.GetComponent<EncycButton>().Type = "Enemy";
                            game.GetComponent<EncycButton>().Id = int.Parse(EnemyList[i - FighterList.Count][0]);
                            game.name = EnemyList[i - FighterList.Count][1];
                        }
                    }
                    
                    game.SetActive(true);
                }
                // Warship && SS
                for (int i = 0; i < WarshipList.Count + SStationList.Count; i++)
                {
                    Debug.Log("g");
                    // Create group
                    if (i == 0)
                    {
                        GameObject group = Instantiate(Item.transform.GetChild(0).gameObject, Item.transform.GetChild(0).gameObject.transform.position, Quaternion.identity);
                        group.transform.SetParent(Content.transform);
                        group.transform.localScale = new Vector2(1, 1);
                        group.transform.GetChild(0).GetComponent<TMP_Text>().text = GroupName[count];
                        group.name = GroupName[count];
                        group.SetActive(true);
                        count++;
                    }
                    //Create item 
                    GameObject game = Instantiate(Item.transform.GetChild(1).gameObject, Item.transform.GetChild(1).gameObject.transform.position, Quaternion.identity);
                    game.transform.SetParent(Content.transform);
                    game.transform.localScale = Item.transform.GetChild(1).gameObject.transform.localScale;
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
                break;
            case "DE":
                for (int i = 0; i < DmgElementList.Count; i++)
                {
                    // Create group
                    if (i == 0)
                    {
                        GameObject group = Instantiate(Item.transform.GetChild(0).gameObject, Item.transform.GetChild(0).gameObject.transform.position, Quaternion.identity);
                        group.transform.SetParent(Content.transform);
                        group.transform.localScale = new Vector2(1, 1);
                        group.transform.GetChild(0).GetComponent<TMP_Text>().text = GroupName[count];
                        group.name = GroupName[count];
                        group.SetActive(true);
                        count++;
                    }
                    GameObject game = Instantiate(Item.transform.GetChild(1).gameObject, Item.transform.GetChild(1).gameObject.transform.position, Quaternion.identity);
                    game.transform.SetParent(Content.transform);
                    game.transform.localScale = Item.transform.GetChild(1).gameObject.transform.localScale;
                    if (DmgElementList[i][1].Contains("Nano"))
                    {
                        game.transform.GetChild(0).GetComponent<TMP_Text>().text = "Weapon Details - " + DmgElementList[i][1];
                    } else
                    {
                        game.transform.GetChild(0).GetComponent<TMP_Text>().text = "Damage Elements - " + DmgElementList[i][1];
                    }
                    game.GetComponent<EncycButton>().Type = "DMG";
                    game.GetComponent<EncycButton>().Id = i + 1;
                    game.name = DmgElementList[i][1];
                    game.SetActive(true);
                }
                break;
            case "Shop":
                // Weapon
                for (int i = 0; i < WeaponList.Count; i++)
                {
                    // Create group
                    if (i == 0)
                    {
                        GameObject group = Instantiate(Item.transform.GetChild(0).gameObject, Item.transform.GetChild(0).gameObject.transform.position, Quaternion.identity);
                        group.transform.SetParent(Content.transform);
                        group.transform.localScale = new Vector2(1, 1);
                        group.transform.GetChild(0).GetComponent<TMP_Text>().text = GroupName[count];
                        group.name = GroupName[count];
                        group.SetActive(true);
                        count++;
                    }
                    GameObject game = Instantiate(Item.transform.GetChild(1).gameObject, Item.transform.GetChild(1).gameObject.transform.position, Quaternion.identity);
                    game.transform.SetParent(Content.transform);
                    game.transform.localScale = Item.transform.GetChild(1).gameObject.transform.localScale;
                    game.transform.GetChild(0).GetComponent<TMP_Text>().text = WeaponList[i][2];
                    game.GetComponent<EncycButton>().Type = "Weapon";
                    game.GetComponent<EncycButton>().Id = int.Parse(WeaponList[i][0]);
                    game.name = WeaponList[i][2];
                    game.SetActive(true);

                }
                // Power
                for (int i = 0; i < PowerList.Count; i++)
                {
                    // Create group
                    if (i == 0)
                    {
                        GameObject group = Instantiate(Item.transform.GetChild(0).gameObject, Item.transform.GetChild(0).gameObject.transform.position, Quaternion.identity);
                        group.transform.SetParent(Content.transform);
                        group.transform.localScale = new Vector2(1, 1);
                        group.transform.GetChild(0).GetComponent<TMP_Text>().text = GroupName[count];
                        group.name = GroupName[count];
                        group.SetActive(true);
                        count++;
                    }
                    GameObject game = Instantiate(Item.transform.GetChild(1).gameObject, Item.transform.GetChild(1).gameObject.transform.position, Quaternion.identity);
                    game.transform.SetParent(Content.transform);
                    game.transform.localScale = Item.transform.GetChild(1).gameObject.transform.localScale;
                    game.transform.GetChild(0).GetComponent<TMP_Text>().text = PowerList[i][2];
                    game.GetComponent<EncycButton>().Type = "Power";
                    game.GetComponent<EncycButton>().Id = int.Parse(PowerList[i][0]);
                    game.name = PowerList[i][2];
                    game.SetActive(true);

                }
                // Power
                for (int i = 0; i < ConsumList.Count; i++)
                {
                    // Create group
                    if (i == 0)
                    {
                        GameObject group = Instantiate(Item.transform.GetChild(0).gameObject, Item.transform.GetChild(0).gameObject.transform.position, Quaternion.identity);
                        group.transform.SetParent(Content.transform);
                        group.transform.localScale = new Vector2(1, 1);
                        group.transform.GetChild(0).GetComponent<TMP_Text>().text = GroupName[count];
                        group.name = GroupName[count];
                        group.SetActive(true);
                        count++;
                    }
                    GameObject game = Instantiate(Item.transform.GetChild(1).gameObject, Item.transform.GetChild(1).gameObject.transform.position, Quaternion.identity);
                    game.transform.SetParent(Content.transform);
                    game.transform.localScale = Item.transform.GetChild(1).gameObject.transform.localScale;
                    game.transform.GetChild(0).GetComponent<TMP_Text>().text = ConsumList[i][1];
                    game.GetComponent<EncycButton>().Type = "Consumable";
                    game.GetComponent<EncycButton>().Id = int.Parse(ConsumList[i][0]);
                    game.name = ConsumList[i][1];
                    game.SetActive(true);

                }
                break;
        }
    }
    #endregion
}
