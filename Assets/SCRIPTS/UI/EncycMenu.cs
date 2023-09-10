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
    public GameObject Arrow;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
    public List<List<string>> WeaponList;
    public List<List<string>> FighterList;
    public List<List<string>> PowerList;
    #endregion
    #region Start & Update
    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        WeaponList = FindAnyObjectByType<AccessDatabase>().GetAllArsenalWeapon();
        FighterList = FindAnyObjectByType<AccessDatabase>().GetAllFighter();
        PowerList = FindAnyObjectByType<AccessDatabase>().GetAllPower();
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
    
        if ("Weapon".Equals(gameObject.name))
        {
            DeleteAllChild();
            for (int i = 0; i < WeaponList.Count; i++)
            {
                GameObject game = Instantiate(Item, Item.transform.position, Quaternion.identity);
                game.transform.SetParent(Content.transform);
                game.transform.localScale = new Vector3(2, 7, 0);
                game.transform.GetChild(0).GetComponent<TMP_Text>().text = "W" + (i + 1);
                game.SetActive(true);
            }
        } else
        {
            if ("Fighter".Equals(gameObject.name))
            {
                DeleteAllChild();
                for (int i = 0; i < FighterList.Count; i++)
                {
                    GameObject game = Instantiate(Item, Item.transform.position, Quaternion.identity);
                    game.transform.SetParent(Content.transform);
                    game.transform.localScale = new Vector3(2, 7, 0);
                    game.transform.GetChild(0).GetComponent<TMP_Text>().text = "F" + (i + 1);
                    game.SetActive(true);
                }
            } else
            {
                if ("WWSS".Equals(gameObject.name))
                {
                    DeleteAllChild();

                } else
                {
                    if ("Power".Equals(gameObject.name))
                    {
                        DeleteAllChild();
                        for (int i = 0; i < PowerList.Count; i++)
                        {
                            GameObject game = Instantiate(Item, Item.transform.position, Quaternion.identity);
                            game.transform.SetParent(Content.transform);
                            game.transform.localScale = new Vector3(2, 7, 0);
                            game.transform.GetChild(0).GetComponent<TMP_Text>().text = "P" + (i + 1);
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
            Arrow.SetActive(false);
        }
    }
    #endregion
}
