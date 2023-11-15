using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EncycButton : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    // Variables that will be initialize in Unity Design, will not initialize these variables in Start function
    // Must be public
    // All importants number related to how a game object behave will be declared in this part
    public GameObject EncyclMenu;
    public GameObject WeapImage;
    public List<SpriteRenderer> FighterImage;
    public List<SpriteRenderer> PowerImage;
    public List<SpriteRenderer> EnemyImage;
    public List<SpriteRenderer> ConsumableImage;
    public List<SpriteRenderer> WarshipImage;
    public List<SpriteRenderer> SpaceStationImage;
    public List<GameObject> AttributeImage;
    public GameObject Content;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public string Type;
    public int Id;
    public int IdInGroup;
    private EncycMenu Menu;
    private List<List<string>> WeapList;
    private List<string> wlist;
    private string Tier;
    private GameObject CurrentItem;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        WeapList = FindAnyObjectByType<AccessDatabase>().GetAllArsenalWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Show the information of the item for each type
    // Group all function that serve the same algorithm
    private void OnMouseDown()
    {
        FindObjectOfType<SoundSFXGeneratorController>().GenerateSound("ButtonClick");
        ShowTheCurrentItem(Type);
    }
    #endregion
    #region Show item's information
    // Group all function that serve the same algorithm
    private void ShowInforOfItem(List<List<string>> list, int NameIndex, int TierIndex, Vector3 ImageScale, int DescIndex)
    {
        Menu.ItemImage.transform.localScale = ImageScale;
        wlist = list[Id - 1];
        if (TierIndex > 0)
        {
            switch(wlist[TierIndex])
            {
                case "#36b37e": Tier = "Tier III";
                                break;
                case "#4c9aff": Tier = "Tier II";
                                break;
                case "#bf2600": Tier = "Tier I";
                                break;
                case "#ff0d11": Tier = "Tier I";
                                break;
                default: Tier = "Special"; break;
            }
            Menu.ItemTier.GetComponent<TMP_Text>().text = "<color=" + wlist[TierIndex] + ">" + Tier + "</color>";           
        } else
        {
            wlist[TierIndex] = "white";
        }
        Menu.ItemDesc.GetComponent<TMP_Text>().text = wlist[DescIndex];
        Menu.ItemName.GetComponent<TMP_Text>().text = "<color=" + wlist[TierIndex] +">"  + wlist[NameIndex] + "</color>";

    }
    public void ShowTheCurrentItem(string ItemType)
    {
        Menu = EncyclMenu.GetComponent<EncycMenu>();
        CurrentItem = gameObject;
        if (ItemType == "Weapon")
        {
            ShowInforOfItem(Menu.WeaponList, 2, 9, new Vector3(0.4f, 0.4f, 0f), 3);
            ChangeColorWhenChoosen();
            Menu.ItemImage.GetComponent<SpriteRenderer>().sprite = WeapImage.transform.GetChild(Id - 1).GetComponent<SpriteRenderer>().sprite;

        }
        else
        {
            //Fighter
            if (ItemType == "Fighter")
            {
                ChangeColorWhenChoosen();
                if (Id != 666)
                {
                    ShowInforOfItem(Menu.FighterList, 1, 6, new Vector3(0.5f, 0.5f, 0f), 7);
                    if (wlist[1] == "SSS-MKL")
                    {
                        Menu.ItemImage.GetComponent<SpriteRenderer>().sprite = FighterImage[FighterImage.FindIndex(item => item.name == "SSSL")].sprite;
                    }
                    else
                    {
                        if (wlist[1].Contains("UEC29-MKL"))
                        {
                            Menu.ItemImage.GetComponent<SpriteRenderer>().sprite = FighterImage[FighterImage.FindIndex(item => item.name == "UEC29L")].sprite;
                        }
                        else
                        {
                            Menu.ItemImage.GetComponent<SpriteRenderer>().sprite = FighterImage[FighterImage.FindIndex(item => wlist[1].ToLower().Contains(item.name.ToLower()))].sprite;
                        }
                    }
                } else
                {
                    Menu.ItemDesc.GetComponent<TMP_Text>().text = "Transporting goodies between Space Stations is extremely crucial for the people of UEC to survive during the war. Always accompanied by a squad of fighters.";
                    Menu.ItemName.GetComponent<TMP_Text>().text = "<color=#36b37e>SSTP</color>";
                    Menu.ItemTier.GetComponent<TMP_Text>().text = "";
                    Menu.ItemImage.transform.localScale = new Vector3(0.9f, 0.9f, 0f);
                    Menu.ItemImage.GetComponent<SpriteRenderer>().sprite = FighterImage[FighterImage.FindIndex(item => item.name == "UECZ_Transporter")].sprite;               }
            }
            else
            {
                //Power
                if (ItemType == "Power")
                {
                    ShowInforOfItem(Menu.PowerList, 2, 9, new Vector3(0.7f, 0.7f, 0f), 3);
                    ChangeColorWhenChoosen();
                    Menu.ItemImage.GetComponent<SpriteRenderer>().sprite = PowerImage[PowerImage.FindIndex(item => wlist[2].Replace(" ", "").ToLower() == item.name.ToLower())].sprite;
                }
                else
                {
                    //Enemy
                    if (ItemType == "Enemy")
                    {

                        ShowInforOfItem(Menu.EnemyList, 1, 8, new Vector3(0.5f, 0.5f, 0f), 7);
                        ChangeColorWhenChoosen();
                        if (wlist[1] == "ZatWR-MKL")
                        {
                            Menu.ItemImage.GetComponent<SpriteRenderer>().sprite = EnemyImage[EnemyImage.FindIndex(item => item.name == "ZatWRL")].sprite;
                        }
                        else
                        {
                            if (wlist[1].Contains("KazaT-MKL"))
                            {
                                Menu.ItemImage.GetComponent<SpriteRenderer>().sprite = EnemyImage[EnemyImage.FindIndex(item => item.name == "KazaTL")].sprite;
                            }
                            else
                            {
                                Menu.ItemImage.GetComponent<SpriteRenderer>().sprite = EnemyImage[EnemyImage.FindIndex(item => wlist[1].ToLower().Contains(item.name.ToLower()))].sprite;
                            }
                        }

                    }
                    else
                    {
                        if (ItemType == "Consumable")
                        {
                            ShowInforOfItem(Menu.ConsumList, 1, 10, new Vector3(0.7f, 0.7f, 0f), 2);
                            ChangeColorWhenChoosen();
                            Menu.ItemImage.GetComponent<SpriteRenderer>().sprite = ConsumableImage[ConsumableImage.FindIndex(item => wlist[1].Replace(" ", "").Replace("-", "").ToLower() == (item.name.ToLower()))].sprite;
                        }
                        else
                        {
                            if (ItemType == "Warship")
                            {
                                ShowInforOfItem(Menu.WarshipList, 1, 4, new Vector2(0.065f, 0.065f), 2);
                                ChangeColorWhenChoosen();
                                Menu.ItemImage.GetComponent<SpriteRenderer>().sprite = WarshipImage[WarshipImage.FindIndex(item => wlist[1].ToLower().Contains(item.name.ToLower()))].sprite;
                            }
                            else
                            {
                                if (ItemType == "SpaceStation")
                                {
                                    ShowInforOfItem(Menu.SStationList, 1, 4, new Vector2(0.065f, 0.065f), 2);
                                    ChangeColorWhenChoosen();
                                    Menu.ItemImage.GetComponent<SpriteRenderer>().sprite = SpaceStationImage[SpaceStationImage.FindIndex(item => wlist[1].ToLower().Contains(item.name.ToLower()))].sprite;
                                }
                                else
                                {
                                    if (ItemType == "DMG")
                                    {
                                        ShowInforOfItem(Menu.DmgElementList, 1, 0, new Vector2(0.2f, 0.2f), 2);
                                        ChangeColorWhenChoosen();
                                    }
                                    else
                                    {
                                        if (ItemType == "ATT")
                                        {
                                            if (Menu.ItemImage.transform.parent.childCount > 1)
                                            {
                                                Destroy(Menu.ItemImage.transform.parent.GetChild(1).gameObject);
                                            }
                                            ShowInforOfItem(Menu.AttributeList, 1, 0, new Vector2(0.5f, 0.5f), 2);
                                            ChangeColorWhenChoosen();
                                            if (wlist[1].Contains("Slot") | wlist[1].Contains("PoE") | wlist[1].Contains("(AE)"))
                                            {
                                                Menu.ItemImage.GetComponent<SpriteRenderer>().sprite = null;
                                            }
                                            else
                                            {
                                                GameObject g = AttributeImage[AttributeImage.FindIndex(item => wlist[1].Replace(" ", "").ToLower().Contains(item.name.ToLower()))];
                                                GameObject clone = Instantiate(g, Menu.ItemImage.transform.position, Quaternion.identity);
                                                clone.GetComponent<SpriteRenderer>().sortingOrder = 3;
                                                clone.transform.SetParent(Menu.ItemImage.transform.parent);

                                            }
                                        }
                                        else
                                        {
                                            if (Menu.ItemImage.transform.parent.childCount > 1)
                                            {
                                                Destroy(Menu.ItemImage.transform.parent.GetChild(1).gameObject);
                                            }
                                            ChangeColorWhenChoosen();
                                            Menu.ItemDesc.GetComponent<TMP_Text>().text = Menu.Story[Id - 1];
                                            Menu.ItemName.GetComponent<TMP_Text>().text = Menu.StoryName[Id - 1];
                                            Menu.ItemImage.GetComponent<SpriteRenderer>().sprite = null;
                                        }
                                        

                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    #endregion
    #region Change the color when choosen
    public void ChangeColorWhenChoosen()
    {
        for (int i = 0; i < Content.transform.childCount; i++)
        {
            if (Content.transform.GetChild(i).GetComponent<EncycButton>() != null)
            {
                Content.transform.GetChild(i).GetComponent<Image>().color = Color.white;
            }
        }
        CurrentItem.GetComponent<Image>().color = Color.green;
    }
    #endregion
}

