using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    private EncycMenu Menu;
    private string Type;
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
        Menu.Arrow.SetActive(true);
        Menu.Arrow.transform.position = new Vector3(Menu.Arrow.transform.position.x, gameObject.transform.position.y, 0);
        Type = gameObject.transform.GetComponentInChildren<TMP_Text>().text;
        if (Type.Substring(0,1) == "W")
        {
            ShowInforOfItem(Menu.WeaponList, 2, 9, new Vector3(2.3f, 2.3f, 0f));
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
            if (Type.Substring(0,1) == "F")
            {
                ShowInforOfItem(Menu.FighterList, 1, 6, new Vector3(1f, 1f, 0f));
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
                if (Type.Substring(0, 1) == "P")
                {
                    ShowInforOfItem(Menu.PowerList, 2, 9, new Vector3(1f, 1f, 0f));
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
        wlist = list[int.Parse(Type.Remove(0, 1)) - 1];
        Color c = Menu.ItemName.GetComponent<TMP_Text>().color;
        Color c1 = Menu.ItemTier.GetComponent<TMP_Text>().color;
        if ("#36b37e".Equals(wlist[TierIndex]))
        {
            Tier = "Tier III";

            c.r = 54;
            c.g = 179;
            c.b = 133;

            c1.r = 54;
            c1.g = 179;
            c1.b = 133;
        }
        else
        {
            if ("#4c9aff".Equals(wlist[TierIndex]))
            {
                Tier = "Tier II";

                c.r = 76;
                c.g = 154;
                c.b = 255;

                c1.r = 76;
                c1.g = 154;
                c1.b = 255;
            }
            else
            {
                if ("#bf2600".Equals(wlist[TierIndex]))
                {
                    Tier = "Tier I";

                    c.r = 191;
                    c.g = 38;
                    c.b = 0;

                    c1.r = 191;
                    c1.g = 38;
                    c1.b = 0;
                } else
                {
                    if (Type.Substring(0, 1) == "W")
                    {
                        if ("#800080".Equals(wlist[9]))
                        {
                            Tier = "Special";

                            c.r = 128;
                            c.g = 0;
                            c.b = 128;

                            c1.r = 128;
                            c1.g = 0;
                            c1.b = 128;
                        }
                    }
                }
            }
        }
        Menu.ItemName.GetComponent<TMP_Text>().text = wlist[NameIndex];
        Menu.ItemTier.GetComponent<TMP_Text>().text = Tier;

        Menu.ItemName.GetComponent<TMP_Text>().color = c;
        Menu.ItemTier.GetComponent<TMP_Text>().color = c1;
    }
    #endregion
}

