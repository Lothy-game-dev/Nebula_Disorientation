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
    public List<SpriteRenderer> WeapImage;
    public List<SpriteRenderer> FighterImage;
    public List<SpriteRenderer> PowerImage;
    public GameObject Content;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public string Type;
    public int Id;
    private EncycMenu Menu;
    private List<List<string>> WeapList;
    private List<string> wlist;
    private string Tier;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        Menu = EncyclMenu.GetComponent<EncycMenu>();
        WeapList = FindAnyObjectByType<AccessDatabase>().GetAllArsenalWeapon();
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
        
        if (Type == "Weapon")
        {
            ShowInforOfItem(Menu.WeaponList, 2, 9, new Vector3(2.3f, 2.3f, 0f));
            ChangeColorWhenChoosen(Id.ToString());
            if (wlist[2] == "Star Blaster")
            {
                Menu.ItemImage.GetComponent<SpriteRenderer>().sprite = WeapImage[WeapImage.FindIndex(item => item.name == "Star")].sprite;
            } else
            {
                if (wlist[2].Contains("Nano Flame Thrower"))
                {
                    Menu.ItemImage.GetComponent<SpriteRenderer>().sprite = WeapImage[WeapImage.FindIndex(item => item.name == "NanoFlame")].sprite;
                } else
                {
                    Menu.ItemImage.GetComponent<SpriteRenderer>().sprite = WeapImage[WeapImage.FindIndex(item => wlist[2].ToLower().Contains(item.name.ToLower()))].sprite;
                }
            }
           

        } else
        {
            //Fighter
            if (Type == "Fighter")
            {
                ShowInforOfItem(Menu.FighterList, 1, 6, new Vector3(0.5f, 0.5f, 0f));
                ChangeColorWhenChoosen(Id.ToString());
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
                //Power
                if (Type == "Power")
                {
                    ShowInforOfItem(Menu.PowerList, 2, 9, new Vector3(0.6f, 0.6f, 0f));
                    ChangeColorWhenChoosen(Id.ToString());
                    Menu.ItemImage.GetComponent<SpriteRenderer>().sprite = PowerImage[PowerImage.FindIndex(item => wlist[2].Replace(" ", "").ToLower() == item.name.ToLower())].sprite;                                          
                }
            }
        }
    }
    #endregion
    #region Function group ...
    // Group all function that serve the same algorithm
    private void ShowInforOfItem(List<List<string>> list, int NameIndex, int TierIndex, Vector3 ImageScale)
    {
        Menu.ItemImage.transform.localScale = ImageScale;
        wlist = list[Id - 1];
        
        Menu.ItemName.GetComponent<TMP_Text>().text = "<color=" + wlist[TierIndex] +">"  + wlist[NameIndex] + "</color>";

        switch(wlist[TierIndex])
        {
            case "#36b37e": Tier = "Tier III";
                            break;
            case "#4c9aff": Tier = "Tier II";
                            break;
            case "#bf2600": Tier = "Tier I";
                            break;
            default: Tier = "Special"; break;
        }
        Menu.ItemTier.GetComponent<TMP_Text>().text = "<color=" + wlist[TierIndex] + ">" + Tier + "</color>";

 
    }
    #endregion
    #region
    public void ChangeColorWhenChoosen(string Id)
    {
        Debug.Log(Id);
        for (int i = 0; i < Content.transform.childCount; i++)
        {
            Content.transform.GetChild(i).GetComponent<Image>().color = Color.white;
        }
        Content.transform.GetChild(int.Parse(Id) - 1).GetComponent<Image>().color = Color.green;
    }
    #endregion
}

