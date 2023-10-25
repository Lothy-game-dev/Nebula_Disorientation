using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UECSessionItemBox : MonoBehaviour
{
    #region ComponentVariables
    // Variables used for calling componenets attached to the game object only
    // Can be public or private
    #endregion
    #region InitializeVariables
    public bool isMainBox;
    public string Type;
    public GameObject Bar;
    #endregion
    #region NormalVariables
    // All other variables apart from the two aforementioned types
    // Can be public or private, prioritize private if possible
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
    #region Function group 1
    // Group all function that serve the same algorithm
    private void OnMouseDown()
    {
        if (isMainBox)
        {
            if (Type=="Consumable")
            {
                Bar.SetActive(true);
                GetComponent<BoxCollider2D>().enabled = false;
                for (int i=0;i<4;i++)
                {
                    transform.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = 200;
                    transform.GetChild(i).GetChild(0).GetComponent<TextMeshPro>().sortingOrder = 202;
                    if (transform.GetChild(i).childCount > 1)
                    transform.GetChild(i).GetChild(1).GetComponent<SpriteRenderer>().sortingOrder = 201;
                }
            } else
            {
                transform.GetChild(1).gameObject.SetActive(true);
                GetComponent<BoxCollider2D>().enabled = false;
            }
        } else
        {
            if (Type=="Weapon")
            {
                if (Bar.GetComponent<UECSessionWeaponBox>()!=null)
                {
                    Bar.GetComponent<UECSessionWeaponBox>().ChooseItem(gameObject);
                }
            } else if (Type=="Power")
            {
                if (Bar.GetComponent<UECSessionPowerBox>()!=null)
                {
                    Bar.GetComponent<UECSessionPowerBox>().ChooseItem(gameObject);
                }
            } else if (Type == "Consumable")
            {
                if (Bar.GetComponent<UECSessionConsumableBox>() != null)
                {
                    if (transform.GetChild(2).gameObject.activeSelf)
                    {
                        if (int.Parse(transform.GetChild(2).GetComponent<TextMeshProUGUI>().text) <
                            int.Parse(transform.GetChild(1).GetComponent<TextMeshProUGUI>().text))
                        {
                            string check = Bar.GetComponent<UECSessionConsumableBox>().AddItem(gameObject);
                            if (check.Equals("Success"))
                            {
                                transform.GetChild(2).GetComponent<TextMeshProUGUI>().text =
                                    (int.Parse(transform.GetChild(2).GetComponent<TextMeshProUGUI>().text) + 1).ToString();
                            }
                        }
                    } else
                    {
                        string check = Bar.GetComponent<UECSessionConsumableBox>().AddItem(gameObject);
                        if (check.Equals("Success"))
                        {
                            transform.GetChild(2).GetComponent<TextMeshProUGUI>().text ="1";
                            transform.GetChild(2).gameObject.SetActive(true);
                            transform.GetChild(3).gameObject.SetActive(true);
                            GetComponent<Image>().color = Color.green;
                        }
                    }
                }
            }
        }
    }
    #endregion
}
