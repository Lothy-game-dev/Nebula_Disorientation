using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArsenalButton : MonoBehaviour
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
    public GameObject WeaponImage;
    public List<SpriteRenderer> PowerImage;
    public GameObject OtherButton;
    public List<GameObject> WeaponStatus;
    public List<GameObject> PowerStatus;
    public GameObject StatusContent;
    public GameObject OtherStatusContent;
    public GameObject OtherContent;
    public GameObject Arsenal;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    private Arsenal ArsenalController;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        ArsenalController = Arsenal.GetComponent<Arsenal>();
    }

    // Update is called once per frame
    void Update()
    {
        // Call function and timer only if possible
    }
    #endregion
    #region Generate item list 
    private void OnMouseDown()
    {
        FindObjectOfType<SoundSFXGeneratorController>().GenerateSound("ButtonClick");
        Switch(gameObject.name);
    }
    #endregion
    #region Delete all item before generating a new list
    // Group all function that serve the same algorithm
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

        //reset the information
        ArsenalController.DescContent.GetComponent<TMP_Text>().text = "";
        ArsenalController.ItemCash.GetComponentInChildren<TextMeshPro>().text = "";
        ArsenalController.ItemTimelessShard.GetComponentInChildren<TextMeshPro>().text = "";
        ArsenalController.Rank.GetComponentInChildren<TextMeshPro>().text = "";
        for (int i = 0; i < OtherStatusContent.transform.childCount; i++)
        {
            OtherStatusContent.transform.GetChild(i).GetComponentInChildren<TextMeshPro>().text = "";
        }

        OtherStatusContent.SetActive(false);
    }
    #endregion
    #region Animation
    public IEnumerator StartAnimation()
    {
        // get the color of the object
        GameObject game = Content.transform.parent.parent.parent.parent.gameObject;
        GameObject otherGame = OtherContent.transform.parent.parent.parent.parent.gameObject;
        Color c = game.GetComponent<SpriteRenderer>().color;
        Color c1 = otherGame.GetComponent<SpriteRenderer>().color;
        Color x = gameObject.GetComponent<SpriteRenderer>().color;
        Color x1 = OtherButton.GetComponent<SpriteRenderer>().color;
        Color y = gameObject.GetComponentInChildren<TextMeshPro>().color;
        Color y1 = OtherButton.GetComponentInChildren<TextMeshPro>().color;
        gameObject.GetComponent<Collider2D>().enabled = false;
        OtherButton.GetComponent<Collider2D>().enabled = false;
        otherGame.AddComponent<Rigidbody2D>();

        // this loop will increase or decrease the transparency
        for (int i = 0; i < 10; i++)
        {
            
                c.a += 0.05f;
                c1.a -= 0.05f;
                x.a += 0.05f;
                x1.a -= 0.05f;
                y.a += 0.05f;
                y1.a -= 0.05f;

                game.GetComponent<SpriteRenderer>().color = c;
                otherGame.GetComponent<SpriteRenderer>().color = c1;
                gameObject.GetComponent<SpriteRenderer>().color = x;
                OtherButton.GetComponent<SpriteRenderer>().color = x1;
                gameObject.GetComponentInChildren<TextMeshPro>().color = y;
                OtherButton.GetComponentInChildren<TextMeshPro>().color = y1;


                if (i < 5)
                {
                    // the weapon tab will move to left to let the power tab shows up
                    if (ArsenalController.CurrentTab == "Weapon")
                    {
                        otherGame.GetComponent<Rigidbody2D>().velocity = new Vector2(1f, 0);
                        OtherButton.GetComponent<Rigidbody2D>().velocity = new Vector2(1f, 0);
                    }
                    else
                    // the power tab is opposite to the weapon tab
                    {
                        otherGame.GetComponent<Rigidbody2D>().velocity = new Vector2(-1f, 0);
                        OtherButton.GetComponent<Rigidbody2D>().velocity = new Vector2(-1f, 0);
                    }
                }
                else
                {
                    // and move back to the original pos after the power/weapon tab shows up
                    if (ArsenalController.CurrentTab == "Weapon")
                    {
                        otherGame.GetComponent<Rigidbody2D>().velocity = new Vector2(-1f, 0);
                        OtherButton.GetComponent<Rigidbody2D>().velocity = new Vector2(-1f, 0);
                    }
                    else
                    {
                        // same
                        otherGame.GetComponent<Rigidbody2D>().velocity = new Vector2(1f, 0);
                        OtherButton.GetComponent<Rigidbody2D>().velocity = new Vector2(1f, 0);
                    }
                }

                // setting the sortingorder
                game.GetComponent<SpriteRenderer>().sortingOrder = 3;
                otherGame.GetComponent<SpriteRenderer>().sortingOrder = 2;
                gameObject.GetComponent<SpriteRenderer>().sortingOrder = 4;
                OtherButton.GetComponent<SpriteRenderer>().sortingOrder = 2;
                gameObject.GetComponentInChildren<TextMeshPro>().sortingOrder = 5;
                OtherButton.GetComponentInChildren<TextMeshPro>().sortingOrder = 3;

           
            yield return new WaitForSeconds(0.1f);
        }      
            // delete the component to prevent the bug
            Destroy(otherGame.GetComponent<Rigidbody2D>());
            //set the velocity = 0
            otherGame.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            OtherButton.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);       
            // and enable the col of the object
            gameObject.GetComponent<Collider2D>().enabled = true;
            OtherButton.GetComponent<Collider2D>().enabled = true;
    }
    #endregion
    #region Switch category
    public void Switch(string category)
    {
        if ("Weapon" == category)
        {
            DeleteAllChild();
            //Start animation
            if (ArsenalController.CurrentTab != "Weapon")
            {
                ArsenalController.CurrentTab = "Weapon";
                ArsenalController.CurrentCoroutine = StartCoroutine(StartAnimation());
            }
            StatusContent.SetActive(true);
            OtherButton.GetComponent<SpriteRenderer>().color = Color.white;
            gameObject.GetComponent<SpriteRenderer>().color = Color.green;
            //Generate item
            for (int i = 0; i < ArsenalController.WeaponList.Count; i++)
            {
                GameObject g = Instantiate(Item, Item.transform.position, Quaternion.identity);
                g.name = ArsenalController.WeaponList[i][2];
                g.transform.SetParent(Content.transform);
                g.transform.localScale = new Vector3(1, 1, 0);
                g.transform.GetChild(1).GetComponent<TMP_Text>().text = "<color=" + ArsenalController.WeaponList[i][9].ToUpper() + ">" + ArsenalController.WeaponList[i][2] + "</color>";
                g.GetComponent<ArsenalItem>().Id = ArsenalController.WeaponList[i][0];
                g.GetComponent<ArsenalItem>().Type = "Weapon";
                g.GetComponent<ArsenalItem>().ItemStatusList = WeaponStatus;
                g.GetComponent<ArsenalItem>().Content = Content;
                g.GetComponent<ArsenalItem>().ArItemList = ArsenalController.WeaponList;
                g.transform.GetChild(0).GetComponent<Image>().sprite = WeaponImage.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite;
                // check owned item
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
                ArsenalController.LockItem(g, ArsenalController.WeaponList[i][8], ArsenalController.WeaponList[i][0]);
                g.SetActive(true);
                if (i == 0)
                {
                    g.GetComponent<ArsenalItem>().ArsenalInformation(ArsenalController.WeaponList, "1");

                }

            }
        }
        else
        {
            if ("Power" == category)
            {
                DeleteAllChild();
                //Start animation
                if (ArsenalController.CurrentTab != "Power")
                {
                    ArsenalController.CurrentTab = "Power";
                    ArsenalController.CurrentCoroutine = StartCoroutine(StartAnimation());

                }
                StatusContent.SetActive(true);
                OtherButton.GetComponent<SpriteRenderer>().color = Color.white;
                gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                //Generate item
                Debug.Log(ArsenalController);
                for (int i = 0; i < ArsenalController.PowerList.Count; i++)
                {
                    GameObject g = Instantiate(Item, Item.transform.position, Quaternion.identity);
                    g.name = ArsenalController.PowerList[i][2];
                    g.transform.SetParent(Content.transform);
                    g.transform.localScale = new Vector3(1, 1, 0);
                    g.transform.GetChild(1).GetComponent<TMP_Text>().text = "<color=" + ArsenalController.PowerList[i][9].ToUpper() + ">" + ArsenalController.PowerList[i][2] + "</color>";
                    g.GetComponent<ArsenalItem>().Id = ArsenalController.PowerList[i][0];
                    g.GetComponent<ArsenalItem>().Type = "Power";
                    g.GetComponent<ArsenalItem>().ItemStatusList = PowerStatus;
                    g.GetComponent<ArsenalItem>().Content = Content;
                    g.GetComponent<ArsenalItem>().ArItemList = ArsenalController.PowerList;
                    g.transform.GetChild(0).GetComponent<Image>().sprite = PowerImage[PowerImage.FindIndex(item => ArsenalController.PowerList[i][2].Replace(" ", "").ToLower() == (item.name.ToLower()))].sprite;
                    g.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                    g.SetActive(true);
                    ArsenalController.LockItem(g, ArsenalController.PowerList[i][8], ArsenalController.PowerList[i][0]);
                    if (i == 0)
                    {
                        g.GetComponent<ArsenalItem>().ArsenalInformation(ArsenalController.PowerList, "1");
                    }
                }

            }
        }
    }
    #endregion
}
